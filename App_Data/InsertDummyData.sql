-- =====================================================
-- The Analytical Dojo - dummy data
-- Run AFTER CreateDatabaseA.sql, against databaseA.mdf
-- Assumes all tables are EMPTY (IDs start at 1)
-- =====================================================

-- ---------- 1. users (IDs 1-8) ----------
INSERT INTO users (username, full_name, email, password_hash, [role], peer_leader_since, created_at) VALUES
('admin_sarah', 'Sarah Ibrahim',  'sarah@dojo.test',  'hash_demo_admin_1',  'ADMIN',       NULL,         '2026-01-05'),
('peer_amir',   'Amir Zulkifli',  'amir@dojo.test',   'hash_demo_peer_1',   'PEER LEADER', '2026-03-01', '2026-01-10'),
('peer_li',     'Li Wei Chen',    'li@dojo.test',     'hash_demo_peer_2',   'PEER LEADER', '2026-03-15', '2026-01-12'),
('aiman_h',     'Aiman Hassan',   'aiman@dojo.test',  'hash_demo_student_1','STUDENT',     NULL,         '2026-02-01'),
('mei_tan',     'Mei Tan',        'mei@dojo.test',    'hash_demo_student_2','STUDENT',     NULL,         '2026-02-03'),
('raj_kumar',   'Raj Kumar',      'raj@dojo.test',    'hash_demo_student_3','STUDENT',     NULL,         '2026-02-10'),
('sofia_lim',   'Sofia Lim',      'sofia@dojo.test',  'hash_demo_student_4','STUDENT',     NULL,         '2026-02-15'),
('daniel_w',    'Daniel Wong',    'daniel@dojo.test', 'hash_demo_student_5','STUDENT',     NULL,         '2026-02-20');

-- ---------- 2. courses (IDs 1-4) ----------
INSERT INTO courses (course_title, description, course_image, course_level, [status], created_by, created_at) VALUES
('Intro to Data Analytics',   'Learn the basics of data, statistics and spreadsheets.',      'images/course1.jpg', 'basic',        'PUBLISHED', 1, '2026-01-20'),
('SQL for Analysts',          'Query databases with SELECT, JOIN, GROUP BY and more.',       'images/course2.jpg', 'intermediate', 'PUBLISHED', 1, '2026-01-25'),
('Python for Data Science',   'Use pandas and matplotlib to analyse and visualise data.',    'images/course3.jpg', 'intermediate', 'PUBLISHED', 1, '2026-02-01'),
('Advanced Machine Learning', 'Regression, classification and model evaluation in depth.',  'images/course4.jpg', 'advanced',     'PENDING',   1, '2026-03-10');

-- ---------- 3. course_materials (IDs 1-8) ----------
INSERT INTO course_materials (course_id, material_title, description, material_type, file_url, material_image, uploaded_by, uploaded_at) VALUES
(1, 'What is Data Analytics?',   'Overview video',                 'VIDEO', 'materials/c1_intro.mp4',      'images/m1.jpg', 1, '2026-01-21'),
(1, 'Descriptive Statistics',    'Mean, median, mode slides',      'PPT',   'materials/c1_stats.pptx',     'images/m2.jpg', 1, '2026-01-22'),
(1, 'Working with CSV Files',    'Hands-on video walkthrough',     'VIDEO', 'materials/c1_csv.mp4',        'images/m3.jpg', 1, '2026-01-23'),
(2, 'SELECT and WHERE',          'Basic querying video',           'VIDEO', 'materials/c2_select.mp4',     'images/m4.jpg', 1, '2026-01-26'),
(2, 'Joins Explained',           'INNER, LEFT and RIGHT joins',    'PPT',   'materials/c2_joins.pptx',     'images/m5.jpg', 1, '2026-01-27'),
(2, 'GROUP BY and Aggregates',   'COUNT, SUM, AVG demo',           'VIDEO', 'materials/c2_groupby.mp4',    'images/m6.jpg', 1, '2026-01-28'),
(3, 'Getting Started with pandas','DataFrames introduction',       'VIDEO', 'materials/c3_pandas.mp4',     'images/m7.jpg', 1, '2026-02-02'),
(3, 'Plotting with matplotlib',  'Charts and styling slides',      'PPT',   'materials/c3_plots.pptx',     'images/m8.jpg', 1, '2026-02-03');

-- ---------- 4. course_enrollments (IDs 1-8) ----------
INSERT INTO course_enrollments (user_id, course_id, enrolled_at, completion_percentage, average_score_percentage, completed_at) VALUES
(4, 1, '2026-02-05', 100.00, 66.67,  '2026-02-20'),  -- 1 Aiman  - Intro (done)
(4, 2, '2026-02-22',  33.33, 50.00,  NULL),          -- 2 Aiman  - SQL
(5, 1, '2026-02-06', 100.00, 100.00, '2026-02-18'),  -- 3 Mei    - Intro (done)
(5, 3, '2026-02-20',   0.00, NULL,   NULL),          -- 4 Mei    - Python
(6, 2, '2026-02-12',   0.00, NULL,   NULL),          -- 5 Raj    - SQL
(7, 1, '2026-02-16',  66.67, NULL,   NULL),          -- 6 Sofia  - Intro
(8, 1, '2026-02-21', 100.00, 100.00, '2026-03-01'),  -- 7 Daniel - Intro (done)
(8, 2, '2026-03-02',   0.00, NULL,   NULL);          -- 8 Daniel - SQL

-- ---------- 5. student_material_progress ----------
INSERT INTO student_material_progress (enrollment_id, material_id, completed_at) VALUES
(1, 1, '2026-02-08'), (1, 2, '2026-02-12'), (1, 3, '2026-02-20'),
(2, 4, '2026-02-25'),
(3, 1, '2026-02-09'), (3, 2, '2026-02-12'), (3, 3, '2026-02-18'),
(6, 1, '2026-02-17'), (6, 2, '2026-02-19'),
(7, 1, '2026-02-22'), (7, 2, '2026-02-25'), (7, 3, '2026-03-01');

-- ---------- 6. certificates ----------
INSERT INTO certificates (enrollment_id, certificate_number) VALUES
(1, 'CERT-2026-0001'),
(3, 'CERT-2026-0002'),
(7, 'CERT-2026-0003');

-- ---------- 7. badges (IDs 1-4) ----------
INSERT INTO badges (badges_name, description, distinction_count, icon) VALUES
('First Steps',      'Completed your first learning material.', 1, 'icons/first_steps.png'),
('Quiz Master',      'Scored 100% on a quiz.',                  1, 'icons/quiz_master.png'),
('Perfectionist',    'Finished a course with a 100% average.',  1, 'icons/perfectionist.png'),
('Course Completer', 'Completed an entire course.',             1, 'icons/completer.png');

-- ---------- 8. student_badges ----------
INSERT INTO student_badges (enrollment_id, badge_id, earned_at) VALUES
(1, 1, '2026-02-08'),
(1, 4, '2026-02-20'),
(3, 1, '2026-02-09'),
(3, 2, '2026-02-15'),
(3, 3, '2026-02-18'),
(3, 4, '2026-02-18'),
(7, 2, '2026-02-26'),
(7, 4, '2026-03-01');

-- ---------- 9. forum_posts ----------
INSERT INTO forum_posts (user_id, course_id, content, created_at, is_deleted) VALUES
(4, 1, 'Can someone explain the difference between mean and median?',            '2026-02-10 10:15', 0),
(2, 1, 'The median is the middle value, so it is less affected by outliers.',    '2026-02-10 11:02', 0),
(5, 1, 'Great course so far, the CSV video was really helpful!',                 '2026-02-14 09:30', 0),
(6, 2, 'Is LEFT JOIN the same as LEFT OUTER JOIN?',                              '2026-02-13 14:45', 0),
(3, 2, 'Yes, OUTER is optional in most SQL dialects.',                           '2026-02-13 15:20', 0),
(8, 1, 'Spam message that an admin removed.',                                    '2026-03-01 08:00', 1);

-- ---------- 10. peer_call_requests ----------
INSERT INTO peer_call_requests (user_id, assigned_peer_leader_id, student_request_description, [status], meeting_url, rejection_reason) VALUES
(4, 2,    'I need help understanding standard deviation.',          'COMPLETED', 'https://meet.example.com/abc-1111', NULL),
(6, 3,    'Struggling with SQL joins, can we go through examples?', 'APPROVED',  'https://meet.example.com/def-2222', NULL),
(7, NULL, 'Need help preparing for the Intro quiz.',                'PENDING',   NULL,                                NULL),
(8, 2,    'Can we chat about career paths in analytics?',           'REJECTED',  NULL,                                'Outside the scope of peer support sessions.');

-- ---------- 11. quizzes (IDs 1-3) ----------
INSERT INTO quizzes (course_id, quiz_title, description, quiz_type, created_by, created_at) VALUES
(1, 'Statistics Basics Quiz',   'Check your understanding of basic statistics.', 'OBJ',  1, '2026-02-01'),
(2, 'SQL Fundamentals Quiz',    'Test your SQL knowledge.',                      'OBJ',  1, '2026-02-05'),
(1, 'Written Reflection',       'Short written questions on analytics.',         'SUBJ', 1, '2026-02-08');

-- ---------- 12. quiz_questions (IDs 1-6) ----------
INSERT INTO quiz_questions (quiz_id, question_text, question_type) VALUES
(1, 'Which of these is a measure of central tendency?',                         'MCQ'),
(1, 'The median is strongly affected by extreme outliers.',                     'TRUE_FALSE'),
(1, 'What does CSV stand for?',                                                 'MCQ'),
(2, 'Which SQL clause filters rows BEFORE grouping?',                           'MCQ'),
(2, 'An INNER JOIN returns rows that have no match in the other table.',        'TRUE_FALSE'),
(3, 'Explain the difference between descriptive and inferential statistics.',   'SUBJECTIVE');

-- ---------- 13. question_answer (IDs 1-14) ----------
INSERT INTO question_answer (question_id, answer_text, is_correct) VALUES
(1, 'Mean',                 1),   -- 1
(1, 'Range',                0),   -- 2
(1, 'Variance',             0),   -- 3
(2, 'True',                 0),   -- 4
(2, 'False',                1),   -- 5
(3, 'Comma-Separated Values', 1), -- 6
(3, 'Computer Style Vector',  0), -- 7
(3, 'Column Sorted Variable', 0), -- 8
(4, 'WHERE',                1),   -- 9
(4, 'HAVING',               0),   -- 10
(4, 'ORDER BY',             0),   -- 11
(5, 'True',                 0),   -- 12
(5, 'False',                1),   -- 13
(6, 'Descriptive statistics summarise a dataset; inferential statistics draw conclusions about a population from a sample.', NULL); -- 14 (model answer)

-- ---------- 14. quiz_attempts (IDs 1-5) ----------
INSERT INTO quiz_attempts (quiz_id, user_id, started_at, submitted_at, score_earned) VALUES
(1, 4, '2026-02-15 10:00', '2026-02-15 10:08',  66.67),  -- 1 Aiman, Stats quiz
(1, 5, '2026-02-15 11:00', '2026-02-15 11:06', 100.00),  -- 2 Mei,   Stats quiz
(2, 4, '2026-02-25 09:00', '2026-02-25 09:07',  50.00),  -- 3 Aiman, SQL quiz
(1, 8, '2026-02-26 14:00', '2026-02-26 14:05', 100.00),  -- 4 Daniel, Stats quiz
(3, 5, '2026-02-17 16:00', '2026-02-17 16:20',  NULL);   -- 5 Mei, written (awaiting marking)

-- ---------- 15. question_submitted ----------
INSERT INTO question_submitted (attempt_id, question_id, selected_obj, answer_subjective, is_correct) VALUES
-- Attempt 1 (Aiman): got Q2 wrong
(1, 1, 1,    NULL, 1),
(1, 2, 4,    NULL, 0),
(1, 3, 6,    NULL, 1),
-- Attempt 2 (Mei): all correct
(2, 1, 1,    NULL, 1),
(2, 2, 5,    NULL, 1),
(2, 3, 6,    NULL, 1),
-- Attempt 3 (Aiman, SQL quiz): got Q5 wrong
(3, 4, 9,    NULL, 1),
(3, 5, 12,   NULL, 0),
-- Attempt 4 (Daniel): all correct
(4, 1, 1,    NULL, 1),
(4, 2, 5,    NULL, 1),
(4, 3, 6,    NULL, 1),
-- Attempt 5 (Mei): subjective answer, not yet marked
(5, 6, NULL, 'Descriptive statistics describe the data you have, like averages. Inferential statistics use a sample to make predictions about a larger population.', NULL);

-- ---------- Quick check ----------
SELECT 'users' AS tbl, COUNT(*) AS row_count FROM users
UNION ALL SELECT 'courses', COUNT(*) FROM courses
UNION ALL SELECT 'course_materials', COUNT(*) FROM course_materials
UNION ALL SELECT 'course_enrollments', COUNT(*) FROM course_enrollments
UNION ALL SELECT 'student_material_progress', COUNT(*) FROM student_material_progress
UNION ALL SELECT 'certificates', COUNT(*) FROM certificates
UNION ALL SELECT 'badges', COUNT(*) FROM badges
UNION ALL SELECT 'student_badges', COUNT(*) FROM student_badges
UNION ALL SELECT 'forum_posts', COUNT(*) FROM forum_posts
UNION ALL SELECT 'peer_call_requests', COUNT(*) FROM peer_call_requests
UNION ALL SELECT 'quizzes', COUNT(*) FROM quizzes
UNION ALL SELECT 'quiz_questions', COUNT(*) FROM quiz_questions
UNION ALL SELECT 'question_answer', COUNT(*) FROM question_answer
UNION ALL SELECT 'quiz_attempts', COUNT(*) FROM quiz_attempts
UNION ALL SELECT 'question_submitted', COUNT(*) FROM question_submitted;