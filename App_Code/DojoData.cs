using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

public static class DojoData
{
    public const decimal PassMark = 50m;
    public const decimal MeritMark = 65m;
    public const decimal DistinctionMark = 80m;

    public static SqlParameter P(string n, object v)
    {
        return new SqlParameter(n, v ?? DBNull.Value);
    }

    public static DataTable Query(string sql, params SqlParameter[] ps)
    {
        using (var c = DojoDb.GetConnection())
        using (var cmd = new SqlCommand(sql, c))
        {
            cmd.Parameters.AddRange(ps);
            cmd.CommandTimeout = 20;
            using (var da = new SqlDataAdapter(cmd))
            {
                var t = new DataTable();
                da.Fill(t);
                return t;
            }
        }
    }

    // One-off statements that do not need a shared transaction.
    public static object Scalar(string sql, params SqlParameter[] ps)
    {
        using (var c = DojoDb.GetConnection())
        {
            c.Open();
            return Scalar(c, null, sql, ps);
        }
    }

    public static int Execute(string sql, params SqlParameter[] ps)
    {
        using (var c = DojoDb.GetConnection())
        {
            c.Open();
            return Execute(c, null, sql, ps);
        }
    }

    public static object Scalar(
        SqlConnection c,
        SqlTransaction tx,
        string sql,
        params SqlParameter[] ps)
    {
        using (var cmd = new SqlCommand(sql, c, tx))
        {
            cmd.Parameters.AddRange(ps);
            return cmd.ExecuteScalar();
        }
    }

    public static int Execute(
        SqlConnection c,
        SqlTransaction tx,
        string sql,
        params SqlParameter[] ps)
    {
        using (var cmd = new SqlCommand(sql, c, tx))
        {
            cmd.Parameters.AddRange(ps);
            return cmd.ExecuteNonQuery();
        }
    }

    public static string Num(object v)
    {
        return Convert.ToDecimal(v == DBNull.Value ? 0 : v)
            .ToString("0.#", CultureInfo.InvariantCulture);
    }

    public static string Band(object score)
    {
        if (score == null || score == DBNull.Value)
        {
            return "Ungraded";
        }

        decimal pct = Convert.ToDecimal(score);

        if (pct >= DistinctionMark)
        {
            return "Distinction";
        }

        if (pct >= MeritMark)
        {
            return "Merit";
        }

        if (pct >= PassMark)
        {
            return "Pass";
        }

        return "Fail";
    }

    public static bool IsPass(object score)
    {
        if (score == null || score == DBNull.Value)
        {
            return false;
        }

        return Convert.ToDecimal(score) >= PassMark;
    }

    public const string ResultsSql = @"
        SELECT
            a.attempt_id,
            a.quiz_id,
            c.course_id,
            c.course_title AS course,
            q.quiz_title AS quiz,
            q.quiz_type,
            a.submitted_at,
            a.score_earned,
            n.total,
            CASE
                WHEN n.total > 0
                    AND a.score_earned IS NOT NULL
                THEN CAST(
                    100.0 * a.score_earned / n.total
                    AS decimal(6,2)
                )
            END AS score
        FROM quiz_attempts a
        JOIN quizzes q ON q.quiz_id = a.quiz_id
        JOIN course_chapters ch ON ch.chapter_id = q.chapter_id
        JOIN courses c ON c.course_id = ch.course_id
        CROSS APPLY (
            SELECT COUNT(*) total
            FROM quiz_questions x
            WHERE x.quiz_id = q.quiz_id
                AND x.question_type IN ('MCQ', 'TRUE_FALSE')
        ) n
        WHERE a.user_id = @uid
            AND a.submitted_at IS NOT NULL";

    public static DataTable Results(int uid)
    {
        return Query(
            ResultsSql +
            @" AND a.attempt_id = (
                SELECT TOP 1 z.attempt_id
                FROM quiz_attempts z
                WHERE z.quiz_id = a.quiz_id
                    AND z.user_id = a.user_id
                    AND z.submitted_at IS NOT NULL
                ORDER BY z.submitted_at DESC, z.attempt_id DESC
            )
            ORDER BY a.submitted_at DESC, a.attempt_id DESC",
            P("@uid", uid)
        );
    }

    public static DataTable Courses(int uid)
    {
        return Query(@"
            SELECT
                c.course_id,
                c.course_title,
                c.description,
                c.course_level,
                c.course_image,
                e.enrollment_id,
                e.completion_percentage,
                e.average_score_percentage,
                e.completed_at,
                (
                    SELECT COUNT(*)
                    FROM course_materials m
                    JOIN course_chapters h ON h.chapter_id = m.chapter_id
                    WHERE h.course_id = c.course_id
                ) AS materials_total,
                (
                    SELECT COUNT(*)
                    FROM student_material_progress p
                    JOIN course_materials m ON m.material_id = p.material_id
                    JOIN course_chapters h ON h.chapter_id = m.chapter_id
                    WHERE p.enrollment_id = e.enrollment_id
                        AND p.completed_at IS NOT NULL
                        AND h.course_id = c.course_id
                ) AS materials_done,
                (
                    SELECT COUNT(*)
                    FROM quizzes q
                    JOIN course_chapters h ON h.chapter_id = q.chapter_id
                    CROSS APPLY (
                        SELECT COUNT(*) total
                        FROM quiz_questions x
                        WHERE x.quiz_id = q.quiz_id
                    ) n
                    WHERE h.course_id = c.course_id
                        AND n.total > 0
                ) AS quizzes_total,
                (
                    SELECT COUNT(*)
                    FROM quizzes q
                    JOIN course_chapters h ON h.chapter_id = q.chapter_id
                    CROSS APPLY (
                        SELECT TOP 1
                            a.score_earned
                        FROM quiz_attempts a
                        WHERE a.quiz_id = q.quiz_id
                            AND a.user_id = @uid
                            AND a.submitted_at IS NOT NULL
                        ORDER BY a.submitted_at DESC, a.attempt_id DESC
                    ) latest
                    CROSS APPLY (
                        SELECT
                            COUNT(*) total,
                            SUM(CASE
                                WHEN x.question_type IN ('MCQ', 'TRUE_FALSE')
                                THEN 1 ELSE 0
                            END) objective_total
                        FROM quiz_questions x
                        WHERE x.quiz_id = q.quiz_id
                    ) n
                    WHERE h.course_id = c.course_id
                        AND n.total > 0
                        AND (
                            n.objective_total = 0
                            OR 100.0 * latest.score_earned /
                                n.objective_total >= 50
                        )
                ) AS quizzes_done
            FROM course_enrollments e
            JOIN courses c ON c.course_id = e.course_id
            WHERE e.user_id = @uid
            ORDER BY e.enrolled_at DESC",
            P("@uid", uid)
        );
    }

    public static int LockEnrollment(
        SqlConnection c,
        SqlTransaction tx,
        int uid,
        int course)
    {
        object id = Scalar(
            c,
            tx,
            @"
            SELECT enrollment_id
            FROM course_enrollments WITH (UPDLOCK, HOLDLOCK)
            WHERE user_id = @u AND course_id = @c",
            P("@u", uid),
            P("@c", course)
        );

        if (id == null)
        {
            throw new InvalidOperationException("Enrollment not found.");
        }

        return Convert.ToInt32(id);
    }

    public static DataRow LatestAttempt(int uid, int quizId)
    {
        DataTable t = Query(@"
            SELECT TOP 1
                a.attempt_id,
                a.quiz_id,
                q.quiz_type,
                a.submitted_at,
                a.score_earned,
                n.total,
                CASE
                    WHEN n.total > 0
                        AND a.score_earned IS NOT NULL
                    THEN CAST(
                        100.0 * a.score_earned / n.total
                        AS decimal(6,2)
                    )
                END AS score
            FROM quiz_attempts a
            JOIN quizzes q ON q.quiz_id = a.quiz_id
            CROSS APPLY (
                SELECT COUNT(*) total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
                    AND x.question_type IN ('MCQ', 'TRUE_FALSE')
            ) n
            WHERE a.user_id = @uid
                AND a.quiz_id = @quizId
                AND a.submitted_at IS NOT NULL
            ORDER BY a.submitted_at DESC, a.attempt_id DESC",
            P("@uid", uid),
            P("@quizId", quizId)
        );

        return t.Rows.Count == 0 ? null : t.Rows[0];
    }

    public static bool CanAttemptQuiz(int uid, int quizId)
    {
        DataRow latest = LatestAttempt(uid, quizId);

        if (latest == null)
        {
            return true;
        }

        if (latest["score"] == DBNull.Value)
        {
            return false;
        }

        return !IsPass(latest["score"]);
    }

    public static DataTable Leaderboard(int quizId)
    {
        return Query(@"
            SELECT
                u.user_id,
                u.full_name,
                u.[role],
                CAST(
                    100.0 * a.score_earned / n.total
                    AS decimal(6,2)
                ) AS score,
                a.submitted_at
            FROM quiz_attempts a
            JOIN users u ON u.user_id = a.user_id
            JOIN quizzes q ON q.quiz_id = a.quiz_id
            CROSS APPLY (
                SELECT COUNT(*) total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
                    AND x.question_type IN ('MCQ', 'TRUE_FALSE')
            ) n
            WHERE a.quiz_id = @quizId
                AND a.submitted_at IS NOT NULL
                AND n.total > 0
                AND a.attempt_id = (
                    SELECT TOP 1 z.attempt_id
                    FROM quiz_attempts z
                    WHERE z.quiz_id = a.quiz_id
                        AND z.user_id = a.user_id
                        AND z.submitted_at IS NOT NULL
                    ORDER BY z.submitted_at DESC, z.attempt_id DESC
                )
            ORDER BY score DESC, a.submitted_at ASC, u.full_name",
            P("@quizId", quizId)
        );
    }

    public static DataTable LeaderboardQuizzes(int uid)
    {
        return Query(@"
            SELECT
                q.quiz_id,
                q.quiz_title,
                c.course_title
            FROM quizzes q
            JOIN course_chapters ch ON ch.chapter_id = q.chapter_id
            JOIN courses c ON c.course_id = ch.course_id
            JOIN course_enrollments e
                ON e.course_id = c.course_id
                AND e.user_id = @uid
            CROSS APPLY (
                SELECT COUNT(*) total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
                    AND x.question_type IN ('MCQ', 'TRUE_FALSE')
            ) n
            WHERE n.total > 0
            ORDER BY c.course_title, q.quiz_title",
            P("@uid", uid)
        );
    }

    public static DataTable UnlockBadges(int uid)
    {
        return Query(@"
            SELECT
                b.badges_id,
                b.badges_name,
                b.description,
                b.distinction_count,
                b.icon,
                (
                    SELECT MIN(sb.earned_at)
                    FROM student_badges sb
                    JOIN course_enrollments e
                        ON e.enrollment_id = sb.enrollment_id
                    WHERE e.user_id = @uid
                        AND sb.badge_id = b.badges_id
                ) AS earned_at
            FROM badges b
            WHERE b.distinction_count > 0
            ORDER BY b.distinction_count, b.badges_id",
            P("@uid", uid)
        );
    }

    public static int Enroll(int uid, int courseId)
    {
        using (SqlConnection c = DojoDb.GetConnection())
        {
            c.Open();

            using (SqlTransaction tx = c.BeginTransaction())
            {
                object published = Scalar(
                    c,
                    tx,
                    @"
                    SELECT course_id
                    FROM courses
                    WHERE course_id = @c
                        AND [status] = 'PUBLISHED'",
                    P("@c", courseId)
                );

                if (published == null)
                {
                    throw new InvalidOperationException(
                        "This course is not open for enrollment."
                    );
                }

                object existing = Scalar(
                    c,
                    tx,
                    @"
                    SELECT enrollment_id
                    FROM course_enrollments
                    WHERE user_id = @u AND course_id = @c",
                    P("@u", uid),
                    P("@c", courseId)
                );

                int enrollmentId;

                if (existing != null)
                {
                    enrollmentId = Convert.ToInt32(existing);
                }
                else
                {
                    enrollmentId = Convert.ToInt32(
                        Scalar(
                            c,
                            tx,
                            @"
                            INSERT INTO course_enrollments
                            (user_id, course_id)
                            OUTPUT INSERTED.enrollment_id
                            VALUES (@u, @c)",
                            P("@u", uid),
                            P("@c", courseId)
                        )
                    );
                }

                Refresh(c, tx, uid, courseId, enrollmentId);
                tx.Commit();
                return enrollmentId;
            }
        }
    }

    public static void Refresh(
        SqlConnection c,
        SqlTransaction tx,
        int uid,
        int course,
        int enrollment)
    {
        EnsureUnlockBadges(c, tx);

        Execute(
            c,
            tx,
            @"
            DECLARE @mt int, @md int, @qt int, @qd int;
            DECLARE @pct decimal(5,2), @avg decimal(5,2);

            SELECT @mt = COUNT(*)
            FROM course_materials m
            JOIN course_chapters h ON h.chapter_id = m.chapter_id
            WHERE h.course_id = @c;

            SELECT @md = COUNT(*)
            FROM student_material_progress p
            JOIN course_materials m ON m.material_id = p.material_id
            JOIN course_chapters h ON h.chapter_id = m.chapter_id
            WHERE p.enrollment_id = @e
                AND p.completed_at IS NOT NULL
                AND h.course_id = @c;

            SELECT @qt = COUNT(*)
            FROM quizzes q
            JOIN course_chapters h ON h.chapter_id = q.chapter_id
            CROSS APPLY (
                SELECT COUNT(*) total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
            ) n
            WHERE h.course_id = @c
                AND n.total > 0;

            SELECT @qd = COUNT(*)
            FROM quizzes q
            JOIN course_chapters h ON h.chapter_id = q.chapter_id
            CROSS APPLY (
                SELECT TOP 1 a.score_earned
                FROM quiz_attempts a
                WHERE a.quiz_id = q.quiz_id
                    AND a.user_id = @u
                    AND a.submitted_at IS NOT NULL
                ORDER BY a.submitted_at DESC, a.attempt_id DESC
            ) latest
            CROSS APPLY (
                SELECT
                    COUNT(*) total,
                    SUM(CASE
                        WHEN x.question_type IN ('MCQ', 'TRUE_FALSE')
                        THEN 1 ELSE 0
                    END) objective_total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
            ) n
            WHERE h.course_id = @c
                AND n.total > 0
                AND (
                    n.objective_total = 0
                    OR 100.0 * latest.score_earned /
                        n.objective_total >= 50
                );

            SET @pct = CASE
                WHEN @mt + @qt = 0 THEN 0
                ELSE 100.0 * (@md + @qd) / (@mt + @qt)
            END;

            SELECT @avg = CASE
                WHEN COUNT(*) = 0 THEN NULL
                WHEN COUNT(latest.attempt_id) < COUNT(*) THEN NULL
                ELSE AVG(CAST(
                    100.0 * latest.score_earned / n.total
                    AS decimal(6,2)
                ))
            END
            FROM quizzes q
            JOIN course_chapters h ON h.chapter_id = q.chapter_id
            CROSS APPLY (
                SELECT COUNT(*) total
                FROM quiz_questions x
                WHERE x.quiz_id = q.quiz_id
                    AND x.question_type IN ('MCQ', 'TRUE_FALSE')
            ) n
            OUTER APPLY (
                SELECT TOP 1 a.attempt_id, a.score_earned
                FROM quiz_attempts a
                WHERE a.quiz_id = q.quiz_id
                    AND a.user_id = @u
                    AND a.submitted_at IS NOT NULL
                ORDER BY a.submitted_at DESC, a.attempt_id DESC
            ) latest
            WHERE h.course_id = @c
                AND n.total > 0;

            UPDATE course_enrollments
            SET
                completion_percentage = @pct,
                average_score_percentage = @avg,
                completed_at = CASE
                    WHEN @pct = 100
                    THEN COALESCE(completed_at, SYSDATETIME())
                    ELSE NULL
                END
            WHERE enrollment_id = @e;

            IF @pct = 100
                AND NOT EXISTS (
                    SELECT 1 FROM certificates WHERE enrollment_id = @e
                )
            INSERT INTO certificates
            (enrollment_id, certificate_number)
            VALUES (
                @e,
                'AD-' + CONVERT(varchar(36), NEWID())
            );

            DECLARE @dist int;
            SELECT @dist = COUNT(*)
            FROM course_enrollments
            WHERE user_id = @u
                AND average_score_percentage >= 80;

            INSERT INTO student_badges (enrollment_id, badge_id)
            SELECT @e, b.badges_id
            FROM badges b
            WHERE b.distinction_count > 0
                AND @dist >= b.distinction_count
                AND NOT EXISTS (
                    SELECT 1
                    FROM student_badges sb
                    JOIN course_enrollments e2
                        ON e2.enrollment_id = sb.enrollment_id
                    WHERE e2.user_id = @u
                        AND sb.badge_id = b.badges_id
                );",
            P("@u", uid),
            P("@c", course),
            P("@e", enrollment)
        );
    }

    private static void EnsureUnlockBadges(
        SqlConnection c,
        SqlTransaction tx)
    {
        Execute(
            c,
            tx,
            @"
            IF NOT EXISTS (
                SELECT 1 FROM badges WHERE badges_name = N'Join Master'
            )
            INSERT INTO badges
            (badges_name, description, distinction_count, icon)
            VALUES (
                N'Join Master',
                N'Earn Distinction (80%+) in 3 courses.',
                3,
                N'link'
            );

            IF NOT EXISTS (
                SELECT 1 FROM badges WHERE badges_name = N'Chart Maker'
            )
            INSERT INTO badges
            (badges_name, description, distinction_count, icon)
            VALUES (
                N'Chart Maker',
                N'Earn Distinction (80%+) in 8 courses.',
                8,
                N'chart'
            );

            IF NOT EXISTS (
                SELECT 1 FROM badges WHERE badges_name = N'Clean Sweep'
            )
            INSERT INTO badges
            (badges_name, description, distinction_count, icon)
            VALUES (
                N'Clean Sweep',
                N'Earn Distinction (80%+) in 12 courses.',
                12,
                N'broom'
            );

            IF NOT EXISTS (
                SELECT 1 FROM badges WHERE badges_name = N'Perfect Score'
            )
            INSERT INTO badges
            (badges_name, description, distinction_count, icon)
            VALUES (
                N'Perfect Score',
                N'Earn Distinction (80%+) in 16 courses.',
                16,
                N'target'
            );"
        );
    }
}
