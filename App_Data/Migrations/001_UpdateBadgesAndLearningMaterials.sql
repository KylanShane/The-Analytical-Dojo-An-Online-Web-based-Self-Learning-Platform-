/*
    Run once against databaseA after CreateDatabaseA.sql and InsertDummyData.sql.
    This file is the Git-safe record of the database change. Do not commit the
    generated databaseA.mdf or databaseA_log.ldf files.

    Video sources: Maven Analytics, Chandoo, freeCodeCamp and Corey Schafer.
    Slide sources: open university and educational course materials.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Remove the obsolete result-band badges. Results are still shown from quiz
-- scores; these rows are not part of the current four-badge achievement UI.
DELETE sb
FROM student_badges sb
JOIN badges b ON b.badges_id = sb.badge_id
WHERE b.badges_name IN (N'Distinction', N'Merit', N'Pass');

DELETE FROM badges
WHERE badges_name IN (N'Distinction', N'Merit', N'Pass');

-- Keep the four badge definitions used by the current website in sync.
MERGE badges AS target
USING (VALUES
    (N'Join Master', N'Earn Distinction (80%+) in 3 courses.', 3, N'link'),
    (N'Chart Maker', N'Earn Distinction (80%+) in 8 courses.', 8, N'chart'),
    (N'Clean Sweep', N'Earn Distinction (80%+) in 12 courses.', 12, N'broom'),
    (N'Perfect Score', N'Earn Distinction (80%+) in 16 courses.', 16, N'target')
) AS source (badges_name, description, distinction_count, icon)
ON target.badges_name = source.badges_name
WHEN MATCHED THEN UPDATE SET
    description = source.description,
    distinction_count = source.distinction_count,
    icon = source.icon
WHEN NOT MATCHED THEN INSERT
    (badges_name, description, distinction_count, icon)
VALUES
    (source.badges_name, source.description, source.distinction_count, source.icon);

-- Data Analytics Foundations
UPDATE course_materials SET file_url = N'https://www.niua.in/intranet/sites/default/files/publications/NIUA_SUDSC%20II_CB%20Module_Data%20Analytics%20and%20Visualisation-Training%20Manual.pdf' WHERE material_id IN (1, 3, 5);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=kWZEXXd2tZA' WHERE material_id IN (2, 4, 6);

-- Excel for Data Analysis
UPDATE course_materials SET file_url = N'https://raw.githubusercontent.com/Brown-University-Library/geodata_pdf_tutorials/main/excel/bul_s4gis_excel_dataprocess.pdf' WHERE material_id IN (7, 9, 11);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=Y8xhrUa3KH4' WHERE material_id = 8;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=H6k28jhclwI' WHERE material_id = 10;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=kSNxzoe9RcM' WHERE material_id = 12;

-- SQL for Data Analysis
UPDATE course_materials SET file_url = N'https://ms.sites.cs.wisc.edu/cs544/s24/lec/18-sql/slides.pdf' WHERE material_id IN (13, 15);
UPDATE course_materials SET file_url = N'https://cdn.cs50.net/sql/2023/x/lectures/1/lecture1.pdf' WHERE material_id = 17;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=HXV3zeQKqGY' WHERE material_id IN (14, 16, 18);

-- Statistics for Analysts
UPDATE course_materials SET file_url = N'https://www.stat.uchicago.edu/~yibi/teaching/stat220/17aut/Lectures/L0102.pdf' WHERE material_id = 19;
UPDATE course_materials SET file_url = N'https://ocw.mit.edu/courses/18-05-introduction-to-probability-and-statistics-spring-2022/mit18_05_s22_lec02.pdf' WHERE material_id = 21;
UPDATE course_materials SET file_url = N'https://ocw.mit.edu/courses/18-05-introduction-to-probability-and-statistics-spring-2022/mit18_05_s22_lec07.pdf' WHERE material_id = 23;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=xxpc-HPKN28' WHERE material_id IN (20, 22, 24);

-- Python and pandas for Analytics
UPDATE course_materials SET file_url = N'https://uomus.edu.iq/uploads/lectures/48/49579a1ecb2a4d2e80a1f05a9398dd1f.pdf' WHERE material_id IN (25, 27, 29);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=zmdjNSmRXF4' WHERE material_id = 26;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=KdmPHEnPJPs' WHERE material_id = 28;
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=txMdrV1Ut64' WHERE material_id = 30;

-- Power BI and Data Visualization
UPDATE course_materials SET file_url = N'https://raw.githubusercontent.com/Shaheer-khan-github/Power-BI-Skill-Track-Data-Camp/main/Course%202.%20Introduction%20to%20DAX%20in%20Power%20BI/Chapters_slides/chapter1.pdf' WHERE material_id IN (31, 33, 35);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=cyWVzAQF9YU' WHERE material_id IN (32, 34, 36);

-- Machine Learning for Data Analytics
UPDATE course_materials SET file_url = N'https://gubri.eu/pdf/teaching/2021/slides_2021_ML_SLT.pdf' WHERE material_id IN (37, 39, 41);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=i_LwzRVP7bg' WHERE material_id IN (38, 40, 42);

-- Time Series Analytics
UPDATE course_materials SET file_url = N'https://www.mlds.tuc.gr/fileadmin/users_data/ece-master-mlds/lecture_notes/D_Hristopulos_Time_Series_Modeling_and_Analysis.pdf' WHERE material_id IN (43, 45, 47);
UPDATE course_materials SET file_url = N'https://www.youtube.com/watch?v=fxx_E0ojKrc' WHERE material_id IN (44, 46, 48);

COMMIT TRANSACTION;
