using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminForum : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCourses();
                LoadStatistics();
                LoadForumContent();
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        course_id,
                        course_title
                    FROM courses
                    ORDER BY course_title";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                ddlCourse.Items.Clear();

                ddlCourse.Items.Add(
                    new ListItem(
                        "All Courses",
                        ""
                    )
                );

                while (reader.Read())
                {
                    ddlCourse.Items.Add(
                        new ListItem(
                            reader["course_title"].ToString(),
                            reader["course_id"].ToString()
                        )
                    );
                }
            }
        }

        private void LoadStatistics()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        (
                            SELECT COUNT(*)
                            FROM forum_posts
                            WHERE is_deleted = 0
                        ) AS ActivePosts,

                        (
                            SELECT COUNT(*)
                            FROM forum_replies
                            WHERE is_deleted = 0
                        ) AS ActiveReplies,

                        (
                            SELECT COUNT(*)
                            FROM forum_posts
                            WHERE is_deleted = 1
                        ) AS DeletedPosts,

                        (
                            SELECT COUNT(*)
                            FROM forum_replies
                            WHERE is_deleted = 1
                        ) AS DeletedReplies";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblActivePosts.Text =
                        reader["ActivePosts"].ToString();

                    lblActiveReplies.Text =
                        reader["ActiveReplies"].ToString();

                    lblDeletedPosts.Text =
                        reader["DeletedPosts"].ToString();

                    lblDeletedReplies.Text =
                        reader["DeletedReplies"].ToString();
                }
            }
        }

        private void LoadForumContent()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        content_id,
                        content_type,
                        user_id,
                        course_id,
                        content,
                        created_at,
                        is_deleted,
                        full_name,
                        username,
                        course_title

                    FROM
                    (
                        SELECT
                            p.post_id AS content_id,
                            'POST' AS content_type,
                            p.user_id,
                            p.course_id,
                            p.content,
                            p.created_at,
                            p.is_deleted,

                            COALESCE(
                                NULLIF(u.full_name, ''),
                                u.username,
                                'Unknown User'
                            ) AS full_name,

                            COALESCE(
                                NULLIF(u.username, ''),
                                'unknown'
                            ) AS username,

                            c.course_title

                        FROM forum_posts p

                        INNER JOIN users u
                            ON p.user_id = u.user_id

                        INNER JOIN courses c
                            ON p.course_id = c.course_id

                        UNION ALL

                        SELECT
                            r.reply_id AS content_id,
                            'REPLY' AS content_type,
                            r.user_id,
                            p.course_id,
                            r.content,
                            r.created_at,
                            r.is_deleted,

                            COALESCE(
                                NULLIF(u.full_name, ''),
                                u.username,
                                'Unknown User'
                            ) AS full_name,

                            COALESCE(
                                NULLIF(u.username, ''),
                                'unknown'
                            ) AS username,

                            c.course_title

                        FROM forum_replies r

                        INNER JOIN forum_posts p
                            ON r.post_id = p.post_id

                        INNER JOIN users u
                            ON r.user_id = u.user_id

                        INNER JOIN courses c
                            ON p.course_id = c.course_id

                    ) AS ForumContent

                    WHERE
                        (
                            @search = ''
                            OR content LIKE '%' + @search + '%'
                            OR full_name LIKE '%' + @search + '%'
                            OR username LIKE '%' + @search + '%'
                            OR course_title LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @courseId = ''
                            OR CONVERT(NVARCHAR(20), course_id) = @courseId
                        )

                        AND
                        (
                            @type = ''
                            OR content_type = @type
                        )

                        AND
                        (
                            @status = ''

                            OR
                            (
                                @status = 'ACTIVE'
                                AND is_deleted = 0
                            )

                            OR
                            (
                                @status = 'DELETED'
                                AND is_deleted = 1
                            )
                        )

                    ORDER BY
                        created_at DESC,
                        content_id DESC";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@search",
                    txtSearch.Text.Trim()
                );

                cmd.Parameters.AddWithValue(
                    "@courseId",
                    ddlCourse.SelectedValue
                );

                cmd.Parameters.AddWithValue(
                    "@type",
                    ddlType.SelectedValue
                );

                cmd.Parameters.AddWithValue(
                    "@status",
                    ddlStatus.SelectedValue
                );

                SqlDataAdapter adapter =
                    new SqlDataAdapter(cmd);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                gvForum.DataSource =
                    dt;

                gvForum.DataBind();
            }
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            lblMessage.Visible = false;

            LoadForumContent();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";

            ddlCourse.SelectedIndex = 0;
            ddlType.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;

            lblMessage.Visible = false;

            LoadForumContent();
        }

        protected void gvForum_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteContent" &&
                e.CommandName != "RestoreContent")
            {
                return;
            }

            string argument =
                Convert.ToString(e.CommandArgument);

            if (string.IsNullOrWhiteSpace(argument))
            {
                ShowMessage(
                    "The selected forum content could not be identified.",
                    false
                );

                return;
            }

            string[] values =
                argument.Split('|');

            if (values.Length != 2)
            {
                ShowMessage(
                    "The selected forum content could not be identified.",
                    false
                );

                return;
            }

            string contentType =
                values[0].Trim().ToUpperInvariant();

            int contentId;

            if (!int.TryParse(
                values[1],
                out contentId))
            {
                ShowMessage(
                    "The selected forum content could not be identified.",
                    false
                );

                return;
            }

            bool isDelete =
                e.CommandName == "DeleteContent";

            bool updated =
                UpdateContentStatus(
                    contentType,
                    contentId,
                    isDelete
                );

            if (updated)
            {
                if (isDelete)
                {
                    ShowMessage(
                        contentType == "POST"
                            ? "Forum post removed successfully."
                            : "Forum reply removed successfully.",
                        true
                    );
                }
                else
                {
                    ShowMessage(
                        contentType == "POST"
                            ? "Forum post restored successfully."
                            : "Forum reply restored successfully.",
                        true
                    );
                }
            }
            else
            {
                ShowMessage(
                    "No forum content was updated.",
                    false
                );
            }

            LoadStatistics();
            LoadForumContent();
        }

        private bool UpdateContentStatus(
            string contentType,
            int contentId,
            bool isDeleted)
        {
            string query;

            if (contentType == "POST")
            {
                query = @"
                    UPDATE forum_posts

                    SET is_deleted = @isDeleted

                    WHERE post_id = @contentId";
            }
            else if (contentType == "REPLY")
            {
                query = @"
                    UPDATE forum_replies

                    SET is_deleted = @isDeleted

                    WHERE reply_id = @contentId";
            }
            else
            {
                return false;
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.Add(
                    "@isDeleted",
                    SqlDbType.Bit
                ).Value = isDeleted;

                cmd.Parameters.Add(
                    "@contentId",
                    SqlDbType.Int
                ).Value = contentId;

                con.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool IsDeleted(
            object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return false;
            }

            return Convert.ToBoolean(value);
        }

        public string FormatDate(
            object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return "-";
            }

            DateTime date =
                Convert.ToDateTime(value);

            return date.ToString(
                "dd MMM yyyy, hh:mm tt"
            );
        }

        public string GetTypeClass(
            object value)
        {
            string type =
                Convert.ToString(value);

            if (string.Equals(
                type,
                "POST",
                StringComparison.OrdinalIgnoreCase))
            {
                return "type-badge type-post";
            }

            return "type-badge type-reply";
        }

        public string GetStatusText(
            object value)
        {
            return IsDeleted(value)
                ? "REMOVED"
                : "ACTIVE";
        }

        public string GetStatusClass(
            object value)
        {
            return IsDeleted(value)
                ? "status-badge status-deleted"
                : "status-badge status-active";
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text =
                message;

            lblMessage.Visible =
                true;

            lblMessage.CssClass =
                success
                ? "feedback-message success-message"
                : "feedback-message error-message";
        }
    }
}