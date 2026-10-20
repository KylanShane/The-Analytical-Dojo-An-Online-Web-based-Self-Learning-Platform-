using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            LoadUsers();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";

            ddlRoleFilter.SelectedIndex = 0;

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

                cmd.Parameters.AddWithValue(
                    "@userId",
                    userId
                );

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