using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace The_Analytical_Dojo
{
    public partial class Signup : System.Web.UI.Page
    {
        private const string GoogleClientId = "348938239435-rbvohc5nk1m9s0ahv5hjsbfqh4epurrp.apps.googleusercontent.com";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["UserId"] != null)
            {
                Response.Redirect("StudentMainPage.aspx");
            }
        }

        // ---------- Email + password sign-up ----------

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            string fullName = (txtFullName.Text ?? string.Empty).Trim();
            string username = (txtUsername.Text ?? string.Empty).Trim();
            string email = (txtEmail.Text ?? string.Empty).Trim();
            string password = txtPassword.Text ?? string.Empty;
            string confirm = txtConfirm.Text ?? string.Empty;

            // Same rules as the Profile page, so both pages agree.
            if (fullName.Length < 2 || fullName.Length > 100)
            {
                ShowError("Enter your full name.");
                return;
            }

            if (!Regex.IsMatch(username, @"^[A-Za-z0-9][A-Za-z0-9._-]{2,49}$"))
            {
                ShowError("Username must be 3–50 characters, start with a letter or number, and use only letters, numbers, dots, hyphens or underscores.");
                return;
            }

            if (email.Length > 150 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowError("Enter a valid email address.");
                return;
            }

            // BCrypt only uses the first 72 bytes, so cap it rather than silently truncate.
            if (System.Text.Encoding.UTF8.GetByteCount(password) > 72)
            {
                ShowError("Password is too long (72 characters maximum).");
                return;
            }

            var missing = new System.Collections.Generic.List<string>();
            if (password.Length < 8) missing.Add("at least 8 characters");
            if (!Regex.IsMatch(password, "[A-Z]")) missing.Add("an uppercase letter");
            if (!Regex.IsMatch(password, "[a-z]")) missing.Add("a lowercase letter");
            if (!Regex.IsMatch(password, "[0-9]")) missing.Add("a number");
            if (!Regex.IsMatch(password, @"[!-/:-@\[-`{-~]")) missing.Add("a symbol");

            if (missing.Count > 0)
            {
                ShowError("Password needs " + string.Join(", ", missing) + ".");
                return;
            }

            if (password != confirm)
            {
                ShowError("Password and confirmation do not match.");
                return;
            }

            int userId;

            try
            {
                using (SqlConnection conn = DojoDb.GetConnection())
                {
                    conn.Open();

                    // Friendly duplicate check (the database is still the final judge, see catch below).
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 1 username, email FROM users WHERE username = @username OR email = @email", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                bool emailTaken = string.Equals(
                                    Convert.ToString(r["email"]), email, StringComparison.OrdinalIgnoreCase);
                                ShowError(emailTaken
                                    ? "An account with that email already exists. Try logging in."
                                    : "That username is already taken.");
                                return;
                            }
                        }
                    }

                    string hash = BCrypt.Net.BCrypt.HashPassword(password);

                    using (var cmd = new SqlCommand(@"
                        INSERT INTO users (username, full_name, email, password_hash, [role])
                        OUTPUT INSERTED.user_id
                        VALUES (@username, @fullName, @email, @hash, 'STUDENT')", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@hash", hash);
                        userId = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                // 2627 / 2601 = unique constraint hit (two people signing up at the same moment).
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ShowError("That username or email is already in use.");
                    return;
                }
                throw;
            }

            StartSession(userId, username, fullName, "STUDENT");
            Response.Redirect("StudentMainPage.aspx");
        }

        // ---------- Google sign-up (creates the account, or signs in if it already exists) ----------

        protected void btnGoogle_Click(object sender, EventArgs e)
        {
            string idToken = hfGoogleToken.Value;
            if (string.IsNullOrEmpty(idToken))
            {
                ShowError("Google sign-up failed. Please try again.");
                return;
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { GoogleClientId }
                };
                payload = Task.Run(() => GoogleJsonWebSignature.ValidateAsync(idToken, settings))
                              .GetAwaiter().GetResult();
            }
            catch (InvalidJwtException)
            {
                ShowError("Google sign-up could not be verified.");
                return;
            }

            if (!payload.EmailVerified)
            {
                ShowError("Your Google email is not verified.");
                return;
            }

            int? userId = null;
            string username = null, fullName = null, role = null;

            using (SqlConnection conn = DojoDb.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand(@"
                    SELECT user_id, username, full_name, [role]
                    FROM users
                    WHERE google_sub = @sub OR email = @email", conn))
                {
                    cmd.Parameters.AddWithValue("@sub", payload.Subject);
                    cmd.Parameters.AddWithValue("@email", payload.Email);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            userId = (int)r["user_id"];
                            username = r["username"].ToString();
                            fullName = r["full_name"].ToString();
                            role = r["role"].ToString();
                        }
                    }
                }

                if (userId.HasValue)
                {
                    // Account already exists: link Google to it (if not yet linked) and sign in.
                    using (var cmd = new SqlCommand(
                        "UPDATE users SET google_sub = @sub WHERE user_id = @id AND google_sub IS NULL", conn))
                    {
                        cmd.Parameters.AddWithValue("@sub", payload.Subject);
                        cmd.Parameters.AddWithValue("@id", userId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    username = payload.Email;
                    fullName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name;
                    role = "STUDENT";

                    using (var cmd = new SqlCommand(@"
                        INSERT INTO users (username, email, full_name, password_hash, [role], google_sub)
                        OUTPUT INSERTED.user_id
                        VALUES (@username, @email, @fullName, NULL, @role, @sub)", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", payload.Email);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@sub", payload.Subject);
                        userId = (int)cmd.ExecuteScalar();
                    }
                }
            }

            StartSession(userId.Value, username, fullName, role);
            Response.Redirect("StudentMainPage.aspx");
        }

        // ---------- helpers ----------

        private void StartSession(int userId, string username, string fullName, string role)
        {
            Session["UserId"] = userId;
            Session["Username"] = username;
            Session["FullName"] = fullName;
            Session["Role"] = role;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}