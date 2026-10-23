using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace The_Analytical_Dojo
{
    // Create / edit one material (title, description, type, file) of a chapter.
    // A new chapter can be created inline when the course has none yet.
    public partial class AdminCreateMaterial : System.Web.UI.Page
    {
        private const int MaxFileBytes = 50 * 1024 * 1024;
        private const string UploadUrlPrefix = "/Uploads/Materials/";
        private const string NewChapterValue = "-1";

        private static readonly string[] PptExtensions =
            { ".ppt", ".pptx", ".pdf" };

        private static readonly string[] VideoExtensions =
            { ".mp4", ".webm", ".mov" };

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DojoDB"].ConnectionString;

        private class MaterialRow
        {
            public int MaterialId;
            public int ChapterId;
            public int CourseId;
            public string Title;
            public string Description;
            public string Type;
            public string FileUrl;
        }

        private MaterialRow existingRow;
        private bool existingLoaded;

        // 0 = creating a new material, otherwise the material being edited.
        private int EditMaterialId
        {
            get
            {
                int id;

                return int.TryParse(Request.QueryString["id"], out id) && id > 0
                    ? id
                    : 0;
            }
        }

        // Used by the page markup so the script knows the stored file.
        protected string ExistingFileName
        {
            get
            {
                MaterialRow row = GetExisting();

                if (row == null || string.IsNullOrEmpty(row.FileUrl))
                {
                    return "";
                }

                return row.FileUrl.Substring(row.FileUrl.LastIndexOf('/') + 1);
            }
        }

        protected string ExistingFileType
        {
            get
            {
                MaterialRow row = GetExisting();

                return row == null ? "" : row.Type;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EditMaterialId > 0)
            {
                Page.Title = "Edit Material - The Analytical Dojo";
                litBreadcrumb.Text = "Edit Material";
                litHeading.Text = "Edit Material";
                litSubtitle.Text = "Update the material information or replace its file.";
                litActionNote.Text = "EDITING MATERIAL #" + EditMaterialId + " &middot; UNSAVED CHANGES";
                btnSave.Text = "Save Changes";
            }

            if (IsPostBack)
            {
                return;
            }

            LoadCourses();

            if (EditMaterialId > 0)
            {
                LoadMaterialForEdit();
            }
            else
            {
                LoadChapters(0, null);
            }
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

        // selectChapterId: chapter to preselect (null = default behaviour).
        private void LoadChapters(int courseId, int? selectChapterId)
        {
            ddlChapter.Items.Clear();
            ddlChapter.Items.Add(new ListItem("Select a chapter", "0"));

            pnlNewChapter.Visible = false;

            if (courseId == 0)
            {
                return;
            }

            int count = 0;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT chapter_id, chapter_title, chapter_order
                    FROM course_chapters
                    WHERE course_id = @courseId
                    ORDER BY chapter_order, chapter_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@courseId", courseId);

                con.Open();

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

            ddlChapter.Items.Add(new ListItem("+ Create new chapter", NewChapterValue));

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
            else if (count == 0)
            {
                // A course without chapters needs one: open the new chapter fields.
                ddlChapter.ClearSelection();
                ddlChapter.Items.FindByValue(NewChapterValue).Selected = true;
                pnlNewChapter.Visible = true;
            }
        }

        protected void ddlCourse_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadChapters(Convert.ToInt32(ddlCourse.SelectedValue), null);
        }

        protected void ddlChapter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            pnlNewChapter.Visible =
                ddlChapter.SelectedValue == NewChapterValue;
        }

        // ------------------------------------------------------------
        // Edit mode
        // ------------------------------------------------------------

        private MaterialRow GetExisting()
        {
            if (existingLoaded)
            {
                return existingRow;
            }

            existingLoaded = true;

            if (EditMaterialId == 0)
            {
                return null;
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        m.material_id,
                        m.chapter_id,
                        c.course_id,
                        m.material_title,
                        m.description,
                        m.material_type,
                        m.file_url

                    FROM course_materials m

                    INNER JOIN course_chapters c
                        ON m.chapter_id = c.chapter_id

                    WHERE m.material_id = @materialId";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@materialId", EditMaterialId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existingRow = new MaterialRow
                        {
                            MaterialId = Convert.ToInt32(reader["material_id"]),
                            ChapterId = Convert.ToInt32(reader["chapter_id"]),
                            CourseId = Convert.ToInt32(reader["course_id"]),
                            Title = reader["material_title"].ToString(),

                            Description =
                                reader["description"] == DBNull.Value
                                ? ""
                                : reader["description"].ToString(),

                            Type =
                                reader["material_type"] == DBNull.Value
                                ? ""
                                : reader["material_type"].ToString().ToUpperInvariant(),

                            FileUrl =
                                reader["file_url"] == DBNull.Value
                                ? ""
                                : reader["file_url"].ToString()
                        };
                    }
                }
            }

            return existingRow;
        }

        private void LoadMaterialForEdit()
        {
            MaterialRow row = GetExisting();

            if (row == null)
            {
                Response.Redirect("AdminMaterial.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            ListItem courseItem =
                ddlCourse.Items.FindByValue(row.CourseId.ToString());

            if (courseItem != null)
            {
                ddlCourse.ClearSelection();
                courseItem.Selected = true;
            }

            LoadChapters(row.CourseId, row.ChapterId);

            txtTitle.Text = row.Title;
            txtDescription.Text = row.Description;

            ListItem typeItem = rblType.Items.FindByValue(row.Type);

            if (typeItem != null)
            {
                rblType.ClearSelection();
                typeItem.Selected = true;
            }
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

            int courseId, chapterValue;

            int.TryParse(ddlCourse.SelectedValue, out courseId);
            int.TryParse(ddlChapter.SelectedValue, out chapterValue);

            if (courseId == 0 || chapterValue == 0)
            {
                ShowMessage("Please select a course and a chapter.", false);
                return;
            }

            bool isEdit = EditMaterialId > 0;
            MaterialRow existing = GetExisting();

            if (isEdit && existing == null)
            {
                ShowMessage("This material no longer exists.", false);
                return;
            }

            string type = rblType.SelectedValue == "VIDEO" ? "VIDEO" : "PPT";
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();

            bool hasFile =
                fuFile.HasFile && fuFile.PostedFile.ContentLength > 0;

            bool needsFile =
                !isEdit
                || string.IsNullOrEmpty(existing.FileUrl)
                || existing.Type != type;

            if (needsFile && !hasFile)
            {
                ShowMessage(
                    "Please select a file for this material (a chosen file is cleared when the page reloads, so select it again).",
                    false
                );

                return;
            }

            string ext = "";

            if (hasFile)
            {
                ext = Path.GetExtension(fuFile.FileName).ToLowerInvariant();

                string[] allowed =
                    type == "VIDEO" ? VideoExtensions : PptExtensions;

                if (Array.IndexOf(allowed, ext) < 0)
                {
                    ShowMessage(
                        "Unsupported file type. Allowed: " + string.Join(", ", allowed) + ".",
                        false
                    );

                    return;
                }

                if (fuFile.PostedFile.ContentLength > MaxFileBytes)
                {
                    ShowMessage("The file is larger than 50MB.", false);
                    return;
                }
            }

            // An existing chapter must belong to the selected course.
            if (chapterValue > 0 && !ChapterBelongsToCourse(chapterValue, courseId))
            {
                ShowMessage("The selected chapter does not belong to this course.", false);
                return;
            }

            int creatorId = GetCreatorId();

            if (creatorId == 0)
            {
                ShowMessage("No administrator account could be found.", false);
                return;
            }

            string savedPath = null;
            string newUrl = null;

            try
            {
                if (hasFile)
                {
                    string folder = Server.MapPath("~/Uploads/Materials");

                    Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString("N") + ext;

                    savedPath = Path.Combine(folder, fileName);

                    fuFile.PostedFile.SaveAs(savedPath);

                    newUrl = UploadUrlPrefix + fileName;
                }

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        try
                        {
                            int chapterId = chapterValue;

                            if (chapterValue == -1)
                            {
                                chapterId =
                                    InsertChapter(
                                        con, tx, courseId,
                                        txtChapterTitle.Text.Trim(),
                                        txtChapterDescription.Text.Trim()
                                    );
                            }

                            if (isEdit)
                            {
                                UpdateMaterial(
                                    con, tx, existing.MaterialId,
                                    chapterId, title, description,
                                    type, newUrl, creatorId
                                );
                            }
                            else
                            {
                                InsertMaterial(
                                    con, tx, chapterId, title,
                                    description, type, newUrl, creatorId
                                );
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

                // The replaced file is no longer needed.
                if (isEdit && newUrl != null)
                {
                    DeleteUploadedFile(existing.FileUrl);
                }

                Response.Redirect(
                    isEdit ? "AdminMaterial.aspx?updated=1" : "AdminMaterial.aspx?created=1",
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException)
            {
                DeleteFile(savedPath);

                ShowMessage(
                    "The material could not be saved. Nothing was changed. Please try again and re-select the file.",
                    false
                );
            }
            catch (IOException)
            {
                DeleteFile(savedPath);

                ShowMessage(
                    "The file could not be saved. Check that the website has write permission to the Uploads folder.",
                    false
                );
            }
            catch (UnauthorizedAccessException)
            {
                DeleteFile(savedPath);

                ShowMessage(
                    "The file could not be saved. Check that the website has write permission to the Uploads folder.",
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

        // ------------------------------------------------------------
        // Database writes. IDs continue from the highest existing value
        // instead of the identity counter, which can jump (e.g. to 1002).
        // ------------------------------------------------------------

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

        private static string WithIdentityInsert(
            string table,
            string insertSql)
        {
            return
                "SET IDENTITY_INSERT " + table + " ON; " +
                "BEGIN TRY " + insertSql + " END TRY " +
                "BEGIN CATCH SET IDENTITY_INSERT " + table + " OFF; THROW; END CATCH; " +
                "SET IDENTITY_INSERT " + table + " OFF;";
        }

        private int InsertChapter(
            SqlConnection con,
            SqlTransaction tx,
            int courseId,
            string title,
            string description)
        {
            int chapterId =
                NextId(con, tx, "course_chapters", "chapter_id");

            int order;

            using (SqlCommand cmd =
                new SqlCommand(
                    "SELECT ISNULL(MAX(chapter_order), 0) + 1 FROM course_chapters WHERE course_id = @courseId",
                    con, tx))
            {
                cmd.Parameters.AddWithValue("@courseId", courseId);

                order = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string query = WithIdentityInsert("course_chapters", @"
                INSERT INTO course_chapters
                (
                    chapter_id,
                    course_id,
                    chapter_title,
                    description,
                    chapter_order
                )
                VALUES
                (
                    @id,
                    @courseId,
                    @title,
                    @description,
                    @order
                );");

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@id", chapterId);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@order", order);

                cmd.ExecuteNonQuery();
            }

            return chapterId;
        }

        private void InsertMaterial(
            SqlConnection con,
            SqlTransaction tx,
            int chapterId,
            string title,
            string description,
            string type,
            string fileUrl,
            int uploadedBy)
        {
            int materialId =
                NextId(con, tx, "course_materials", "material_id");

            string query = WithIdentityInsert("course_materials", @"
                INSERT INTO course_materials
                (
                    material_id,
                    chapter_id,
                    material_title,
                    description,
                    material_type,
                    file_url,
                    uploaded_by,
                    uploaded_at
                )
                VALUES
                (
                    @id,
                    @chapterId,
                    @title,
                    @description,
                    @type,
                    @fileUrl,
                    @uploadedBy,
                    GETDATE()
                );");

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@id", materialId);
                cmd.Parameters.AddWithValue("@chapterId", chapterId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@fileUrl", fileUrl);
                cmd.Parameters.AddWithValue("@uploadedBy", uploadedBy);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateMaterial(
            SqlConnection con,
            SqlTransaction tx,
            int materialId,
            int chapterId,
            string title,
            string description,
            string type,
            string newFileUrl,
            int uploadedBy)
        {
            string query;

            if (newFileUrl != null)
            {
                query = @"
                    UPDATE course_materials

                    SET
                        chapter_id = @chapterId,
                        material_title = @title,
                        description = @description,
                        material_type = @type,
                        file_url = @fileUrl,
                        uploaded_by = @uploadedBy,
                        uploaded_at = GETDATE()

                    WHERE material_id = @materialId";
            }
            else
            {
                query = @"
                    UPDATE course_materials

                    SET
                        chapter_id = @chapterId,
                        material_title = @title,
                        description = @description

                    WHERE material_id = @materialId";
            }

            using (SqlCommand cmd =
                new SqlCommand(query, con, tx))
            {
                cmd.Parameters.AddWithValue("@chapterId", chapterId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@materialId", materialId);

                if (newFileUrl != null)
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@fileUrl", newFileUrl);
                    cmd.Parameters.AddWithValue("@uploadedBy", uploadedBy);
                }

                cmd.ExecuteNonQuery();
            }
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

        // ------------------------------------------------------------
        // File cleanup
        // ------------------------------------------------------------

        private void DeleteFile(string path)
        {
            if (path == null)
            {
                return;
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Best effort cleanup only.
            }
        }

        // Only removes files this site uploaded (never the demo material paths).
        private void DeleteUploadedFile(string url)
        {
            if (url == null ||
                !url.StartsWith(UploadUrlPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                string root =
                    Path.GetFullPath(Server.MapPath("~/Uploads/Materials"));

                string path =
                    Path.GetFullPath(Server.MapPath("~" + url));

                if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    DeleteFile(path);
                }
            }
            catch
            {
                // Best effort cleanup only.
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