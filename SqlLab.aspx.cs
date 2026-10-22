using System;
using System.Data;

namespace The_Analytical_Dojo
{
    public partial class SqlLab : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                QueryText.Text = DojoPractice.Starter;
            }
        }

        protected void Run_Click(object sender, EventArgs e)
        {
            Results.Text = string.Empty;
            Status.Visible = true;

            try
            {
                // The helper only supports approved practice syntax.
                // Results are restricted to the signed-in student.
                DataTable results = DojoPractice.Run(
                    UserId,
                    QueryText.Text
                );

                Results.Text = DojoUi.Table(results);

                Status.Text =
                    results.Rows.Count +
                    " row(s) returned from your results.";
            }
            catch (ArgumentException ex)
            {
                // Display validation errors safely.
                Status.Text = H(ex.Message);
            }
        }
    }
}