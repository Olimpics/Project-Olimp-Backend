using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IRatingService
{
    Task GenerateRatingAsync(GenerateRatingQueryDto query);
}

public class RatingService : IRatingService
{
    private readonly IRatingRepository _repository;

    public RatingService(IRatingRepository repository)
    {
        _repository = repository;
    }

    public async Task GenerateRatingAsync(GenerateRatingQueryDto query)
    {
        // 1. Filter students
        var students = await _repository.GetStudentsForRatingAsync(query.EducationalProgramId, query.Course, query.IsAccelerated);
        if (!students.Any()) return;

        var studentIds = students.Select(s => s.IdStudent).ToList();
        int targetSemester = (query.Course - 1) * 2 + query.SemesterType;

        // 2. Extract grades
        var mainGrades = await _repository.GetMainGradesAsync(studentIds, targetSemester);
        var selectiveGrades = await _repository.GetSelectiveGradesAsync(studentIds, targetSemester);

        var academicScores = new Dictionary<int, double>();

        foreach (var student in students)
        {
            var studentMainGrades = mainGrades.Where(mg => mg.StudentId == student.IdStudent).ToList();
            var studentSelectiveGrades = selectiveGrades.Where(sg => sg.StudentId == student.IdStudent).ToList();

            int totalCourses = studentMainGrades.Count + studentSelectiveGrades.Count;
            if (totalCourses == 0)
            {
                academicScores[student.IdStudent] = 0;
                continue;
            }

            double sumGrades = 0;
            foreach (var mg in studentMainGrades) sumGrades += ParseGrade(mg.MainGrade1);
            foreach (var sg in studentSelectiveGrades) sumGrades += ParseGrade(sg.Grade);

            double averageGrade = sumGrades / totalCourses;
            academicScores[student.IdStudent] = averageGrade * 0.9;
        }

        // 3. Calculate extra points
        var extraPointsMap = await CalculateExtraPointsAsync(students);

        // 4. Final calculation and persistence
        var ratings = new List<BindRating>();
        foreach (var student in students)
        {
            double academicScore = academicScores.ContainsKey(student.IdStudent) ? academicScores[student.IdStudent] : 0;
            double extraScore = extraPointsMap.ContainsKey(student.IdStudent) ? extraPointsMap[student.IdStudent] : 0;

            ratings.Add(new BindRating
            {
                StudentId = student.IdStudent,
                Year = query.CatalogYearId,
                Semestr = new BitArray(new bool[] { query.SemesterType == 2 }),
                FinalScore = (float)(academicScore + extraScore),
                IsRedo = new BitArray(new bool[] { false })
            });
        }

        await _repository.AddRatingsAsync(ratings);

        // 5. Record calculation time
        var calcTime = new RatingCalculationTime
        {
            SpecialityId = students.First().EducationalProgram?.SpecialityId, 
            Course = query.Course,
            Semestr = query.SemesterType,
            IsShorted = new BitArray(new bool[] { query.IsAccelerated }),
            Date = DateOnly.FromDateTime(DateTime.Now),
            YearId = query.CatalogYearId
        };
        
        // If specialityId is null from student, try to get it from program if possible.
        // Actually, we can just use query.EducationalProgramId and look up the speciality if needed,
        // but speciality is linked to program.

        await _repository.AddCalculationTimeAsync(calcTime);
        await _repository.SaveChangesAsync();
    }

    private async Task<Dictionary<int, double>> CalculateExtraPointsAsync(List<Student> students)
    {
        var studentIds = students.Select(s => s.IdStudent).ToList();
        var eventPoints = await _repository.GetEventPointsAsync(studentIds);
        var extraActivityPoints = await _repository.GetExtraActivityPointsAsync(studentIds);
        
        var sgStudents = students.Where(s => s.IsInSg != null && s.IsInSg.Length > 0 && s.IsInSg[0]).Select(s => s.IdStudent).ToList();
        var sgPointsMap = sgStudents.Any() ? await _repository.GetSgPointsMapAsync(sgStudents) : new Dictionary<int, int>();

        var rawExtraPoints = new Dictionary<int, int>();
        foreach (var sid in studentIds)
        {
            int totalRaw = 0;
            totalRaw += eventPoints.Where(ep => ep.StudentId == sid).Sum(ep => ep.Point ?? 0);
            totalRaw += extraActivityPoints.Where(eap => eap.StudentId == sid).Sum(eap => eap.Points ?? 0);
            if (sgPointsMap.ContainsKey(sid))
            {
                totalRaw += sgPointsMap[sid];
            }
            rawExtraPoints[sid] = totalRaw;
        }

        int maxRaw = rawExtraPoints.Values.Any() ? rawExtraPoints.Values.Max() : 0;
        var normalizedPoints = new Dictionary<int, double>();

        foreach (var kvp in rawExtraPoints)
        {
            if (maxRaw == 0)
            {
                normalizedPoints[kvp.Key] = 0;
            }
            else
            {
                normalizedPoints[kvp.Key] = (kvp.Value / (double)maxRaw) * 10;
            }
        }

        return normalizedPoints;
    }

    private double ParseGrade(string? gradeStr)
    {
        if (string.IsNullOrWhiteSpace(gradeStr)) return 0;
        if (double.TryParse(gradeStr, out double result)) return result;
        return 0;
    }
}
