-- =====================================================
-- The Analytical Dojo - full schema with chapters
-- LocalDB / SQL Server
-- =====================================================

-- ---------- 1. users ----------
CREATE TABLE users (
    user_id           INT IDENTITY(1,1) PRIMARY KEY,
    username          NVARCHAR(50)  NOT NULL UNIQUE,
    full_name         NVARCHAR(100) NOT NULL,
    email             NVARCHAR(150) NOT NULL UNIQUE,
    password_hash     NVARCHAR(255) NOT NULL,
    [role]            NVARCHAR(20)  NOT NULL DEFAULT 'STUDENT',
    peer_leader_since DATETIME2     NULL,
    created_at        DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT CK_users_role
        CHECK ([role] IN ('STUDENT', 'ADMIN', 'PEER LEADER'))
);

-- ---------- 2. courses ----------
CREATE TABLE courses (
    course_id     INT IDENTITY(1,1) PRIMARY KEY,
    course_title  NVARCHAR(150) NOT NULL,
    description   NVARCHAR(MAX) NULL,
    course_image  NVARCHAR(255) NULL,
    course_level  NVARCHAR(20)  NOT NULL DEFAULT 'basic',
    [status]      NVARCHAR(20)  NOT NULL DEFAULT 'PENDING',
    created_by    INT           NOT NULL,
    created_at    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_courses_created_by
        FOREIGN KEY (created_by) REFERENCES users(user_id),

    CONSTRAINT CK_courses_level
        CHECK (course_level IN ('basic', 'intermediate', 'advanced')),

    CONSTRAINT CK_courses_status
        CHECK ([status] IN ('PENDING', 'PUBLISHED', 'ARCHIVED'))
);

-- ---------- 3. course_chapters ----------
CREATE TABLE course_chapters (
    chapter_id    INT IDENTITY(1,1) PRIMARY KEY,
    course_id     INT           NOT NULL,
    chapter_title NVARCHAR(150) NOT NULL,
    description   NVARCHAR(MAX) NULL,
    chapter_order INT           NOT NULL,

    CONSTRAINT FK_chapters_course
        FOREIGN KEY (course_id) REFERENCES courses(course_id),

    CONSTRAINT UQ_chapters_order
        UNIQUE (course_id, chapter_order),

    CONSTRAINT CK_chapters_order
        CHECK (chapter_order > 0)
);

-- ---------- 4. course_materials ----------
CREATE TABLE course_materials (
    material_id    INT IDENTITY(1,1) PRIMARY KEY,
    chapter_id     INT           NOT NULL,
    material_title NVARCHAR(150) NOT NULL,
    description    NVARCHAR(MAX) NULL,
    material_type  NVARCHAR(10)  NOT NULL,
    file_url       NVARCHAR(500) NOT NULL,
    material_image NVARCHAR(255) NULL,
    uploaded_by    INT           NOT NULL,
    uploaded_at    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_materials_chapter
        FOREIGN KEY (chapter_id) REFERENCES course_chapters(chapter_id),

    CONSTRAINT FK_materials_uploader
        FOREIGN KEY (uploaded_by) REFERENCES users(user_id),

    CONSTRAINT CK_materials_type
        CHECK (material_type IN ('VIDEO', 'PPT'))
);

-- ---------- 5. course_enrollments ----------
CREATE TABLE course_enrollments (
    enrollment_id            INT IDENTITY(1,1) PRIMARY KEY,
    user_id                  INT          NOT NULL,
    course_id                INT          NOT NULL,
    enrolled_at              DATETIME2    NOT NULL DEFAULT SYSDATETIME(),
    completion_percentage    DECIMAL(5,2) NOT NULL DEFAULT 0,
    average_score_percentage DECIMAL(5,2) NULL,
    completed_at             DATETIME2    NULL,

    CONSTRAINT FK_enroll_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),

    CONSTRAINT FK_enroll_course
        FOREIGN KEY (course_id) REFERENCES courses(course_id),

    CONSTRAINT UQ_enroll_user_course
        UNIQUE (user_id, course_id),

    CONSTRAINT CK_enroll_completion
        CHECK (completion_percentage BETWEEN 0 AND 100),

    CONSTRAINT CK_enroll_average
        CHECK (
            average_score_percentage IS NULL
            OR average_score_percentage BETWEEN 0 AND 100
        )
);

-- ---------- 6. student_material_progress ----------
CREATE TABLE student_material_progress (
    progress_id   INT IDENTITY(1,1) PRIMARY KEY,
    enrollment_id INT       NOT NULL,
    material_id   INT       NOT NULL,
    completed_at  DATETIME2 NULL,

    CONSTRAINT FK_progress_enrollment
        FOREIGN KEY (enrollment_id)
        REFERENCES course_enrollments(enrollment_id),

    CONSTRAINT FK_progress_material
        FOREIGN KEY (material_id)
        REFERENCES course_materials(material_id),

    CONSTRAINT UQ_progress
        UNIQUE (enrollment_id, material_id)
);

-- ---------- 7. certificates ----------
CREATE TABLE certificates (
    certificate_id     INT IDENTITY(1,1) PRIMARY KEY,
    enrollment_id      INT          NOT NULL,
    certificate_number NVARCHAR(50) NOT NULL UNIQUE,

    CONSTRAINT FK_cert_enrollment
        FOREIGN KEY (enrollment_id)
        REFERENCES course_enrollments(enrollment_id)
);

-- ---------- 8. badges ----------
CREATE TABLE badges (
    badges_id         INT IDENTITY(1,1) PRIMARY KEY,
    badges_name       NVARCHAR(100) NOT NULL,
    description       NVARCHAR(500) NULL,
    distinction_count INT           NOT NULL DEFAULT 0,
    icon              NVARCHAR(255) NULL
);

-- ---------- 9. student_badges ----------
CREATE TABLE student_badges (
    student_badge_id INT IDENTITY(1,1) PRIMARY KEY,
    enrollment_id    INT       NOT NULL,
    badge_id         INT       NOT NULL,
    earned_at        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_sbadge_enrollment
        FOREIGN KEY (enrollment_id)
        REFERENCES course_enrollments(enrollment_id),

    CONSTRAINT FK_sbadge_badge
        FOREIGN KEY (badge_id) REFERENCES badges(badges_id)
);

-- ---------- 10. forum_posts ----------
CREATE TABLE forum_posts (
    post_id    INT IDENTITY(1,1) PRIMARY KEY,
    user_id    INT           NOT NULL,
    course_id  INT           NOT NULL,
    content    NVARCHAR(MAX) NOT NULL,
    created_at DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    is_deleted BIT           NOT NULL DEFAULT 0,

    CONSTRAINT FK_forum_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),

    CONSTRAINT FK_forum_course
        FOREIGN KEY (course_id) REFERENCES courses(course_id)
);

-- ---------- 11. peer_call_requests ----------
CREATE TABLE peer_call_requests (
    request_id                  INT IDENTITY(1,1) PRIMARY KEY,
    user_id                     INT           NOT NULL,
    assigned_peer_leader_id     INT           NULL,
    student_request_description NVARCHAR(MAX) NOT NULL,
    requested_datetime          DATETIME2     NOT NULL, -- Requested call time (Malaysia local time)
    [status]                    NVARCHAR(20)  NOT NULL DEFAULT 'PENDING',
    meeting_url                 NVARCHAR(500) NULL,
    rejection_reason            NVARCHAR(500) NULL,

    CONSTRAINT FK_peer_user
        FOREIGN KEY (user_id) REFERENCES users(user_id),

    CONSTRAINT FK_peer_leader
        FOREIGN KEY (assigned_peer_leader_id) REFERENCES users(user_id),

    CONSTRAINT CK_peer_status
        CHECK ([status] IN ('PENDING', 'APPROVED', 'REJECTED'))
);

-- ---------- 12. quizzes ----------
CREATE TABLE quizzes (
    quiz_id     INT IDENTITY(1,1) PRIMARY KEY,
    chapter_id  INT           NOT NULL,
    quiz_title  NVARCHAR(150) NOT NULL,
    description NVARCHAR(MAX) NULL,
    quiz_type   NVARCHAR(10)  NOT NULL,
    created_by  INT           NOT NULL,
    created_at  DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_quizzes_chapter
        FOREIGN KEY (chapter_id) REFERENCES course_chapters(chapter_id),

    CONSTRAINT FK_quizzes_creator
        FOREIGN KEY (created_by) REFERENCES users(user_id),

    CONSTRAINT CK_quizzes_type
        CHECK (quiz_type IN ('OBJ', 'SUBJ'))
);

-- ---------- 13. quiz_questions ----------
CREATE TABLE quiz_questions (
    question_id   INT IDENTITY(1,1) PRIMARY KEY,
    quiz_id       INT           NOT NULL,
    question_text NVARCHAR(MAX) NOT NULL,
    question_type NVARCHAR(20)  NOT NULL,

    CONSTRAINT FK_questions_quiz
        FOREIGN KEY (quiz_id) REFERENCES quizzes(quiz_id),

    CONSTRAINT CK_questions_type
        CHECK (question_type IN ('MCQ', 'TRUE_FALSE', 'SUBJECTIVE'))
);

-- ---------- 14. question_answer ----------
CREATE TABLE question_answer (
    answer_id   INT IDENTITY(1,1) PRIMARY KEY,
    question_id INT           NOT NULL,
    answer_text NVARCHAR(MAX) NOT NULL,
    is_correct  BIT           NULL,

    CONSTRAINT FK_answer_question
        FOREIGN KEY (question_id) REFERENCES quiz_questions(question_id)
);

-- ---------- 15. quiz_attempts ----------
CREATE TABLE quiz_attempts (
    attempt_id   INT IDENTITY(1,1) PRIMARY KEY,
    quiz_id      INT          NOT NULL,
    user_id      INT          NOT NULL,
    started_at   DATETIME2    NOT NULL DEFAULT SYSDATETIME(),
    submitted_at DATETIME2    NULL,
    score_earned DECIMAL(5,2) NULL, -- NULL for subjective questions until marked

    CONSTRAINT FK_attempt_quiz
        FOREIGN KEY (quiz_id) REFERENCES quizzes(quiz_id),

    CONSTRAINT FK_attempt_user
        FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- ---------- 16. question_submitted ----------
CREATE TABLE question_submitted (
    submitted_id      INT IDENTITY(1,1) PRIMARY KEY,
    attempt_id        INT           NOT NULL,
    question_id       INT           NOT NULL,
    selected_obj      INT           NULL, -- chosen answer_id for MCQ / True-False
    answer_subjective NVARCHAR(MAX) NULL, -- free text for subjective questions
    is_correct        BIT           NULL,

    CONSTRAINT FK_submitted_attempt
        FOREIGN KEY (attempt_id) REFERENCES quiz_attempts(attempt_id),

    CONSTRAINT FK_submitted_question
        FOREIGN KEY (question_id) REFERENCES quiz_questions(question_id),

    CONSTRAINT FK_submitted_selected
        FOREIGN KEY (selected_obj) REFERENCES question_answer(answer_id)
);

-- ---------- Quick check ----------
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
ORDER BY TABLE_NAME;