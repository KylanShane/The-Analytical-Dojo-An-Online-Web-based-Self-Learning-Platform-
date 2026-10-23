using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class CoursePage : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int courseId = QueryId("id");

            DataTable courseTable = DojoData.Query(
                @"
                SELECT
                    c.course_id,
                    c.course_title,
                    c.description,
                    c.course_level,
                    e.enrollment_id,
                    e.completion_percentage,
                    e.average_score_percentage
                FROM courses c
                LEFT JOIN course_enrollments e
                    ON e.course_id = c.course_id
                    AND e.user_id = @userId
                WHERE c.course_id = @courseId
                    AND c.[status] <> 'ARCHIVED'",
                DojoData.P("@courseId", courseId),
                DojoData.P("@userId", UserId)
            );

            if (courseTable.Rows.Count == 0)
            {
                throw new HttpException(
                    404,
                    "This course was not found."
                );
            }

            if (Request.HttpMethod == "POST" && Request.Form["enroll"] != null)
            {
                try
                {
                    DojoData.Enroll(UserId, courseId);
                    Response.Redirect("CoursePage.aspx?id=" + courseId, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    Message.Visible = true;
                    Message.Text = H(ex.Message);
                }
            }

            // Handle the Mark complete button.
            if (
                Request.HttpMethod == "POST" &&
                Request.Form["completeMaterial"] != null
            )
            {
                int materialId;

                bool validId = int.TryParse(
                    Request.Form["completeMaterial"],
                    out materialId
                );

                if (!validId || materialId <= 0)
                {
                    throw new HttpException(
                        400,
                        "Invalid learning material."
                    );
                }

                if (courseTable.Rows[0]["enrollment_id"] == DBNull.Value)
                {
                    throw new HttpException(
                        403,
                        "Enrol in this course before opening materials."
                    );
                }

                DojoMaterials.Complete(UserId, courseId, materialId);

                // Reload the page to show the updated progress.
                Response.Redirect(
                    "CoursePage.aspx?id=" + courseId,
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            DataRow course = courseTable.Rows[0];
            bool enrolled = course["enrollment_id"] != DBNull.Value;
            int enrollmentId = enrolled
                ? Convert.ToInt32(course["enrollment_id"])
                : 0;

            DisplayCourseIntroduction(course, enrolled);
            DisplayCommunity(courseId, enrolled);
            DisplayChapters(courseId, enrollmentId, enrolled);
        }

        private void DisplayCourseIntroduction(DataRow course, bool enrolled)
        {
            string percentage = enrolled
                ? DojoData.Num(course["completion_percentage"])
                : "0";

            StringBuilder html = new StringBuilder();

            html.Append("<div class='page-intro learning-course-intro'>");

            html.Append("<span class='pill'>");
            html.Append(H(course["course_level"]));
            html.Append("</span>");

            html.Append("<h1>");
            html.Append(H(course["course_title"]));
            html.Append("</h1>");

            html.Append("<p class='muted'>");
            html.Append(H(course["description"]));
            html.Append("</p>");

            if (!enrolled)
            {
                html.Append("<p class='muted'>Enrol to open slides and quizzes. This course will then appear in My courses.</p>");
                html.Append("<button class='btn' type='submit' name='enroll' value='");
                html.Append(H(course["course_id"]));
                html.Append("'>Enrol</button>");
            }
            else
            {
                html.Append("<div class='progress-label'>");
                html.Append("<span>Materials + scored quizzes (MCQ / true-false)</span>");
                html.Append("<strong>");
                html.Append(percentage);
                html.Append("%</strong>");
                html.Append("</div>");

                html.Append(
                    "<progress " +
                    "aria-label='Course completion' " +
                    "max='100' " +
                    "value='" + percentage + "'>"
                );

                html.Append(percentage);
                html.Append("%</progress>");

                object avg = course["average_score_percentage"];
                html.Append("<p class='muted'>Course mark: ");
                if (avg == DBNull.Value)
                {
                    html.Append("not available yet. Average is calculated after every scored quiz in this course is submitted. Subjective questions do not count.");
                }
                else
                {
                    html.Append(DojoData.Num(avg));
                    html.Append("% · ");
                    html.Append(DojoData.Band(avg));
                    html.Append(" (average of MCQ and true/false quizzes)");
                }

                html.Append("</p>");
            }

            html.Append("</div>");

            Intro.Text = html.ToString();
        }

        private void DisplayCommunity(int courseId, bool enrolled)
        {
            if (!enrolled)
            {
                Community.Text = string.Empty;
                return;
            }

            Community.Text =
                "<section class='course-community course-community-single' aria-label='Course forum'>" +
                "<a class='course-community-card course-community-forum' href='CourseForum.aspx?course=" + courseId + "'>" +
                "<span class='learning-kicker'>01 / LEARN TOGETHER</span><span class='community-icon' aria-hidden='true'>↗</span>" +
                "<h2>Course forum</h2><p>Start a discussion and explore what your classmates are thinking about this course.</p>" +
                "<span class='community-cta'>Enter the forum <b>→</b></span></a></section>";
        }

        private void DisplayChapters(
            int courseId,
            int enrollmentId,
            bool enrolled
        )
        {
            DataTable chapters = DojoData.Query(
                @"
                SELECT
                    chapter_id,
                    chapter_title,
                    description,
                    chapter_order
                FROM course_chapters
                WHERE course_id = @courseId
                ORDER BY chapter_order",
                DojoData.P("@courseId", courseId)
            );

            if (chapters.Rows.Count == 0)
            {
                Chapters.Text =
                    "<p class='empty'>" +
                    "No chapters have been added to this course yet." +
                    "</p>";

                return;
            }

            StringBuilder html = new StringBuilder();

            foreach (DataRow chapter in chapters.Rows)
            {
                int chapterId = Convert.ToInt32(
                    chapter["chapter_id"]
                );

                html.Append(
                    "<details class='panel chapter' open>"
                );

                html.Append("<summary>");

                html.Append("<span class='chapter-number'>");
                html.Append(H(chapter["chapter_order"]));
                html.Append("</span>");

                html.Append(H(chapter["chapter_title"]));
                html.Append("</summary>");

                html.Append("<p class='muted'>");
                html.Append(H(chapter["description"]));
                html.Append("</p>");

                html.Append("<h3>Learning materials</h3>");

                AppendMaterials(
                    html,
                    chapterId,
                    enrollmentId,
                    enrolled
                );

                html.Append(
                    "<h3 class='space-top'>Chapter quizzes</h3>"
                );

                AppendQuizzes(html, chapterId, enrolled);

                html.Append("</details>");
            }

            Chapters.Text = html.ToString();
        }

        private void AppendMaterials(
            StringBuilder html,
            int chapterId,
            int enrollmentId,
            bool enrolled
        )
        {
            DataTable materials = DojoData.Query(
                @"
                SELECT
                    m.material_id,
                    m.material_title,
                    m.description,
                    m.material_type,
                    m.file_url,
                    p.completed_at
                FROM course_materials m
                LEFT JOIN student_material_progress p
                    ON p.material_id = m.material_id
                    AND p.enrollment_id = @enrollmentId
                WHERE m.chapter_id = @chapterId
                ORDER BY m.material_id",
                DojoData.P("@enrollmentId", enrollmentId),
                DojoData.P("@chapterId", chapterId)
            );

            if (materials.Rows.Count == 0)
            {
                html.Append(
                    "<p class='muted'>" +
                    "No materials uploaded yet." +
                    "</p>"
                );

                return;
            }

            foreach (DataRow material in materials.Rows)
            {
                bool completed =
                    material["completed_at"] != DBNull.Value;

                string materialType = Convert.ToString(
                    material["material_type"]
                );

                string materialUrl = DojoMaterials.SafeUrl(
                    Convert.ToString(material["file_url"])
                );

                string embedUrl = DojoMaterials.EmbedUrl(
                    materialType,
                    materialUrl
                );

                string viewUrl =
                    "MaterialView.aspx?id=" + H(material["material_id"]);

                html.Append("<div class='lesson'>");

                // Preview tile that opens the full-size viewer.
                if (enrolled && embedUrl != null)
                {
                    string thumbnail = DojoMaterials.ThumbnailUrl(
                        materialType,
                        materialUrl
                    );
                    bool video = DojoMaterials.IsVideo(materialType);

                    html.Append(
                        "<a class='material-tile" +
                        (video ? " is-video" : " is-slides") +
                        "' href='" + viewUrl + "' " +
                        "aria-label='View " + H(material["material_title"]) + "'>"
                    );

                    if (thumbnail != null)
                    {
                        html.Append(
                            "<img src='" + H(thumbnail) + "' alt='' loading='lazy' />"
                        );
                    }
                    else
                    {
                        html.Append(
                            "<span class='material-tile-art' aria-hidden='true'>" +
                            "<i></i><i></i><i></i></span>"
                        );
                    }

                    html.Append(
                        "<span class='material-tile-play' aria-hidden='true'>" +
                        (video ? "▶" : "⤢") +
                        "</span></a>"
                    );
                }
                html.Append("<div class='lesson-info'>");

                html.Append("<span class='pill'>");
                html.Append(H(material["material_type"]));
                html.Append("</span>");

                html.Append("<strong>");
                html.Append(H(material["material_title"]));
                html.Append("</strong>");

                html.Append("<small>");
                html.Append(H(material["description"]));
                html.Append("</small>");

                html.Append("</div>");

                html.Append("<div class='lesson-actions'>");

                if (!enrolled)
                {
                    html.Append("<span class='muted'>Enrol to view this material</span>");
                }
                else if (embedUrl != null)
                {
                    html.Append(
                        "<a class='btn ghost' href='" + viewUrl + "'>" +
                        (DojoMaterials.IsVideo(materialType) ? "▶ Watch" : "⤢ View slides") +
                        "</a>"
                    );
                }
                else
                {
                    html.Append(
                        "<span class='muted'>" +
                        "Preview unavailable" +
                        "</span>"
                    );
                }

                if (enrolled && completed)
                {
                    html.Append(
                        "<button " +
                        "class='btn ghost' " +
                        "type='button' " +
                        "disabled>"
                    );

                    html.Append("✓ Completed");
                    html.Append("</button>");
                }
                else if (enrolled)
                {
                    html.Append(
                        "<button " +
                        "class='btn' " +
                        "type='submit' " +
                        "name='completeMaterial' " +
                        "value='" +
                        H(material["material_id"]) +
                        "'>"
                    );

                    html.Append("Mark complete");
                    html.Append("</button>");
                }

                html.Append("</div>");
                html.Append("</div>");
            }
        }

        private void AppendQuizzes(
            StringBuilder html,
            int chapterId,
            bool enrolled
        )
        {
            DataTable quizzes = DojoData.Query(
                @"
                SELECT
                    q.quiz_id,
                    q.quiz_title,
                    q.quiz_type
                FROM quizzes q
                WHERE q.chapter_id = @chapterId
                ORDER BY q.quiz_id",
                DojoData.P("@chapterId", chapterId)
            );

            if (quizzes.Rows.Count == 0)
            {
                html.Append(
                    "<p class='muted'>" +
                    "No quizzes added yet." +
                    "</p>"
                );

                return;
            }

            foreach (DataRow quiz in quizzes.Rows)
            {
                int quizId = Convert.ToInt32(quiz["quiz_id"]);
                bool subjective = string.Equals(
                    Convert.ToString(quiz["quiz_type"]),
                    "SUBJ",
                    StringComparison.OrdinalIgnoreCase
                );
                DataRow latest = enrolled
                    ? DojoData.LatestAttempt(UserId, quizId)
                    : null;
                bool canAttempt = enrolled && DojoData.CanAttemptQuiz(UserId, quizId);
                object score = latest == null ? null : latest["score"];
                bool scored = score != null && score != DBNull.Value;

                string description = subjective
                    ? "Required reflection · submit once to complete"
                    : "Pass at 50%+ to complete";

                if (latest != null && scored)
                {
                    description +=
                        " · latest " +
                        DojoData.Num(score) +
                        "% (" +
                        DojoData.Band(score) +
                        ")";
                }
                else if (latest != null)
                {
                    description =
                        "Submitted " +
                        Convert.ToDateTime(latest["submitted_at"])
                            .ToString("dd MMM yyyy, HH:mm") +
                        " · completion counted";
                }

                html.Append("<div class='lesson'>");
                html.Append("<div>");

                html.Append("<strong>");
                html.Append(H(quiz["quiz_title"]));
                html.Append("</strong>");

                html.Append("<small>");
                html.Append(description);
                html.Append("</small>");

                html.Append("</div>");
                html.Append("<div class='lesson-actions'>");

                if (!enrolled)
                {
                    html.Append("<span class='muted'>Enrol to attempt</span>");
                }
                else if (latest != null)
                {
                    html.Append(
                        "<a class='btn ghost' href='ReviewQuiz.aspx?attempt=" +
                        H(latest["attempt_id"]) +
                        "'>Review</a>"
                    );
                }

                if (enrolled && canAttempt)
                {
                    html.Append(
                        "<a class='btn' href='TakeQuiz.aspx?id=" +
                        H(quiz["quiz_id"]) +
                        "'>"
                    );
                    html.Append(latest == null ? "Start quiz" : "Reattempt");
                    html.Append("</a>");
                }
                else if (enrolled)
                {
                    html.Append(
                        "<button class='btn ghost' type='button' disabled>" +
                        (subjective ? "Submitted ✓" : "Passed ✓") +
                        "</button>"
                    );
                }

                html.Append("</div></div>");
            }
        }
    }
}
