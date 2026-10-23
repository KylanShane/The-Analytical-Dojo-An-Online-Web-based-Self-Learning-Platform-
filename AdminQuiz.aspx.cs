using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminQuiz : System.Web.UI.Page
    {
        // Name of the answers table. Change this if your table is named differently.
        private const string AnswerTable = "question_answer";

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                // The View window only stays open for the View click itself.
                pnlView.Visible = false;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["created"] == "1")
                {
                    ShowMessage("Quiz created successfully.", true);
                }

                if (Request.QueryString["updated"] == "1")
                {
                    ShowMessage("Quiz updated successfully.", true);
                }

                LoadQuizzes();
            }
        }

        // ------------------------------------------------------------
        // Quiz index
        // ------------------------------------------------------------

        private void LoadQuizzes()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        qz.quiz_id,
                        qz.quiz_title,
                        qz.quiz_type,
                        qz.created_at,
                        c.chapter_title,
                        co.course_title,

                        (
                            SELECT COUNT(*)
                            FROM quiz_questions x
                            WHERE x.quiz_id = qz.quiz_id
                        ) AS question_count

                    FROM quizzes qz

                    INNER JOIN course_chapters c
                        ON qz.chapter_id = c.chapter_id

                    INNER JOIN courses co
                        ON c.course_id = co.course_id

                    WHERE
                        (
                            @search = ''
                            OR qz.quiz_title LIKE '%' + @search + '%'
                            OR co.course_title LIKE '%' + @search + '%'
                            OR c.chapter_title LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @type = ''
                            OR qz.quiz_type = @type
                        )

                    ORDER BY qz.quiz_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@search", txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@type", ddlTypeFilter.SelectedValue);

                DataTable dt = new DataTable();

                new SqlDataAdapter(cmd).Fill(dt);

                int questions = 0;

                foreach (DataRow row in dt.Rows)
                {
                    questions += Convert.ToInt32(row["question_count"]);
                }

                lblIndexSummary.Text =
                    dt.Rows.Count + (dt.Rows.Count == 1 ? " quiz" : " quizzes") +
                    " \u00B7 " +
                    questions + (questions == 1 ? " question" : " questions");

                gvQuizzes.DataSource = dt;
                gvQuizzes.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadQuizzes();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlTypeFilter.SelectedIndex = 0;

            LoadQuizzes();
        }

        // ------------------------------------------------------------
        // Row actions
        // ------------------------------------------------------------

        protected void gvQuizzes_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int quizId;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out quizId))
            {
                return;
            }

            if (e.CommandName == "ViewQuiz")
            {
                ShowViewPanel(quizId);
            }
            else if (e.CommandName == "UpdateQuiz")
            {
                Response.Redirect(
                    "AdminCreateQuiz.aspx?id=" + quizId,
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            else if (e.CommandName == "DeleteQuiz")
            {
                DeleteQuiz(quizId);

                LoadQuizzes();
            }
        }

        private void ShowViewPanel(int quizId)
        {
            DataTable quiz = new DataTable();
            DataTable questions = new DataTable();

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string quizQuery = @"
                    SELECT
                        qz.quiz_title,
                        qz.description,
                        qz.quiz_type,
                        qz.created_at,
                        c.chapter_title,
                        co.course_title,

                        COALESCE(
                            u.full_name,
                            u.username,
                            'Unknown User'
                        ) AS creator_name

                    FROM quizzes qz

                    INNER JOIN course_chapters c
                        ON qz.chapter_id = c.chapter_id

                    INNER JOIN courses co
                        ON c.course_id = co.course_id

                    LEFT JOIN users u
                        ON qz.created_by = u.user_id

                    WHERE qz.quiz_id = @quizId";

                SqlDataAdapter quizAdapter =
                    new SqlDataAdapter(quizQuery, con);

                quizAdapter.SelectCommand.Parameters
                    .AddWithValue("@quizId", quizId);

                quizAdapter.Fill(quiz);

                string questionQuery = @"
                    SELECT
                        q.question_id,
                        q.question_text,
                        q.question_type,
                        a.answer_text,
                        a.is_correct

                    FROM quiz_questions q

                    LEFT JOIN " + AnswerTable + @" a
                        ON a.question_id = q.question_id

                    WHERE q.quiz_id = @quizId

                    ORDER BY q.question_id, a.answer_id";

                SqlDataAdapter questionAdapter =
                    new SqlDataAdapter(questionQuery, con);

                questionAdapter.SelectCommand.Parameters
                    .AddWithValue("@quizId", quizId);

                questionAdapter.Fill(questions);
            }

            if (quiz.Rows.Count == 0)
            {
                ShowMessage("The quiz could not be found.", false);
                return;
            }

            DataRow row = quiz.Rows[0];

            string createdAt =
                row["created_at"] == DBNull.Value
                ? "-"
                : Convert.ToDateTime(row["created_at"]).ToString("dd MMM yyyy");

            StringBuilder sb = new StringBuilder();

            sb.Append("<h4>Quiz information</h4>");
            sb.Append("<div class='detail-grid'>");

            AppendField(sb, "QUIZ TITLE", HttpUtility.HtmlEncode(row["quiz_title"].ToString()));

            AppendField(
                sb,
                "TYPE",
                "<span class='" + GetTypeClass(row["quiz_type"]) + "'>" +
                HttpUtility.HtmlEncode(FormatType(row["quiz_type"])) + "</span>"
            );

            AppendField(sb, "COURSE", HttpUtility.HtmlEncode(row["course_title"].ToString()));
            AppendField(sb, "CHAPTER", HttpUtility.HtmlEncode(row["chapter_title"].ToString()));
            AppendField(sb, "CREATED BY", HttpUtility.HtmlEncode(row["creator_name"].ToString()));
            AppendField(sb, "CREATED", HttpUtility.HtmlEncode(createdAt));

            AppendField(
                sb,
                "DESCRIPTION",
                HttpUtility.HtmlEncode(
                    row["description"] == DBNull.Value || row["description"].ToString().Trim().Length == 0
                    ? "-"
                    : row["description"].ToString()
                ),
                true
            );

            sb.Append("</div>");

            sb.Append("<h4>Questions</h4>");
            sb.Append(RenderQuestions(questions));

            litViewContent.Text = sb.ToString();
            pnlView.Visible = true;
        }

        private static string RenderQuestions(DataTable rows)
        {
            if (rows.Rows.Count == 0)
            {
                return "<p class='detail-empty'>No questions have been added to this quiz.</p>";
            }

            StringBuilder sb = new StringBuilder();

            int lastQuestionId = -1;
            int number = 0;
            bool listOpen = false;

            foreach (DataRow row in rows.Rows)
            {
                int questionId = Convert.ToInt32(row["question_id"]);

                if (questionId != lastQuestionId)
                {
                    if (lastQuestionId != -1)
                    {
                        sb.Append(listOpen ? "</ul></div>" : "</div>");
                    }

                    number++;
                    lastQuestionId = questionId;
                    listOpen = false;

                    string type = row["question_type"].ToString().ToUpperInvariant();

                    string typeLabel =
                        type == "TRUE_FALSE" ? "True / False"
                        : type == "SUBJECTIVE" ? "Subjective"
                        : "MCQ";

                    sb.Append("<div class='detail-chapter'>");

                    sb.Append("<div class='detail-chapter-head'><span class='chapter-no'>Question ");
                    sb.Append(number);
                    sb.Append("</span><span class='badge level-basic'>");
                    sb.Append(HttpUtility.HtmlEncode(typeLabel));
                    sb.Append("</span></div>");

                    sb.Append("<p>");
                    sb.Append(HttpUtility.HtmlEncode(row["question_text"].ToString()));
                    sb.Append("</p>");

                    if (type == "SUBJECTIVE")
                    {
                        sb.Append("<p class='detail-empty'>Written answer, marked manually.</p>");
                    }
                }

                if (row["answer_text"] == DBNull.Value)
                {
                    continue;
                }

                if (!listOpen)
                {
                    sb.Append("<ul class='answer-list'>");
                    listOpen = true;
                }

                bool correct =
                    row["is_correct"] != DBNull.Value &&
                    Convert.ToBoolean(row["is_correct"]);

                sb.Append(correct ? "<li class='correct'>&#10003; " : "<li>");
                sb.Append(HttpUtility.HtmlEncode(row["answer_text"].ToString()));
                sb.Append("</li>");
            }

            sb.Append(listOpen ? "</ul></div>" : "</div>");

            return sb.ToString();
        }

        private static void AppendField(
            StringBuilder sb,
            string label,
            string htmlValue,
            bool fullWidth = false)
        {
            sb.Append(fullWidth ? "<div class='detail-full'>" : "<div>");
            sb.Append("<span class='detail-label'>");
            sb.Append(label);
            sb.Append("</span><span class='detail-value'>");
            sb.Append(htmlValue);
            sb.Append("</span></div>");
        }

        private void DeleteQuiz(int quizId)
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        try
                        {
                            string query =
                                "DELETE FROM " + AnswerTable + @"
                                 WHERE question_id IN
                                 (
                                     SELECT question_id
                                     FROM quiz_questions
                                     WHERE quiz_id = @quizId
                                 );

                                 DELETE FROM quiz_questions WHERE quiz_id = @quizId;

                                 DELETE FROM quizzes WHERE quiz_id = @quizId;";

                            using (SqlCommand cmd =
                                new SqlCommand(query, con, tx))
                            {
                                cmd.Parameters.AddWithValue("@quizId", quizId);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    "SELECT COUNT(*) FROM quizzes WHERE quiz_id = @quizId",
                                    con, tx))
                            {
                                cmd.Parameters.AddWithValue("@quizId", quizId);

                                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                {
                                    tx.Rollback();

                                    ShowMessage("The quiz could not be deleted.", false);

                                    return;
                                }
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                ShowMessage("Quiz deleted successfully.", true);
            }
            catch (SqlException ex)
            {
                // 547 = still referenced elsewhere (e.g. student attempts)
                ShowMessage(
                    ex.Number == 547
                        ? "This quiz cannot be deleted because students have already attempted it."
                        : "The quiz could not be deleted.",
                    false
                );
            }
        }

        // ------------------------------------------------------------
        // Display helpers
        // ------------------------------------------------------------

        public string FormatType(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "-";
            }

            return value.ToString().ToUpperInvariant() == "SUBJ"
                ? "Subjective"
                : "Objective";
        }

        public string GetTypeClass(object value)
        {
            if (value != null &&
                value != DBNull.Value &&
                value.ToString().ToUpperInvariant() == "SUBJ")
            {
                return "badge level-advanced";
            }

            return "badge level-basic";
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;

            lblMessage.Visible = true;

            lblMessage.CssClass = success
                ? "feedback-message success-message"
                : "feedback-message error-message";

            ClientScript.RegisterStartupScript(
                GetType(),
                "scrollTop",
                "window.addEventListener('load', function () { setTimeout(function () { window.scrollTo({ top: 0, behavior: 'smooth' }); }, 50); });",
                true
            );
        }
    }
}