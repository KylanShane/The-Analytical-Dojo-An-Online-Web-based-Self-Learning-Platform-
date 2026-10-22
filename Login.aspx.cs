using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace The_Analytical_Dojo
{
    public partial class Login : System.Web.UI.Page
    {
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

                    string storedHash = reader["password_hash"].ToString();
                    bool passwordOk;
                    try
                    {
                        passwordOk = BCrypt.Net.BCrypt.Verify(password, storedHash);
                    }
                    catch (BCrypt.Net.SaltParseException)
                    {
                        passwordOk = false; // malformed hash in the DB -- never crash the login page over it
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

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }

}