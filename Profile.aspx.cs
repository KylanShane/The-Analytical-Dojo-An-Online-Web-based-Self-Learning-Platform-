using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class Profile : DojoPage
    {
        public string ProfileInitials
        {
            get
            {
                return ((SiteMaster)Master).Initials;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Google-linked accounts never see or use the password fields.
            bool google = IsGoogleAccount();
            PasswordPanel.Visible = !google;
            GoogleNote.Visible = google;
            if (google)
            {
                IntroText.Text = "You can change your name, username and email.";
            }

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private bool IsGoogleAccount()
        {
            DataTable t = DojoData.Query(@"
                SELECT google_sub
                FROM users
                WHERE user_id = @userId",
                DojoData.P("@userId", UserId)
            );

            return t.Rows.Count > 0 && !(t.Rows[0]["google_sub"] is DBNull);
        }

        private void LoadProfile()
        {
            DataTable user = DojoData.Query(@"
                SELECT username, full_name, email
                FROM users
                WHERE user_id = @userId",
                DojoData.P("@userId", UserId)
            );

            if (user.Rows.Count == 0)
            {
                throw new HttpException(404, "Profile not found.");
            }

            DataRow row = user.Rows[0];
            Username.Text = Convert.ToString(row["username"]);
            FullName.Text = Convert.ToString(row["full_name"]);
            Email.Text = Convert.ToString(row["email"]);
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            string username = (Username.Text ?? string.Empty).Trim();
            string fullName = (FullName.Text ?? string.Empty).Trim();
            string email = (Email.Text ?? string.Empty).Trim();

            if (username.Length < 3 || username.Length > 50
                || !Regex.IsMatch(username, @"^[A-Za-z0-9._-]+$"))
            {
                Show("Use a username of 3–50 letters, numbers, dots, hyphens or underscores.", true);
                return;
            }

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                Show("Enter your full name.", true);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || email.Length > 150)
            {
                Show("Enter a valid email address.", true);
                return;
            }

            using (SqlConnection connection = DojoDb.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    DataTable stored = new DataTable();

                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT password_hash, google_sub
                        FROM users WITH (UPDLOCK, HOLDLOCK)
                        WHERE user_id = @userId", connection, transaction))
                    {
                        cmd.Parameters.Add(DojoData.P("@userId", UserId));
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(stored);
                        }
                    }

                    if (stored.Rows.Count == 0)
                    {
                        throw new HttpException(404, "Profile not found.");
                    }

                    // Enforced server-side: hiding the fields alone is not enough.
                    bool google = !(stored.Rows[0]["google_sub"] is DBNull);
                    string hash = null;
                    string newHash = null;

                    if (!google)
                    {
                        string current = CurrentPassword.Text ?? string.Empty;
                        string next = NewPassword.Text ?? string.Empty;
                        string confirm = ConfirmPassword.Text ?? string.Empty;

                        if (string.IsNullOrEmpty(current))
                        {
                            transaction.Rollback();
                            Show("Enter your current password to save changes.", true);
                            return;
                        }

                        if (!string.IsNullOrEmpty(next))
                        {
                            if (next.Length < 8)
                            {
                                transaction.Rollback();
                                Show("New password must be at least 8 characters.", true);
                                return;
                            }

                            if (next != confirm)
                            {
                                transaction.Rollback();
                                Show("New password and confirmation do not match.", true);
                                return;
                            }
                        }

                        hash = stored.Rows[0]["password_hash"] as string;
                        bool passwordOk = false;

                        if (!string.IsNullOrEmpty(hash))
                        {
                            try
                            {
                                passwordOk = BCrypt.Net.BCrypt.Verify(current, hash);
                            }
                            catch (BCrypt.Net.SaltParseException)
                            {
                                passwordOk = false;
                            }
                        }

                        if (!passwordOk)
                        {
                            transaction.Rollback();
                            Show("Current password is incorrect.", true);
                            return;
                        }

                        newHash = string.IsNullOrEmpty(next)
                            ? hash
                            : BCrypt.Net.BCrypt.HashPassword(next);
                    }

                    object taken = DojoData.Scalar(
                        connection,
                        transaction,
                        @"
                        SELECT TOP 1 username
                        FROM users
                        WHERE user_id <> @userId
                            AND (username = @username OR email = @email)",
                        DojoData.P("@userId", UserId),
                        DojoData.P("@username", username),
                        DojoData.P("@email", email)
                    );

                    if (taken != null)
                    {
                        transaction.Rollback();
                        Show("That username or email is already in use.", true);
                        return;
                    }

                    if (google)
                    {
                        // Google accounts: password_hash is never touched.
                        DojoData.Execute(
                            connection,
                            transaction,
                            @"
                            UPDATE users
                            SET
                                username = @username,
                                full_name = @fullName,
                                email = @email
                            WHERE user_id = @userId",
                            DojoData.P("@username", username),
                            DojoData.P("@fullName", fullName),
                            DojoData.P("@email", email),
                            DojoData.P("@userId", UserId)
                        );
                    }
                    else
                    {
                        DojoData.Execute(
                            connection,
                            transaction,
                            @"
                            UPDATE users
                            SET
                                username = @username,
                                full_name = @fullName,
                                email = @email,
                                password_hash = @hash
                            WHERE user_id = @userId",
                            DojoData.P("@username", username),
                            DojoData.P("@fullName", fullName),
                            DojoData.P("@email", email),
                            DojoData.P("@hash", newHash),
                            DojoData.P("@userId", UserId)
                        );
                    }

                    transaction.Commit();
                }
            }

            Session["Username"] = username;
            Session["FullName"] = fullName;
            CurrentPassword.Text = string.Empty;
            NewPassword.Text = string.Empty;
            ConfirmPassword.Text = string.Empty;
            Show("Your profile was updated.", false);
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        private void Show(string message, bool error)
        {
            Status.Visible = true;
            Status.Text = H(message);
            Status.CssClass = error ? "notice notice-error" : "notice";
        }
    }
}