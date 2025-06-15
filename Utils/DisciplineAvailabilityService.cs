using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Utils
{

    public static class DisciplineAvailabilityService
    {
        public static async Task<DisciplineAvailabilityContext?> BuildAvailabilityContext(int studentId, AppDbContext _context)
        {
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindAddDisciplines)
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.IdStudent == studentId);

            if (student == null)
                return null;

            int currentCourse = student.Course;

            var boundDisciplineIds = student.BindAddDisciplines
                .Select(b => b.AddDisciplinesId)
                .ToHashSet();

            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess == 1)
                .GroupBy(b => b.AddDisciplinesId)
                .Select(g => new { DisciplineId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DisciplineId, x => x.Count);

            return new DisciplineAvailabilityContext
            {
                Student = student,
                CurrentCourse = currentCourse,
                FacultyAbbreviation = student.Faculty?.Abbreviation,
                BoundDisciplineIds = boundDisciplineIds,
                DisciplineCounts = disciplineCounts
            };
        }
        public static bool IsDisciplineAvailable(AddDiscipline discipline, DisciplineAvailabilityContext context)
        {
            if (context.BoundDisciplineIds.Contains(discipline.IdAddDisciplines))
                return false;

            if (discipline.DegreeLevel != null &&
                discipline.DegreeLevel != context.Student.EducationalDegree)
                return false;

            if (discipline.Faculty != context.Student.FacultyId)
                return false;

            if (discipline.MinCourse.HasValue && context.CurrentCourse < discipline.MinCourse.Value)
                return false;

            if (discipline.MaxCourse.HasValue && context.CurrentCourse > discipline.MaxCourse.Value)
                return false;

            if (discipline.MinCountPeople.HasValue)
            {
                var currentCount = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var count) ? count : 0;
                if (currentCount < discipline.MinCountPeople.Value)
                    return false;
            }

            if (discipline.MaxCountPeople.HasValue)
            {
                var currentCount = context.DisciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var count) ? count : 0;
                if (currentCount >= discipline.MaxCountPeople.Value)
                    return false;
            }

            return true;
        }


    }

}
