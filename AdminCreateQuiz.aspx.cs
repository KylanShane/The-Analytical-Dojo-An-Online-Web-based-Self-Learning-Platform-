using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    // Question Builder: create or edit one quiz with its questions and answers.
    //   quizzes        (quiz_id, chapter_id, quiz_title, description, quiz_type OBJ/SUBJ, created_by, created_at)
    //   quiz_questions (question_id, quiz_id, question_text, question_type MCQ/TRUE_FALSE/SUBJECTIVE)
    //   answers table  (answer_id, question_id, answer_text, is_correct)
    public partial class AdminCreateQuiz : System.Web.UI.Page
    {
        // Name of the answers table. Change this if your table is named differently.
        private const string AnswerTable = "question_answer";

        private const int MaxOptions = 4;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        private class QuestionInput
        {
            public string Index;
            public int QuestionId;              // 0 = new question
            public string Text;
            public string Format;               // MCQ or TRUE_FALSE (objective quizzes)
            public string[] Options = new string[MaxOptions];
            public int CorrectIndex = -1;       // MCQ
            public string TfCorrect = "";       // TRUE or FALSE
        }

        private class AnswerValue
        {
            public int AnswerId;
            public string Text;
            public bool Correct;
        }

        private class ExistingQuestion
        {
            public int QuestionId;
            public string Text;
            public string Type;
            public List<AnswerValue> Answers = new List<AnswerValue>();
        }

        private class QuizRow
        {
            public int QuizId;
            public int ChapterId;
            public int CourseId;
            public string Title;
            public string Description;
            public string Type;                 // OBJ or SUBJ
        }

        private QuizRow existingQuiz;
        private bool existingQuizLoaded;
        private List<ExistingQuestion> existingQuestions;
        private List<QuestionInput> postedQuestions;
        private readonly Dictionary<string, bool> identityCache =
            new Dictionary<string, bool>();

        // One question card. Tokens are replaced in a single pass.
        private const string QuestionTemplate = @"
<div class='question' data-format='{FORMAT}'>
    <input type='hidden' name='q_idx' value='{IDX}' />
    <input type='hidden' name='q_id_{IDX}' value='{QID}' />

    <div class='q-head'>
        <span class='q-num'>{NUM}</span>
        <button type='button' class='q-remove'>Delete</button>
    </div>

    <div class='form-group'>
        <label>Question <span class='required'>*</span></label>
        <textarea name='q_text_{IDX}' class='form-input form-textarea q-text' rows='3'>{TEXT}</textarea>
    </div>

    <div class='obj-only'>

        <div class='form-group'>
            <label>Question Format</label>
            <div class='segmented'>
                <span>
                    <input type='radio' id='qf_{IDX}_mcq' name='q_fmt_{IDX}' value='MCQ' {MCQ_CHECKED} />
                    <label for='qf_{IDX}_mcq'>MCQ</label>
                </span>
                <span>
                    <input type='radio' id='qf_{IDX}_tf' name='q_fmt_{IDX}' value='TRUE_FALSE' {TF_CHECKED} />
                    <label for='qf_{IDX}_tf'>True / False</label>
                </span>
            </div>
        </div>

        <div class='form-group mcq-only'>
            <label>Answer Options <span class='required'>*</span></label>
            {OPTIONS}
            <span class='field-hint'>Type the four options and mark the correct one.</span>
        </div>

        <div class='form-group tf-only'>
            <label>Correct Answer <span class='required'>*</span></label>
            <div class='segmented'>
                <span>
                    <input type='radio' id='qt_{IDX}_true' name='q_tf_{IDX}' value='TRUE' {TRUE_CHECKED} />
                    <label for='qt_{IDX}_true'>True</label>
                </span>
                <span>
                    <input type='radio' id='qt_{IDX}_false' name='q_tf_{IDX}' value='FALSE' {FALSE_CHECKED} />
                    <label for='qt_{IDX}_false'>False</label>
                </span>
            </div>
        </div>

    </div>

    <div class='subj-only'>
        <p class='field-hint'>Subjective questions are answered in free text and marked manually.</p>
    </div>
</div>";

        // 0 = creating a new quiz, otherwise the quiz being edited.
        private int EditQuizId
        {
            get
            {
                int id;

                return int.TryParse(Request.QueryString["id"], out id) && id > 0
                    ? id
                    : 0;
            }
        }

        private bool IsEdit
        {
            get { return EditQuizId > 0; }
        }

        // Used by the page markup so the cards show the right fields.
        protected string CurrentQuizType
        {
            get
            {
                if (IsEdit)
                {
                    QuizRow quiz = GetQuiz();

                    if (quiz != null)
                    {
                        return quiz.Type;
                    }
                }

                return rblQuizType.SelectedValue == "SUBJ" ? "SUBJ" : "OBJ";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litQuestionTemplate.Text =
                BuildQuestionHtml(BlankQuestion("__IDX__"), "__NUM__");

            if (IsEdit)
            {
                Page.Title = "Update Quiz - The Analytical Dojo";
                litBreadcrumb.Text = "Update Quiz";
                litSubtitle.Text = "Update the quiz details and its questions.";
                litActionNote.Text = "EDITING QUIZ #" + EditQuizId + " &middot; UNSAVED CHANGES";
                btnSave.Text = "Save Changes";

                // Converting a quiz between objective and subjective is not supported.
                rblQuizType.Enabled = false;
            }

            if (!IsPostBack)
            {
                LoadCourses();

                if (IsEdit)
                {
                    LoadQuizForEdit();
                }
                else
                {
                    LoadChapters(0, null);

                    RenderQuestions(
                        new List<QuestionInput> { BlankQuestion("1") }
                    );
                }

                return;
            }

            // Postback: rebuild the question cards from what was submitted,
            // so nothing is lost when the course dropdown reloads the page.
            Dictionary<int, ExistingQuestion> map = null;

            if (IsEdit)
            {
                existingQuestions = LoadExistingQuestions(EditQuizId);
                map = existingQuestions.ToDictionary(x => x.QuestionId);
            }

            postedQuestions = ReadQuestions(map);

            if (postedQuestions.Count == 0)
            {
                postedQuestions.Add(BlankQuestion("1"));
            }

            RenderQuestions(postedQuestions);
        }

        // ------------------------------------------------------------
        // Course / chapter dropdowns
        // ------------------------------------------------------------

        private void LoadCourses()
        {
            ddlCourse.Items.Clear();
            ddlCourse.Items.Add(new ListItem("Select a course", "0"));

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT course_id, course_title FROM courses ORDER BY course_id",
                        con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlCourse.Items.Add(
                            new ListItem(
                                reader["course_title"].ToString(),
                                reader["course_id"].ToString()
                            )
                        );
                    }
                }
            }
        }

        private void LoadChapters(int courseId, int? selectChapterId)
        {
            ddlChapter.Items.Clear();
            ddlChapter.Items.Add(new ListItem("Select a chapter", "0"));

            if (courseId == 0)
            {
                return;
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT chapter_id, chapter_title
                    FROM course_chapters
                    WHERE course_id = @courseId
                    ORDER BY chapter_order, chapter_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@courseId", courseId);

                con.Open();

                int count = 0;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;

                        ddlChapter.Items.Add(
                            new ListItem(
                                "Chapter " + count + ": " + reader["chapter_title"],
                                reader["chapter_id"].ToString()
                            )
                        );
                    }
                }
            }

            if (selectChapterId.HasValue)
            {
                ListItem item =
                    ddlChapter.Items.FindByValue(selectChapterId.Value.ToString());

                if (item != null)
                {
                    ddlChapter.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        protected void ddlCourse_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadChapters(Convert.ToInt32(ddlCourse.SelectedValue), null);
        }

        // ------------------------------------------------------------
        // Edit mode: load the quiz and its questions
        // ------------------------------------------------------------

        private QuizRow GetQuiz()
        {
            if (existingQuizLoaded)
            {
                return existingQuiz;
            }

            existingQuizLoaded = true;

            if (!IsEdit)
            {
                return null;
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        qz.quiz_id,
                        qz.chapter_id,
                        c.course_id,
                        qz.quiz_title,
                        qz.description,
                        qz.quiz_type

                    FROM quizzes qz

                    INNER JOIN course_chapters c
                        ON qz.chapter_id = c.chapter_id

                    WHERE qz.quiz_id = @quizId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@quizId", EditQuizId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existingQuiz = new QuizRow
                        {
                            QuizId = Convert.ToInt32(reader["quiz_id"]),
                            ChapterId = Convert.ToInt32(reader["chapter_id"]),
                            CourseId = Convert.ToInt32(reader["course_id"]),
                            Title = reader["quiz_title"].ToString(),

                            Description =
                                reader["description"] == DBNull.Value
                                ? ""
                                : reader["description"].ToString(),

                            Type =
                                reader["quiz_type"].ToString().ToUpperInvariant() == "SUBJ"
                                ? "SUBJ"
                                : "OBJ"
                        };
                    }
                }
            }

            return existingQuiz;
        }

        private void LoadQuizForEdit()
        {
            QuizRow quiz = GetQuiz();

            if (quiz == null)
            {
                Response.Redirect("AdminQuiz.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            txtTitle.Text = quiz.Title;
            txtDescription.Text = quiz.Description;

            ListItem typeItem = rblQuizType.Items.FindByValue(quiz.Type);

            if (typeItem != null)
            {
                rblQuizType.ClearSelection();
                typeItem.Selected = true;
            }

            ListItem courseItem =
                ddlCourse.Items.FindByValue(quiz.CourseId.ToString());

            if (courseItem != null)
            {
                ddlCourse.ClearSelection();
                courseItem.Selected = true;
            }

            LoadChapters(quiz.CourseId, quiz.ChapterId);

            List<QuestionInput> cards = new List<QuestionInput>();

            foreach (ExistingQuestion ex in LoadExistingQuestions(EditQuizId))
            {
                QuestionInput q = new QuestionInput
                {
                    Index = (cards.Count + 1).ToString(),
                    QuestionId = ex.QuestionId,
                    Text = ex.Text,
                    Format = ex.Type == "TRUE_FALSE" ? "TRUE_FALSE" : "MCQ"
                };

                if (ex.Type == "TRUE_FALSE")
                {
                    AnswerValue correct = ex.Answers.FirstOrDefault(a => a.Correct);

                    if (correct != null)
                    {
                        q.TfCorrect =
                            correct.Text.Trim().Equals("false", StringComparison.OrdinalIgnoreCase)
                            ? "FALSE"
                            : "TRUE";
                    }
                }
                else
                {
                    for (int i = 0; i < MaxOptions && i < ex.Answers.Count; i++)
                    {
                        q.Options[i] = ex.Answers[i].Text;

                        if (ex.Answers[i].Correct)
                        {
                            q.CorrectIndex = i;
                        }
                    }
                }

                cards.Add(q);
            }

            if (cards.Count == 0)
            {
                cards.Add(BlankQuestion("1"));
            }

            RenderQuestions(cards);
        }

        private List<ExistingQuestion> LoadExistingQuestions(int quizId)
        {
            List<ExistingQuestion> list = new List<ExistingQuestion>();

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        q.question_id,
                        q.question_text,
                        q.question_type,
                        a.answer_id,
                        a.answer_text,
                        a.is_correct

                    FROM quiz_questions q

                    LEFT JOIN " + AnswerTable + @" a
                        ON a.question_id = q.question_id

                    WHERE q.quiz_id = @quizId

                    ORDER BY q.question_id, a.answer_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@quizId", quizId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    ExistingQuestion current = null;

                    while (reader.Read())
                    {
                        int questionId = Convert.ToInt32(reader["question_id"]);

                        if (current == null || current.QuestionId != questionId)
                        {
                            current = new ExistingQuestion
                            {
                                QuestionId = questionId,
                                Text = reader["question_text"].ToString(),
                                Type = reader["question_type"].ToString().ToUpperInvariant()
                            };

                            list.Add(current);
                        }

                        if (reader["answer_id"] != DBNull.Value)
                        {
                            current.Answers.Add(new AnswerValue
                            {
                                AnswerId = Convert.ToInt32(reader["answer_id"]),
                                Text = reader["answer_text"].ToString(),
                                Correct = Convert.ToBoolean(reader["is_correct"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        // ------------------------------------------------------------
        // Question cards: read, validate, render
        // ------------------------------------------------------------

        private QuestionInput BlankQuestion(string index)
        {
            return new QuestionInput
            {
                Index = index,
                Text = "",
                Format = "MCQ"
            };
        }

        private List<QuestionInput> ReadQuestions(
            Dictionary<int, ExistingQuestion> existing)
        {
            List<QuestionInput> list = new List<QuestionInput>();

            string[] indexes = Request.Form.GetValues("q_idx");

            if (indexes == null)
            {
                return list;
            }

            HashSet<int> used = new HashSet<int>();

            foreach (string idx in indexes)
            {
                long parsed;

                // Indexes are generated by the page and are always numeric.
                if (!long.TryParse(idx, out parsed))
                {
                    continue;
                }

                QuestionInput q = new QuestionInput
                {
                    Index = idx,
                    Text = (Request.Form["q_text_" + idx] ?? "").Trim(),

                    Format =
                        (Request.Form["q_fmt_" + idx] ?? "MCQ") == "TRUE_FALSE"
                        ? "TRUE_FALSE"
                        : "MCQ"
                };

                for (int i = 0; i < MaxOptions; i++)
                {
                    q.Options[i] = (Request.Form["q_opt_" + idx + "_" + i] ?? "").Trim();
                }

                int correct;

                if (int.TryParse(Request.Form["q_correct_" + idx], out correct) &&
                    correct >= 0 && correct < MaxOptions)
                {
                    q.CorrectIndex = correct;
                }

                string tf = Request.Form["q_tf_" + idx];

                if (tf == "TRUE" || tf == "FALSE")
                {
                    q.TfCorrect = tf;
                }

                // An existing question must belong to this quiz.
                int questionId;

                if (existing != null &&
                    int.TryParse(Request.Form["q_id_" + idx], out questionId) &&
                    existing.ContainsKey(questionId) &&
                    used.Add(questionId))
                {
                    q.QuestionId = questionId;
                }

                list.Add(q);
            }

            return list;
        }

        private static string DbQuestionType(string quizType, QuestionInput q)
        {
            if (quizType == "SUBJ")
            {
                return "SUBJECTIVE";
            }

            return q.Format == "TRUE_FALSE" ? "TRUE_FALSE" : "MCQ";
        }

        private string ValidateQuestions(
            string quizType,
            List<QuestionInput> questions)
        {
            if (questions.Count == 0)
            {
                return "Add at least one question.";
            }

            for (int i = 0; i < questions.Count; i++)
            {
                QuestionInput q = questions[i];
                string label = "Question " + (i + 1) + ": ";

                if (q.Text.Length == 0)
                {
                    return label + "the question text is required.";
                }

                if (quizType == "SUBJ")
                {
                    continue;
                }

                if (q.Format == "TRUE_FALSE")
                {
                    if (q.TfCorrect.Length == 0)
                    {
                        return label + "choose whether the correct answer is True or False.";
                    }

                    continue;
                }

                for (int o = 0; o < MaxOptions; o++)
                {
                    if (q.Options[o].Length == 0)
                    {
                        return label + "all four answer options are required.";
                    }
                }

                if (q.Options.Select(x => x.ToLowerInvariant()).Distinct().Count() < MaxOptions)
                {
                    return label + "the answer options must all be different.";
                }

                if (q.CorrectIndex < 0)
                {
                    return label + "mark the correct answer.";
                }
            }

            return null;
        }

        private void RenderQuestions(List<QuestionInput> questions)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < questions.Count; i++)
            {
                sb.Append(BuildQuestionHtml(questions[i], (i + 1).ToString("00")));
            }

            litQuestions.Text = sb.ToString();
        }

        private string BuildQuestionHtml(
            QuestionInput q,
            string number)
        {
            StringBuilder options = new StringBuilder();

            for (int i = 0; i < MaxOptions; i++)
            {
                options.Append("<div class='opt-row'>");
                options.Append("<span class='opt-letter'>" + (char)('A' + i) + "</span>");

                options.Append(
                    "<input type='text' name='q_opt_" +
                    HttpUtility.HtmlAttributeEncode(q.Index) + "_" + i +
                    "' class='form-input' maxlength='200' value='" +
                    HttpUtility.HtmlAttributeEncode(q.Options[i] ?? "") + "' />"
                );

                options.Append(
                    "<label class='opt-correct'><input type='radio' name='q_correct_" +
                    HttpUtility.HtmlAttributeEncode(q.Index) + "' value='" + i + "' " +
                    (q.CorrectIndex == i ? "checked='checked'" : "") +
                    " /> Correct</label>"
                );

                options.Append("</div>");
            }

            Dictionary<string, string> tokens = new Dictionary<string, string>
            {
                { "IDX", HttpUtility.HtmlAttributeEncode(q.Index) },
                { "QID", q.QuestionId > 0 ? q.QuestionId.ToString() : "" },
                { "NUM", HttpUtility.HtmlEncode(number) },
                { "FORMAT", q.Format == "TRUE_FALSE" ? "TRUE_FALSE" : "MCQ" },
                { "MCQ_CHECKED", q.Format == "TRUE_FALSE" ? "" : "checked='checked'" },
                { "TF_CHECKED", q.Format == "TRUE_FALSE" ? "checked='checked'" : "" },
                { "TRUE_CHECKED", q.TfCorrect == "TRUE" ? "checked='checked'" : "" },
                { "FALSE_CHECKED", q.TfCorrect == "FALSE" ? "checked='checked'" : "" },
                { "TEXT", HttpUtility.HtmlEncode(q.Text ?? "") },
                { "OPTIONS", options.ToString() }
            };

            // Single pass, so user text can never be mistaken for a placeholder.
            return Regex.Replace(
                QuestionTemplate,
                @"\{([A-Z_]+)\}",
                m => tokens.ContainsKey(m.Groups[1].Value)
                    ? tokens[m.Groups[1].Value]
                    : m.Value
            );
        }

        // ------------------------------------------------------------
        // Save
        // ------------------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int chapterId, courseId;

            int.TryParse(ddlChapter.SelectedValue, out chapterId);
            int.TryParse(ddlCourse.SelectedValue, out courseId);

            if (courseId == 0 || chapterId == 0)
            {
                ShowMessage("Please select a course and a chapter.", false);
                return;
            }

            if (!ChapterBelongsToCourse(chapterId, courseId))
            {
                ShowMessage("The selected chapter does not belong to this course.", false);
                return;
            }

            string quizType;

            if (IsEdit)
            {
                QuizRow quiz = GetQuiz();

                if (quiz == null)
                {
                    ShowMessage("This quiz no longer exists.", false);
                    return;
                }

                quizType = quiz.Type;
            }
            else
            {
                quizType = rblQuizType.SelectedValue == "SUBJ" ? "SUBJ" : "OBJ";
            }

            string error = ValidateQuestions(quizType, postedQuestions);

            if (error != null)
            {
                ShowMessage(error, false);
                return;
            }

            int creatorId = GetCreatorId();

            if (creatorId == 0)
            {
                ShowMessage("No administrator account could be found.", false);
                return;
            }

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
                            if (!IsEdit)
                            {
                                CreateQuizTx(con, tx, chapterId, quizType, creatorId);
                            }
                            else if (!UpdateQuizTx(con, tx, chapterId, quizType))
                            {
                                tx.Rollback();
                                ShowMessage("This quiz no longer exists.", false);
                                return;
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

                Response.Redirect(
                    IsEdit ? "AdminQuiz.aspx?updated=1" : "AdminQuiz.aspx?created=1",
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException)
            {
                ShowMessage(
                    "The quiz could not be saved and nothing was changed. If students have already attempted this quiz, its questions and answers can no longer be removed.",
                    false
                );
            }
        }

        private bool ChapterBelongsToCourse(int chapterId, int courseId)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT COUNT(*) FROM course_chapters WHERE chapter_id = @chapterId AND course_id = @courseId",
                        con);

                cmd.Parameters.AddWithValue("@chapterId", chapterId);
                cmd.Parameters.AddWithValue("@courseId", courseId);

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void CreateQuizTx(
            SqlConnection con,
            SqlTransaction tx,
            int chapterId,
            string quizType,
            int creatorId)
        {
            int quizId = NextId(con, tx, "quizzes", "quiz_id");

            string query = InsertSql(con, tx, "quizzes", @"
                INSERT INTO quizzes
                (
                    quiz_id,
                    chapter_id,
                    quiz_title,
                    description,
                    quiz_type,
                    created_by,
                    created_at
                )
                VALUES
                (
                    @id,
                    @chapterId,
                    @title,
                    @description,
                    @type,
                    @createdBy,
                    GETDATE()
                );");

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@id", quizId);
                cmd.Parameters.AddWithValue("@chapterId", chapterId);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@type", quizType);
                cmd.Parameters.AddWithValue("@createdBy", creatorId);

                cmd.ExecuteNonQuery();
            }

            foreach (QuestionInput q in postedQuestions)
            {
                string dbType = DbQuestionType(quizType, q);

                int questionId = InsertQuestion(con, tx, quizId, q.Text, dbType);

                foreach (AnswerValue a in DesiredAnswers(dbType, q))
                {
                    InsertAnswer(con, tx, questionId, a);
                }
            }
        }

        // Returns false when the quiz no longer exists.
        private bool UpdateQuizTx(
            SqlConnection con,
            SqlTransaction tx,
            int chapterId,
            string quizType)
        {
            using (SqlCommand cmd =
                new SqlCommand(@"
                    UPDATE quizzes

                    SET
                        chapter_id = @chapterId,
                        quiz_title = @title,
                        description = @description

                    WHERE quiz_id = @quizId",
                    con, tx))
            {
                cmd.Parameters.AddWithValue("@chapterId", chapterId);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@quizId", EditQuizId);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    return false;
                }
            }

            // Questions removed on the page: delete their answers, then the question.
            HashSet<int> kept =
                new HashSet<int>(
                    postedQuestions.Where(q => q.QuestionId > 0).Select(q => q.QuestionId)
                );

            foreach (ExistingQuestion ex in existingQuestions)
            {
                if (kept.Contains(ex.QuestionId))
                {
                    continue;
                }

                using (SqlCommand cmd =
                    new SqlCommand(
                        "DELETE FROM " + AnswerTable + " WHERE question_id = @id; " +
                        "DELETE FROM quiz_questions WHERE question_id = @id;",
                        con, tx))
                {
                    cmd.Parameters.AddWithValue("@id", ex.QuestionId);
                    cmd.ExecuteNonQuery();
                }
            }

            Dictionary<int, ExistingQuestion> existingMap =
                existingQuestions.ToDictionary(x => x.QuestionId);

            foreach (QuestionInput q in postedQuestions)
            {
                string dbType = DbQuestionType(quizType, q);
                List<AnswerValue> desired = DesiredAnswers(dbType, q);

                if (q.QuestionId == 0)
                {
                    int questionId = InsertQuestion(con, tx, EditQuizId, q.Text, dbType);

                    foreach (AnswerValue a in desired)
                    {
                        InsertAnswer(con, tx, questionId, a);
                    }

                    continue;
                }

                using (SqlCommand cmd =
                    new SqlCommand(@"
                        UPDATE quiz_questions

                        SET
                            question_text = @text,
                            question_type = @type

                        WHERE question_id = @id
                        AND quiz_id = @quizId",
                        con, tx))
                {
                    cmd.Parameters.AddWithValue("@text", q.Text);
                    cmd.Parameters.AddWithValue("@type", dbType);
                    cmd.Parameters.AddWithValue("@id", q.QuestionId);
                    cmd.Parameters.AddWithValue("@quizId", EditQuizId);
                    cmd.ExecuteNonQuery();
                }

                SyncAnswers(con, tx, q.QuestionId, desired, existingMap[q.QuestionId].Answers);
            }

            return true;
        }

        // Answers are updated in place when the count is unchanged, so rows that
        // other data may refer to keep their IDs. Otherwise they are replaced.
        private void SyncAnswers(
            SqlConnection con,
            SqlTransaction tx,
            int questionId,
            List<AnswerValue> desired,
            List<AnswerValue> current)
        {
            if (current.Count == desired.Count)
            {
                for (int i = 0; i < desired.Count; i++)
                {
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE " + AnswerTable +
                            " SET answer_text = @text, is_correct = @correct WHERE answer_id = @id",
                            con, tx))
                    {
                        cmd.Parameters.AddWithValue("@text", desired[i].Text);
                        cmd.Parameters.AddWithValue("@correct", desired[i].Correct);
                        cmd.Parameters.AddWithValue("@id", current[i].AnswerId);
                        cmd.ExecuteNonQuery();
                    }
                }

                return;
            }

            using (SqlCommand cmd =
                new SqlCommand(
                    "DELETE FROM " + AnswerTable + " WHERE question_id = @id",
                    con, tx))
            {
                cmd.Parameters.AddWithValue("@id", questionId);
                cmd.ExecuteNonQuery();
            }

            foreach (AnswerValue a in desired)
            {
                InsertAnswer(con, tx, questionId, a);
            }
        }

        private static List<AnswerValue> DesiredAnswers(
            string dbType,
            QuestionInput q)
        {
            List<AnswerValue> list = new List<AnswerValue>();

            if (dbType == "TRUE_FALSE")
            {
                list.Add(new AnswerValue { Text = "True", Correct = q.TfCorrect == "TRUE" });
                list.Add(new AnswerValue { Text = "False", Correct = q.TfCorrect == "FALSE" });
            }
            else if (dbType == "MCQ")
            {
                for (int i = 0; i < MaxOptions; i++)
                {
                    list.Add(new AnswerValue
                    {
                        Text = q.Options[i],
                        Correct = q.CorrectIndex == i
                    });
                }
            }

            // SUBJECTIVE questions have no stored answers.
            return list;
        }

        // ------------------------------------------------------------
        // Inserts. IDs continue from the highest existing value instead of the
        // identity counter, which can jump (e.g. to 1002).
        // ------------------------------------------------------------

        private int InsertQuestion(
            SqlConnection con,
            SqlTransaction tx,
            int quizId,
            string text,
            string dbType)
        {
            int questionId =
                NextId(con, tx, "quiz_questions", "question_id");

            string query = InsertSql(con, tx, "quiz_questions", @"
                INSERT INTO quiz_questions
                (
                    question_id,
                    quiz_id,
                    question_text,
                    question_type
                )
                VALUES
                (
                    @id,
                    @quizId,
                    @text,
                    @type
                );");

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@id", questionId);
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Parameters.AddWithValue("@text", text);
                cmd.Parameters.AddWithValue("@type", dbType);

                cmd.ExecuteNonQuery();
            }

            return questionId;
        }

        private void InsertAnswer(
            SqlConnection con,
            SqlTransaction tx,
            int questionId,
            AnswerValue answer)
        {
            int answerId =
                NextId(con, tx, AnswerTable, "answer_id");

            string query = InsertSql(con, tx, AnswerTable, @"
                INSERT INTO " + AnswerTable + @"
                (
                    answer_id,
                    question_id,
                    answer_text,
                    is_correct
                )
                VALUES
                (
                    @id,
                    @questionId,
                    @text,
                    @correct
                );");

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@id", answerId);
                cmd.Parameters.AddWithValue("@questionId", questionId);
                cmd.Parameters.AddWithValue("@text", answer.Text);
                cmd.Parameters.AddWithValue("@correct", answer.Correct);

                cmd.ExecuteNonQuery();
            }
        }

        private int NextId(
            SqlConnection con,
            SqlTransaction tx,
            string table,
            string column)
        {
            string query =
                "SELECT ISNULL(MAX(" + column + "), 0) + 1 FROM " +
                table + " WITH (UPDLOCK, HOLDLOCK)";

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private bool HasIdentity(
            SqlConnection con,
            SqlTransaction tx,
            string table)
        {
            bool result;

            if (identityCache.TryGetValue(table, out result))
            {
                return result;
            }

            using (SqlCommand cmd =
                new SqlCommand(
                    "SELECT OBJECTPROPERTY(OBJECT_ID(@table), 'TableHasIdentity')",
                    con, tx))
            {
                cmd.Parameters.AddWithValue("@table", "dbo." + table);

                object value = cmd.ExecuteScalar();

                result = value != null &&
                         value != DBNull.Value &&
                         Convert.ToInt32(value) == 1;
            }

            identityCache[table] = result;

            return result;
        }

        // Wraps the insert so an explicit ID can be written to an identity column.
        private string InsertSql(
            SqlConnection con,
            SqlTransaction tx,
            string table,
            string insertSql)
        {
            if (!HasIdentity(con, tx, table))
            {
                return insertSql;
            }

            return
                "SET IDENTITY_INSERT " + table + " ON; " +
                "BEGIN TRY " + insertSql + " END TRY " +
                "BEGIN CATCH SET IDENTITY_INSERT " + table + " OFF; THROW; END CATCH; " +
                "SET IDENTITY_INSERT " + table + " OFF;";
        }

        private int GetCreatorId()
        {
            // Use the logged-in admin when the login system exists.
            if (Session["user_id"] != null)
            {
                return Convert.ToInt32(Session["user_id"]);
            }

            // Until login is connected, fall back to the first admin account.
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 1 user_id
                    FROM users
                    WHERE role = 'ADMIN'
                    ORDER BY user_id ASC";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? 0
                    : Convert.ToInt32(result);
            }
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
                "window.scrollTo({ top: 0, behavior: 'smooth' });",
                true
            );
        }
    }
}