using System;
using System.Data;
using System.Text;

public static class DojoUi
{
    public static string Cards(DataTable t, int max)
    {
        var b = new StringBuilder();
        int n = 0;

        foreach (DataRow r in t.Rows)
        {
            if (n++ >= max)
            {
                break;
            }

            string pct = DojoData.Num(r["completion_percentage"]);
            bool complete = Convert.ToDecimal(r["completion_percentage"]) >= 100;
            string status = complete ? "complete" : "progress";
            string openLabel = complete ? "Revisit course" : "Continue learning";

            b.Append("<article class='course-card' data-course data-title='");
            b.Append(DojoPage.H(r["course_title"]));
            b.Append("' data-status='");
            b.Append(status);
            b.Append("'>");

            b.Append(CourseArt(r, n));

            b.Append("<div class='course-body'><h3>");
            b.Append(DojoPage.H(r["course_title"]));
            b.Append("</h3><p class='muted counts'>");
            b.Append(r["materials_done"]);
            b.Append(" of ");
            b.Append(r["materials_total"]);
            b.Append(" materials done, ");
            b.Append(r["quizzes_done"]);
            b.Append(" of ");
            b.Append(r["quizzes_total"]);
            b.Append(" quizzes completed");
            if (r["average_score_percentage"] != DBNull.Value)
            {
                b.Append(" · ");
                b.Append(DojoData.Num(r["average_score_percentage"]));
                b.Append("% ");
                b.Append(DojoData.Band(r["average_score_percentage"]));
            }
            b.Append("</p>");

            b.Append("<div class='progress-label'><span>Course progress</span><strong>");
            b.Append(pct);
            b.Append("%</strong></div>");
            b.Append("<progress max='100' value='");
            b.Append(pct);
            b.Append("'>");
            b.Append(pct);
            b.Append("%</progress>");

            b.Append("<div class='course-actions'>");
            b.Append("<a class='course-open' href='CoursePage.aspx?id=");
            b.Append(r["course_id"]);
            b.Append("'>");
            b.Append(openLabel);
            b.Append(" <span>→</span></a>");

            if (complete && r.Table.Columns.Contains("enrollment_id"))
            {
                b.Append("<a class='btn ghost' href='Certificate.aspx?enrollment=");
                b.Append(r["enrollment_id"]);
                b.Append("'>Certificate</a>");
            }

            b.Append("</div></div></article>");
        }

        return b.Length == 0
            ? "<p class='empty'>No courses yet. Browse courses to enrol.</p>"
            : b.ToString();
    }

    public static string CourseRows(DataTable t)
    {
        if (t.Rows.Count == 0)
        {
            return "<p class='empty'>No enrolled courses yet. Browse the catalogue to join one.</p>";
        }

        var b = new StringBuilder();
        string[] tags = { "SQL", "Py", "Viz", "R", "BI" };
        int n = 0;

        foreach (DataRow r in t.Rows)
        {
            string pct = DojoData.Num(r["completion_percentage"]);
            bool complete = Convert.ToDecimal(r["completion_percentage"]) >= 100;
            string status = complete ? "complete" : "progress";

            b.Append("<article class='course-row' data-course data-title='");
            b.Append(DojoPage.H(r["course_title"]));
            b.Append("' data-status='");
            b.Append(status);
            b.Append("'>");

            b.Append("<span class='course-chip chip-");
            b.Append(n % 5);
            b.Append("'>");
            b.Append(DojoPage.H(tags[n % tags.Length]));
            b.Append("</span><div class='course-row-body'><strong>");
            b.Append(DojoPage.H(r["course_title"]));
            b.Append("</strong><small>");

            if (complete)
            {
                object done = r["completed_at"];
                b.Append("Completed");
                if (done != DBNull.Value)
                {
                    b.Append(" on ");
                    b.Append(Convert.ToDateTime(done).ToString("dd MMM yyyy"));
                }
            }
            else
            {
                b.Append(r["materials_done"]);
                b.Append(" of ");
                b.Append(r["materials_total"]);
                b.Append(" materials done, ");
                b.Append(r["quizzes_done"]);
                b.Append(" of ");
                b.Append(r["quizzes_total"]);
                b.Append(" quizzes completed");
                if (r["average_score_percentage"] != DBNull.Value)
                {
                    b.Append(" · ");
                    b.Append(DojoData.Num(r["average_score_percentage"]));
                    b.Append("% ");
                    b.Append(DojoData.Band(r["average_score_percentage"]));
                }
            }

            b.Append("</small><div class='progress-label'><span></span><strong>");
            b.Append(pct);
            b.Append("%</strong></div><progress max='100' value='");
            b.Append(pct);
            b.Append("'></progress></div>");

            if (complete)
            {
                b.Append("<a class='btn ghost' href='Certificate.aspx?enrollment=");
                b.Append(r["enrollment_id"]);
                b.Append("'><span>↓</span> Certificate</a>");
            }
            else
            {
                b.Append("<a class='btn' href='CoursePage.aspx?id=");
                b.Append(r["course_id"]);
                b.Append("'>Resume</a>");
            }

            b.Append("</article>");
            n++;
        }

        return b.ToString();
    }

    public static string Table(DataTable t)
    {
        if (t.Rows.Count == 0)
        {
            return "<p class='empty'>No results to display.</p>";
        }

        var b = new StringBuilder("<div class='table-scroll'><table><thead><tr>");

        foreach (DataColumn c in t.Columns)
        {
            b.Append("<th scope='col'>");
            b.Append(DojoPage.H(c.ColumnName));
            b.Append("</th>");
        }

        b.Append("</tr></thead><tbody>");

        foreach (DataRow r in t.Rows)
        {
            b.Append("<tr>");

            foreach (DataColumn col in t.Columns)
            {
                object x = r[col];
                b.Append("<td>");
                b.Append(DojoPage.H(x));

                decimal score;
                if ((col.ColumnName == "score" || col.ColumnName == "avg_score")
                    && decimal.TryParse(Convert.ToString(x), out score))
                {
                    b.Append("<progress aria-label='Score percentage' max='100' value='");
                    b.Append(DojoData.Num(Math.Max(0, Math.Min(100, score))));
                    b.Append("'></progress>");
                }

                b.Append("</td>");
            }

            b.Append("</tr>");
        }

        return b.Append("</tbody></table></div>").ToString();
    }

    public static string BadgeGrid(DataTable badges)
    {
        if (badges.Rows.Count == 0)
        {
            return "<p class='muted'>Badges unlock from Distinction (80%+) course averages, not individual quizzes.</p>";
        }

        var b = new StringBuilder("<div class='badge-grid'>");

        foreach (DataRow badge in badges.Rows)
        {
            bool earned = badge["earned_at"] != DBNull.Value;
            string icon = Convert.ToString(badge["icon"]);

            b.Append("<button type='button' class='hex-badge");
            b.Append(earned ? " earned" : " locked");
            b.Append("' data-badge-detail='");
            b.Append(DojoPage.H(badge["description"]));
            b.Append("' title='");
            b.Append(DojoPage.H(badge["description"]));
            b.Append("'>");
            b.Append("<span class='hex hex-");
            b.Append(DojoPage.H(icon));
            b.Append("' aria-hidden='true'>");
            b.Append(BadgeGlyph(icon));
            b.Append("</span><strong>");
            b.Append(DojoPage.H(badge["badges_name"]));
            b.Append("</strong><small>");

            if (earned)
            {
                b.Append("Earned ");
                b.Append(Convert.ToDateTime(badge["earned_at"]).ToString("d MMM yyyy"));
            }
            else
            {
                b.Append("Locked");
            }

            b.Append("</small></button>");
        }

        return b.Append("</div><p class='muted' id='badgeDetail'>Tap a badge to see how it was earned.</p>").ToString();
    }

    // Small badge naming a user's role (forum posts, replies, leaderboard).
    public static string RolePill(object role)
    {
        switch (Convert.ToString(role))
        {
            case "ADMIN":
                return "<span class='role-pill role-admin'>Admin</span>";
            case "PEER LEADER":
                return "<span class='role-pill role-peer'>Peer Leader</span>";
            default:
                return "<span class='role-pill role-student'>Student</span>";
        }
    }

    // Course cover: the course image when one is set, otherwise the code-style art.
    // A missing image file falls back to the art too (no broken-image icon).
    public static string CourseArt(DataRow r, int n)
    {
        string image = r.Table.Columns.Contains("course_image") && r["course_image"] != DBNull.Value
            ? DojoMaterials.SafeUrl(Convert.ToString(r["course_image"]))
            : null;

        var b = new StringBuilder();
        b.Append("<div class='course-art art-");
        b.Append(n % 3);
        b.Append(image != null ? " has-image" : "");
        b.Append("'>");

        if (image != null)
        {
            b.Append("<img src='");
            b.Append(DojoPage.H(image));
            b.Append("' alt='' loading='lazy' onerror=\"this.parentNode.classList.remove('has-image');this.remove();\" />");
        }

        b.Append("<span>");
        b.Append(n % 3 == 1 ? "SELECT *" : "{ data }");
        b.Append("</span><b>");
        b.Append(DojoPage.H(r["course_level"]));
        b.Append("</b></div>");
        return b.ToString();
    }

    public static string LeaderboardList(DataTable ranks, int userId)
    {
        if (ranks.Rows.Count == 0)
        {
            return "<p class='muted'>No submitted scores for this quiz yet.</p>";
        }

        var b = new StringBuilder("<ol class='rank-list'>");
        int place = 0;

        foreach (DataRow row in ranks.Rows)
        {
            place++;
            bool you = Convert.ToInt32(row["user_id"]) == userId;
            string name = Convert.ToString(row["full_name"]);

            b.Append("<li class='rank");
            if (you)
            {
                b.Append(" you");
            }

            if (place <= 3)
            {
                b.Append(" medal-");
                b.Append(place);
            }

            b.Append("'><span class='rank-n'>");
            b.Append(place);
            b.Append("</span><span class='rank-name'>");
            b.Append(DojoPage.H(name));
            if (you)
            {
                b.Append(" (you)");
            }

            b.Append("</span><span class='rank-score'>");
            b.Append(DojoData.Num(row["score"]));
            b.Append("%</span></li>");
        }

        return b.Append("</ol><p class='muted rank-note'>Ranked by score, then earliest submission.</p>").ToString();
    }

    private static string BadgeGlyph(string icon)
    {
        switch (icon)
        {
            case "link":
                return "<svg viewBox='0 0 24 24' width='34' height='34' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'><path d='M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71'/><path d='M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71'/></svg>";
            case "chart":
                return "<svg viewBox='0 0 24 24' width='34' height='34' fill='currentColor'><rect x='4' y='12' width='4' height='8' rx='1'/><rect x='10' y='7' width='4' height='13' rx='1'/><rect x='16' y='3' width='4' height='17' rx='1'/></svg>";
            case "broom":
                return "<svg viewBox='0 0 24 24' width='34' height='34' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'><path d='M14 4l6 6'/><path d='M12.5 6.5L4 19l3 1 2-4 4 2 7.5-12.5z'/></svg>";
            case "target":
                return "<svg viewBox='0 0 24 24' width='34' height='34' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='8'/><circle cx='12' cy='12' r='4'/><circle cx='12' cy='12' r='1.4' fill='currentColor'/></svg>";
            default:
                return "<svg viewBox='0 0 24 24' width='34' height='34' fill='currentColor'><path d='M12 2l2.4 7.2H22l-6 4.4 2.3 7.2L12 16.8 5.7 20.8 8 13.6 2 9.2h7.6z'/></svg>";
        }
    }
}
