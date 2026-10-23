using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace The_Analytical_Dojo
{
    public partial class Login : System.Web.UI.Page
    {

        private const string GoogleClientId = "348938239435-rbvohc5nk1m9s0ahv5hjsbfqh4epurrp.apps.googleusercontent.com";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["UserId"] != null)
            {
                Response.Redirect("StudentMainPage.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string loginId = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password))
            {
                ShowError("Enter your username/email and password.");
                return;
            }

            const string sql = @"
            SELECT user_id, username, full_name, password_hash, [role]
            FROM users
            WHERE username = @login OR email = @login";

            using (SqlConnection conn = DojoDb.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@login", loginId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Always run BCrypt.Verify even on a "user not found" path in
                    // a real system, to avoid timing attacks that reveal which
                    // logins exist. Kept simple here since this is a student
                    // project, but worth knowing for production code.
                    if (!reader.Read())
                    {
                        ShowError("Invalid username/email or password.");
                        return;
                    }

                    string storedHash = reader["password_hash"] as string;
                    bool passwordOk = false;
                    if (!string.IsNullOrEmpty(storedHash))
                    {
                        try { passwordOk = BCrypt.Net.BCrypt.Verify(password, storedHash); }
                        catch (BCrypt.Net.SaltParseException) { passwordOk = false; }
                    }

                    if (!passwordOk)
                    {
                        ShowError("Invalid username/email or password.");
                        return;
                    }

                    Session["UserId"] = (int)reader["user_id"];
                    Session["Username"] = reader["username"].ToString();
                    Session["FullName"] = reader["full_name"].ToString();
                    Session["Role"] = reader["role"].ToString(); // 'STUDENT' | 'ADMIN' | 'PEER LEADER'
                }
            }

            Response.Redirect("StudentMainPage.aspx");
        }

        protected void btnGoogle_Click(object sender, EventArgs e)
        {
            string idToken = hfGoogleToken.Value;
            if (string.IsNullOrEmpty(idToken))
            {
                ShowError("Google sign-in failed. Please try again.");
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
                ShowError("Google sign-in could not be verified.");
                return;
            }

            if (!payload.EmailVerified)
            {
                ShowError("Your Google email is not verified.");
                return;
            }

            using (SqlConnection conn = DojoDb.GetConnection())
            {
                conn.Open();

                int? userId = null; string username = null, fullName = null, role = null;

                const string findSql = @"
            SELECT user_id, username, full_name, [role], google_sub
            FROM users
            WHERE google_sub = @sub OR email = @email";

                using (var cmd = new SqlCommand(findSql, conn))
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

                    const string insertSql = @"
                INSERT INTO users (username, email, full_name, password_hash, [role], google_sub)
                OUTPUT INSERTED.user_id
                VALUES (@username, @email, @fullName, NULL, @role, @sub)";

                    using (var cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", payload.Email);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@sub", payload.Subject);
                        userId = (int)cmd.ExecuteScalar();
                    }
                }

                Session["UserId"] = userId.Value;
                Session["Username"] = username;
                Session["FullName"] = fullName;
                Session["Role"] = role;
            }

            Response.Redirect("StudentMainPage.aspx");
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }

}