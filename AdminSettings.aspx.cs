using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace The_Analytical_Dojo
{
    public partial class AdminSettings : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            LoadAdminProfile(userId);
        }

        // ------------------------------------------------------------
        // Who is logged in?
        // ------------------------------------------------------------

        // Login.aspx.cs stores the user id in Session["UserId"].
        private int GetLoggedInUserId()
        {
            int id;

            if (Session["UserId"] != null &&
                int.TryParse(Convert.ToString(Session["UserId"]), out id) &&
                id > 0)
            {
                return id;
            }

            return 0;
        }

        // ------------------------------------------------------------
        // Profile
        // ------------------------------------------------------------

        private void LoadAdminProfile(int userId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        username,
                        full_name,
                        email,
                        role,
                        created_at

                    FROM users

                    WHERE user_id = @userId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userId", userId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        DisableSettings();

                        ShowMessage(
                            "Your account could not be found.",
                            false
                        );

                        return;
                    }

                    string role = reader["role"].ToString();

                    if (!role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                    {
                        DisableSettings();

                        ShowMessage(
                            "This page is only available to administrator accounts.",
                            false
                        );

                        return;
                    }

                    string username = reader["username"].ToString();

                    string fullName =
                        reader["full_name"] == DBNull.Value
                        ? ""
                        : reader["full_name"].ToString();

                    txtUsername.Text = username;
                    txtRole.Text = role;
                    txtFullName.Text = fullName;

                    txtEmail.Text =
                        reader["email"] == DBNull.Value
                        ? ""
                        : reader["email"].ToString();

                    txtCreatedAt.Text =
                        reader["created_at"] == DBNull.Value
                        ? "-"
                        : Convert.ToDateTime(reader["created_at"]).ToString("dd MMM yyyy");

                    string nameForInitial =
                        fullName.Trim().Length > 0 ? fullName.Trim() : username;

                    lblInitial.Text =
                        nameForInitial.Substring(0, 1).ToUpperInvariant();
                }
            }
        }

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (fullName.Length == 0)
            {
                ShowMessage("Full name is required.", false);
                return;
            }

            if (email.Length == 0)
            {
                ShowMessage("Email address is required.", false);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowMessage("Enter a valid email address.", false);
                return;
            }

            if (EmailBelongsToAnotherUser(email, userId))
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

                cmd.Parameters.AddWithValue("@fullName", fullName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@userId", userId);

                con.Open();

                if (cmd.ExecuteNonQuery() > 0)
                {
                    LoadAdminProfile(userId);

                    ShowMessage(
                        "Account information updated successfully.",
                        true
                    );
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

                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@userId", currentUserId);

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
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
            lblMessage.Text = message;

            lblMessage.Visible = true;

            lblMessage.CssClass = success
                ? "feedback-message success-message"
                : "feedback-message error-message";
        }
    }
}