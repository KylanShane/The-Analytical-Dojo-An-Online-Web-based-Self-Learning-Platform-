# Database migrations

Run these scripts in filename order after `CreateDatabaseA.sql` and
`InsertDummyData.sql`.

Each migration is intended to be committed to Git and retained permanently.
Do not commit generated `.mdf` or `.ldf` changes.

Current order:

1. `001_UpdateBadgesAndLearningMaterials.sql`

