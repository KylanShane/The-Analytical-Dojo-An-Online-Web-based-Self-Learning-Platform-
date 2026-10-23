using System;
using System.Data.SqlClient;
using System.Web;

// Shared learning-material helpers used by CoursePage and MaterialView.
public static class DojoMaterials
{
    public static string SafeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        url = url.Trim();

        if (
            url.Contains("\\") ||
            url.Contains("\r") ||
            url.Contains("\n") ||
            url.StartsWith("//")
        )
        {
            return null;
        }

        // Allow external HTTP and HTTPS material links.
        Uri absoluteUri;

        if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
        {
            if (
                absoluteUri.Scheme == Uri.UriSchemeHttps ||
                absoluteUri.Scheme == Uri.UriSchemeHttp
            )
            {
                return url;
            }

            return null;
        }

        if (url.Contains(":"))
        {
            return null;
        }

        // Resolve material paths inside this application.
        if (url.StartsWith("~/"))
        {
            return VirtualPathUtility.ToAbsolute(url);
        }

        return VirtualPathUtility.ToAbsolute("~/" + url.TrimStart('/'));
    }

    public static string EmbedUrl(string materialType, string materialUrl)
    {
        if (string.IsNullOrWhiteSpace(materialUrl))
        {
            return null;
        }

        if (IsVideo(materialType))
        {
            string videoId = YouTubeId(materialUrl);

            if (videoId == null)
            {
                return null;
            }

            return "https://www.youtube-nocookie.com/embed/" + videoId + "?rel=0&modestbranding=1";
        }

        if (!string.Equals(materialType, "PPT", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        Uri absoluteUri;

        if (!Uri.TryCreate(materialUrl, UriKind.Absolute, out absoluteUri))
        {
            return materialUrl.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? materialUrl
                : null;
        }

        string path = absoluteUri.AbsolutePath;

        if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return materialUrl;
        }

        if (
            path.EndsWith(".ppt", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase)
        )
        {
            return
                "https://view.officeapps.live.com/op/embed.aspx?src=" +
                HttpUtility.UrlEncode(materialUrl);
        }

        return null;
    }

    public static bool IsPdf(string url)
    {
        Uri absoluteUri;
        string path = Uri.TryCreate(url, UriKind.Absolute, out absoluteUri)
            ? absoluteUri.AbsolutePath
            : url.Split('?')[0];

        return path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsVideo(string materialType)
    {
        return string.Equals(materialType, "VIDEO", StringComparison.OrdinalIgnoreCase);
    }

    // Thumbnail for the course page preview tile; null for non-YouTube materials.
    public static string ThumbnailUrl(string materialType, string materialUrl)
    {
        if (!IsVideo(materialType) || materialUrl == null)
        {
            return null;
        }

        string videoId = YouTubeId(materialUrl);

        return videoId == null
            ? null
            : "https://i.ytimg.com/vi/" + videoId + "/hqdefault.jpg";
    }

    public static string YouTubeId(string url)
    {
        Uri uri;

        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            return null;
        }

        string host = uri.Host.ToLowerInvariant();
        string videoId = null;

        if (host == "youtu.be" || host == "www.youtu.be")
        {
            videoId = uri.AbsolutePath.Trim('/').Split('/')[0];
        }
        else if (
            host == "youtube.com" ||
            host == "www.youtube.com" ||
            host == "m.youtube.com"
        )
        {
            if (uri.AbsolutePath == "/watch")
            {
                videoId = HttpUtility.ParseQueryString(uri.Query)["v"];
            }
            else if (uri.AbsolutePath.StartsWith("/embed/"))
            {
                videoId = uri.AbsolutePath.Substring(7).Split('/')[0];
            }
        }

        if (string.IsNullOrWhiteSpace(videoId) || videoId.Length != 11)
        {
            return null;
        }

        foreach (char value in videoId)
        {
            if (!char.IsLetterOrDigit(value) && value != '-' && value != '_')
            {
                return null;
            }
        }

        return videoId;
    }

    public static void Complete(int userId, int courseId, int materialId)
    {
        using (SqlConnection connection = DojoDb.GetConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                int enrollmentId = DojoData.LockEnrollment(
                    connection,
                    transaction,
                    userId,
                    courseId
                );

                // Verify that this material belongs to this course.
                object result = DojoData.Scalar(
                    connection,
                    transaction,
                    @"
                    SELECT COUNT(*)
                    FROM course_materials m
                    INNER JOIN course_chapters ch
                        ON ch.chapter_id = m.chapter_id
                    WHERE m.material_id = @materialId
                        AND ch.course_id = @courseId",
                    DojoData.P("@materialId", materialId),
                    DojoData.P("@courseId", courseId)
                );

                if (Convert.ToInt32(result) != 1)
                {
                    throw new HttpException(
                        400,
                        "This material does not belong to the course."
                    );
                }

                // Insert progress or complete an existing progress row.
                // Preserve the original completion date on repeat clicks.
                DojoData.Execute(
                    connection,
                    transaction,
                    @"
                    IF EXISTS
                    (
                        SELECT 1
                        FROM student_material_progress
                        WHERE enrollment_id = @enrollmentId
                            AND material_id = @materialId
                    )
                    BEGIN
                        UPDATE student_material_progress
                        SET completed_at = COALESCE(completed_at, SYSDATETIME())
                        WHERE enrollment_id = @enrollmentId
                            AND material_id = @materialId;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO student_material_progress
                        (enrollment_id, material_id, completed_at)
                        VALUES
                        (@enrollmentId, @materialId, SYSDATETIME());
                    END",
                    DojoData.P("@enrollmentId", enrollmentId),
                    DojoData.P("@materialId", materialId)
                );

                // Recalculate course progress and achievements.
                DojoData.Refresh(
                    connection,
                    transaction,
                    userId,
                    courseId,
                    enrollmentId
                );

                transaction.Commit();
            }
        }
    }
}
