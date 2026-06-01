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
                .Include(s => s.Group.EducationalProgram.Degree)
                .Include(s => s.BindSelectiveDisciplines)
                .Include(s => s.Group.EducationalProgram.Speciality.Department.Faculty)
                .FirstOrDefaultAsync(s => s.IdStudent == studentId);

            if (student == null)
                return null;

            int currentCourse = student.Group?.Course ?? 0;

            var boundDisciplineIds = student.BindSelectiveDisciplines
                .Select(b => b.SelectiveDisciplineId)
                .Where(id => id != Guid.Empty)
                .Select(id => id!)
                .ToHashSet();

            return new DisciplineAvailabilityContext
            {
                Student = student,
                CurrentCourse = currentCourse,
                FacultyAbbreviation = student.Group?.EducationalProgram?.Speciality?.Department?.Faculty?.Abbreviation,
                BoundDisciplineIds = boundDisciplineIds
            };
        }
        public static bool IsDisciplineAvailable(SelectiveDiscipline discipline, DisciplineAvailabilityContext context, int currentOccupancy)
        {
            if (context.BoundDisciplineIds.Contains(discipline.IdSelectiveDisciplines))
                return false;

            if (discipline.DegreeLevelId != Guid.Empty &&
                discipline.DegreeLevelId != context.Student.Group?.EducationalProgram?.DegreeId)
                return false;

            if (discipline.Department?.FacultyId != null && 
                discipline.Department?.FacultyId != context.Student.Group?.EducationalProgram?.Speciality?.Department?.FacultyId)
                return false;

            if (discipline.Courses != null && discipline.Courses.Any() && !discipline.Courses.Contains(context.CurrentCourse))
                return false;

            if (discipline.MaxCountPeople.HasValue)
            {
                if (currentOccupancy >= discipline.MaxCountPeople.Value)
                    return false;
            }

            return true;
        }

    }

}
