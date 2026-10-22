using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace The_Analytical_Dojo
{
    public partial class SiteMaster : MasterPage
    {
        public int UnreadCount { get; private set; }
        public string NotificationHtml { get; private set; }
        public string CourseSearchJson { get; private set; }

        public string Initials
        {
            get
            {
                string name = Convert.ToString(Session["FullName"]);
                if (string.IsNullOrWhiteSpace(name)) name = Convert.ToString(Session["Username"]);
                if (string.IsNullOrWhiteSpace(name)) return "ME";
                string[] parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
                return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            NotificationHtml = "<div class='notification-empty'><span>✓</span><strong>You're all caught up</strong><small>New course discussions will appear here.</small></div>";
            CourseSearchJson = "[]";
            if (Session["UserId"] == null) return;
            int userId = Convert.ToInt32(Session["UserId"]);
            EnsureForumReads(userId);
            LoadCourseSearch(userId);
            LoadNotifications(userId);
        }

        private static void EnsureForumReads(int userId)
        {
            using (var connection = DojoDb.GetConnection())
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(@"
                    IF OBJECT_ID(N'forum_notification_reads', N'U') IS NULL
                    BEGIN
                        CREATE TABLE forum_notification_reads (
                            user_id INT NOT NULL, course_id INT NOT NULL, last_seen_at DATETIME2 NOT NULL,
                            CONSTRAINT PK_forum_notification_reads PRIMARY KEY (user_id, course_id),
                            CONSTRAINT FK_forum_read_user FOREIGN KEY (user_id) REFERENCES users(user_id),
                            CONSTRAINT FK_forum_read_course FOREIGN KEY (course_id) REFERENCES courses(course_id)
                        );
                    END;
                    INSERT INTO forum_notification_reads (user_id, course_id, last_seen_at)
                    SELECT @userId, e.course_id, SYSDATETIME() FROM course_enrollments e
                    WHERE e.user_id = @userId AND NOT EXISTS (
                        SELECT 1 FROM forum_notification_reads r WHERE r.user_id = @userId AND r.course_id = e.course_id);", connection))
                {
                    command.Parameters.Add(DojoData.P("@userId", userId));
                    command.ExecuteNonQuery();
                }
            }
        }

        private void LoadCourseSearch(int userId)
        {
            DataTable courses = DojoData.Query(@"
                SELECT c.course_id AS id, c.course_title AS title, c.course_level AS level,
                       CASE WHEN e.enrollment_id IS NULL THEN 0 ELSE 1 END AS enrolled
                FROM courses c LEFT JOIN course_enrollments e ON e.course_id = c.course_id AND e.user_id = @userId
                WHERE c.[status] <> 'ARCHIVED'
                ORDER BY CASE WHEN e.enrollment_id IS NULL THEN 1 ELSE 0 END, c.course_title", DojoData.P("@userId", userId));
            var rows = new System.Collections.Generic.List<object>();
            foreach (DataRow row in courses.Rows)
                rows.Add(new { id = row["id"], title = row["title"], level = row["level"], enrolled = Convert.ToInt32(row["enrolled"]) == 1 });
            CourseSearchJson = new JavaScriptSerializer().Serialize(rows).Replace("</", "<\\/");
        }

        private void LoadNotifications(int userId)
        {
            DataTable notices = DojoData.Query(@"
                SELECT TOP 8 p.post_id, p.course_id, p.content, p.created_at, u.full_name, c.course_title
                FROM forum_posts p JOIN users u ON u.user_id = p.user_id JOIN courses c ON c.course_id = p.course_id
                JOIN course_enrollments e ON e.course_id = p.course_id AND e.user_id = @userId
                JOIN forum_notification_reads r ON r.user_id = @userId AND r.course_id = p.course_id
                WHERE p.user_id <> @userId AND p.is_deleted = 0 AND p.created_at > r.last_seen_at
                ORDER BY p.created_at DESC, p.post_id DESC", DojoData.P("@userId", userId));
            UnreadCount = notices.Rows.Count;
            if (UnreadCount == 0) return;
            StringBuilder html = new StringBuilder("<div class='notification-list'>");
            foreach (DataRow row in notices.Rows)
            {
                string name = Convert.ToString(row["full_name"]);
                string content = Convert.ToString(row["content"]);
                if (content.Length > 82) content = content.Substring(0, 82) + "…";
                string initial = string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();
                html.Append("<a class='notification-item' href='CourseForum.aspx?course=").Append(HttpUtility.HtmlEncode(row["course_id"])).Append("'><span class='notification-avatar'>")
                    .Append(HttpUtility.HtmlEncode(initial)).Append("</span><span><strong>").Append(HttpUtility.HtmlEncode(name)).Append(" posted in ")
                    .Append(HttpUtility.HtmlEncode(row["course_title"])).Append("</strong><small>").Append(HttpUtility.HtmlEncode(content)).Append("</small></span></a>");
            }
            NotificationHtml = html.Append("</div>").ToString();
        }
    }
}
