using System;
using System.Data;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class BrowseCourses : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST" && Request.Form["enroll"] != null)
            {
                int courseId;

                if (!int.TryParse(Request.Form["enroll"], out courseId) || courseId <= 0)
                {
                    throw new HttpException(400, "Invalid course.");
                }

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

            LoadCatalogue();
        }

        private void LoadCatalogue()
        {
            DataTable courses = DojoData.Query(@"
                SELECT
                    c.course_id,
                    c.course_title,
                    c.description,
                    c.course_level,
                    c.course_image,
                    c.[status],
                    e.enrollment_id
                FROM courses c
                LEFT JOIN course_enrollments e
                    ON e.course_id = c.course_id
                    AND e.user_id = @userId
                WHERE c.[status] <> 'ARCHIVED'
                ORDER BY c.course_title",
                DojoData.P("@userId", UserId)
            );

            if (courses.Rows.Count == 0)
            {
                Catalogue.Text = "<p class='empty'>No published courses are available yet.</p>";
                return;
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class='course-grid'>");
            int n = 0;

            foreach (DataRow course in courses.Rows)
            {
                n++;
                bool enrolled = course["enrollment_id"] != DBNull.Value;

                html.Append("<article class='course-card'>");
                html.Append(DojoUi.CourseArt(course, n));
                html.Append("<div class='course-body'>");
                if (enrolled)
                {
                    html.Append("<span class='course-enrolled'>✓ Enrolled</span>");
                }
                html.Append("<h3>");
                html.Append(H(course["course_title"]));
                html.Append("</h3><p class='muted'>");
                html.Append(H(course["description"]));
                html.Append("</p>");

                if (enrolled)
                {
                    html.Append("<a class='btn' href='CoursePage.aspx?id=");
                    html.Append(H(course["course_id"]));
                    html.Append("'>Open course</a>");
                }
                else if (Convert.ToString(course["status"]) != "PUBLISHED")
                {
                    html.Append("<button class='btn ghost' type='button' disabled>Coming soon</button>");
                }
                else
                {
                    html.Append("<button class='btn' type='submit' name='enroll' value='");
                    html.Append(H(course["course_id"]));
                    html.Append("'>Enrol</button>");
                }

                html.Append("</div></article>");
            }

            html.Append("</div>");
            Catalogue.Text = html.ToString();
        }
    }
}
