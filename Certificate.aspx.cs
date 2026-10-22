using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class Certificate : DojoPage
    {
        private DataRow certificate;

        protected void Page_Load(object sender, EventArgs e)
        {
            int enrollmentId = QueryId("enrollment");

            DataTable table = DojoData.Query(@"
                SELECT
                    cert.certificate_number,
                    c.course_title,
                    u.full_name,
                    e.completed_at,
                    e.average_score_percentage,
                    e.enrollment_id
                FROM certificates cert
                JOIN course_enrollments e ON e.enrollment_id = cert.enrollment_id
                JOIN courses c ON c.course_id = e.course_id
                JOIN users u ON u.user_id = e.user_id
                WHERE cert.enrollment_id = @enrollmentId
                    AND e.user_id = @userId",
                DojoData.P("@enrollmentId", enrollmentId),
                DojoData.P("@userId", UserId)
            );

            if (table.Rows.Count == 0)
            {
                throw new HttpException(
                    404,
                    "Certificate not found. Complete the course materials and pass every scored quiz first."
                );
            }

            certificate = table.Rows[0];
            Cert.Text = BuildCertificateHtml();

            if (Request.QueryString["download"] == "1")
            {
                SendDownload();
            }
        }

        protected void Download_Click(object sender, EventArgs e)
        {
            SendDownload();
        }

        private void SendDownload()
        {
            string number = Convert.ToString(certificate["certificate_number"]);
            string fileName = "Analytical-Dojo-Certificate-" +
                number.Replace("\"", "").Replace("/", "-") + ".png";

            using (Bitmap bitmap = DrawCertificate())
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                byte[] bytes = stream.ToArray();

                Response.Clear();
                Response.ContentType = "image/png";
                Response.AddHeader(
                    "Content-Disposition",
                    "attachment; filename=\"" + fileName + "\""
                );
                Response.BinaryWrite(bytes);
                Response.Flush();
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private string AwardDate()
        {
            return certificate["completed_at"] == DBNull.Value
                ? DateTime.Now.ToString("dd MMMM yyyy")
                : Convert.ToDateTime(certificate["completed_at"]).ToString("dd MMMM yyyy");
        }

        private string AverageText()
        {
            if (certificate["average_score_percentage"] == DBNull.Value)
            {
                return "Course completed";
            }

            return DojoData.Num(certificate["average_score_percentage"]) +
                "% · " +
                DojoData.Band(certificate["average_score_percentage"]);
        }

        private string BuildCertificateHtml()
        {
            StringBuilder html = new StringBuilder();
            html.Append("<article class='certificate' aria-label='Certificate of completion'>");
            html.Append("<div class='certificate-frame'><div class='certificate-inner'>");
            html.Append("<p class='cert-kicker'>The Analytical Dojo</p>");
            html.Append("<h1>Certificate of Completion</h1>");
            html.Append("<p class='cert-line'>This is to certify that</p>");
            html.Append("<p class='cert-name'>");
            html.Append(H(certificate["full_name"]));
            html.Append("</p>");
            html.Append("<p class='cert-line'>has successfully completed</p>");
            html.Append("<p class='cert-course'>");
            html.Append(H(certificate["course_title"]));
            html.Append("</p>");
            html.Append("<div class='cert-rule'></div>");
            html.Append("<p class='cert-meta'>Awarded ");
            html.Append(H(AwardDate()));
            html.Append("<br />Objective average ");
            html.Append(H(AverageText()));
            html.Append("</p>");
            html.Append("<p class='cert-number'>Certificate ");
            html.Append(H(certificate["certificate_number"]));
            html.Append("</p>");
            html.Append("</div></div></article>");
            return html.ToString();
        }

        private Bitmap DrawCertificate()
        {
            int width = 1600;
            int height = 1132;
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            using (SolidBrush navy = new SolidBrush(Color.FromArgb(14, 13, 43)))
            using (SolidBrush gold = new SolidBrush(Color.FromArgb(201, 162, 39)))
            using (SolidBrush cream = new SolidBrush(Color.FromArgb(236, 235, 255)))
            using (SolidBrush muted = new SolidBrush(Color.FromArgb(169, 168, 221)))
            using (SolidBrush yellow = new SolidBrush(Color.FromArgb(255, 200, 61)))
            using (Pen goldPen = new Pen(Color.FromArgb(201, 162, 39), 14))
            using (Pen innerPen = new Pen(Color.FromArgb(91, 69, 240), 2))
            using (Font kicker = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Font title = new Font("Georgia", 42, FontStyle.Bold))
            using (Font line = new Font("Segoe UI", 18, FontStyle.Regular))
            using (Font name = new Font("Georgia", 48, FontStyle.Bold))
            using (Font course = new Font("Georgia", 28, FontStyle.Italic))
            using (Font meta = new Font("Segoe UI", 16, FontStyle.Regular))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(Color.FromArgb(14, 13, 43));
                g.FillRectangle(navy, 0, 0, width, height);
                g.DrawRectangle(goldPen, 36, 36, width - 72, height - 72);
                g.DrawRectangle(innerPen, 64, 64, width - 128, height - 128);

                StringFormat center = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                RectangleF area = new RectangleF(120, 140, width - 240, 80);
                g.DrawString("THE ANALYTICAL DOJO", kicker, gold, area, center);

                area.Y = 230;
                area.Height = 90;
                g.DrawString("Certificate of Completion", title, cream, area, center);

                area.Y = 340;
                area.Height = 40;
                g.DrawString("This is to certify that", line, muted, area, center);

                area.Y = 390;
                area.Height = 90;
                g.DrawString(Convert.ToString(certificate["full_name"]), name, yellow, area, center);

                area.Y = 490;
                area.Height = 40;
                g.DrawString("has successfully completed", line, muted, area, center);

                area.Y = 540;
                area.Height = 90;
                g.DrawString(Convert.ToString(certificate["course_title"]), course, cream, area, center);

                using (Pen rule = new Pen(Color.FromArgb(91, 69, 240), 2))
                {
                    g.DrawLine(rule, 520, 660, width - 520, 660);
                }

                area.Y = 700;
                area.Height = 70;
                g.DrawString(
                    "Awarded " + AwardDate() + "    ·    Objective average " + AverageText(),
                    meta,
                    muted,
                    area,
                    center
                );

                area.Y = 790;
                area.Height = 50;
                g.DrawString(
                    "Certificate " + Convert.ToString(certificate["certificate_number"]),
                    meta,
                    gold,
                    area,
                    center
                );
            }

            return bitmap;
        }
    }
}
