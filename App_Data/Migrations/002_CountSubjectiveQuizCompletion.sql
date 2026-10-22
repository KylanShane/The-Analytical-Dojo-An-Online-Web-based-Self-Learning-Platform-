/*
    Recalculate existing enrollment progress after subjective quizzes become
    required completion items.

    Completion rules:
    - Every material must be marked complete.
    - Every non-empty objective quiz must be passed at 50% or above.
    - Every non-empty subjective quiz must be submitted once.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

WITH progress AS (
    SELECT
        e.enrollment_id,
        totals.materials_total,
        totals.materials_done,
        totals.quizzes_total,
        totals.quizzes_done,
        CAST(CASE
            WHEN totals.materials_total + totals.quizzes_total = 0 THEN 0
            ELSE 100.0 *
                (totals.materials_done + totals.quizzes_done) /
                (totals.materials_total + totals.quizzes_total)
        END AS decimal(5,2)) AS completion_percentage
    FROM course_enrollments e
    CROSS APPLY (
        SELECT
            (
                SELECT COUNT(*)
                FROM course_materials m
                JOIN course_chapters ch
                    ON ch.chapter_id = m.chapter_id
                WHERE ch.course_id = e.course_id
            ) AS materials_total,
            (
                SELECT COUNT(*)
                FROM student_material_progress smp
                JOIN course_materials m
                    ON m.material_id = smp.material_id
                JOIN course_chapters ch
                    ON ch.chapter_id = m.chapter_id
                WHERE smp.enrollment_id = e.enrollment_id
                    AND smp.completed_at IS NOT NULL
                    AND ch.course_id = e.course_id
            ) AS materials_done,
            (
                SELECT COUNT(*)
                FROM quizzes q
                JOIN course_chapters ch
                    ON ch.chapter_id = q.chapter_id
                WHERE ch.course_id = e.course_id
                    AND EXISTS (
                        SELECT 1
                        FROM quiz_questions qq
                        WHERE qq.quiz_id = q.quiz_id
                    )
            ) AS quizzes_total,
            (
                SELECT COUNT(*)
                FROM quizzes q
                JOIN course_chapters ch
                    ON ch.chapter_id = q.chapter_id
                CROSS APPLY (
                    SELECT TOP 1 qa.score_earned
                    FROM quiz_attempts qa
                    WHERE qa.quiz_id = q.quiz_id
                        AND qa.user_id = e.user_id
                        AND qa.submitted_at IS NOT NULL
                    ORDER BY qa.submitted_at DESC, qa.attempt_id DESC
                ) latest
                CROSS APPLY (
                    SELECT
                        COUNT(*) AS question_total,
                        SUM(CASE
                            WHEN qq.question_type IN ('MCQ', 'TRUE_FALSE')
                            THEN 1 ELSE 0
                        END) AS objective_total
                    FROM quiz_questions qq
                    WHERE qq.quiz_id = q.quiz_id
                ) questions
                WHERE ch.course_id = e.course_id
                    AND questions.question_total > 0
                    AND (
                        questions.objective_total = 0
                        OR 100.0 * latest.score_earned /
                            questions.objective_total >= 50
                    )
            ) AS quizzes_done
    ) totals
)
UPDATE enrollment
SET
    completion_percentage = progress.completion_percentage,
    completed_at = CASE
        WHEN progress.completion_percentage = 100
        THEN COALESCE(enrollment.completed_at, SYSDATETIME())
        ELSE NULL
    END
FROM course_enrollments enrollment
JOIN progress ON progress.enrollment_id = enrollment.enrollment_id;

INSERT INTO certificates (enrollment_id, certificate_number)
SELECT
    e.enrollment_id,
    'AD-' + CONVERT(varchar(36), NEWID())
FROM course_enrollments e
WHERE e.completion_percentage = 100
    AND NOT EXISTS (
        SELECT 1
        FROM certificates c
        WHERE c.enrollment_id = e.enrollment_id
    );

COMMIT TRANSACTION;
