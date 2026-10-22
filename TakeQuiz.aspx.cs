using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class TakeQuiz : DojoPage
    {
        public int CourseId;

        private int quizId;
        private string quizType;
        private DataTable questionTable;

        private readonly Dictionary<int, Control> answerControls =
            new Dictionary<int, Control>();

        protected override void OnInit(EventArgs e)
        {
            // Run authentication and request validation first.
            base.OnInit(e);

            quizId = QueryId("id");

            LoadQuiz();

            // Dynamic controls must be recreated during Init
            // so ASP.NET can restore submitted answers.
            CreateQuestionControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string token = Guid.NewGuid().ToString("N");

                ViewState["submissionToken"] = token;

                // Store the start time on the server.
                Session["quiz:" + token] = DateTime.Now;
            }
        }

        private void LoadQuiz()
        {
            DataTable quizTable = DojoData.Query(
                @"
                SELECT
                    q.quiz_id,
                    q.quiz_title,
                    q.description,
                    q.quiz_type,
                    ch.course_id
                FROM quizzes q
                INNER JOIN course_chapters ch
                    ON ch.chapter_id = q.chapter_id
                INNER JOIN course_enrollments e
                    ON e.course_id = ch.course_id
                WHERE q.quiz_id = @quizId
                    AND e.user_id = @userId",
                DojoData.P("@quizId", quizId),
                DojoData.P("@userId", UserId)
            );

            if (quizTable.Rows.Count == 0)
            {
                throw new HttpException(
                    404,
                    "This quiz was not found in your courses."
                );
            }

            DataRow quiz = quizTable.Rows[0];

            CourseId = Convert.ToInt32(quiz["course_id"]);
            quizType = Convert.ToString(quiz["quiz_type"]);

            if (!DojoData.CanAttemptQuiz(UserId, quizId))
            {
                DataRow latest = DojoData.LatestAttempt(UserId, quizId);

                if (latest != null)
                {
                    Response.Redirect(
                        "ReviewQuiz.aspx?attempt=" + latest["attempt_id"],
                        true
                    );
                    return;
                }

                Response.Redirect("CoursePage.aspx?id=" + CourseId, true);
                return;
            }

            Intro.Text =
                "<h1>" +
                H(quiz["quiz_title"]) +
                "</h1>" +
                "<p class='muted'>" +
                H(quiz["description"]) +
                "</p>";
        }

        private void CreateQuestionControls()
        {
            questionTable = DojoData.Query(
                @"
                SELECT
                    question_id,
                    question_text,
                    question_type
                FROM quiz_questions
                WHERE quiz_id = @quizId
                ORDER BY question_id",
                DojoData.P("@quizId", quizId)
            );

            int questionNumber = 0;

            foreach (DataRow question in questionTable.Rows)
            {
                questionNumber++;

                int questionId = Convert.ToInt32(
                    question["question_id"]
                );

                string questionText = Convert.ToString(
                    question["question_text"]
                );

                bool subjective =
                    Convert.ToString(question["question_type"]) ==
                    "SUBJECTIVE";

                Questions.Controls.Add(
                    new LiteralControl(
                        "<fieldset class='panel question'>" +
                        "<legend>Question " +
                        questionNumber +
                        "</legend>" +
                        "<h2>" +
                        H(questionText) +
                        "</h2>"
                    )
                );

                if (subjective)
                {
                    AddSubjectiveQuestion(
                        questionId,
                        questionText
                    );
                }
                else
                {
                    AddObjectiveQuestion(questionId);
                }

                Questions.Controls.Add(
                    new LiteralControl("</fieldset>")
                );
            }

            Submit.Visible = questionTable.Rows.Count > 0;

            if (!Submit.Visible)
            {
                ShowError("No questions are available yet.");
            }
        }

        private void AddSubjectiveQuestion(
            int questionId,
            string questionText
        )
        {
            TextBox answerBox = new TextBox
            {
                ID = "q" + questionId,
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                CssClass = "input"
            };

            answerBox.Attributes["aria-label"] = questionText;
            answerBox.Attributes["maxlength"] = "4000";

            Questions.Controls.Add(answerBox);

            answerControls.Add(
                questionId,
                answerBox
            );
        }

        private void AddObjectiveQuestion(int questionId)
        {
            DataTable options = DojoData.Query(
                @"
                SELECT
                    answer_id,
                    answer_text
                FROM question_answer
                WHERE question_id = @questionId
                ORDER BY answer_id",
                DojoData.P("@questionId", questionId)
            );

            RadioButtonList answerList = new RadioButtonList
            {
                ID = "q" + questionId,
                CssClass = "quiz-options",
                RepeatLayout = RepeatLayout.Flow,
                RepeatDirection = RepeatDirection.Vertical
            };

            foreach (DataRow option in options.Rows)
            {
                answerList.Items.Add(
                    new ListItem(
                        H(option["answer_text"]),
                        Convert.ToString(option["answer_id"])
                    )
                );
            }

            Questions.Controls.Add(answerList);

            answerControls.Add(
                questionId,
                answerList
            );
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            string token = Convert.ToString(
                ViewState["submissionToken"]
            );

            object savedState = Session["quiz:" + token];

            // This form was already submitted successfully.
            if (savedState is int)
            {
                RedirectToReview((int)savedState);
                return;
            }

            if (!(savedState is DateTime))
            {
                ShowError(
                    "This quiz session has expired. " +
                    "Open the quiz again to begin."
                );

                return;
            }

            if (questionTable.Rows.Count == 0)
            {
                ShowError("There are no questions to submit.");
                return;
            }

            if (!ValidateAnswers())
            {
                return;
            }

            DateTime startedAt = (DateTime)savedState;

            int attemptId = SaveAttempt(startedAt);

            // Remember the saved attempt to prevent duplicate
            // submissions from the same quiz form.
            Session["quiz:" + token] = attemptId;

            RedirectToReview(attemptId);
        }

        private bool ValidateAnswers()
        {
            foreach (
                KeyValuePair<int, Control> entry in answerControls
            )
            {
                TextBox answerBox = entry.Value as TextBox;

                if (answerBox != null)
                {
                    if (
                        string.IsNullOrWhiteSpace(answerBox.Text) ||
                        answerBox.Text.Length > 4000
                    )
                    {
                        ShowError(
                            "Answer every reflection using " +
                            "1–4,000 characters."
                        );

                        return false;
                    }
                }
                else
                {
                    RadioButtonList answerList =
                        (RadioButtonList)entry.Value;

                    if (
                        string.IsNullOrEmpty(
                            answerList.SelectedValue
                        )
                    )
                    {
                        ShowError(
                            "Please answer every question " +
                            "before submitting."
                        );

                        return false;
                    }
                }
            }

            return true;
        }

        private int SaveAttempt(DateTime startedAt)
        {
            using (
                SqlConnection connection =
                    DojoDb.GetConnection()
            )
            {
                connection.Open();

                using (
                    SqlTransaction transaction =
                        connection.BeginTransaction()
                )
                {
                    int enrollmentId = DojoData.LockEnrollment(
                        connection,
                        transaction,
                        UserId,
                        CourseId
                    );

                    int attemptId = Convert.ToInt32(
                        DojoData.Scalar(
                            connection,
                            transaction,
                            @"
                            INSERT INTO quiz_attempts
                            (
                                quiz_id,
                                user_id,
                                started_at
                            )
                            OUTPUT INSERTED.attempt_id
                            VALUES
                            (
                                @quizId,
                                @userId,
                                @startedAt
                            )",
                            DojoData.P("@quizId", quizId),
                            DojoData.P("@userId", UserId),
                            DojoData.P("@startedAt", startedAt)
                        )
                    );

                    int totalMarks = 0;
                    int scoredQuestions = 0;

                    foreach (
                        KeyValuePair<int, Control> entry
                        in answerControls
                    )
                    {
                        int questionId = entry.Key;

                        object selectedAnswer = DBNull.Value;
                        object subjectiveAnswer = DBNull.Value;
                        object isCorrect = DBNull.Value;

                        TextBox answerBox =
                            entry.Value as TextBox;

                        if (answerBox != null)
                        {
                            subjectiveAnswer =
                                answerBox.Text.Trim();
                        }
                        else
                        {
                            RadioButtonList answerList =
                                (RadioButtonList)entry.Value;

                            int answerId = Convert.ToInt32(
                                answerList.SelectedValue
                            );

                            bool correct = CheckObjectiveAnswer(
                                connection,
                                transaction,
                                questionId,
                                answerId
                            );

                            selectedAnswer = answerId;
                            isCorrect = correct;
                            scoredQuestions++;

                            if (correct)
                            {
                                totalMarks++;
                            }
                        }

                        DojoData.Execute(
                            connection,
                            transaction,
                            @"
                            INSERT INTO question_submitted
                            (
                                attempt_id,
                                question_id,
                                selected_obj,
                                answer_subjective,
                                is_correct
                            )
                            VALUES
                            (
                                @attemptId,
                                @questionId,
                                @selectedAnswer,
                                @subjectiveAnswer,
                                @isCorrect
                            )",
                            DojoData.P("@attemptId", attemptId),
                            DojoData.P("@questionId", questionId),
                            DojoData.P(
                                "@selectedAnswer",
                                selectedAnswer
                            ),
                            DojoData.P(
                                "@subjectiveAnswer",
                                subjectiveAnswer
                            ),
                            DojoData.P("@isCorrect", isCorrect)
                        );
                    }

                    object finalScore = scoredQuestions > 0
                        ? (object)totalMarks
                        : DBNull.Value;

                    DojoData.Execute(
                        connection,
                        transaction,
                        @"
                        UPDATE quiz_attempts
                        SET
                            submitted_at = SYSDATETIME(),
                            score_earned = @score
                        WHERE attempt_id = @attemptId",
                        DojoData.P("@score", finalScore),
                        DojoData.P("@attemptId", attemptId)
                    );

                    DojoData.Refresh(
                        connection,
                        transaction,
                        UserId,
                        CourseId,
                        enrollmentId
                    );

                    transaction.Commit();

                    return attemptId;
                }
            }
        }

        private bool CheckObjectiveAnswer(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId,
            int answerId
        )
        {
            // Validate that the selected option belongs to
            // the current question and quiz.
            object result = DojoData.Scalar(
                connection,
                transaction,
                @"
                SELECT a.is_correct
                FROM question_answer a
                INNER JOIN quiz_questions q
                    ON q.question_id = a.question_id
                WHERE a.answer_id = @answerId
                    AND a.question_id = @questionId
                    AND q.quiz_id = @quizId",
                DojoData.P("@answerId", answerId),
                DojoData.P("@questionId", questionId),
                DojoData.P("@quizId", quizId)
            );

            if (result == null || result == DBNull.Value)
            {
                throw new HttpException(
                    409,
                    "An answer key is missing or has changed. " +
                    "Please ask your administrator to check the quiz."
                );
            }

            int correctOptions = Convert.ToInt32(
                DojoData.Scalar(
                    connection,
                    transaction,
                    @"
                    SELECT COUNT(*)
                    FROM question_answer
                    WHERE question_id = @questionId
                        AND is_correct = 1",
                    DojoData.P("@questionId", questionId)
                )
            );

            if (correctOptions != 1)
            {
                throw new HttpException(
                    409,
                    "Each objective question must have " +
                    "exactly one correct option."
                );
            }

            return Convert.ToBoolean(result);
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = H(message);
            ErrorMessage.Visible = true;
        }

        private void RedirectToReview(int attemptId)
        {
            Response.Redirect(
                "ReviewQuiz.aspx?attempt=" + attemptId,
                false
            );

            Context.ApplicationInstance.CompleteRequest();
        }
    }
}