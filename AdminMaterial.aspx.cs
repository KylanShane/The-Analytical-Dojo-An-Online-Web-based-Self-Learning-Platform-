using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    public partial class AdminMaterial : System.Web.UI.Page
    {
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
                    ShowMessage("Material created successfully.", true);
                }

                if (Request.QueryString["updated"] == "1")
                {
                    ShowMessage("Material updated successfully.", true);
                }

                LoadStatistics();
                LoadCourseFilter();
                LoadMaterials();
            }
        }

        // ------------------------------------------------------------
        // Summary cards, filters, table
        // ------------------------------------------------------------

        private void LoadStatistics()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        COUNT(*) AS Total,

                        SUM(CASE WHEN m.material_type = 'PPT'
                                 THEN 1 ELSE 0 END) AS Ppt,

                        SUM(CASE WHEN m.material_type = 'VIDEO'
                                 THEN 1 ELSE 0 END) AS Video,

                        COUNT(DISTINCT m.chapter_id) AS Chapters,
                        COUNT(DISTINCT c.course_id) AS Courses,

                        (SELECT COUNT(*) FROM courses) AS AllCourses

                    FROM course_materials m

                    INNER JOIN course_chapters c
                        ON m.chapter_id = c.chapter_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int chapters = ReadInt(reader, "Chapters");
                        int courses = ReadInt(reader, "Courses");
                        int allCourses = ReadInt(reader, "AllCourses");

                        lblTotalMaterials.Text = ReadInt(reader, "Total").ToString();

                        lblTotalSub.Text =
                            "In " + chapters + (chapters == 1 ? " chapter" : " chapters");

                        lblPptMaterials.Text = ReadInt(reader, "Ppt").ToString();
                        lblVideoMaterials.Text = ReadInt(reader, "Video").ToString();

                        lblCoursesCovered.Text = courses.ToString();
                        lblCoveredSub.Text = "Of " + allCourses + " courses";
                    }
                }
            }
        }

        private static int ReadInt(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? 0
                : Convert.ToInt32(reader[column]);
        }

        private void LoadCourseFilter()
        {
            ddlCourseFilter.Items.Clear();
            ddlCourseFilter.Items.Add(new ListItem("All Courses", "0"));

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
                        ddlCourseFilter.Items.Add(
                            new ListItem(
                                reader["course_title"].ToString(),
                                reader["course_id"].ToString()
                            )
                        );
                    }
                }
            }
        }

        private void LoadMaterials()
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        m.material_id,
                        m.material_title,
                        m.material_type,
                        c.chapter_title,
                        co.course_id,
                        co.course_title

                    FROM course_materials m

                    INNER JOIN course_chapters c
                        ON m.chapter_id = c.chapter_id

                    INNER JOIN courses co
                        ON c.course_id = co.course_id

                    WHERE
                        (
                            @search = ''
                            OR m.material_title LIKE '%' + @search + '%'
                            OR c.chapter_title LIKE '%' + @search + '%'
                        )

                        AND
                        (
                            @courseId = 0
                            OR co.course_id = @courseId
                        )

                        AND
                        (
                            @type = ''
                            OR m.material_type = @type
                        )

                    ORDER BY
                        co.course_id,
                        c.chapter_order,
                        m.material_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@search", txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@courseId", Convert.ToInt32(ddlCourseFilter.SelectedValue));
                cmd.Parameters.AddWithValue("@type", ddlTypeFilter.SelectedValue);

                DataTable dt = new DataTable();

                new SqlDataAdapter(cmd).Fill(dt);

                gvMaterials.DataSource = dt;
                gvMaterials.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadMaterials();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlCourseFilter.SelectedIndex = 0;
            ddlTypeFilter.SelectedIndex = 0;

            LoadMaterials();
        }

        // ------------------------------------------------------------
        // Row actions
        // ------------------------------------------------------------

        protected void gvMaterials_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int materialId;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out materialId))
            {
                return;
            }

            if (e.CommandName == "ViewMaterial")
            {
                ShowViewPanel(materialId);
            }
            else if (e.CommandName == "EditMaterial")
            {
                Response.Redirect(
                    "AdminCreateMaterial.aspx?id=" + materialId,
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            else if (e.CommandName == "DeleteMaterial")
            {
                DeleteMaterial(materialId);

                LoadStatistics();
                LoadMaterials();
            }
        }

        private void ShowViewPanel(int materialId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        m.material_id,
                        m.material_title,
                        m.description,
                        m.material_type,
                        m.file_url,
                        m.uploaded_at,
                        c.chapter_title,
                        co.course_title,

                        COALESCE(
                            u.full_name,
                            u.username,
                            'Unknown User'
                        ) AS uploader_name

                    FROM course_materials m

                    INNER JOIN course_chapters c
                        ON m.chapter_id = c.chapter_id

                    INNER JOIN courses co
                        ON c.course_id = co.course_id

                    LEFT JOIN users u
                        ON m.uploaded_by = u.user_id

                    WHERE m.material_id = @materialId";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con);

                adapter.SelectCommand.Parameters
                    .AddWithValue("@materialId", materialId);

                adapter.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                ShowMessage("The material could not be found.", false);
                return;
            }

            DataRow row = dt.Rows[0];

            string uploadedAt =
                row["uploaded_at"] == DBNull.Value
                ? "-"
                : Convert.ToDateTime(row["uploaded_at"]).ToString("dd MMM yyyy");

            string url =
                row["file_url"] == DBNull.Value
                ? ""
                : row["file_url"].ToString();

            string fileHtml = "-";

            if (url.Length > 0)
            {
                string name = url.Substring(url.LastIndexOf('/') + 1);

                fileHtml = HttpUtility.HtmlEncode(name);

                // Only same-site paths become links.
                if (url.StartsWith("/") && !url.StartsWith("//"))
                {
                    fileHtml +=
                        " &nbsp;<a href='" +
                        HttpUtility.HtmlAttributeEncode(ResolveUrl("~" + url)) +
                        "' target='_blank' rel='noopener'>Open file</a>";
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("<h4>Material information</h4>");
            sb.Append("<div class='detail-grid'>");

            AppendField(sb, "MATERIAL TITLE", HttpUtility.HtmlEncode(row["material_title"].ToString()));

            AppendField(
                sb,
                "TYPE",
                "<span class='" + GetTypeClass(row["material_type"]) + "'>" +
                HttpUtility.HtmlEncode(FormatType(row["material_type"])) + "</span>"
            );

            AppendField(sb, "COURSE", HttpUtility.HtmlEncode(row["course_title"].ToString()));
            AppendField(sb, "CHAPTER", HttpUtility.HtmlEncode(row["chapter_title"].ToString()));
            AppendField(sb, "UPLOADED BY", HttpUtility.HtmlEncode(row["uploader_name"].ToString()));
            AppendField(sb, "UPLOADED", HttpUtility.HtmlEncode(uploadedAt));

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

            AppendField(sb, "FILE", fileHtml, true);

            sb.Append("</div>");

            litViewContent.Text = sb.ToString();
            pnlView.Visible = true;
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

        private void DeleteMaterial(int materialId)
        {
            string fileUrl = null;

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(
                            "SELECT file_url FROM course_materials WHERE material_id = @id",
                            con))
                    {
                        cmd.Parameters.AddWithValue("@id", materialId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            fileUrl = result.ToString();
                        }
                    }

                    int rows;

                    using (SqlCommand cmd =
                        new SqlCommand(
                            "DELETE FROM course_materials WHERE material_id = @id",
                            con))
                    {
                        cmd.Parameters.AddWithValue("@id", materialId);

                        rows = cmd.ExecuteNonQuery();
                    }

                    if (rows == 0)
                    {
                        ShowMessage("The material could not be deleted.", false);
                        return;
                    }
                }

                DeleteUploadedFile(fileUrl);

                ShowMessage("Material deleted successfully.", true);
            }
            catch (SqlException ex)
            {
                // 547 = still referenced elsewhere (e.g. student progress)
                ShowMessage(
                    ex.Number == 547
                        ? "This material cannot be deleted because it is still in use."
                        : "The material could not be deleted.",
                    false
                );
            }
        }

        // Only removes files this site uploaded (never the demo material paths).
        private void DeleteUploadedFile(string url)
        {
            if (url == null ||
                !url.StartsWith("/Uploads/Materials/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                string root =
                    Path.GetFullPath(Server.MapPath("~/Uploads/Materials"));

                string path =
                    Path.GetFullPath(Server.MapPath("~" + url));

                if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase) &&
                    File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Best effort cleanup only.
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

            string type = value.ToString().ToUpperInvariant();

            return type == "VIDEO" ? "Video" : type;
        }

        public string GetTypeClass(object value)
        {
            if (value != null &&
                value != DBNull.Value &&
                value.ToString().ToUpperInvariant() == "VIDEO")
            {
                return "badge level-intermediate";
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