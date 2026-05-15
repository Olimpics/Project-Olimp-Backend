using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Utils
{

    public static class DisciplineAvailabilityService
    {
        public static async Task<DisciplineAvailabilityContext?> BuildAvailabilityContext(Guid studentId, AppDbContext _context)
        {
            var student = await _context.Students
                .Include(s => s.Group.DegreeLevel)
                .Include(s => s.BindSelectiveDisciplines)
                .Include(s => s.Group.EducationalProgram.Speciality.Department.Faculty)
                .FirstOrDefaultAsync(s => s.IdStudent == studentId);

            if (student == null)
                return null;

            int currentCourse = student.Course;

            var boundDisciplineIds = student.BindSelectiveDisciplines
                .Select(b => b.SelectiveDisciplineId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            var disciplineCounts = await _context.BindSelectiveDisciplines
                .Where(b => b.InProcess == true && b.SelectiveDisciplineId != null)
                .GroupBy(b => b.SelectiveDisciplineId!.Value)
                .Select(g => new { DisciplineId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DisciplineId, x => x.Count);

            return new DisciplineAvailabilityContext
            {
                Student = student,
                CurrentCourse = currentCourse,
                FacultyAbbreviation = student.Group?.EducationalProgram?.Speciality?.Department?.Faculty?.Abbreviation,
                BoundDisciplineIds = boundDisciplineIds,
                DisciplineCounts = disciplineCounts
            };
        }
        public static bool IsDisciplineAvailable(SelectiveDiscipline discipline, DisciplineAvailabilityContext context)
        {
            if (context.BoundDisciplineIds.Contains(discipline.IdSelectiveDisciplines))
                return false;

            if (discipline.DegreeLevelId != null &&
                discipline.DegreeLevelId != context.Student.Group?.DegreeLevelId)
                return false;

            if (discipline.Department?.FacultyId != context.Student.Group?.EducationalProgram?.Speciality?.Department?.FacultyId)
                return false;

            if (discipline.Courses != null && discipline.Courses.Any() && !discipline.Courses.Contains(context.CurrentCourse))
                return false;

            // Note: MinCountPeople logic might be different from what was here.
            // Usually availability means students CAN join, so currentCount < MaxCount.
            // MinCount usually matters AFTER the period to see if the discipline will actually happen.
            
            if (discipline.MaxCountPeople.HasValue)
            {
                var currentCount = context.DisciplineCounts.TryGetValue(discipline.IdSelectiveDisciplines, out var count) ? count : 0;
                if (currentCount >= discipline.MaxCountPeople.Value)
                    return false;
            }

            return true;
        }

    }

}
