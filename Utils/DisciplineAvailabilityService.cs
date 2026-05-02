using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Utils
{

    public static class DisciplineAvailabilityService
    {
        public static async Task<DisciplineAvailabilityContext?> BuildAvailabilityContext(int studentId, AppDbContext _context)
        {
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindSelectiveDisciplines)
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.IdStudent == studentId);

            if (student == null)
                return null;

            int currentCourse = student.Course;

            var boundDisciplineIds = student.BindSelectiveDisciplines
                .Select(b => b.SelectiveDisciplinesId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

// Исправлено сравнение BitArray с int на проверку первого бита (InProcess)
            var disciplineCounts = await _context.BindSelectiveDisciplines
                .Where(b => b.InProcess != null && b.InProcess.Length > 0 && b.InProcess[0] && b.SelectiveDisciplinesId != null)
                .GroupBy(b => b.SelectiveDisciplinesId!.Value)
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
        public static bool IsDisciplineAvailable(SelectiveDiscipline discipline, DisciplineAvailabilityContext context)
        {
            if (discipline.IdSelectiveDisciplines == null || context.BoundDisciplineIds.Contains(discipline.IdSelectiveDisciplines))
                return false;

            if (discipline.DegreeLevel != null &&
                discipline.DegreeLevel != context.Student.EducationalDegree)
                return false;

            if (discipline.FacultyId != context.Student.FacultyId)
                return false;

            if (discipline.MinCourse.HasValue && context.CurrentCourse < discipline.MinCourse.Value)
                return false;

            if (discipline.MaxCourse.HasValue && context.CurrentCourse > discipline.MaxCourse.Value)
                return false;

            if (discipline.MinCountPeople.HasValue)
            {
                var currentCount = context.DisciplineCounts.TryGetValue(discipline.IdSelectiveDisciplines!, out var count) ? count : 0;
                if (currentCount < discipline.MinCountPeople.Value)
                    return false;
            }

            if (discipline.MaxCountPeople.HasValue)
            {
                var currentCount = context.DisciplineCounts.TryGetValue(discipline.IdSelectiveDisciplines!, out var count) ? count : 0;
                if (currentCount >= discipline.MaxCountPeople.Value)
                    return false;
            }

            return true;
        }

    }

}
