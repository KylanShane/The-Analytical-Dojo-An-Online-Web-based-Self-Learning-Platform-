using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminCourse : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        // Chapters and materials of the pending courses, used while data binding.
        private DataTable pendingChapters;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                // The View window only stays open for the View click itself.
                pnlView.Visible = false;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["created"] == "1")
                {
                    ShowMessage(
                        "Course created successfully.",
                        true
                    );
                }
                if (Request.QueryString["updated"] == "1")
                {
                    ShowMessage(
                        "Course updated successfully.",
                        true
                    );
                }

                LoadCourseStatistics();
                LoadPendingCourses();
                LoadCourses();
            }
        }

        private void LoadCourseStatistics()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        COUNT(*) AS TotalCourses,

                        SUM(CASE WHEN status = 'PUBLISHED'
                                 THEN 1 ELSE 0 END) AS TotalPublished,

                        SUM(CASE WHEN course_level = 'basic'
                                 THEN 1 ELSE 0 END) AS BasicTotal,
                        SUM(CASE WHEN course_level = 'basic'
                                 AND status = 'PENDING'
                                 THEN 1 ELSE 0 END) AS BasicPending,

                        SUM(CASE WHEN course_level = 'intermediate'
                                 THEN 1 ELSE 0 END) AS IntermediateTotal,
                        SUM(CASE WHEN course_level = 'intermediate'
                                 AND status = 'PENDING'
                                 THEN 1 ELSE 0 END) AS IntermediatePending,

                        SUM(CASE WHEN course_level = 'advanced'
                                 THEN 1 ELSE 0 END) AS AdvancedTotal,
                        SUM(CASE WHEN course_level = 'advanced'
                                 AND status = 'PENDING'
                                 THEN 1 ELSE 0 END) AS AdvancedPending

                    FROM courses";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTotalCourses.Text =
                            ReadInt(reader, "TotalCourses").ToString();

                        lblTotalSub.Text =
                            ReadInt(reader, "TotalPublished") + " Published";

                        lblBasicCourses.Text =
                            ReadInt(reader, "BasicTotal").ToString();

                        lblBasicSub.Text =
                            PendingText(ReadInt(reader, "BasicPending"));

                        lblIntermediateCourses.Text =
                            ReadInt(reader, "IntermediateTotal").ToString();

                        lblIntermediateSub.Text =
                            PendingText(ReadInt(reader, "IntermediatePending"));

                        lblAdvancedCourses.Text =
                            ReadInt(reader, "AdvancedTotal").ToString();

                        lblAdvancedSub.Text =
                            PendingText(ReadInt(reader, "AdvancedPending"));
                    }
                }
            }
        }

        private static int ReadInt(
            SqlDataReader reader,
            string column)
        {
            object value = reader[column];

            return value == DBNull.Value
                ? 0
                : Convert.ToInt32(value);
        }

        private static string PendingText(int pending)
        {
            return pending == 0
                ? "All Published"
                : pending + " Pending For Verification";
        }

        private void LoadCourses()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        course_id,
                        course_title,
                        course_level,
                        status

                    FROM courses

                    WHERE
                        (
                            @search = ''
                            OR course_title LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @level = ''
                            OR course_level = @level
                        )

                        AND
                        (
                            @status = ''
                            OR status = @status
                        )

                    ORDER BY course_id ASC";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@search",
                    txtSearch.Text.Trim()
                );

                cmd.Parameters.AddWithValue(
                    "@level",
                    ddlLevelFilter.SelectedValue
                );

                cmd.Parameters.AddWithValue(
                    "@status",
                    ddlStatusFilter.SelectedValue
                );

                SqlDataAdapter adapter =
                    new SqlDataAdapter(cmd);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            LoadCourses();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";

            ddlLevelFilter.SelectedIndex = 0;
            ddlStatusFilter.SelectedIndex = 0;

            LoadCourses();
        }

        protected void gvCourses_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewCourse")
            {
                ShowViewPanel(Convert.ToInt32(e.CommandArgument));
            }
            else if (e.CommandName == "EditCourse")
            {
                int courseId =
                    Convert.ToInt32(e.CommandArgument);

                Response.Redirect(
                    "AdminCreateCourse.aspx?id=" + courseId,
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            else if (e.CommandName == "DeleteCourse")
            {
                int courseId =
                    Convert.ToInt32(e.CommandArgument);

                DeleteCourse(courseId);

                LoadCourseStatistics();
                LoadPendingCourses();
                LoadCourses();
            }
        }

        private void DeleteCourse(int courseId)
        {
            List<string> fileUrls = new List<string>();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        try
                        {
                            // 1) Remember the uploaded files so they can be removed too.
                            string filesQuery = @"
                                SELECT m.file_url
                                FROM course_materials m
                                INNER JOIN course_chapters c
                                    ON m.chapter_id = c.chapter_id
                                WHERE c.course_id = @courseId";

                            using (SqlCommand cmd =
                                new SqlCommand(filesQuery, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@courseId", courseId);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader[0] != DBNull.Value)
                                        {
                                            fileUrls.Add(reader[0].ToString());
                                        }
                                    }
                                }
                            }

                            // 2) Materials -> chapters -> course.
                            string deleteQuery = @"
                                DELETE m
                                FROM course_materials m
                                INNER JOIN course_chapters c
                                    ON m.chapter_id = c.chapter_id
                                WHERE c.course_id = @courseId;

                                DELETE FROM course_chapters
                                WHERE course_id = @courseId;

                                DELETE FROM courses
                                WHERE course_id = @courseId;";

                            using (SqlCommand cmd =
                                new SqlCommand(deleteQuery, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@courseId", courseId);

                                cmd.ExecuteNonQuery();
                            }

                            string existsQuery =
                                "SELECT COUNT(*) FROM courses WHERE course_id = @courseId";

                            using (SqlCommand cmd =
                                new SqlCommand(existsQuery, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@courseId", courseId);

                                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                {
                                    tx.Rollback();

                                    ShowMessage(
                                        "The course could not be deleted.",
                                        false
                                    );

                                    return;
                                }
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                DeleteUploadedFiles(fileUrls);

                ShowMessage(
                    "Course deleted successfully.",
                    true
                );
            }
            catch (SqlException ex)
            {
                // 547 = still referenced elsewhere (enrollments, quizzes, ...)
                if (ex.Number == 547)
                {
                    ShowMessage(
                        "This course cannot be deleted because students are enrolled or quizzes are linked to it. Set its status to Archived instead.",
                        false
                    );
                }
                else
                {
                    ShowMessage(
                        "The course could not be deleted.",
                        false
                    );
                }
            }
        }

        // Only removes files this site uploaded (never the demo material paths).
        private void DeleteUploadedFiles(List<string> fileUrls)
        {
            string root =
                Path.GetFullPath(Server.MapPath("~/Uploads/Materials"));

            foreach (string url in fileUrls)
            {
                if (url == null ||
                    !url.StartsWith("/Uploads/Materials/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    string path =
                        Path.GetFullPath(Server.MapPath("~" + url));

                    if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase) &&
                        File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    // Best effort cleanup only.
                }
            }
        }

        private void ScrollToTop()
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "scrollTop",
                "window.addEventListener('load', function () { setTimeout(function () { window.scrollTo({ top: 0, behavior: 'smooth' }); }, 50); });",
                true
            );
        }

        // ------------------------------------------------------------
        // Verify pending courses
        // ------------------------------------------------------------

        private void LoadPendingCourses()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string coursesQuery = @"
                    SELECT
                        c.course_id,
                        c.course_title,
                        c.description,
                        c.course_level,
                        c.status,
                        c.created_at,

                        COALESCE(
                            u.full_name,
                            u.username,
                            'Unknown User'
                        ) AS creator_name

                    FROM courses c

                    LEFT JOIN users u
                        ON c.created_by = u.user_id

                    WHERE c.status = 'PENDING'

                    ORDER BY c.created_at DESC, c.course_id DESC";

                DataTable courses = new DataTable();

                new SqlDataAdapter(coursesQuery, con).Fill(courses);

                string chaptersQuery = @"
                    SELECT
                        c.course_id,
                        c.chapter_id,
                        c.chapter_title,
                        c.description AS chapter_description,
                        m.material_id,
                        m.material_title,
                        m.material_type,
                        m.file_url

                    FROM course_chapters c

                    INNER JOIN courses co
                        ON co.course_id = c.course_id
                        AND co.status = 'PENDING'

                    LEFT JOIN course_materials m
                        ON m.chapter_id = c.chapter_id

                    ORDER BY
                        c.course_id,
                        c.chapter_order,
                        c.chapter_id,
                        m.material_id";

                pendingChapters = new DataTable();

                new SqlDataAdapter(chaptersQuery, con).Fill(pendingChapters);

                lblPendingCount.Text =
                    courses.Rows.Count + " Pending";

                rptPendingCourses.DataSource = courses;
                rptPendingCourses.DataBind();

                pnlNoPending.Visible =
                    courses.Rows.Count == 0;
            }
        }

        protected void rptPendingCourses_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "PublishCourse")
            {
                return;
            }

            int courseId =
                Convert.ToInt32(e.CommandArgument);

            if (PublishCourse(courseId))
            {
                ShowMessage(
                    "Course published successfully.",
                    true
                );
            }
            else
            {
                ShowMessage(
                    "The course could not be published. It may no longer be pending.",
                    false
                );
            }

            LoadCourseStatistics();
            LoadPendingCourses();
            LoadCourses();
        }

        private bool PublishCourse(int courseId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE courses

                    SET status = 'PUBLISHED'

                    WHERE course_id = @courseId
                    AND status = 'PENDING'";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@courseId",
                    courseId
                );

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Read-only review of a pending course's chapters and materials.
        public string GetChapterDetailsHtml(object courseIdValue)
        {
            if (pendingChapters == null ||
                courseIdValue == null ||
                courseIdValue == DBNull.Value)
            {
                return NoChaptersHtml;
            }

            return RenderChapterDetails(
                pendingChapters,
                Convert.ToInt32(courseIdValue)
            );
        }

        private const string NoChaptersHtml =
            "<p class='detail-empty'>No chapters have been added to this course.</p>";

        private string RenderChapterDetails(
            DataTable chapters,
            int courseId)
        {
            const string none = NoChaptersHtml;

            StringBuilder sb = new StringBuilder();

            int lastChapterId = -1;
            int number = 0;
            bool listOpen = false;

            foreach (DataRow row in
                chapters.Select("course_id = " + courseId))
            {
                int chapterId = Convert.ToInt32(row["chapter_id"]);

                if (chapterId != lastChapterId)
                {
                    if (lastChapterId != -1)
                    {
                        sb.Append(listOpen ? "</ul></div>" : "</div>");
                    }

                    number++;
                    lastChapterId = chapterId;
                    listOpen = false;

                    sb.Append("<div class='detail-chapter'>");

                    sb.Append("<div class='detail-chapter-head'><span class='chapter-no'>Chapter ");
                    sb.Append(number);
                    sb.Append("</span><strong>");
                    sb.Append(HttpUtility.HtmlEncode(row["chapter_title"].ToString()));
                    sb.Append("</strong></div>");

                    sb.Append("<p>");
                    sb.Append(HttpUtility.HtmlEncode(OrDash(row["chapter_description"])));
                    sb.Append("</p>");
                }

                if (row["material_id"] == DBNull.Value)
                {
                    continue;
                }

                if (!listOpen)
                {
                    sb.Append("<ul class='detail-materials'>");
                    listOpen = true;
                }

                string type =
                    row["material_type"].ToString().ToUpperInvariant();

                string typeLabel = type == "VIDEO" ? "Video" : type;

                string typeClass =
                    type == "VIDEO"
                    ? "badge level-intermediate"
                    : "badge level-basic";

                sb.Append("<li><span class='");
                sb.Append(typeClass);
                sb.Append("'>");
                sb.Append(HttpUtility.HtmlEncode(typeLabel));
                sb.Append("</span> ");
                sb.Append(HttpUtility.HtmlEncode(row["material_title"].ToString()));

                string url =
                    row["file_url"] == DBNull.Value
                    ? ""
                    : row["file_url"].ToString();

                // Only same-site paths become links.
                if (url.StartsWith("/") && !url.StartsWith("//"))
                {
                    sb.Append(" <a href='");
                    sb.Append(HttpUtility.HtmlAttributeEncode(ResolveUrl("~" + url)));
                    sb.Append("' target='_blank' rel='noopener'>Open file</a>");
                }

                sb.Append("</li>");
            }

            if (number == 0)
            {
                return none;
            }

            sb.Append(listOpen ? "</ul></div>" : "</div>");

            return sb.ToString();
        }

        // ------------------------------------------------------------
        // View a course (read-only)
        // ------------------------------------------------------------

        private void ShowViewPanel(int courseId)
        {
            DataTable course = new DataTable();
            DataTable chapters = new DataTable();

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string courseQuery = @"
                    SELECT
                        c.course_id,
                        c.course_title,
                        c.description,
                        c.course_level,
                        c.status,
                        c.created_at,

                        COALESCE(
                            u.full_name,
                            u.username,
                            'Unknown User'
                        ) AS creator_name

                    FROM courses c

                    LEFT JOIN users u
                        ON c.created_by = u.user_id

                    WHERE c.course_id = @courseId";

                SqlDataAdapter courseAdapter =
                    new SqlDataAdapter(courseQuery, con);

                courseAdapter.SelectCommand.Parameters
                    .AddWithValue("@courseId", courseId);

                courseAdapter.Fill(course);

                string chaptersQuery = @"
                    SELECT
                        c.course_id,
                        c.chapter_id,
                        c.chapter_title,
                        c.description AS chapter_description,
                        m.material_id,
                        m.material_title,
                        m.material_type,
                        m.file_url

                    FROM course_chapters c

                    LEFT JOIN course_materials m
                        ON m.chapter_id = c.chapter_id

                    WHERE c.course_id = @courseId

                    ORDER BY
                        c.chapter_order,
                        c.chapter_id,
                        m.material_id";

                SqlDataAdapter chapterAdapter =
                    new SqlDataAdapter(chaptersQuery, con);

                chapterAdapter.SelectCommand.Parameters
                    .AddWithValue("@courseId", courseId);

                chapterAdapter.Fill(chapters);
            }

            if (course.Rows.Count == 0)
            {
                ShowMessage(
                    "The course could not be found.",
                    false
                );

                return;
            }

            DataRow row = course.Rows[0];

            string createdAt =
                row["created_at"] == DBNull.Value
                ? "-"
                : Convert.ToDateTime(row["created_at"]).ToString("dd MMM yyyy");

            StringBuilder sb = new StringBuilder();

            sb.Append("<h4>Course information</h4>");
            sb.Append("<div class='detail-grid'>");

            AppendField(sb, "COURSE TITLE", HttpUtility.HtmlEncode(row["course_title"].ToString()));
            AppendField(sb, "CREATED BY", HttpUtility.HtmlEncode(row["creator_name"].ToString()));

            AppendField(
                sb,
                "LEVEL",
                "<span class='" + GetLevelClass(row["course_level"]) + "'>" +
                HttpUtility.HtmlEncode(FormatText(row["course_level"])) + "</span>"
            );

            AppendField(
                sb,
                "STATUS",
                "<span class='" + GetStatusClass(row["status"]) + "'>" +
                HttpUtility.HtmlEncode(FormatText(row["status"])) + "</span>"
            );

            AppendField(sb, "CREATED", HttpUtility.HtmlEncode(createdAt));
            AppendField(sb, "COURSE ID", row["course_id"].ToString());

            AppendField(
                sb,
                "DESCRIPTION",
                HttpUtility.HtmlEncode(OrDash(row["description"])),
                true
            );

            sb.Append("</div>");

            sb.Append("<h4>Course details</h4>");
            sb.Append(RenderChapterDetails(chapters, courseId));

            litViewContent.Text = sb.ToString();
            pnlView.Visible = true;
        }

        private static void AppendField(
            StringBuilder sb,
            string label,
            string htmlValue,
            bool fullWidth = false)
        {
            sb.Append(fullWidth ? "<div class='detail-full'>" : "<div>");
            sb.Append("<span class='detail-label'>");
            sb.Append(label);
            sb.Append("</span><span class='detail-value'>");
            sb.Append(htmlValue);
            sb.Append("</span></div>");
        }

        public string OrDash(object value)
        {
            if (value == null ||
                value == DBNull.Value ||
                value.ToString().Trim().Length == 0)
            {
                return "-";
            }

            return value.ToString();
        }

        public string FormatText(object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return "-";
            }

            return CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(
                    value.ToString().ToLowerInvariant()
                );
        }

        public string GetLevelClass(object levelValue)
        {
            if (levelValue == null ||
                levelValue == DBNull.Value)
            {
                return "badge";
            }

            switch (levelValue.ToString().ToLowerInvariant())
            {
                case "advanced":
                    return "badge level-advanced";

                case "intermediate":
                    return "badge level-intermediate";

                default:
                    return "badge level-basic";
            }
        }

        public string GetStatusClass(object statusValue)
        {
            if (statusValue == null ||
                statusValue == DBNull.Value)
            {
                return "badge";
            }

            switch (statusValue.ToString().ToUpperInvariant())
            {
                case "PUBLISHED":
                    return "badge status-published";

                case "PENDING":
                    return "badge status-pending";

                default:
                    return "badge status-archived";
            }
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;

            lblMessage.Visible = true;

            ScrollToTop();

            if (success)
            {
                lblMessage.CssClass =
                    "feedback-message success-message";
            }
            else
            {
                lblMessage.CssClass =
                    "feedback-message error-message";
            }
        }
    }
}