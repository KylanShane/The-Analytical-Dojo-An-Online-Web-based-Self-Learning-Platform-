using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    // Create / edit the course record only (title, description, level, status).
    // Chapters and materials are managed from the Materials menu.
    public partial class AdminCreateCourse : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        // 0 = creating a new course, otherwise the course being edited.
        private int EditCourseId
        {
            get
            {
                int id;

                return int.TryParse(Request.QueryString["id"], out id) && id > 0
                    ? id
                    : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EditCourseId > 0)
            {
                Page.Title = "Edit Course - The Analytical Dojo";
                litBreadcrumb.Text = "Edit Course";
                litHeading.Text = "Edit Course";
                litSubtitle.Text = "Update the course information.";
                litActionNote.Text = "EDITING COURSE #" + EditCourseId + " &middot; UNSAVED CHANGES";
                btnCreate.Text = "Save Changes";
            }

            if (!IsPostBack && EditCourseId > 0)
            {
                LoadCourseForEdit(EditCourseId);
            }
        }

        private void LoadCourseForEdit(int courseId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        course_title,
                        description,
                        course_level,
                        status

                    FROM courses

                    WHERE course_id = @courseId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@courseId", courseId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Response.Redirect("AdminCourse.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    txtTitle.Text = reader["course_title"].ToString();

                    txtDescription.Text =
                        reader["description"] == DBNull.Value
                        ? ""
                        : reader["description"].ToString();

                    string level =
                        reader["course_level"].ToString().ToLowerInvariant();

                    string status =
                        reader["status"].ToString().ToUpperInvariant();

                    ListItem levelItem = ddlLevel.Items.FindByValue(level);

                    if (levelItem != null)
                    {
                        ddlLevel.ClearSelection();
                        levelItem.Selected = true;
                    }

                    // Courses can also be archived, which the create page does not offer.
                    if (rblStatus.Items.FindByValue("ARCHIVED") == null)
                    {
                        rblStatus.Items.Add(new ListItem("Archived", "ARCHIVED"));
                    }

                    ListItem statusItem = rblStatus.Items.FindByValue(status);

                    if (statusItem == null)
                    {
                        statusItem = new ListItem(status, status);
                        rblStatus.Items.Add(statusItem);
                    }

                    rblStatus.ClearSelection();
                    statusItem.Selected = true;
                }
            }
        }

        protected void btnCreate_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int courseId = EditCourseId;
            bool isEdit = courseId > 0;

            int creatorId = 0;

            if (!isEdit)
            {
                creatorId = GetCreatorId();

                if (creatorId == 0)
                {
                    ShowMessage(
                        "No administrator account could be found to own this course.",
                        false
                    );

                    return;
                }
            }

            try
            {
                if (isEdit)
                {
                    if (!UpdateCourse(courseId))
                    {
                        ShowMessage("This course no longer exists.", false);
                        return;
                    }
                }
                else
                {
                    InsertCourse(creatorId);
                }

                Response.Redirect(
                    isEdit ? "AdminCourse.aspx?updated=1" : "AdminCourse.aspx?created=1",
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException)
            {
                ShowMessage(
                    "The course could not be saved. Please check the details and try again.",
                    false
                );
            }
        }

        private bool UpdateCourse(int courseId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE courses

                    SET
                        course_title = @title,
                        description = @description,
                        course_level = @level,
                        status = @status

                    WHERE course_id = @courseId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@level", ddlLevel.SelectedValue);
                cmd.Parameters.AddWithValue("@status", rblStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // IDs continue from the highest existing value (e.g. 8 -> 9) instead of
        // the identity counter, which can jump (e.g. to 1002).
        private void InsertCourse(int creatorId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int courseId;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                "SELECT ISNULL(MAX(course_id), 0) + 1 FROM courses WITH (UPDLOCK, HOLDLOCK)",
                                con, tx))
                        {
                            courseId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string query = @"
                            SET IDENTITY_INSERT courses ON;

                            BEGIN TRY
                                INSERT INTO courses
                                (
                                    course_id,
                                    course_title,
                                    description,
                                    course_level,
                                    status,
                                    created_by,
                                    created_at
                                )
                                VALUES
                                (
                                    @id,
                                    @title,
                                    @description,
                                    @level,
                                    @status,
                                    @createdBy,
                                    GETDATE()
                                );
                            END TRY
                            BEGIN CATCH
                                SET IDENTITY_INSERT courses OFF;
                                THROW;
                            END CATCH;

                            SET IDENTITY_INSERT courses OFF;";

                        using (SqlCommand cmd =
                            new SqlCommand(query, con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", courseId);
                            cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                            cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                            cmd.Parameters.AddWithValue("@level", ddlLevel.SelectedValue);
                            cmd.Parameters.AddWithValue("@status", rblStatus.SelectedValue);
                            cmd.Parameters.AddWithValue("@createdBy", creatorId);

                            cmd.ExecuteNonQuery();
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
        }

        private int GetCreatorId()
        {
            // Use the logged-in admin when the login system exists.
            if (Session["user_id"] != null)
            {
                return Convert.ToInt32(Session["user_id"]);
            }

            // Until login is connected, fall back to the first admin account.
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 1 user_id
                    FROM users
                    WHERE role = 'ADMIN'
                    ORDER BY user_id ASC";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? 0
                    : Convert.ToInt32(result);
            }
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;

            lblMessage.Visible = true;

            lblMessage.CssClass = success
                ? "feedback-message success-message"
                : "feedback-message error-message";

            ClientScript.RegisterStartupScript(
                GetType(),
                "scrollTop",
                "window.scrollTo({ top: 0, behavior: 'smooth' });",
                true
            );
        }
    }
}