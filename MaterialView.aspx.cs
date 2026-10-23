using System;
using System.Data;
using System.Text;
using System.Web;

namespace The_Analytical_Dojo
{
    public partial class MaterialView : DojoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int materialId = QueryId("id");

            // Only materials from courses this student is enrolled in.
            DataTable materialTable = DojoData.Query(
                @"
                SELECT
                    m.material_id,
                    m.material_title,
                    m.description,
                    m.material_type,
                    m.file_url,
                    ch.course_id,
                    ch.chapter_title,
                    ch.chapter_order,
                    c.course_title,
                    p.completed_at
                FROM course_materials m
                INNER JOIN course_chapters ch
                    ON ch.chapter_id = m.chapter_id
                INNER JOIN courses c
                    ON c.course_id = ch.course_id
                INNER JOIN course_enrollments e
                    ON e.course_id = c.course_id
                    AND e.user_id = @userId
                LEFT JOIN student_material_progress p
                    ON p.material_id = m.material_id
                    AND p.enrollment_id = e.enrollment_id
                WHERE m.material_id = @materialId",
                DojoData.P("@materialId", materialId),
                DojoData.P("@userId", UserId)
            );

            if (materialTable.Rows.Count == 0)
            {
                throw new HttpException(
                    404,
                    "This material was not found in your courses."
                );
            }

            DataRow material = materialTable.Rows[0];
            int courseId = Convert.ToInt32(material["course_id"]);

            if (
                Request.HttpMethod == "POST" &&
                Request.Form["completeMaterial"] != null
            )
            {
                DojoMaterials.Complete(UserId, courseId, materialId);

                Response.Redirect("MaterialView.aspx?id=" + materialId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            DataTable playlist = DojoData.Query(
                @"
                SELECT
                    m.material_id,
                    m.material_title,
                    m.material_type,
                    ch.chapter_order,
                    ch.chapter_title,
                    p.completed_at
                FROM course_materials m
                INNER JOIN course_chapters ch
                    ON ch.chapter_id = m.chapter_id
                INNER JOIN course_enrollments e
                    ON e.course_id = ch.course_id
                    AND e.user_id = @userId
                LEFT JOIN student_material_progress p
                    ON p.material_id = m.material_id
                    AND p.enrollment_id = e.enrollment_id
                WHERE ch.course_id = @courseId
                ORDER BY ch.chapter_order, m.material_id",
                DojoData.P("@courseId", courseId),
                DojoData.P("@userId", UserId)
            );

            int previousId = 0;
            int nextId = 0;
            int position = 0;

            for (int i = 0; i < playlist.Rows.Count; i++)
            {
                if (Convert.ToInt32(playlist.Rows[i]["material_id"]) != materialId)
                {
                    continue;
                }

                position = i + 1;

                if (i > 0)
                {
                    previousId = Convert.ToInt32(playlist.Rows[i - 1]["material_id"]);
                }

                if (i < playlist.Rows.Count - 1)
                {
                    nextId = Convert.ToInt32(playlist.Rows[i + 1]["material_id"]);
                }

                break;
            }

            string materialType = Convert.ToString(material["material_type"]);
            string materialUrl = DojoMaterials.SafeUrl(
                Convert.ToString(material["file_url"])
            );
            string embedUrl = DojoMaterials.EmbedUrl(materialType, materialUrl);

            // Load PDFs through our own authenticated endpoint. Many source
            // websites block cross-origin iframe/PDF.js requests, which makes
            // the embedded viewer show "refused to connect" even though the
            // original PDF URL itself is valid.
            if (embedUrl != null && DojoMaterials.IsPdf(embedUrl))
            {
                embedUrl = "MaterialContent.ashx?id=" + materialId;
            }

            Title = material["material_title"] + " | The Analytical Dojo";

            DisplayHead(material, courseId, previousId, nextId, materialUrl, position, playlist.Rows.Count);
            // Slide decks stored as images (one row per page).
            DataTable slides = DojoData.Query(@"
                SELECT slide_order, image_url, alt_text
                FROM material_slides
                WHERE material_id = @materialId
                ORDER BY slide_order",
                DojoData.P("@materialId", materialId)
            );

            DisplayStage(material, materialType, embedUrl, slides);
            DisplayPlaylist(playlist, materialId);
        }

        private void DisplayHead(
            DataRow material,
            int courseId,
            int previousId,
            int nextId,
            string materialUrl,
            int position,
            int total
        )
        {
            bool completed = material["completed_at"] != DBNull.Value;
            StringBuilder html = new StringBuilder();

            html.Append("<header class='viewer-head'>");
            html.Append("<div class='viewer-title'>");

            html.Append("<a class='learning-back-link' href='CoursePage.aspx?id=" + courseId + "'>← ");
            html.Append(H(material["course_title"]));
            html.Append("</a>");

            html.Append("<span class='viewer-crumb'>CH ");
            html.Append(H(material["chapter_order"]));
            html.Append(" · ");
            html.Append(H(material["chapter_title"]));
            html.Append(" · ");
            html.Append(position + " / " + total);
            html.Append("</span>");

            html.Append("<h1>");
            html.Append(H(material["material_title"]));
            html.Append("</h1>");

            if (material["description"] != DBNull.Value)
            {
                html.Append("<p class='muted'>");
                html.Append(H(material["description"]));
                html.Append("</p>");
            }

            html.Append("</div>");

            html.Append("<div class='viewer-actions'>");

            if (completed)
            {
                html.Append("<button class='btn ghost' type='button' disabled>✓ Completed</button>");
            }
            else
            {
                html.Append(
                    "<button class='btn' type='submit' name='completeMaterial' value='" +
                    H(material["material_id"]) +
                    "'>Mark complete</button>"
                );
            }

            if (nextId > 0)
            {
                html.Append(
                    "<a class='btn ghost' data-viewer-next href='MaterialView.aspx?id=" +
                    nextId +
                    "'>Next →</a>"
                );
            }

            if (previousId > 0)
            {
                html.Append(
                    "<a class='viewer-hidden-link' data-viewer-prev href='MaterialView.aspx?id=" +
                    previousId +
                    "'>Previous</a>"
                );
            }

            if (materialUrl != null)
            {
                html.Append(
                    "<a class='icon-btn' href='" + H(materialUrl) +
                    "' target='_blank' rel='noopener' title='Open original in a new tab' aria-label='Open original in a new tab'>↗</a>"
                );
            }

            html.Append("</div>");
            html.Append("</header>");

            ViewerHead.Text = html.ToString();
        }

        private void DisplayStage(DataRow material, string materialType, string embedUrl, DataTable slides)
        {
            bool video = DojoMaterials.IsVideo(materialType);
            bool imageDeck = slides.Rows.Count > 0;
            bool pdf = !imageDeck && embedUrl != null && DojoMaterials.IsPdf(embedUrl);
            StringBuilder html = new StringBuilder();

            // PDF slides are drawn page-by-page by Scripts/dojo.js (PDF.js);
            // the iframe stays as the fallback.
            html.Append(
                "<section class='viewer-stage " +
                (video ? "is-video" : "is-slides") +
                "' data-viewer-stage" +
                (imageDeck ? " data-image-deck" : "") +
                (pdf ? " data-pdf='" + H(embedUrl) + "'" : "") +
                ">"
            );

            html.Append("<div class='viewer-frame'>");

            if (imageDeck)
            {
                html.Append(ImageDeck(slides, Convert.ToString(material["material_title"])));
            }
            else if (embedUrl == null)
            {
                html.Append(
                    "<div class='viewer-empty'><span>⌁</span>" +
                    "<strong>Preview unavailable</strong>" +
                    "<small>Use the ↗ button to open the original file.</small></div>"
                );
            }
            else
            {
                // A PDF iframe would trigger a download for octet-stream
                // files, so it only gets its src if PDF.js cannot load the file.
                html.Append(
                    "<iframe " + (pdf ? "data-src='" : "src='") + H(embedUrl) + "' " +
                    "title='" + H(material["material_title"]) + "' " +
                    "allow='accelerometer; autoplay; clipboard-write; " +
                    "encrypted-media; gyroscope; picture-in-picture; web-share; fullscreen' " +
                    "allowfullscreen></iframe>"
                );
            }

            html.Append("</div>");

            if (imageDeck)
            {
                html.Append(SlideStrip(slides));
            }

            html.Append("<div class='viewer-bar'>");
            html.Append("<span class='viewer-type'>");
            html.Append(video ? "▶ VIDEO" : "▤ SLIDES");
            html.Append("</span>");
            html.Append("<span class='viewer-keys'><kbd>F</kbd> fullscreen <kbd>T</kbd> theatre <kbd>←</kbd><kbd>→</kbd> prev / next</span>");
            html.Append("<span class='viewer-bar-actions'>");
            html.Append("<button type='button' class='viewer-tool' data-viewer-theatre aria-pressed='false' title='Theatre mode (T)'>▭ Theatre</button>");
            html.Append("<button type='button' class='viewer-tool' data-viewer-fullscreen title='Fullscreen (F)'>⤢ Fullscreen</button>");
            html.Append("</span>");
            html.Append("</div>");

            html.Append("</section>");

            Stage.Text = html.ToString();
        }

        // Every slide is in the page; Scripts/dojo.js shows one at a time.
        private static string ImageDeck(DataTable slides, string title)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div class='slide-deck image-deck' data-slides>");

            for (int i = 0; i < slides.Rows.Count; i++)
            {
                DataRow slide = slides.Rows[i];
                string src = DojoMaterials.SafeUrl(Convert.ToString(slide["image_url"]));
                string alt = slide["alt_text"] == DBNull.Value
                    ? title + " - slide " + (i + 1)
                    : Convert.ToString(slide["alt_text"]);

                html.Append("<figure class='image-slide" + (i == 0 ? " is-active" : "") + "' data-slide>");
                html.Append("<div class='image-slide-missing' aria-hidden='true'><span>" + (i + 1).ToString("00") + "</span><small>Slide image not found</small></div>");
                if (src != null)
                {
                    html.Append(
                        "<img src='" + H(src) + "' alt='" + H(alt) + "'" +
                        (i > 1 ? " loading='lazy'" : "") +
                        " onerror=\"this.remove();\" />"
                    );
                }
                html.Append("</figure>");
            }

            html.Append("</div>");
            html.Append("<button type='button' class='slide-zone is-prev' data-slide-step='-1' aria-label='Previous slide'>‹</button>");
            html.Append("<button type='button' class='slide-zone is-next' data-slide-step='1' aria-label='Next slide'>›</button>");
            html.Append("<div class='slide-progress' aria-hidden='true'><span data-slide-progress></span></div>");
            return html.ToString();
        }

        private static string SlideStrip(DataTable slides)
        {
            StringBuilder html = new StringBuilder("<div class='slide-strip' aria-label='All slides'>");

            for (int i = 0; i < slides.Rows.Count; i++)
            {
                string src = DojoMaterials.SafeUrl(Convert.ToString(slides.Rows[i]["image_url"]));
                html.Append("<button type='button' class='slide-thumb" + (i == 0 ? " is-active" : "") + "' data-slide-go='" + i + "' aria-label='Go to slide " + (i + 1) + "'>");
                html.Append("<span>" + (i + 1) + "</span>");
                if (src != null)
                {
                    html.Append("<img src='" + H(src) + "' alt='' loading='lazy' onerror=\"this.remove();\" />");
                }
                html.Append("</button>");
            }

            return html.Append("</div>").ToString();
        }

        private void DisplayPlaylist(DataTable playlist, int currentId)
        {
            StringBuilder html = new StringBuilder();
            int done = 0;

            foreach (DataRow row in playlist.Rows)
            {
                if (row["completed_at"] != DBNull.Value)
                {
                    done++;
                }
            }

            html.Append("<aside class='panel viewer-playlist'>");
            html.Append("<div class='viewer-playlist-head'>");
            html.Append("<span class='eyebrow'>COURSE MATERIALS</span>");
            html.Append("<strong>" + done + " / " + playlist.Rows.Count + " completed</strong>");
            html.Append(
                "<progress max='" + Math.Max(1, playlist.Rows.Count) +
                "' value='" + done + "'></progress>"
            );
            html.Append("</div>");
            html.Append("<ol>");

            string lastChapter = null;

            foreach (DataRow row in playlist.Rows)
            {
                string chapter = Convert.ToString(row["chapter_title"]);

                if (chapter != lastChapter)
                {
                    html.Append("<li class='viewer-chapter'>CH ");
                    html.Append(H(row["chapter_order"]));
                    html.Append(" · ");
                    html.Append(H(chapter));
                    html.Append("</li>");
                    lastChapter = chapter;
                }

                bool current = Convert.ToInt32(row["material_id"]) == currentId;
                bool completed = row["completed_at"] != DBNull.Value;
                bool video = DojoMaterials.IsVideo(Convert.ToString(row["material_type"]));

                html.Append("<li><a class='viewer-item");
                html.Append(current ? " is-current" : "");
                html.Append(completed ? " is-done" : "");
                html.Append("' href='MaterialView.aspx?id=");
                html.Append(H(row["material_id"]));
                html.Append("'");
                html.Append(current ? " aria-current='page'" : "");
                html.Append(">");
                html.Append("<span class='viewer-item-icon' aria-hidden='true'>");
                html.Append(completed ? "✓" : (video ? "▶" : "▤"));
                html.Append("</span><span>");
                html.Append(H(row["material_title"]));
                html.Append("</span></a></li>");
            }

            html.Append("</ol>");
            html.Append("</aside>");

            Playlist.Text = html.ToString();
        }
    }
}
