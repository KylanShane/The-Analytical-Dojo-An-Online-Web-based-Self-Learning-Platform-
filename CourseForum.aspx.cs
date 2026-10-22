using System;
using System.Data;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class CourseForum : DojoPage
    {
        public int CourseId { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CourseId = QueryId("course");
            DataTable course = DojoData.Query(@"
                SELECT c.course_title
                FROM courses c
                INNER JOIN course_enrollments e ON e.course_id = c.course_id
                WHERE c.course_id = @courseId AND e.user_id = @userId
                    AND c.[status] <> 'ARCHIVED'",
                DojoData.P("@courseId", CourseId), DojoData.P("@userId", UserId));

            if (course.Rows.Count == 0)
                throw new HttpException(403, "Enrol in this course to open its forum.");

            CourseTitle.Text = H(course.Rows[0]["course_title"]);
            MarkForumRead();

            if (Request.HttpMethod == "POST" && Request.Form["deletePost"] != null)
            {
                int postId;
                if (!int.TryParse(Request.Form["deletePost"], out postId) || postId <= 0)
                    throw new HttpException(400, "Invalid discussion.");

                using (var connection = DojoDb.GetConnection())
                {
                    connection.Open();
                    using (var command = new System.Data.SqlClient.SqlCommand(@"
                        UPDATE forum_posts SET is_deleted = 1
                        WHERE post_id = @postId AND user_id = @userId
                            AND course_id = @courseId AND is_deleted = 0", connection))
                    {
                        command.Parameters.AddRange(new[] {
                            DojoData.P("@postId", postId), DojoData.P("@userId", UserId),
                            DojoData.P("@courseId", CourseId)
                        });
                        command.ExecuteNonQuery();
                    }
                }
                Response.Redirect("CourseForum.aspx?course=" + CourseId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            LoadFeed();
        }

        private void MarkForumRead()
        {
            using (var connection = DojoDb.GetConnection())
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(@"
                    UPDATE forum_notification_reads
                    SET last_seen_at = SYSDATETIME()
                    WHERE user_id = @userId AND course_id = @courseId", connection))
                {
                    command.Parameters.AddRange(new[] {
                        DojoData.P("@userId", UserId), DojoData.P("@courseId", CourseId)
                    });
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void Post_Click(object sender, EventArgs e)
        {
            string content = (PostContent.Text ?? string.Empty).Trim();
            if (content.Length < 8 || content.Length > 4000)
            {
                Status.Visible = true;
                Status.Text = "Write between 8 and 4,000 characters before publishing.";
                return;
            }

            using (var connection = DojoDb.GetConnection())
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(@"
                    INSERT INTO forum_posts (user_id, course_id, content)
                    SELECT @userId, @courseId, @content
                    WHERE EXISTS (
                        SELECT 1 FROM course_enrollments
                        WHERE user_id = @userId AND course_id = @courseId)", connection))
                {
                    command.Parameters.AddRange(new[] {
                        DojoData.P("@userId", UserId), DojoData.P("@courseId", CourseId),
                        DojoData.P("@content", content)
                    });
                    if (command.ExecuteNonQuery() != 1)
                        throw new HttpException(403, "Enrol in this course before posting.");
                }
            }

            Response.Redirect("CourseForum.aspx?course=" + CourseId + "&posted=1#discussion-heading", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadFeed()
        {
            DataTable posts = DojoData.Query(@"
                SELECT p.post_id, p.content, p.created_at, p.user_id, u.full_name
                FROM forum_posts p
                INNER JOIN users u ON u.user_id = p.user_id
                WHERE p.course_id = @courseId AND p.is_deleted = 0
                ORDER BY p.created_at DESC, p.post_id DESC",
                DojoData.P("@courseId", CourseId));

            PostCount.Text = "<span class='forum-count'>" + posts.Rows.Count + "</span>";
            if (Request.QueryString["posted"] == "1")
            {
                Status.Visible = true;
                Status.Text = "Your discussion is live. Nice work starting the conversation!";
            }
            if (posts.Rows.Count == 0)
            {
                Feed.Text = "<div class='panel forum-empty'><span aria-hidden='true'>✳</span><h3>Start the conversation</h3><p>No discussions yet. Share a question or an idea from this course.</p><a class='btn' href='#new-post'>Write the first post →</a></div>";
                return;
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow post in posts.Rows)
            {
                string name = Convert.ToString(post["full_name"]);
                string initial = string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();
                html.Append("<article class='panel forum-post' data-forum-post><div class='forum-post-top'><span class='forum-avatar' aria-hidden='true'>");
                html.Append(H(initial));
                html.Append("</span><div><strong>");
                html.Append(H(name));
                html.Append("</strong><small>");
                html.Append(H(Convert.ToDateTime(post["created_at"]).ToString("dd MMM yyyy · HH:mm")));
                html.Append("</small></div><span class='forum-post-index'>DISCUSSION #");
                html.Append(H(post["post_id"]));
                html.Append("</span></div><p>");
                html.Append(H(post["content"]).Replace("\r\n", "<br />").Replace("\n", "<br />"));
                html.Append("</p>");
                if (Convert.ToInt32(post["user_id"]) == UserId)
                {
                    html.Append("<div class='forum-post-actions'><button class='forum-remove' type='submit' name='deletePost' value='");
                    html.Append(H(post["post_id"]));
                    html.Append("' onclick=\"return confirm('Remove this discussion?');\">Remove post</button></div>");
                }
                html.Append("</article>");
            }
            Feed.Text = html.ToString();
        }
    }
}
