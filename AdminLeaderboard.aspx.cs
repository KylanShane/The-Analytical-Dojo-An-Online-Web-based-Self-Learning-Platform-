using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminLeaderboard : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLeaderboard();
                LoadTopThree();
            }
        }

        private void LoadLeaderboard()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    WITH RankedStudents AS
                    (
                        SELECT
                            u.user_id,
                            u.full_name,
                            u.username,
                            u.role,

                            COUNT(ce.enrollment_id)
                                AS courses_enrolled,

                            AVG(
                                CAST(
                                    ce.average_score_percentage
                                    AS FLOAT
                                )
                            ) AS average_score,

                            AVG(
                                CAST(
                                    ce.completion_percentage
                                    AS FLOAT
                                )
                            ) AS average_completion,

                            RANK() OVER
                            (
                                ORDER BY

                                    AVG(
                                        CAST(
                                            ce.average_score_percentage
                                            AS FLOAT
                                        )
                                    ) DESC,

                                    AVG(
                                        CAST(
                                            ce.completion_percentage
                                            AS FLOAT
                                        )
                                    ) DESC,

                                    u.full_name ASC
                            ) AS actual_rank

                        FROM users u

                        INNER JOIN course_enrollments ce
                            ON u.user_id = ce.user_id

                        WHERE
                            u.role IN ('STUDENT', 'PEER LEADER')

                        GROUP BY
                            u.user_id,
                            u.full_name,
                            u.username,
                            u.role
                    )

                    SELECT
                        user_id,
                        full_name,
                        username,
                        role,
                        courses_enrolled,
                        average_score,
                        average_completion,
                        actual_rank

                    FROM RankedStudents

                    WHERE
                        (
                            @search = ''
                            OR full_name LIKE '%' + @search + '%'
                            OR username LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @role = ''
                            OR role = @role
                        )

                    ORDER BY
                        actual_rank ASC,
                        full_name ASC";

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

                gvLeaderboard.DataSource =
                    dt;

                gvLeaderboard.DataBind();
            }
        }

        private void LoadTopThree()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 3
                        u.full_name,
                        u.role,

                        AVG(
                            CAST(
                                ce.average_score_percentage
                                AS FLOAT
                            )
                        ) AS average_score,

                        AVG(
                            CAST(
                                ce.completion_percentage
                                AS FLOAT
                            )
                        ) AS average_completion

                    FROM users u

                    INNER JOIN course_enrollments ce
                        ON u.user_id = ce.user_id

                    WHERE
                        u.role IN ('STUDENT', 'PEER LEADER')

                    GROUP BY
                        u.user_id,
                        u.full_name,
                        u.role

                    ORDER BY

                        AVG(
                            CAST(
                                ce.average_score_percentage
                                AS FLOAT
                            )
                        ) DESC,

                        AVG(
                            CAST(
                                ce.completion_percentage
                                AS FLOAT
                            )
                        ) DESC,

                        u.full_name ASC";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                SetTopStudent(
                    dt,
                    0,
                    lblFirstName,
                    lblFirstRole,
                    lblFirstScore
                );

                SetTopStudent(
                    dt,
                    1,
                    lblSecondName,
                    lblSecondRole,
                    lblSecondScore
                );

                SetTopStudent(
                    dt,
                    2,
                    lblThirdName,
                    lblThirdRole,
                    lblThirdScore
                );
            }
        }

        private void SetTopStudent(
            DataTable dt,
            int index,
            Label nameLabel,
            Label roleLabel,
            Label scoreLabel)
        {
            if (dt.Rows.Count > index)
            {
                DataRow row =
                    dt.Rows[index];

                nameLabel.Text =
                    row["full_name"].ToString();

                roleLabel.Text =
                    row["role"].ToString();

                scoreLabel.Text =
                    FormatPercentage(
                        row["average_score"]
                    );
            }
            else
            {
                nameLabel.Text =
                    "No Data";

                roleLabel.Text =
                    "-";

                scoreLabel.Text =
                    "0%";
            }
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            LoadLeaderboard();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text =
                "";

            ddlRoleFilter.SelectedIndex =
                0;

            LoadLeaderboard();
        }

        public string FormatPercentage(
            object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return "0%";
            }

            double number =
                Convert.ToDouble(value);

            return number.ToString(
                "0.0"
            ) + "%";
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

            if (role == "PEER LEADER")
            {
                return "role-badge role-peer";
            }

            return "role-badge role-student";
        }
    }
}