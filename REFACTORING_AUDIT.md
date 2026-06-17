# Backend Refactoring Audit (Phase 5)

Scope: controllers, DTOs, AutoMapper, services, EF Core queries, performance.
Constraint: this environment has no .NET SDK, so changes below were applied by hand and
must be validated with `dotnet build` / runtime testing.

## Fixes applied in this pass

1. **`DisciplineChoicePeriodRepository.GetAllDtoAsync` — `IsClose` filter bug (correctness).**
   The predicate `p.IsClose != null && p.IsClose && p.IsClose == (queryDto.IsClose != false)`
   forced `IsClose == true` and then contradicted itself when filtering for open periods,
   so **filtering periods by `IsClose=false` always returned an empty list**.
   Replaced with `p.IsClose == queryDto.IsClose.Value`.

2. **`PeriodResolverService.GetStudentsForPeriodAsync` — added `AsNoTracking()`** to the
   read-only students query (used only by cache init/invalidation). Avoids change-tracker
   overhead on potentially large student sets.

3. **`DisciplineChoicePeriodCleanupService`** (done in Phase 1) — removed the hard-coded
   `...0003` GUID and the in-memory `Courses.Contains` filtering; now delegates to
   `PeriodResultProcessingService` (capacity-based distribution).

4. **Period resolution centralised** (Phase 1) — duplicated period-matching logic that lived
   inside `StudentChoiceCacheService` and `DisciplineTabService` is now in
   `PeriodResolverService` + `PeriodCacheService` (single source of truth).

## Prioritised recommendations (not yet applied — need a build to verify safely)

### High impact
- **Global static `SemaphoreSlim` in cache services.**
  `StudentChoiceCacheService` and `DisciplineCacheService` use a single `static SemaphoreSlim _lock`
  shared across *all* students/disciplines. Every cache miss serialises globally — a real
  throughput bottleneck under load. Recommend per-key locking (e.g. a striped lock or
  `LazyCache`/`SemaphoreSlim` keyed by id), or a Redis Lua script for atomic updates.

- **`DisciplineTabAdminService.GetStudentsWithDisciplineChoicesAsync` — in-memory pagination.**
  It loads the full student set, then filters/sorts/paginates in memory (lines ~103-123).
  For large cohorts this is expensive. Push `SelectionStatus`/`ConfirmationStatus`/sort/paging
  into the SQL query where possible, or pre-aggregate per-student counts in the DB.

### Medium impact
- **Duplicated student projection.** `StudentChoicesProjection`, `AdminStudentBySelectiveDisciplineDto`,
  `AdminStudentByMainDisciplineDto`, and `StudentIdNameDto` repeat the same
  `SecondName + FirstName + ThirdName` full-name composition. Consider a shared value object or
  AutoMapper resolver to avoid drift.

- **AutoMapper profile size.** `MappingProfiles/MappingProfile.cs` is ~400 lines and mixes every
  domain. Split per feature (Disciplines, Periods, Sg, Rbac) into separate `Profile` classes for
  readability; behaviour is unchanged because all profiles are auto-registered.

- **`CountOfPeople` / occupancy semantics.** Admin list uses `d.BindSelectiveDisciplines.Count`
  (all binds) while availability uses the Redis occupancy (only `InProcess`). Align these so the
  admin catalog count and the student-facing occupancy mean the same thing.

### Low impact / consistency
- Add `AsNoTracking()` to remaining read-only list/projection queries that omit it.
- Several controllers mix `[RequirePermission]` usage (e.g. `DisciplineTabAdminController.GetAllDisciplines`
  has the attribute commented out) — re-enable consistent authorization.
- Standardise service return shapes: some return `(bool, string?)`, others `(bool, int, string?)`,
  others throw. A small `Result<T>` type would unify controller error mapping.

## Notes for cross-cutting work added in Phases 1–4
- New DB columns are applied via `Migrations/manual/01_period_resolver_changes.sql` (EF model +
  `OnModelCreating` updated to match). The EF migrations snapshot was intentionally not regenerated
  per the chosen "models + SQL" approach; if you later run `dotnet ef migrations add`, it will emit
  the delta for these columns — review before applying.
