using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminUsers : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserStatistics();
                LoadUsers();
            }
        }

        private void LoadUserStatistics()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        COUNT(*) AS TotalUsers,

                        SUM(
                            CASE
                                WHEN role = 'STUDENT'
                                THEN 1
                                ELSE 0
                            END
                        ) AS TotalStudents,

                        SUM(
                            CASE
                                WHEN role = 'PEER LEADER'
                                THEN 1
                                ELSE 0
                            END
                        ) AS TotalPeerLeaders,

                        SUM(
                            CASE
                                WHEN role = 'ADMIN'
                                THEN 1
                                ELSE 0
                            END
                        ) AS TotalAdmins

                    FROM users";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblTotalUsers.Text =
                        reader["TotalUsers"].ToString();

                    lblTotalStudents.Text =
                        reader["TotalStudents"] == DBNull.Value
                        ? "0"
                        : reader["TotalStudents"].ToString();

                    lblTotalPeerLeaders.Text =
                        reader["TotalPeerLeaders"] == DBNull.Value
                        ? "0"
                        : reader["TotalPeerLeaders"].ToString();

                    lblTotalAdmins.Text =
                        reader["TotalAdmins"] == DBNull.Value
                        ? "0"
                        : reader["TotalAdmins"].ToString();
                }
            }
        }

        private void LoadUsers()
        {
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
                        peer_leader_since,
                        created_at

                    FROM users

                    WHERE
                        (
                            @search = ''
                            OR username LIKE '%' + @search + '%'
                            OR full_name LIKE '%' + @search + '%'
                            OR email LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @role = ''
                            OR role = @role
                        )

                    ORDER BY created_at DESC";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@search",
                    txtSearch.Text.Trim()
                );

                cmd.Parameters.AddWithValue(
                    "@role",
                    ddlRoleFilter.SelectedValue
                );

                SqlDataAdapter adapter =
                    new SqlDataAdapter(cmd);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void btnAddAdmin_Click(
            object sender,
            EventArgs e)
        {
            lblMessage.Visible = false;

            string fullName =
                txtAdminFullName.Text.Trim();

            string username =
                txtAdminUsername.Text.Trim();

            string email =
                txtAdminEmail.Text.Trim();

            string password =
                txtAdminPassword.Text;

            string confirmPassword =
                txtAdminConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowMessage(
                    "Please complete all admin account fields.",
                    false
                );

                return;
            }

            if (!IsValidEmail(email))
            {
                ShowMessage(
                    "Please enter a valid email address.",
                    false
                );

                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage(
                    "Password and confirm password do not match.",
                    false
                );

                return;
            }

            if (UsernameOrEmailExists(
                username,
                email))
            {
                ShowMessage(
                    "The username or email is already registered.",
                    false
                );

                return;
            }

            string passwordHash;

            try
            {
                passwordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        password
                    );
            }
            catch
            {
                ShowMessage(
                    "The password could not be processed.",
                    false
                );

                return;
            }

            bool created =
                CreateAdmin(
                    username,
                    fullName,
                    email,
                    passwordHash
                );

            if (created)
            {
                ClearAdminForm();

                ShowMessage(
                    "Admin account created successfully.",
                    true
                );

                LoadUserStatistics();
                LoadUsers();
            }
            else
            {
                ShowMessage(
                    "Admin account could not be created.",
                    false
                );
            }
        }

        protected void btnClearAdmin_Click(
            object sender,
            EventArgs e)
        {
            ClearAdminForm();

            lblMessage.Visible = false;
        }

        private bool UsernameOrEmailExists(
            string username,
            string email)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(*)

                    FROM users

                    WHERE username = @username
                    OR email = @email";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.Add(
                    "@username",
                    SqlDbType.NVarChar
                ).Value = username;

                cmd.Parameters.Add(
                    "@email",
                    SqlDbType.NVarChar
                ).Value = email;

                con.Open();

                int count =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );

                return count > 0;
            }
        }

        private bool CreateAdmin(
            string username,
            string fullName,
            string email,
            string passwordHash)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO users
                    (
                        username,
                        full_name,
                        email,
                        password_hash,
                        role,
                        peer_leader_since,
                        created_at
                    )

                    VALUES
                    (
                        @username,
                        @fullName,
                        @email,
                        @passwordHash,
                        'ADMIN',
                        NULL,
                        GETDATE()
                    )";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.Add(
                    "@username",
                    SqlDbType.NVarChar
                ).Value = username;

                cmd.Parameters.Add(
                    "@fullName",
                    SqlDbType.NVarChar
                ).Value = fullName;

                cmd.Parameters.Add(
                    "@email",
                    SqlDbType.NVarChar
                ).Value = email;

                cmd.Parameters.Add(
                    "@passwordHash",
                    SqlDbType.NVarChar
                ).Value = passwordHash;

                con.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        private bool IsValidEmail(
            string email)
        {
            try
            {
                MailAddress address =
                    new MailAddress(email);

                return address.Address
                    .Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase
                    );
            }
            catch
            {
                return false;
            }
        }

        private void ClearAdminForm()
        {
            txtAdminFullName.Text = "";
            txtAdminUsername.Text = "";
            txtAdminEmail.Text = "";
            txtAdminPassword.Text = "";
            txtAdminConfirmPassword.Text = "";
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            lblMessage.Visible = false;

            LoadUsers();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";

            ddlRoleFilter.SelectedIndex = 0;

            lblMessage.Visible = false;

            LoadUsers();
        }

        protected void gvUsers_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemovePeerLeader")
            {
                int userId =
                    Convert.ToInt32(
                        e.CommandArgument
                    );

                bool removed =
                    RemovePeerLeader(
                        userId
                    );

                if (removed)
                {
                    ShowMessage(
                        "Peer Leader access removed successfully.",
                        true
                    );
                }
                else
                {
                    ShowMessage(
                        "Peer Leader access could not be removed.",
                        false
                    );
                }

                LoadUserStatistics();
                LoadUsers();
            }
        }

        private bool RemovePeerLeader(
            int userId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE users

                    SET
                        role = 'STUDENT',
                        peer_leader_since = NULL

                    WHERE user_id = @userId
                    AND role = 'PEER LEADER'";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.Add(
                    "@userId",
                    SqlDbType.Int
                ).Value = userId;

                con.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool IsPeerLeader(
            object roleValue)
        {
            if (roleValue == null ||
                roleValue == DBNull.Value)
            {
                return false;
            }

            return roleValue
                .ToString()
                .Equals(
                    "PEER LEADER",
                    StringComparison.OrdinalIgnoreCase
                );
        }

        public string GetRoleClass(
            object roleValue)
        {
            if (roleValue == null ||
                roleValue == DBNull.Value)
            {
                return "role-badge";
            }

            string role =
                roleValue.ToString();

            if (role == "ADMIN")
            {
                return "role-badge role-admin";
            }

            if (role == "PEER LEADER")
            {
                return "role-badge role-peer";
            }

            return "role-badge role-student";
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
                "dd MMM yyyy"
            );
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;

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