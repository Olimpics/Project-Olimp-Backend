using OlimpBack.Models;

namespace OlimpBack.Utils
{
    public static class CourseCalculator
    {
        public static async Task<int> CalculateCurrentCourse(Student student)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var yearsSinceAdmission = currentDate.Year - student.EducationStart.Year;
            
            // If we haven't reached July of the current academic year, we're still in the previous course
            if (currentDate.Month < 7)
            {
                yearsSinceAdmission--;
            }
            
            // Calculate the new course (students start from course 1)
            int calculatedCourse = yearsSinceAdmission + 1;

            // Update the student's course if it has changed
            if (student.Course != calculatedCourse)
            {
                student.Course = calculatedCourse;
                await _context.SaveChangesAsync();
            }

            return calculatedCourse;
        }
} 