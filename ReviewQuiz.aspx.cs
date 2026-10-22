using System;
using System.Data;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class ReviewQuiz : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int attemptId = QueryId("attempt");

                DataTable attempts = DojoData.Results(UserId);

                if (attemptId == 0)
                {
                    DisplayAttemptHistory(attempts);
                }
                else
                {
                    DisplayAttemptReview(attempts, attemptId);
                }
            }
        }

        private void DisplayAttemptHistory(DataTable attempts)
        {
            StringBuilder summary = new StringBuilder();

            summary.Append("<div class='filterbar'>");

            summary.Append(
                "<label class='search-label' for='reviewSearch'>"
            );

            summary.Append("Search attempts");

            summary.Append(
                "<input " +
                "type='search' " +
                "id='reviewSearch' " +
                "placeholder='Search course or quiz…' />"
            );

            summary.Append("</label>");

            summary.Append("<span class='pill'>");
            summary.Append(attempts.Rows.Count);
            summary.Append(" submitted attempts</span>");

            summary.Append("</div>");

            Summary.Text = summary.ToString();

            if (attempts.Rows.Count == 0)
            {
                History.Text =
                    "<p class='empty'>" +
                    "No submitted quizzes yet. " +
                    "<a href='MyCourses.aspx'>" +
                    "Open a course to begin." +
                    "</a></p>";

                return;
            }

            StringBuilder html = new StringBuilder();

            html.Append("<div class='panel table-scroll'>");
            html.Append("<table>");

            html.Append("<thead><tr>");
            html.Append("<th scope='col'>Quiz / course</th>");
            html.Append("<th scope='col'>Submitted</th>");
            html.Append("<th scope='col'>Score</th>");
            html.Append("<th scope='col'>Review</th>");
            html.Append("</tr></thead>");

            html.Append("<tbody>");

            foreach (DataRow attempt in attempts.Rows)
            {
                bool objective =
                    Convert.ToString(attempt["quiz_type"]) == "OBJ";

                string quizType = objective
                    ? "Objective"
                    : "Reflection";

                string submittedDate = Convert.ToDateTime(
                    attempt["submitted_at"]
                ).ToString("dd MMM yyyy, HH:mm");

                html.Append("<tr data-review>");

                html.Append("<td>");
                html.Append("<strong>");
                html.Append(H(attempt["quiz"]));
                html.Append("</strong>");

                html.Append("<small>");
                html.Append(H(attempt["course"]));
                html.Append(" · ");
                html.Append(quizType);
                html.Append("</small>");
                html.Append("</td>");

                html.Append("<td>");
                html.Append(submittedDate);
                html.Append("</td>");

                html.Append("<td>");
                html.Append("<span class='pill'>");
                html.Append(GetScoreText(attempt));
                html.Append("</span>");
                html.Append("</td>");

                html.Append("<td>");

                html.Append(
                    "<a class='text-link' " +
                    "href='ReviewQuiz.aspx?attempt=" +
                    H(attempt["attempt_id"]) +
                    "'>"
                );

                html.Append("Review →</a>");
                html.Append("</td>");

                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");

            html.Append(
                "<p id='reviewEmpty' class='empty' hidden>" +
                "No attempts match your search." +
                "</p>"
            );

            html.Append("</div>");

            History.Text = html.ToString();
        }

        private void DisplayAttemptReview(
            DataTable attempts,
            int attemptId
        )
        {
            DataRow selectedAttempt = null;

            // The table only contains this student's submitted attempts.
            foreach (DataRow attempt in attempts.Rows)
            {
                if (
                    Convert.ToInt32(attempt["attempt_id"]) ==
                    attemptId
                )
                {
                    selectedAttempt = attempt;
                    break;
                }
            }

            if (selectedAttempt == null)
            {
                throw new HttpException(
                    404,
                    "Submitted attempt not found."
                );
            }

            DisplaySelectedAttemptSummary(selectedAttempt);
            DisplayAnswers(attemptId, selectedAttempt);
        }

        private void DisplaySelectedAttemptSummary(DataRow attempt)
        {
            string submittedDate = Convert.ToDateTime(
                attempt["submitted_at"]
            ).ToString("dd MMM yyyy, HH:mm");

            StringBuilder html = new StringBuilder();

            html.Append(
                "<a class='text-link' href='ReviewQuiz.aspx'>" +
                "← All attempts" +
                "</a>"
            );

            html.Append("<section class='panel review-summary'>");
            html.Append("<div>");

            html.Append("<span class='eyebrow'>");
            html.Append(H(attempt["course"]));
            html.Append("</span>");

            html.Append("<h2>");
            html.Append(H(attempt["quiz"]));
            html.Append("</h2>");

            html.Append("<p class='muted'>Submitted ");
            html.Append(submittedDate);
            html.Append("</p>");

            html.Append("</div>");

            bool graded =
                Convert.ToString(attempt["quiz_type"]) != "SUBJ" &&
                attempt["score"] != DBNull.Value;

            if (graded)
            {
                // Ring gauge, filled by Scripts/quiz-result.js
                html.Append(
                    "<div class='score-ring' data-score-ring style='--score:" +
                    DojoData.Num(attempt["score"]) +
                    "'><svg viewBox='0 0 120 120' aria-hidden='true'>" +
                    "<circle cx='60' cy='60' r='52'></circle>" +
                    "<circle class='score-ring-fill' cx='60' cy='60' r='52'></circle></svg>" +
                    "<strong>" + DojoData.Num(attempt["score"]) + "%</strong>" +
                    "<small>" + H(DojoData.Band(attempt["score"])) + "</small></div>"
                );
            }
            else
            {
                html.Append("<strong class='score-big'>");
                html.Append(GetScoreText(attempt));
                html.Append("</strong>");
            }

            html.Append("</section>");

            if (Request.QueryString["fresh"] == "1")
            {
                html.Append(ResultCelebration(attempt, graded));
            }

            Summary.Text = html.ToString();
        }

        // Full-screen result card shown once, straight after submitting.
        private string ResultCelebration(DataRow attempt, bool graded)
        {
            int quizId = Convert.ToInt32(attempt["quiz_id"]);
            bool canRetry = DojoData.CanAttemptQuiz(UserId, quizId);
            decimal score = graded ? Convert.ToDecimal(attempt["score"]) : 0m;

            string mood;
            string icon;
            string title;
            string message;

            if (!graded)
            {
                mood = "reflect";
                icon = "✍";
                title = "Reflection submitted!";
                message = "Thoughtful answers build real understanding. Compare yours with the model answers below.";
            }
            else if (score >= DojoData.DistinctionMark)
            {
                mood = "win";
                icon = "🏆";
                title = score >= 100 ? "Perfect score!" : "Outstanding!";
                message = "You crushed it with a Distinction. That is black-belt level analysis.";
            }
            else if (score >= DojoData.PassMark)
            {
                mood = "pass";
                icon = "✨";
                title = "Nice work, you passed!";
                message = "Solid effort. Review the answers below to push for 80% next time.";
            }
            else
            {
                mood = "fail";
                icon = "💪";
                title = "So close. Try next time!";
                message = "Every sensei failed before they mastered it. Review the answers, then give it another go.";
            }

            StringBuilder html = new StringBuilder();

            html.Append(
                "<div class='result-scrim' data-result data-mood='" + mood + "' role='dialog' aria-modal='true' aria-labelledby='resultTitle'>"
            );
            html.Append("<canvas class='result-confetti' data-confetti aria-hidden='true'></canvas>");
            html.Append("<section class='result-card'>");
            html.Append("<span class='result-icon' aria-hidden='true'>" + icon + "</span>");
            html.Append("<span class='result-kicker'>" + H(attempt["quiz"]) + "</span>");
            html.Append("<h2 id='resultTitle'>" + title + "</h2>");

            if (graded)
            {
                html.Append(
                    "<div class='result-score'><strong data-count-to='" +
                    DojoData.Num(score) + "'>0</strong><span>%</span></div>"
                );

                if (attempt["score_earned"] != DBNull.Value)
                {
                    html.Append(
                        "<p class='result-detail'>" +
                        DojoData.Num(attempt["score_earned"]) + " of " + H(attempt["total"]) +
                        " correct · " + H(DojoData.Band(score)) + "</p>"
                    );
                }
            }

            html.Append("<p class='result-message'>" + message + "</p>");
            html.Append("<div class='result-actions'>");
            html.Append("<button type='button' class='btn' data-result-close>Review answers</button>");

            if (canRetry)
            {
                html.Append(
                    "<a class='btn ghost' href='TakeQuiz.aspx?id=" + quizId + "'>↻ Try again</a>"
                );
            }
            else
            {
                html.Append(
                    "<a class='btn ghost' href='CoursePage.aspx?id=" + H(attempt["course_id"]) + "'>Back to course</a>"
                );
            }

            html.Append("</div></section></div>");

            return html.ToString();
        }

        private void DisplayAnswers(
            int attemptId,
            DataRow attempt
        )
        {
            DataTable answers = DojoData.Query(
                @"
                SELECT
                    q.question_id,
                    q.question_text,
                    q.question_type,
                    s.selected_obj,
                    s.answer_subjective,
                    s.is_correct,
                    a.answer_text AS selected_text,
                    (
                        SELECT TOP 1 answer_text
                        FROM question_answer
                        WHERE question_id = q.question_id
                        ORDER BY
                            CASE WHEN is_correct = 1 THEN 0 ELSE 1 END,
                            answer_id
                    ) AS correct_text
                FROM quiz_questions q
                INNER JOIN quiz_attempts t
                    ON t.quiz_id = q.quiz_id
                OUTER APPLY
                (
                    SELECT TOP 1 *
                    FROM question_submitted z
                    WHERE z.attempt_id = t.attempt_id
                        AND z.question_id = q.question_id
                    ORDER BY submitted_id DESC
                ) s
                LEFT JOIN question_answer a
                    ON a.answer_id = s.selected_obj
                    AND a.question_id = q.question_id
                WHERE t.attempt_id = @attemptId
                    AND t.user_id = @userId
                    AND t.submitted_at IS NOT NULL
                ORDER BY q.question_id",
                DojoData.P("@attemptId", attemptId),
                DojoData.P("@userId", UserId)
            );

            StringBuilder html = new StringBuilder();
            int questionNumber = 0;

            foreach (DataRow answer in answers.Rows)
            {
                questionNumber++;

                bool subjective =
                    Convert.ToString(answer["question_type"]) ==
                    "SUBJECTIVE";

                string status;

                if (subjective)
                {
                    status = "Ungraded reflection";
                }
                else if (answer["is_correct"] == DBNull.Value)
                {
                    status = "Not graded";
                }
                else
                {
                    status = Convert.ToBoolean(answer["is_correct"])
                        ? "Correct"
                        : "Incorrect";
                }

                string studentAnswer = Convert.ToString(
                    subjective
                        ? answer["answer_subjective"]
                        : answer["selected_text"]
                );

                if (string.IsNullOrWhiteSpace(studentAnswer))
                {
                    studentAnswer = "No answer recorded";
                }

                html.Append("<article class='panel answer-card'>");

                html.Append("<div class='section-head'>");

                html.Append("<span class='eyebrow'>QUESTION ");
                html.Append(questionNumber);
                html.Append("</span>");

                html.Append(
                    "<span class='pill" +
                    (status == "Correct" ? " pill-correct" : status == "Incorrect" ? " pill-wrong" : "") +
                    "'>"
                );
                html.Append(status == "Correct" ? "✓ " : status == "Incorrect" ? "✕ " : "");
                html.Append(status);
                html.Append("</span>");

                html.Append("</div>");

                html.Append("<h3>");
                html.Append(H(answer["question_text"]));
                html.Append("</h3>");

                html.Append("<div class='answer-box'>");
                html.Append("<small>Your answer</small>");
                html.Append("<p>");
                html.Append(H(studentAnswer));
                html.Append("</p>");
                html.Append("</div>");

                string correctAnswer =
                    answer["correct_text"] == DBNull.Value
                        ? "No model answer available"
                        : Convert.ToString(answer["correct_text"]);

                html.Append("<div class='answer-box correct'>");
                html.Append("<small>");
                html.Append(subjective ? "Model answer" : "Correct answer");
                html.Append("</small>");
                html.Append("<p>");
                html.Append(H(correctAnswer));
                html.Append("</p>");
                html.Append("</div>");

                html.Append("</article>");
            }

            if (answers.Rows.Count == 0)
            {
                html.Append(
                    "<p class='empty'>" +
                    "No questions are available for this attempt." +
                    "</p>"
                );
            }

            int quizId = Convert.ToInt32(attempt["quiz_id"]);

            if (DojoData.CanAttemptQuiz(UserId, quizId))
            {
                html.Append(
                    "<a class='btn' href='TakeQuiz.aspx?id=" +
                    H(quizId) +
                    "'>Reattempt →</a>"
                );
            }
            else
            {
                html.Append(
                    "<p class='muted'>Reattempt is closed. Only failed MCQ / true-false quizzes can be taken again. Subjective questions have no mark and cannot be retaken after submit.</p>"
                );
            }

            bool objective = Convert.ToString(attempt["quiz_type"]) == "OBJ";

            if (objective)
            {
                html.Append("<section class='panel' style='margin-top:24px'><h2>Quiz leaderboard</h2>");
                html.Append(DojoUi.LeaderboardList(
                    DojoData.Leaderboard(quizId),
                    UserId
                ));
                html.Append("</section>");
            }

            Answers.Text = html.ToString();
        }

        private string GetScoreText(DataRow attempt)
        {
            if (
                Convert.ToString(attempt["quiz_type"]) == "SUBJ"
            )
            {
                return "Ungraded";
            }

            if (attempt["score"] == DBNull.Value)
            {
                return "Not graded";
            }

            return DojoData.Num(attempt["score"]) + "% · " + DojoData.Band(attempt["score"]);
        }
    }
}