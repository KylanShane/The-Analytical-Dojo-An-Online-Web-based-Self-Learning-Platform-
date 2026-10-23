using System;
using System.Collections.Generic;
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

            if (Request.HttpMethod == "POST" && Request.Form["deleteReply"] != null)
            {
                int replyId;
                if (!int.TryParse(Request.Form["deleteReply"], out replyId) || replyId <= 0)
                    throw new HttpException(400, "Invalid reply.");

                // Only the author can remove a reply, and only inside this course.
                DojoData.Execute(@"
                    UPDATE r SET is_deleted = 1
                    FROM forum_replies r
                    INNER JOIN forum_posts p ON p.post_id = r.post_id
                    WHERE r.reply_id = @replyId AND r.user_id = @userId
                        AND p.course_id = @courseId AND r.is_deleted = 0",
                    DojoData.P("@replyId", replyId), DojoData.P("@userId", UserId),
                    DojoData.P("@courseId", CourseId));

                Response.Redirect("CourseForum.aspx?course=" + CourseId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Request.HttpMethod == "POST" && Request.Form["sendReply"] != null)
            {
                SaveReply();
                return;
            }

            LoadFeed();
        }

        private void SaveReply()
        {
            int postId;
            int parentId = 0;
            if (!int.TryParse(Request.Form["replyPost"], out postId) || postId <= 0)
                throw new HttpException(400, "Invalid discussion.");
            if (!string.IsNullOrEmpty(Request.Form["replyParent"]) &&
                (!int.TryParse(Request.Form["replyParent"], out parentId) || parentId < 0))
                throw new HttpException(400, "Invalid reply.");

            string content = (Request.Form["replyContent"] ?? string.Empty).Trim();
            if (content.Length < 2 || content.Length > 2000)
            {
                Status.Visible = true;
                Status.Text = "Write between 2 and 2,000 characters before replying.";
                LoadFeed();
                return;
            }

            // The post must be live in this course, the writer enrolled, and a
            // parent reply (when given) must belong to the same post.
            object newId = DojoData.Scalar(@"
                INSERT INTO forum_replies (post_id, user_id, parent_reply_id, content)
                OUTPUT INSERTED.reply_id
                SELECT @postId, @userId, @parentId, @content
                WHERE EXISTS (
                        SELECT 1 FROM forum_posts p
                        INNER JOIN course_enrollments e
                            ON e.course_id = p.course_id AND e.user_id = @userId
                        WHERE p.post_id = @postId AND p.course_id = @courseId
                            AND p.is_deleted = 0)
                    AND (@parentId IS NULL OR EXISTS (
                        SELECT 1 FROM forum_replies r
                        WHERE r.reply_id = @parentId AND r.post_id = @postId
                            AND r.is_deleted = 0))",
                DojoData.P("@postId", postId), DojoData.P("@userId", UserId),
                new System.Data.SqlClient.SqlParameter("@parentId", SqlDbType.Int)
                {
                    Value = parentId > 0 ? (object)parentId : DBNull.Value
                },
                DojoData.P("@content", content), DojoData.P("@courseId", CourseId));

            if (newId == null || newId == DBNull.Value)
                throw new HttpException(403, "This discussion is no longer open for replies.");

            Response.Redirect("CourseForum.aspx?course=" + CourseId + "#reply-" + newId, false);
            Context.ApplicationInstance.CompleteRequest();
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

        // Replies deeper than this are listed flat under their level-3 ancestor,
        // labelled with who they answer, so threads stay readable.
        private const int MaxReplyDepth = 3;

        private Dictionary<object, List<DataRow>> replyChildren;
        private Dictionary<int, string> replyAuthors;

        private void LoadFeed()
        {
            DataTable posts = DojoData.Query(@"
                SELECT p.post_id, p.content, p.created_at, p.user_id, u.full_name, u.[role]
                FROM forum_posts p
                INNER JOIN users u ON u.user_id = p.user_id
                WHERE p.course_id = @courseId AND p.is_deleted = 0
                ORDER BY p.created_at DESC, p.post_id DESC",
                DojoData.P("@courseId", CourseId));

            DataTable replies = DojoData.Query(@"
                SELECT r.reply_id, r.post_id, r.parent_reply_id, r.content, r.created_at,
                    r.user_id, r.is_deleted, u.full_name, u.[role]
                FROM forum_replies r
                INNER JOIN forum_posts p ON p.post_id = r.post_id
                INNER JOIN users u ON u.user_id = r.user_id
                WHERE p.course_id = @courseId AND p.is_deleted = 0
                ORDER BY r.created_at, r.reply_id",
                DojoData.P("@courseId", CourseId));

            // Children keyed by parent reply id, or "p{postId}" for top-level replies.
            replyChildren = new Dictionary<object, List<DataRow>>();
            replyAuthors = new Dictionary<int, string>();
            foreach (DataRow reply in replies.Rows)
            {
                object key = reply["parent_reply_id"] == DBNull.Value
                    ? (object)("p" + reply["post_id"])
                    : Convert.ToInt32(reply["parent_reply_id"]);
                List<DataRow> list;
                if (!replyChildren.TryGetValue(key, out list))
                {
                    list = new List<DataRow>();
                    replyChildren[key] = list;
                }
                list.Add(reply);
                replyAuthors[Convert.ToInt32(reply["reply_id"])] = Convert.ToString(reply["full_name"]);
            }

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
                int postId = Convert.ToInt32(post["post_id"]);
                string name = Convert.ToString(post["full_name"]);
                int replyCount = CountLive("p" + postId);

                html.Append("<article class='panel forum-post' data-forum-post id='post-");
                html.Append(postId);
                html.Append("'><div class='forum-post-top'>");
                html.Append(Avatar(name, post["role"], "forum-avatar"));
                html.Append("<div>");
                html.Append(AuthorLine(name, post["role"], Convert.ToInt32(post["user_id"])));
                html.Append("<small>");
                html.Append(H(Convert.ToDateTime(post["created_at"]).ToString("dd MMM yyyy · HH:mm")));
                html.Append("</small></div><span class='forum-post-index'>DISCUSSION #");
                html.Append(postId);
                html.Append("</span></div><p>");
                html.Append(Paragraphs(post["content"]));
                html.Append("</p><div class='forum-post-actions'>");
                html.Append(ReplyButton(postId, 0, name));

                if (replyCount > 0)
                {
                    html.Append("<button type='button' class='forum-action' data-thread-toggle aria-expanded='true'>");
                    html.Append("<span aria-hidden='true'>💬</span> ");
                    html.Append(replyCount);
                    html.Append(replyCount == 1 ? " reply" : " replies");
                    html.Append("</button>");
                }

                if (Convert.ToInt32(post["user_id"]) == UserId)
                {
                    html.Append("<button class='forum-remove' type='submit' name='deletePost' value='");
                    html.Append(postId);
                    html.Append("' onclick=\"return confirm('Remove this discussion?');\">Remove post</button>");
                }

                html.Append("</div>");

                if (replyCount > 0)
                {
                    html.Append("<div class='forum-thread' data-thread><ol class='thread-list'>");
                    RenderReplies(html, "p" + postId, 1);
                    html.Append("</ol></div>");
                }

                html.Append("</article>");
            }
            Feed.Text = html.ToString();
        }

        private void RenderReplies(StringBuilder html, object parentKey, int depth)
        {
            List<DataRow> children;
            if (!replyChildren.TryGetValue(parentKey, out children))
            {
                return;
            }

            foreach (DataRow reply in children)
            {
                int replyId = Convert.ToInt32(reply["reply_id"]);
                bool deleted = Convert.ToBoolean(reply["is_deleted"]);

                // Hide removed replies unless they still anchor live answers.
                if (deleted && CountLive(replyId) == 0)
                {
                    continue;
                }

                html.Append("<li class='forum-reply' id='reply-");
                html.Append(replyId);
                html.Append("'><div class='reply-body'>");

                if (deleted)
                {
                    html.Append("<p class='reply-removed'>This reply was removed.</p>");
                }
                else
                {
                    string name = Convert.ToString(reply["full_name"]);
                    html.Append("<div class='reply-head'>");
                    html.Append(Avatar(name, reply["role"], "reply-avatar"));
                    html.Append(AuthorLine(name, reply["role"], Convert.ToInt32(reply["user_id"])));
                    html.Append("<small>");
                    html.Append(H(Convert.ToDateTime(reply["created_at"]).ToString("dd MMM · HH:mm")));
                    html.Append("</small></div>");

                    // Flattened deep replies say who they are answering.
                    if (depth > MaxReplyDepth && reply["parent_reply_id"] != DBNull.Value)
                    {
                        string parentName;
                        if (replyAuthors.TryGetValue(Convert.ToInt32(reply["parent_reply_id"]), out parentName))
                        {
                            html.Append("<span class='reply-to'>↪ replying to ");
                            html.Append(H(parentName));
                            html.Append("</span>");
                        }
                    }

                    html.Append("<p>");
                    html.Append(Paragraphs(reply["content"]));
                    html.Append("</p><div class='reply-actions'>");
                    html.Append(ReplyButton(Convert.ToInt32(reply["post_id"]), replyId, name));

                    if (Convert.ToInt32(reply["user_id"]) == UserId)
                    {
                        html.Append("<button class='forum-remove' type='submit' name='deleteReply' value='");
                        html.Append(replyId);
                        html.Append("' onclick=\"return confirm('Remove this reply?');\">Remove</button>");
                    }

                    html.Append("</div>");
                }

                html.Append("</div>");

                if (depth < MaxReplyDepth)
                {
                    if (CountLive(replyId) > 0)
                    {
                        html.Append("<ol class='thread-list'>");
                        RenderReplies(html, replyId, depth + 1);
                        html.Append("</ol>");
                    }
                    html.Append("</li>");
                }
                else
                {
                    html.Append("</li>");
                    RenderReplies(html, replyId, depth + 1);
                }
            }
        }

        // Live (not removed) replies anywhere under a post or reply.
        private int CountLive(object key)
        {
            List<DataRow> children;
            if (!replyChildren.TryGetValue(key, out children))
            {
                return 0;
            }

            int total = 0;
            foreach (DataRow child in children)
            {
                if (!Convert.ToBoolean(child["is_deleted"]))
                {
                    total++;
                }
                total += CountLive(Convert.ToInt32(child["reply_id"]));
            }
            return total;
        }

        private string AuthorLine(string name, object role, int authorId)
        {
            return "<span class='forum-author'><strong>" + H(name) + "</strong>" +
                DojoUi.RolePill(role) +
                (authorId == UserId ? "<span class='role-pill role-you'>You</span>" : "") +
                "</span>";
        }

        private static string Avatar(string name, object role, string cssClass)
        {
            string initial = string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();
            string tone = Convert.ToString(role) == "ADMIN" ? " is-admin"
                : Convert.ToString(role) == "PEER LEADER" ? " is-peer" : "";
            return "<span class='" + cssClass + tone + "' aria-hidden='true'>" + H(initial) + "</span>";
        }

        private static string ReplyButton(int postId, int parentId, string name)
        {
            return "<button type='button' class='forum-action' data-reply-to data-post='" + postId +
                "' data-parent='" + (parentId > 0 ? parentId.ToString() : "") +
                "' data-name='" + H(name) + "'><span aria-hidden='true'>↩</span> Reply</button>";
        }

        private static string Paragraphs(object content)
        {
            return H(content).Replace("\r\n", "<br />").Replace("\n", "<br />");
        }
    }
}
