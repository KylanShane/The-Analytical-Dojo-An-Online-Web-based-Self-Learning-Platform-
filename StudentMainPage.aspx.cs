using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class StudentMainPage : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                QueryText.Text = DojoPractice.Starter;
                LoadQuizPicker();
            }

            LoadHero();
            LoadCourses();
            LoadBadges();
            LoadLeaderboard();
            LoadActivityChart();
        }

        private void LoadHero()
        {
            DataTable next = DojoData.Query(@"
                SELECT TOP 1
                    c.course_id,
                    c.course_title,
                    m.material_title,
                    (
                        SELECT COUNT(*)
                        FROM course_materials mx
                        JOIN course_chapters hx ON hx.chapter_id = mx.chapter_id
                        LEFT JOIN student_material_progress px
                            ON px.material_id = mx.material_id
                            AND px.enrollment_id = e.enrollment_id
                            AND px.completed_at IS NOT NULL
                        WHERE hx.course_id = c.course_id
                            AND px.progress_id IS NULL
                    ) AS remaining_materials
                FROM course_enrollments e
                JOIN courses c ON c.course_id = e.course_id
                JOIN course_chapters ch ON ch.course_id = c.course_id
                JOIN course_materials m ON m.chapter_id = ch.chapter_id
                LEFT JOIN student_material_progress p
                    ON p.material_id = m.material_id
                    AND p.enrollment_id = e.enrollment_id
                WHERE e.user_id = @userId
                    AND e.completion_percentage < 100
                    AND (p.completed_at IS NULL)
                ORDER BY e.enrolled_at DESC, ch.chapter_order, m.material_id",
                DojoData.P("@userId", UserId)
            );

            int courseId = 0;

            if (next.Rows.Count > 0)
            {
                DataRow row = next.Rows[0];
                courseId = Convert.ToInt32(row["course_id"]);
                int remaining = Convert.ToInt32(row["remaining_materials"]);

                NextMaterial.Text =
                    "<h1>You're " + remaining +
                    " material" + (remaining == 1 ? "" : "s") +
                    " away from finishing<br /><span class='accent-link'>" +
                    H(row["course_title"]) +
                    "</span></h1><p class='muted'>Your next material is " +
                    H(row["material_title"]) +
                    ". Continue from the course page to open slides and quizzes.</p>";
            }
            else
            {
                DataTable courses = DojoData.Courses(UserId);

                if (courses.Rows.Count > 0)
                {
                    courseId = Convert.ToInt32(courses.Rows[0]["course_id"]);
                    NextMaterial.Text =
                        "<h1>You're all caught up on materials<span class='yellow'>.</span></h1>" +
                        "<p class='muted'>Open a course to review slides or attempt a quiz you have not passed yet.</p>";
                }
                else
                {
                    NextMaterial.Text =
                        "<h1>Start with a course<span class='yellow'>.</span></h1>" +
                        "<p class='muted'>Browse the catalogue, enrol, then open materials and quizzes.</p>";
                }
            }

            if (courseId > 0)
            {
                ResumeLink.NavigateUrl = "CoursePage.aspx?id=" + courseId;
                AttemptLink.NavigateUrl = "CoursePage.aspx?id=" + courseId;
                ResumeLink.Visible = true;
                AttemptLink.Visible = true;
            }
            else
            {
                ResumeLink.NavigateUrl = "BrowseCourses.aspx";
                ResumeLink.Text = "Browse courses";
                AttemptLink.Visible = false;
            }
        }

        private void LoadCourses()
        {
            Courses.Text = DojoUi.CourseRows(DojoData.Courses(UserId));
        }

        private void LoadBadges()
        {
            Badges.Text = DojoUi.BadgeGrid(DojoData.UnlockBadges(UserId));
        }

        private void LoadQuizPicker()
        {
            DataTable quizzes = DojoData.LeaderboardQuizzes(UserId);
            QuizPick.Items.Clear();

            if (quizzes.Rows.Count == 0)
            {
                QuizPick.Items.Add(new ListItem("No objective quizzes yet", "0"));
                QuizPick.Enabled = false;
                return;
            }

            foreach (DataRow quiz in quizzes.Rows)
            {
                QuizPick.Items.Add(
                    new ListItem(
                        Convert.ToString(quiz["quiz_title"]),
                        Convert.ToString(quiz["quiz_id"])
                    )
                );
            }
        }

        private void LoadLeaderboard()
        {
            int quizId;

            if (!int.TryParse(QuizPick.SelectedValue, out quizId) || quizId <= 0)
            {
                LeaderboardList.Text =
                    "<p class='muted'>Enrol in a course and submit an objective quiz to appear here.</p>";
                return;
            }

            LeaderboardList.Text = DojoUi.LeaderboardList(
                DojoData.Leaderboard(quizId),
                UserId
            );
        }

        protected void QuizPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaderboard();
        }

        private void LoadActivityChart()
        {
            DataTable activity = DojoData.Query(@"
                SELECT
                    'materials' AS metric,
                    CONVERT(CHAR(10), p.completed_at, 23) AS day,
                    COUNT(*) AS n
                FROM student_material_progress p
                INNER JOIN course_enrollments e
                    ON e.enrollment_id = p.enrollment_id
                WHERE e.user_id = @userId
                    AND p.completed_at IS NOT NULL
                GROUP BY CONVERT(CHAR(10), p.completed_at, 23)

                UNION ALL

                SELECT
                    'quizzes' AS metric,
                    CONVERT(CHAR(10), submitted_at, 23) AS day,
                    COUNT(*) AS n
                FROM quiz_attempts
                WHERE user_id = @userId
                    AND submitted_at IS NOT NULL
                GROUP BY CONVERT(CHAR(10), submitted_at, 23)",
                DojoData.P("@userId", UserId)
            );

            List<object> activityRows = new List<object>();

            foreach (DataRow row in activity.Rows)
            {
                activityRows.Add(new
                {
                    metric = Convert.ToString(row["metric"]),
                    day = Convert.ToString(row["day"]),
                    n = Convert.ToInt32(row["n"])
                });
            }

            string today = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityJson.Text = serializer.Serialize(new
            {
                today = today,
                rows = activityRows
            }).Replace("<", "\\u003c");
        }

        protected void RunQuery_Click(object sender, EventArgs e)
        {
            QueryResult.Text = string.Empty;
            QueryStatus.Visible = true;

            try
            {
                DataTable results = DojoPractice.Run(UserId, QueryText.Text);
                QueryResult.Text = DojoUi.Table(results);
                QueryStatus.Text = results.Rows.Count + " row(s) returned from your results.";
            }
            catch (ArgumentException ex)
            {
                QueryStatus.Text = H(ex.Message);
            }
        }
    }
}
