using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace The_Analytical_Dojo
{
    public partial class Leaderboard : DojoPage
    {
        // One row on the board, whichever view produced it.
        private class Entry
        {
            public int UserId;
            public string Name;
            public object Role;
            public decimal Value;
            public string ValueText;
            public string ValueUnit;
            public string Meta;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int quizId = QueryId("quiz");
            int courseId = QueryId("course");
            bool quizView = Request.QueryString["view"] == "quiz" || quizId > 0;

            DataTable quizzes = DojoData.LeaderboardQuizzes(UserId);

            if (quizView && quizId == 0 && quizzes.Rows.Count > 0)
            {
                quizId = Convert.ToInt32(quizzes.Rows[0]["quiz_id"]);
            }

            List<Entry> entries;
            string boardTitle;
            string boardNote;

            if (quizView)
            {
                entries = QuizEntries(quizId);
                boardTitle = QuizTitle(quizzes, quizId);
                boardNote = "Ranked by your latest score on this quiz, then earliest submission.";
            }
            else
            {
                entries = OverallEntries(courseId);
                boardTitle = courseId > 0 ? CourseTitle(courseId) : "All courses";
                boardNote = "Every scored quiz earns Dojo Points equal to your latest score %. Ties go to the higher average.";
            }

            DisplayHero(entries, quizView);
            DisplayFilters(quizView, quizzes, quizId, courseId);
            DisplayBoard(entries, boardTitle, boardNote);
        }

        private List<Entry> OverallEntries(int courseId)
        {
            DataTable rows = DojoData.Query(@"
                WITH latest AS (
                    SELECT
                        a.user_id,
                        CAST(100.0 * a.score_earned / n.total AS decimal(6,2)) AS score
                    FROM quiz_attempts a
                    JOIN quizzes q ON q.quiz_id = a.quiz_id
                    JOIN course_chapters ch ON ch.chapter_id = q.chapter_id
                    CROSS APPLY (
                        SELECT COUNT(*) total
                        FROM quiz_questions x
                        WHERE x.quiz_id = q.quiz_id
                            AND x.question_type IN ('MCQ', 'TRUE_FALSE')
                    ) n
                    WHERE a.submitted_at IS NOT NULL
                        AND a.score_earned IS NOT NULL
                        AND n.total > 0
                        AND (@courseId = 0 OR ch.course_id = @courseId)
                        AND a.attempt_id = (
                            SELECT TOP 1 z.attempt_id
                            FROM quiz_attempts z
                            WHERE z.quiz_id = a.quiz_id
                                AND z.user_id = a.user_id
                                AND z.submitted_at IS NOT NULL
                            ORDER BY z.submitted_at DESC, z.attempt_id DESC
                        )
                )
                SELECT
                    u.user_id,
                    u.full_name,
                    u.[role],
                    COUNT(*) AS quizzes,
                    CAST(AVG(l.score) AS decimal(6,1)) AS avg_score,
                    SUM(CASE WHEN l.score >= @distinction THEN 1 ELSE 0 END) AS distinctions,
                    CAST(ROUND(SUM(l.score), 0) AS int) AS dojo_points
                FROM latest l
                JOIN users u ON u.user_id = l.user_id
                GROUP BY u.user_id, u.full_name, u.[role]
                ORDER BY dojo_points DESC, avg_score DESC, u.full_name",
                DojoData.P("@courseId", courseId),
                DojoData.P("@distinction", DojoData.DistinctionMark));

            var entries = new List<Entry>();
            foreach (DataRow row in rows.Rows)
            {
                int distinctions = Convert.ToInt32(row["distinctions"]);
                entries.Add(new Entry
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Name = Convert.ToString(row["full_name"]),
                    Role = row["role"],
                    Value = Convert.ToDecimal(row["dojo_points"]),
                    ValueText = Convert.ToInt32(row["dojo_points"]).ToString("N0"),
                    ValueUnit = "Dojo Points",
                    Meta = row["quizzes"] + " quizzes · " + DojoData.Num(row["avg_score"]) + "% avg" +
                        (distinctions > 0 ? " · " + distinctions + " ★" : "")
                });
            }
            return entries;
        }

        private List<Entry> QuizEntries(int quizId)
        {
            var entries = new List<Entry>();
            if (quizId == 0)
            {
                return entries;
            }

            foreach (DataRow row in DojoData.Leaderboard(quizId).Rows)
            {
                entries.Add(new Entry
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Name = Convert.ToString(row["full_name"]),
                    Role = row["role"],
                    Value = Convert.ToDecimal(row["score"]),
                    ValueText = DojoData.Num(row["score"]),
                    ValueUnit = "%",
                    Meta = DojoData.Band(row["score"]) + " · " +
                        Convert.ToDateTime(row["submitted_at"]).ToString("dd MMM yyyy")
                });
            }
            return entries;
        }

        private void DisplayHero(List<Entry> entries, bool quizView)
        {
            int myRank = entries.FindIndex(x => x.UserId == UserId) + 1;
            Entry me = myRank > 0 ? entries[myRank - 1] : null;

            var html = new StringBuilder();
            html.Append("<section class='lb-hero'>");
            html.Append("<div class='lb-hero-copy'><span class='learning-kicker'>THE DOJO RANKINGS</span>");
            html.Append("<h1>Leaderboard<span class='yellow'>.</span></h1>");
            html.Append("<p>Climb the ranks by clearing quizzes and chasing Distinctions. Every point counts, sensei.</p></div>");

            html.Append("<div class='lb-me'>");
            if (me == null)
            {
                html.Append("<span class='lb-me-label'>YOUR RANK</span><strong class='lb-me-rank'>—</strong>");
                html.Append("<small>Submit a scored quiz to join the board.</small>");
            }
            else
            {
                html.Append("<span class='lb-me-label'>YOUR RANK</span>");
                html.Append("<strong class='lb-me-rank'>#<span data-count-to='" + myRank + "'>" + myRank + "</span></strong>");
                html.Append("<small>of " + entries.Count + " · " + H(me.ValueText) + (me.ValueUnit == "%" ? "" : " ") + H(me.ValueUnit) + "</small>");

                if (myRank > 1)
                {
                    Entry ahead = entries[myRank - 2];
                    decimal gap = ahead.Value - me.Value;
                    html.Append("<span class='lb-me-gap'>");
                    html.Append(gap <= 0
                        ? "Tied with #" + (myRank - 1) + ". One more push!"
                        : DojoData.Num(gap) + " " + (quizView ? "%" : "Dojo Points") + " to pass #" + (myRank - 1));
                    html.Append("</span>");
                }
                else
                {
                    html.Append("<span class='lb-me-gap is-top'>👑 You are the top of the dojo!</span>");
                }
            }
            html.Append("</div></section>");

            Hero.Text = html.ToString();
        }

        private void DisplayFilters(bool quizView, DataTable quizzes, int quizId, int courseId)
        {
            var html = new StringBuilder();
            html.Append("<div class='lb-filters'>");
            html.Append("<div class='segmented lb-tabs' role='tablist'>");
            html.Append("<a role='tab' href='Leaderboard.aspx' aria-selected='" + (!quizView).ToString().ToLowerInvariant() + "'>Overall Dojo Points</a>");
            html.Append("<a role='tab' href='Leaderboard.aspx?view=quiz' aria-selected='" + quizView.ToString().ToLowerInvariant() + "'>By quiz</a>");
            html.Append("</div>");

            if (quizView)
            {
                html.Append("<label class='lb-select'><span>Quiz</span><select class='input' data-nav-select aria-label='Choose a quiz'>");
                string lastCourse = null;
                foreach (DataRow quiz in quizzes.Rows)
                {
                    string course = Convert.ToString(quiz["course_title"]);
                    if (course != lastCourse)
                    {
                        if (lastCourse != null) html.Append("</optgroup>");
                        html.Append("<optgroup label='" + H(course) + "'>");
                        lastCourse = course;
                    }
                    int id = Convert.ToInt32(quiz["quiz_id"]);
                    html.Append("<option value='Leaderboard.aspx?quiz=" + id + "'" + (id == quizId ? " selected" : "") + ">");
                    html.Append(H(quiz["quiz_title"]));
                    html.Append("</option>");
                }
                if (lastCourse != null) html.Append("</optgroup>");
                html.Append("</select></label>");
            }
            else
            {
                DataTable courses = DojoData.Query(@"
                    SELECT c.course_id, c.course_title
                    FROM courses c
                    JOIN course_enrollments e ON e.course_id = c.course_id AND e.user_id = @userId
                    WHERE c.[status] <> 'ARCHIVED'
                    ORDER BY c.course_title",
                    DojoData.P("@userId", UserId));

                html.Append("<div class='lb-chips' aria-label='Filter by course'>");
                html.Append("<a class='lb-chip' href='Leaderboard.aspx'" + (courseId == 0 ? " aria-current='true'" : "") + ">All courses</a>");
                foreach (DataRow course in courses.Rows)
                {
                    int id = Convert.ToInt32(course["course_id"]);
                    html.Append("<a class='lb-chip' href='Leaderboard.aspx?course=" + id + "'" + (id == courseId ? " aria-current='true'" : "") + ">");
                    html.Append(H(course["course_title"]));
                    html.Append("</a>");
                }
                html.Append("</div>");
            }

            html.Append("</div>");
            Filters.Text = html.ToString();
        }

        private void DisplayBoard(List<Entry> entries, string title, string note)
        {
            var html = new StringBuilder();

            if (entries.Count == 0)
            {
                html.Append("<div class='panel lb-empty'><span aria-hidden='true'>🏁</span><h2>The board is wide open</h2>");
                html.Append("<p class='muted'>No scored attempts here yet. Be the first name on it.</p>");
                html.Append("<a class='btn' href='MyCourses.aspx'>Find a quiz →</a></div>");
                Board.Text = html.ToString();
                return;
            }

            // Podium: 2nd, 1st, 3rd
            html.Append("<section class='lb-podium' aria-label='Top three'>");
            int[] order = { 1, 0, 2 };
            foreach (int i in order)
            {
                if (i >= entries.Count)
                {
                    html.Append("<div class='lb-step lb-step-" + (i + 1) + " is-empty'></div>");
                    continue;
                }
                Entry x = entries[i];
                html.Append("<div class='lb-step lb-step-" + (i + 1) + (x.UserId == UserId ? " is-you" : "") + "'>");
                html.Append("<div class='lb-podium-person'>");
                if (i == 0) html.Append("<span class='lb-crown' aria-hidden='true'>👑</span>");
                html.Append(Avatar(x, "lb-avatar"));
                html.Append("<strong>" + H(x.Name) + "</strong>");
                html.Append(DojoUi.RolePill(x.Role));
                html.Append("<span class='lb-podium-value'><b data-count-to='" + H(x.ValueText.Replace(",", "")) + "'>" + H(x.ValueText) + "</b> " + H(x.ValueUnit) + "</span>");
                html.Append("</div><div class='lb-block'><span>" + (i + 1) + "</span></div></div>");
            }
            html.Append("</section>");

            // Full ranking
            decimal top = Math.Max(1m, entries[0].Value);
            html.Append("<section class='panel lb-board'>");
            html.Append("<div class='section-head'><div><span class='eyebrow'>STANDINGS</span><h2>" + H(title) + "</h2></div>");
            if (entries.Exists(x => x.UserId == UserId))
            {
                html.Append("<button type='button' class='btn ghost' data-jump-me>📍 Jump to me</button>");
            }
            html.Append("</div><ol class='lb-list'>");

            for (int i = 0; i < entries.Count; i++)
            {
                Entry x = entries[i];
                bool you = x.UserId == UserId;
                html.Append("<li class='lb-row" + (you ? " is-you" : "") + (i < 3 ? " is-top" : "") + "'" + (you ? " id='lb-me'" : "") + ">");
                html.Append("<span class='lb-rank'>" + (i < 3 ? new[] { "🥇", "🥈", "🥉" }[i] : (i + 1).ToString()) + "</span>");
                html.Append(Avatar(x, "lb-row-avatar"));
                html.Append("<div class='lb-row-main'><div class='lb-row-name'><strong>" + H(x.Name) + "</strong>" + DojoUi.RolePill(x.Role));
                if (you) html.Append("<span class='role-pill role-you'>You</span>");
                html.Append("</div><small>" + H(x.Meta) + "</small>");
                html.Append("<div class='lb-bar'><span style='--w:" + DojoData.Num(Math.Round(x.Value / top * 100m, 1)) + "%'></span></div></div>");
                html.Append("<span class='lb-value'><b>" + H(x.ValueText) + "</b><small>" + H(x.ValueUnit) + "</small></span>");
                html.Append("</li>");
            }

            html.Append("</ol><p class='muted rank-note'>" + H(note) + "</p></section>");
            Board.Text = html.ToString();
        }

        private static string QuizTitle(DataTable quizzes, int quizId)
        {
            foreach (DataRow quiz in quizzes.Rows)
            {
                if (Convert.ToInt32(quiz["quiz_id"]) == quizId)
                {
                    return Convert.ToString(quiz["quiz_title"]);
                }
            }
            return "Quiz";
        }

        private static string CourseTitle(int courseId)
        {
            object title = DojoData.Scalar(
                "SELECT course_title FROM courses WHERE course_id = @courseId",
                DojoData.P("@courseId", courseId));
            return title == null ? "Course" : Convert.ToString(title);
        }

        private static string Avatar(Entry x, string cssClass)
        {
            string initials = "";
            foreach (string part in (x.Name ?? "?").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (initials.Length < 2) initials += char.ToUpperInvariant(part[0]);
            }
            return "<span class='" + cssClass + "' aria-hidden='true'>" + H(initials == "" ? "?" : initials) + "</span>";
        }
    }
}
