using System;
using System.Data;

namespace The_Analytical_Dojo
{
    public partial class MyCourses : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMyCourses();
            }
        }

        private void LoadMyCourses()
        {
            // Get the current student's enrolled courses.
            DataTable courses = DojoData.Courses(UserId);

            // Display the courses as cards.
            Cards.Text = DojoUi.Cards(
                courses,
                int.MaxValue
            );
        }
    }
}