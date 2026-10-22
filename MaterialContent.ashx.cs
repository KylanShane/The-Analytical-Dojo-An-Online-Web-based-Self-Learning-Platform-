using System;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;

namespace The_Analytical_Dojo
{
    public class MaterialContent : IHttpHandler, IRequiresSessionState
    {
        private const long MaximumPdfBytes = 30L * 1024L * 1024L;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["UserId"] == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            int materialId;

            if (
                !int.TryParse(context.Request.QueryString["id"], out materialId) ||
                materialId <= 0
            )
            {
                throw new HttpException(400, "Invalid material.");
            }

            int userId = Convert.ToInt32(context.Session["UserId"]);
            DataTable table = DojoData.Query(
                @"
                SELECT m.file_url
                FROM course_materials m
                INNER JOIN course_chapters ch
                    ON ch.chapter_id = m.chapter_id
                INNER JOIN course_enrollments e
                    ON e.course_id = ch.course_id
                    AND e.user_id = @userId
                WHERE m.material_id = @materialId
                    AND m.material_type = 'PPT'",
                DojoData.P("@materialId", materialId),
                DojoData.P("@userId", userId)
            );

            if (table.Rows.Count == 0)
            {
                throw new HttpException(404, "Slide material was not found.");
            }

            string sourceUrl = DojoMaterials.SafeUrl(
                Convert.ToString(table.Rows[0]["file_url"])
            );

            if (sourceUrl == null || !DojoMaterials.IsPdf(sourceUrl))
            {
                throw new HttpException(415, "This material is not a PDF slide deck.");
            }

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader(
                "Content-Disposition",
                "inline; filename=slides-" + materialId + ".pdf"
            );
            context.Response.Cache.SetCacheability(HttpCacheability.Private);
            context.Response.Cache.SetMaxAge(TimeSpan.FromMinutes(10));

            Uri remoteUri;

            if (Uri.TryCreate(sourceUrl, UriKind.Absolute, out remoteUri))
            {
                StreamRemotePdf(context, remoteUri);
                return;
            }

            string localPath = context.Server.MapPath(sourceUrl);
            FileInfo localFile = new FileInfo(localPath);

            if (!localFile.Exists || localFile.Length > MaximumPdfBytes)
            {
                throw new HttpException(404, "The slide file is unavailable.");
            }

            context.Response.TransmitFile(localPath);
        }

        private static void StreamRemotePdf(HttpContext context, Uri source)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(source);
            request.AllowAutoRedirect = true;
            request.Timeout = 15000;
            request.ReadWriteTimeout = 30000;
            request.UserAgent = "Mozilla/5.0 (compatible; AnalyticalDojo/1.0)";
            request.Accept = "application/pdf,*/*;q=0.8";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (
                    response.ContentLength > MaximumPdfBytes ||
                    response.ContentLength == 0
                )
                {
                    throw new HttpException(502, "The slide file is unavailable or too large.");
                }

                if (response.ContentLength > 0)
                {
                    context.Response.AddHeader(
                        "Content-Length",
                        response.ContentLength.ToString()
                    );
                }

                using (Stream input = response.GetResponseStream())
                {
                    byte[] buffer = new byte[81920];
                    long total = 0;
                    int read;

                    while (input != null && (read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        total += read;

                        if (total > MaximumPdfBytes)
                        {
                            throw new HttpException(502, "The slide file is too large.");
                        }

                        context.Response.OutputStream.Write(buffer, 0, read);
                    }
                }
            }
        }
    }
}
