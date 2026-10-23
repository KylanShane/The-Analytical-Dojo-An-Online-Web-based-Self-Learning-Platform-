SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID('dbo.material_slides', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.material_slides (
        material_id INT           NOT NULL,
        slide_order INT           NOT NULL,
        image_url   NVARCHAR(500) NOT NULL,
        alt_text    NVARCHAR(255) NULL,
        CONSTRAINT PK_material_slides PRIMARY KEY (material_id, slide_order),
        CONSTRAINT FK_material_slides_material
            FOREIGN KEY (material_id) REFERENCES dbo.course_materials(material_id),
        CONSTRAINT CK_material_slides_order CHECK (slide_order > 0)
    );
END;

UPDATE dbo.courses
SET course_image = CASE course_id
    WHEN 1 THEN '/Images/Courses/data-analytics-foundations.png'
    WHEN 2 THEN '/Images/Courses/excel-for-data-analysis.png'
    WHEN 3 THEN '/Images/Courses/sql-for-data-analysis.png'
    WHEN 4 THEN '/Images/Courses/statistics-for-analysts.png'
    WHEN 5 THEN '/Images/Courses/python-pandas-analytics.png'
    WHEN 6 THEN '/Images/Courses/power-bi-data-visualization.png'
    WHEN 7 THEN '/Images/Courses/machine-learning-data-analytics.png'
    WHEN 8 THEN '/Images/Courses/time-series-analytics.png'
END
WHERE course_id BETWEEN 1 AND 8;

-- Remove legacy demo rows that point to empty Content/slides folders. Each
-- seeded slide material below has one complete, generated 16:9 lesson image.
DELETE FROM dbo.material_slides
WHERE material_id IN (
    1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23,
    25, 27, 29, 31, 33, 35, 37, 39, 41, 43, 45, 47
);

MERGE dbo.material_slides AS target
USING (VALUES
    (1, 1, '/Images/Slides/analytics-workflow/slide-01.png', 'The analytics workflow: Ask, Collect, Clean, Analyze, Share'),
    (3, 1, '/Images/Slides/data-types.png', 'Data types'),
    (5, 1, '/Images/Slides/data-quality.png', 'Data quality'),
    (7, 1, '/Images/Slides/spreadsheet-formulas.png', 'Spreadsheet formulas'),
    (9, 1, '/Images/Slides/cleaning-spreadsheet-data.png', 'Cleaning spreadsheet data'),
    (11, 1, '/Images/Slides/pivot-tables.png', 'Pivot tables'),
    (13, 1, '/Images/Slides/selecting-filtering.png', 'Selecting and filtering'),
    (15, 1, '/Images/Slides/aggregations.png', 'Aggregations'),
    (17, 1, '/Images/Slides/joining-tables.png', 'Joining tables'),
    (19, 1, '/Images/Slides/descriptive-statistics.png', 'Descriptive statistics'),
    (21, 1, '/Images/Slides/probability-sampling.png', 'Probability and sampling'),
    (23, 1, '/Images/Slides/correlation.png', 'Correlation'),
    (25, 1, '/Images/Slides/dataframes.png', 'DataFrames'),
    (27, 1, '/Images/Slides/missing-data.png', 'Missing data'),
    (29, 1, '/Images/Slides/grouping-data.png', 'Grouping data'),
    (31, 1, '/Images/Slides/power-query.png', 'Power Query'),
    (33, 1, '/Images/Slides/data-modeling-dax.png', 'Data modeling and DAX'),
    (35, 1, '/Images/Slides/dashboard-design.png', 'Dashboard design'),
    (37, 1, '/Images/Slides/classification.png', 'Classification'),
    (39, 1, '/Images/Slides/model-evaluation.png', 'Model evaluation'),
    (41, 1, '/Images/Slides/data-leakage.png', 'Data leakage'),
    (43, 1, '/Images/Slides/time-series-patterns.png', 'Time series patterns'),
    (45, 1, '/Images/Slides/forecast-validation.png', 'Forecast validation'),
    (47, 1, '/Images/Slides/forecast-errors.png', 'Forecast errors')
) AS source (material_id, slide_order, image_url, alt_text)
ON target.material_id = source.material_id
AND target.slide_order = source.slide_order
WHEN MATCHED THEN
    UPDATE SET image_url = source.image_url, alt_text = source.alt_text
WHEN NOT MATCHED THEN
    INSERT (material_id, slide_order, image_url, alt_text)
    VALUES (source.material_id, source.slide_order, source.image_url, source.alt_text);

COMMIT TRANSACTION;

SELECT COUNT(*) AS courses_with_images
FROM dbo.courses
WHERE course_image LIKE '/Images/Courses/%';

SELECT COUNT(*) AS configured_slide_images
FROM dbo.material_slides
WHERE image_url LIKE '/Images/Slides/%';
