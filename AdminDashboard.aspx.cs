using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class adminDashboard : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardStatistics();
            }
        }

        private void LoadDashboardStatistics()
        {
            LoadStudents();
            LoadTotalCourses();
            LoadQuizAttemptsToday();
            LoadAverageCourseScore();
            LoadCourseCompletionOverview();
            LoadPendingCourses();
            LoadQuizPerformance();
            LoadRecentActivity();
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM users
                    WHERE role = 'STUDENT'";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                lblActiveStudents.Text =
                    Convert.ToInt32(cmd.ExecuteScalar()).ToString();
            }
        }

        private void LoadTotalCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM courses
                    WHERE status = 'PUBLISHED'";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                lblTotalCourses.Text =
                    Convert.ToInt32(cmd.ExecuteScalar()).ToString();
            }
        }

        private void LoadQuizAttemptsToday()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM quiz_attempts
                    WHERE submitted_at IS NOT NULL
                    AND CAST(submitted_at AS DATE) = CAST(GETDATE() AS DATE)";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                lblQuizAttemptsToday.Text =
                    Convert.ToInt32(cmd.ExecuteScalar()).ToString();
            }
        }

        private void LoadAverageCourseScore()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT AVG(
                        CAST(
                            average_score_percentage
                            AS FLOAT
                        )
                    )
                    FROM course_enrollments
                    WHERE average_score_percentage IS NOT NULL";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    double average =
                        Convert.ToDouble(result);

                    lblAverageCourseScore.Text =
                        average.ToString("0.0") + "%";
                }
                else
                {
                    lblAverageCourseScore.Text = "0%";
                }
            }
        }

        private void LoadCourseCompletionOverview()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        COUNT(*) AS TotalEnrollments,

                        SUM(
                            CASE
                                WHEN completed_at IS NOT NULL
                                THEN 1
                                ELSE 0
                            END
                        ) AS Completed,

                        SUM(
                            CASE
                                WHEN completed_at IS NULL
                                THEN 1
                                ELSE 0
                            END
                        ) AS InProgress,

                        AVG(
                            CAST(
                                completion_percentage
                                AS FLOAT
                            )
                        ) AS AverageCompletion

                    FROM course_enrollments";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblTotalEnrollments.Text =
                        reader["TotalEnrollments"].ToString();

                    lblCompletedEnrollments.Text =
                        reader["Completed"] == DBNull.Value
                        ? "0"
                        : reader["Completed"].ToString();

                    lblInProgressEnrollments.Text =
                        reader["InProgress"] == DBNull.Value
                        ? "0"
                        : reader["InProgress"].ToString();

                    if (reader["AverageCompletion"] != DBNull.Value)
                    {
                        double average =
                            Convert.ToDouble(
                                reader["AverageCompletion"]
                            );

                        lblAverageCompletion.Text =
                            average.ToString("0.0") + "%";
                    }
                    else
                    {
                        lblAverageCompletion.Text = "0%";
                    }
                }
            }
        }

        private void LoadPendingCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        c.course_id,
                        c.course_title,
                        c.status,
                        c.created_at,

                        COALESCE(
                            u.full_name,
                            u.username,
                            'Unknown User'
                        ) AS creator_name

                    FROM courses c

                    LEFT JOIN users u
                        ON c.created_by = u.user_id

                    WHERE c.status = 'PENDING'

                    ORDER BY c.created_at DESC";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                lblPendingCourseCount.Text =
                    dt.Rows.Count + " Pending";

                rptPendingCourses.DataSource = dt;
                rptPendingCourses.DataBind();

                pnlNoPendingCourses.Visible =
                    dt.Rows.Count == 0;
            }
        }

        private void LoadQuizPerformance()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        COUNT(*) AS TotalAttempts,

                        SUM(
                            CASE
                                WHEN submitted_at IS NOT NULL
                                THEN 1
                                ELSE 0
                            END
                        ) AS SubmittedAttempts,

                        AVG(
                            CASE
                                WHEN submitted_at IS NOT NULL
                                THEN CAST(score_earned AS FLOAT)
                            END
                        ) AS AverageScore

                    FROM quiz_attempts";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblTotalQuizAttempts.Text =
                        reader["TotalAttempts"].ToString();

                    lblSubmittedQuizAttempts.Text =
                        reader["SubmittedAttempts"] == DBNull.Value
                        ? "0"
                        : reader["SubmittedAttempts"].ToString();

                    if (reader["AverageScore"] != DBNull.Value)
                    {
                        double average =
                            Convert.ToDouble(
                                reader["AverageScore"]
                            );

                        lblAverageQuizScore.Text =
                            average.ToString("0.0");
                    }
                    else
                    {
                        lblAverageQuizScore.Text = "0";
                    }
                }

                reader.Close();

                string topQuizQuery = @"
                    SELECT TOP 1
                        q.quiz_title,
                        COUNT(*) AS attempt_count

                    FROM quiz_attempts qa

                    INNER JOIN quizzes q
                        ON qa.quiz_id = q.quiz_id

                    GROUP BY
                        q.quiz_id,
                        q.quiz_title

                    ORDER BY
                        COUNT(*) DESC,
                        q.quiz_title ASC";

                SqlCommand topQuizCmd =
                    new SqlCommand(topQuizQuery, con);

                object topQuiz =
                    topQuizCmd.ExecuteScalar();

                if (topQuiz != null &&
                    topQuiz != DBNull.Value)
                {
                    lblMostAttemptedQuiz.Text =
                        topQuiz.ToString();
                }
                else
                {
                    lblMostAttemptedQuiz.Text =
                        "No attempts yet";
                }
            }
        }

        private void LoadRecentActivity()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 8
                        activity_type,
                        activity_title,
                        actor_name,
                        activity_date

                    FROM
                    (
                        SELECT
                            'ENROLLMENT'
                                AS activity_type,

                            'Enrolled in ' +
                            c.course_title
                                AS activity_title,

                            COALESCE(
                                u.full_name,
                                u.username,
                                'Unknown User'
                            )
                                AS actor_name,

                            ce.enrolled_at
                                AS activity_date

                        FROM course_enrollments ce

                        INNER JOIN courses c
                            ON ce.course_id = c.course_id

                        INNER JOIN users u
                            ON ce.user_id = u.user_id


                        UNION ALL


                        SELECT
                            'QUIZ'
                                AS activity_type,

                            'Submitted quiz: ' +
                            q.quiz_title
                                AS activity_title,

                            COALESCE(
                                u.full_name,
                                u.username,
                                'Unknown User'
                            )
                                AS actor_name,

                            qa.submitted_at
                                AS activity_date

                        FROM quiz_attempts qa

                        INNER JOIN quizzes q
                            ON qa.quiz_id = q.quiz_id

                        INNER JOIN users u
                            ON qa.user_id = u.user_id

                        WHERE qa.submitted_at IS NOT NULL


                        UNION ALL


                        SELECT
                            'COURSE'
                                AS activity_type,

                            'Course created: ' +
                            c.course_title
                                AS activity_title,

                            COALESCE(
                                u.full_name,
                                u.username,
                                'Unknown User'
                            )
                                AS actor_name,

                            c.created_at
                                AS activity_date

                        FROM courses c

                        LEFT JOIN users u
                            ON c.created_by = u.user_id

                    ) Activity

                    WHERE activity_date IS NOT NULL

                    ORDER BY activity_date DESC";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con);

                DataTable dt =
                    new DataTable();

                adapter.Fill(dt);

                rptRecentActivity.DataSource = dt;
                rptRecentActivity.DataBind();

                pnlNoRecentActivity.Visible =
                    dt.Rows.Count == 0;
            }
        }

        protected void rptPendingCourses_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            int courseId =
                Convert.ToInt32(
                    e.CommandArgument
                );

            if (e.CommandName == "PublishCourse")
            {
                bool updated =
                    UpdateCourseStatus(
                        courseId,
                        "PUBLISHED"
                    );

                if (updated)
                {
                    ShowMessage(
                        "Course published successfully.",
                        true
                    );
                }
                else
                {
                    ShowMessage(
                        "The course could not be published.",
                        false
                    );
                }
            }
            else if (e.CommandName == "RejectCourse")
            {
                bool updated =
                    UpdateCourseStatus(
                        courseId,
                        "ARCHIVED"
                    );

                if (updated)
                {
                    ShowMessage(
                        "Course rejected successfully.",
                        true
                    );
                }
                else
                {
                    ShowMessage(
                        "The course could not be rejected.",
                        false
                    );
                }
            }

            LoadPendingCourses();
            LoadTotalCourses();
            LoadRecentActivity();
        }

        private bool UpdateCourseStatus(
            int courseId,
            string newStatus)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE courses

                    SET status = @status

                    WHERE course_id = @courseId
                    AND status = 'PENDING'";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@status",
                    newStatus
                );

                cmd.Parameters.AddWithValue(
                    "@courseId",
                    courseId
                );

                con.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
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