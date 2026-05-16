using System;
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
    Task<RatingStatusResponseDto> GetRatingStatusAsync(RatingStatusQueryDto query);
    Task<PaginatedResponseDto<RatingStudentDto>> GetPaginatedRatingsAsync(RatingListQueryDto query);
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

        var academicScores = new Dictionary<Guid, double>();
        var studentRedoMap = new Dictionary<Guid, bool>();

        foreach (var student in students)
        {
            var studentMainGrades = mainGrades.Where(mg => mg.StudentId == student.IdStudent).ToList();
            var studentSelectiveGrades = selectiveGrades.Where(sg => sg.StudentId == student.IdStudent).ToList();

            int totalCourses = studentMainGrades.Count + studentSelectiveGrades.Count;
            bool hasRedo = false;

            if (totalCourses == 0)
            {
                academicScores[student.IdStudent] = 0;
                studentRedoMap[student.IdStudent] = false;
                continue;
            }

            double sumGrades = 0;
            foreach (var mg in studentMainGrades)
            {
                sumGrades += ParseGrade(mg.MainGrade1);
                if (mg.MainDisciplines?.BindMainDisciplines?.Any(bmd => bmd.IsRedo == true) == true)
                {
                    hasRedo = true;
                }
            }
            foreach (var sg in studentSelectiveGrades)
            {
                sumGrades += sg.Grade;
                if (sg.IsRedo == true)
                {
                    hasRedo = true;
                }
            }

            double averageGrade = sumGrades / totalCourses;
            academicScores[student.IdStudent] = averageGrade * 0.9;
            studentRedoMap[student.IdStudent] = hasRedo;
        }

        // 3. Calculate extra points
        var extraPointsMap = await CalculateExtraPointsAsync(students);

        // 4. Final calculation and persistence
        var ratings = new List<BindRating>();
        foreach (var student in students)
        {
            double academicScore = academicScores.ContainsKey(student.IdStudent) ? academicScores[student.IdStudent] : 0;
            double extraScore = extraPointsMap.ContainsKey(student.IdStudent) ? extraPointsMap[student.IdStudent] : 0;
            bool isRedo = studentRedoMap.ContainsKey(student.IdStudent) && studentRedoMap[student.IdStudent];

            ratings.Add(new BindRating
            {
                IdBindRating = Guid.NewGuid(),
                StudentId = student.IdStudent,
                IsEven = (query.SemesterType == 2),
                FinalScore = (float)(academicScore + extraScore),
                IsRedo = isRedo
            });
        }

        await _repository.AddRatingsAsync(ratings);

        // 5. Record calculation time
        var calcTime = new RatingCalculationTime
        {
            IdRatingCalculationTime = Guid.NewGuid(),
            SpecialityId = students.First().Group?.EducationalProgram?.SpecialityId, 
            Course = query.Course,
            IsEven = (query.SemesterType == 2),
            IsShorted = query.IsAccelerated,
            Date = DateOnly.FromDateTime(DateTime.Now),
            YearId = query.CatalogYearId
        };
        
        await _repository.AddCalculationTimeAsync(calcTime);
        await _repository.SaveChangesAsync();
    }

    public async Task<RatingStatusResponseDto> GetRatingStatusAsync(RatingStatusQueryDto query)
    {
        var calcTime = await _repository.GetCalculationTimeAsync(query.SpecialityId, query.Course, query.Semester, query.CatalogYearId, query.IsAccelerated);
        
        return new RatingStatusResponseDto
        {
            Exists = calcTime != null,
            CalculationTime = calcTime?.Date?.ToDateTime(TimeOnly.MinValue)
        };
    }

    public async Task<PaginatedResponseDto<RatingStudentDto>> GetPaginatedRatingsAsync(RatingListQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPaginatedRatingsAsync(query);

        var studentDtos = items.Select(r => new RatingStudentDto
        {
            FullName = r.Student?.NameStudent ?? "Unknown",
            GroupName = r.Student?.Group?.GroupCode ?? "Unknown",
            Score = r.FinalScore
        }).ToList();

        return new PaginatedResponseDto<RatingStudentDto>
        {
            Items = studentDtos,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }

    private async Task<Dictionary<Guid, double>> CalculateExtraPointsAsync(List<Student> students)
    {
        var studentIds = students.Select(s => s.IdStudent).ToList();
        var eventPoints = await _repository.GetEventPointsAsync(studentIds);
        var extraActivityPoints = await _repository.GetExtraActivityPointsAsync(studentIds);
        
        var sgStudents = students.Where(s => s.IsInSg).Select(s => s.IdStudent).ToList();
        var sgPointsMap = sgStudents.Any() ? await _repository.GetSgPointsMapAsync(sgStudents) : new Dictionary<Guid, int>();

        var rawExtraPoints = new Dictionary<Guid, int>();
        foreach (var sid in studentIds)
        {
            int totalRaw = 0;
            totalRaw += eventPoints.Where(ep => ep.StudentId == sid).Sum(ep => ep.Point);
            totalRaw += extraActivityPoints.Where(eap => eap.StudentId == sid).Sum(eap => eap.Points);
            if (sgPointsMap.ContainsKey(sid))
            {
                totalRaw += sgPointsMap[sid];
            }
            rawExtraPoints[sid] = totalRaw;
        }

        int maxRaw = rawExtraPoints.Values.Any() ? rawExtraPoints.Values.Max() : 0;
        var normalizedPoints = new Dictionary<Guid, double>();

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
