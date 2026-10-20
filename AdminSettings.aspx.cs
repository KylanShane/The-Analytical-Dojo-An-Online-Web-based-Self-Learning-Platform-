using System;
using System.Configuration;
using System.Data.SqlClient;

namespace The_Analytical_Dojo
{
    public partial class AdminSettings : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["user_id"] != null)
                {
                    LoadAdminProfile();
                }
                else
                {
                    DisableSettings();

                    ShowMessage(
                        "Login has not been set up yet. Account settings will become available after the login system is connected.",
                        false
                    );
                }
            }
        }

        private void LoadAdminProfile()
        {
            int userId =
                Convert.ToInt32(
                    Session["user_id"]
                );

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        user_id,
                        username,
                        full_name,
                        email,
                        role,
                        created_at

                    FROM users

                    WHERE user_id = @userId
                    AND role = 'ADMIN'";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@userId",
                    userId
                );

                con.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtUsername.Text =
                        reader["username"].ToString();

                    txtFullName.Text =
                        reader["full_name"].ToString();

                    txtEmail.Text =
                        reader["email"].ToString();

                    txtRole.Text =
                        reader["role"].ToString();

                    if (reader["created_at"] != DBNull.Value)
                    {
                        DateTime createdAt =
                            Convert.ToDateTime(
                                reader["created_at"]
                            );

                        txtCreatedAt.Text =
                            createdAt.ToString(
                                "dd MMM yyyy"
                            );
                    }
                }
                else
                {
                    DisableSettings();

                    ShowMessage(
                        "Administrator account could not be found.",
                        false
                    );
                }
            }
        }

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            if (Session["user_id"] == null)
            {
                ShowMessage(
                    "You must be logged in before updating account settings.",
                    false
                );

                return;
            }

            int userId =
                Convert.ToInt32(
                    Session["user_id"]
                );

            string fullName =
                txtFullName.Text.Trim();

            string email =
                txtEmail.Text.Trim();

            if (EmailBelongsToAnotherUser(
                email,
                userId))
            {
                ShowMessage(
                    "That email address is already being used by another account.",
                    false
                );

                return;
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE users

                    SET
                        full_name = @fullName,
                        email = @email

                    WHERE user_id = @userId
                    AND role = 'ADMIN'";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@fullName",
                    fullName
                );

                cmd.Parameters.AddWithValue(
                    "@email",
                    email
                );

                cmd.Parameters.AddWithValue(
                    "@userId",
                    userId
                );

                con.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    ShowMessage(
                        "Account information updated successfully.",
                        true
                    );

                    LoadAdminProfile();
                }
                else
                {
                    ShowMessage(
                        "Account information could not be updated.",
                        false
                    );
                }
            }
        }

        private bool EmailBelongsToAnotherUser(
            string email,
            int currentUserId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(*)

                    FROM users

                    WHERE email = @email
                    AND user_id <> @userId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@email",
                    email
                );

                cmd.Parameters.AddWithValue(
                    "@userId",
                    currentUserId
                );

                con.Open();

                int count =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );

                return count > 0;
            }
        }

        private void DisableSettings()
        {
            txtUsername.Enabled = false;
            txtFullName.Enabled = false;
            txtEmail.Enabled = false;
            txtRole.Enabled = false;
            txtCreatedAt.Enabled = false;
            btnSave.Enabled = false;
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text =
                message;

            lblMessage.Visible =
                true;

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