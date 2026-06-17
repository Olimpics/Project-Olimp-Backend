-- ============================================================================
-- Migration 01: Period resolver / distribution support
-- Phase 1 of discipline-selection overhaul.
--
-- Adds:
--   * BindSelectiveDisciplines.period_id   -> link a choice to its period
--   * BindSelectiveDisciplines.is_rejected -> result of capacity distribution
--   * DisciplineChoicePeriod.catalog_id    -> explicit selective catalog link
--   * DisciplineChoicePeriod.check_period_stopped
--   * DisciplineChoicePeriod.results_processed_at
--
-- Idempotent: safe to run multiple times.
-- Apply against PostgreSQL "project_olymp_db".
-- ============================================================================

BEGIN;

-- 1. BindSelectiveDisciplines ------------------------------------------------
ALTER TABLE "BindSelectiveDisciplines"
    ADD COLUMN IF NOT EXISTS is_rejected boolean NOT NULL DEFAULT false;

ALTER TABLE "BindSelectiveDisciplines"
    ADD COLUMN IF NOT EXISTS period_id uuid NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'bindselectivedisciplines_disciplinechoiceperiod_fk'
    ) THEN
        ALTER TABLE "BindSelectiveDisciplines"
            ADD CONSTRAINT bindselectivedisciplines_disciplinechoiceperiod_fk
            FOREIGN KEY (period_id)
            REFERENCES "DisciplineChoicePeriod" ("idDisciplineChoicePeriod")
            ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ix_bindselectivedisciplines_period_id
    ON "BindSelectiveDisciplines" (period_id);

-- 2. DisciplineChoicePeriod --------------------------------------------------
ALTER TABLE "DisciplineChoicePeriod"
    ADD COLUMN IF NOT EXISTS check_period_stopped boolean NOT NULL DEFAULT false;

ALTER TABLE "DisciplineChoicePeriod"
    ADD COLUMN IF NOT EXISTS results_processed_at date NULL;

ALTER TABLE "DisciplineChoicePeriod"
    ADD COLUMN IF NOT EXISTS catalog_id uuid NULL;

-- Backfill catalog_id from the selective catalog matching the period's year.
UPDATE "DisciplineChoicePeriod" p
SET catalog_id = cys.id_catalog_year_selective
FROM "CatalogYear" cy
JOIN "CatalogYears_Selective" cys ON cys."yearStart" = cy."yearStart"
WHERE p.catalog_id IS NULL
  AND p.catalog_year_id = cy.id_catalog;

-- Fallback for periods with no year-matched selective catalog: use the latest one.
-- Prevents NULLs, since the EF model maps catalog_id as a non-nullable Guid.
UPDATE "DisciplineChoicePeriod" p
SET catalog_id = (
    SELECT cys.id_catalog_year_selective
    FROM "CatalogYears_Selective" cys
    ORDER BY cys."yearStart" DESC
    LIMIT 1
)
WHERE p.catalog_id IS NULL
  AND EXISTS (SELECT 1 FROM "CatalogYears_Selective");

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'disciplinechoiceperiod_catalogyearsselective_fk'
    ) THEN
        ALTER TABLE "DisciplineChoicePeriod"
            ADD CONSTRAINT disciplinechoiceperiod_catalogyearsselective_fk
            FOREIGN KEY (catalog_id)
            REFERENCES "CatalogYears_Selective" ("id_catalog_year_selective");
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ix_disciplinechoiceperiod_catalog_id
    ON "DisciplineChoicePeriod" (catalog_id);

COMMIT;
