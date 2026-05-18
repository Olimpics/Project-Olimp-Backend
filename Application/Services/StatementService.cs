using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class StatementService : IStatementService
{
    private readonly AppDbContext _context;
    private readonly string _templatePath;

    public StatementService(AppDbContext context)
    {
        _context = context;
        // Assuming the template is in Examples/ExampleOfStatements.docx
        // Even if the user said .doc, OpenXml needs .docx.
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Examples", "ExampleOfStatements.docx");
        
        // Fallback for development if BaseDirectory is different
        if (!File.Exists(_templatePath))
        {
            _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Examples", "ExampleOfStatements.docx");
        }
        
        // If still not found, check for .doc just in case we can at least find the file
        if (!File.Exists(_templatePath))
        {
            var docPath = _templatePath.Replace(".docx", ".doc");
            if (File.Exists(docPath))
            {
                // We'll try to use it as if it's docx, but it will likely fail if it's truly binary doc.
                _templatePath = docPath;
            }
        }
    }

    public async Task<List<GroupShortDto>> GetGroupsByMainDisciplineIdAsync(Guid disciplineId)
    {
        return await _context.BindMainDisciplines
            .Where(b => b.MainDisciplinesId == disciplineId)
            .Select(b => b.Student.Group)
            .Distinct()
            .Select(g => new GroupShortDto
            {
                Id = g.IdGroup,
                Code = g.GroupCode
            })
            .ToListAsync();
    }

    public async Task<List<GroupShortDto>> GetGroupsBySelectiveDisciplineIdAsync(Guid disciplineId)
    {
        return await _context.BindSelectiveDisciplines
            .Where(b => b.SelectiveDisciplineId == disciplineId)
            .Select(b => b.Student.Group)
            .Distinct()
            .Select(g => new GroupShortDto
            {
                Id = g.IdGroup,
                Code = g.GroupCode
            })
            .ToListAsync();
    }

    public async Task<List<StatementFileDto>> GenerateMainDisciplineStatementsAsync(Guid disciplineId)
    {
        var discipline = await _context.MainDisciplines
            .Include(d => d.EducationalProgram)
                .ThenInclude(ep => ep.Speciality)
                    .ThenInclude(s => s.Department)
                        .ThenInclude(dept => dept.Faculty)
            .Include(d => d.EducationalProgram)
                .ThenInclude(ep => ep.Degree)
            .Include(d => d.TypeOfControlNavigation)
            .Include(d => d.BindTeacherMains)
                .ThenInclude(btm => btm.Admin)
                    .ThenInclude(a => a.AcademicDegree)
            .FirstOrDefaultAsync(d => d.IdMainDisciplines == disciplineId);

        if (discipline == null) return new List<StatementFileDto>();

        var binds = await _context.BindMainDisciplines
            .Include(b => b.Student)
                .ThenInclude(s => s.Group)
            .Where(b => b.MainDisciplinesId == disciplineId)
            .ToListAsync();

        var marks = await _context.MarkOfScores.ToListAsync();
        var facultyId = discipline.EducationalProgram.Speciality.Department.FacultyId;
        var dean = await GetDeanForFacultyAsync(facultyId);

        var groups = binds.GroupBy(b => b.Student.Group).ToList();
        var files = new List<StatementFileDto>();

        foreach (var groupGroup in groups)
        {
            var group = groupGroup.Key;
            var students = groupGroup.OrderBy(s => s.Student.SecondName).ToList();

            var data = new Dictionary<string, string>
            {
                { "Faculty", discipline.EducationalProgram.Speciality.Department.Faculty.NameFaculty },
                { "Speciality", discipline.EducationalProgram.Speciality.Name },
                { "EducationalProgram", discipline.EducationalProgram.NameEducationalProgram },
                { "DegreeLevel", discipline.EducationalProgram.Degree.NameEducationalDegree },
                { "Course", group.Course.ToString() },
                { "GroupCode", group.GroupCode },
                { "date", DateTime.Now.ToShortDateString() },
                { "MainDisciplineName", discipline.NameMainDisciplines },
                { "SemestrText", GetSemesterText(discipline.Semestr) },
                { "FormControl", discipline.TypeOfControlNavigation?.Type ?? "" },
                { "Hours", discipline.Hours?.ToString() ?? "0" },
                { "Loans", discipline.Loans?.ToString() ?? "0" },
                { "dateNow", DateTime.Now.ToShortDateString() },
                { "DeanOfThisFacultyName", dean != null ? $"{dean.FirstName} {dean.SecondName}".ToUpper() : "" }
            };

            var headTeacherBind = discipline.BindTeacherMains.FirstOrDefault(b => b.IsHead);
            if (headTeacherBind != null)
            {
                var teacher = headTeacherBind.Admin;
                data["TeacherPosition"] = teacher.AcademicDegree.AcademicDegreeShortedName;
                data["TeacherName"] = FormatNameInitials(teacher.FirstName, teacher.SecondName, teacher.ThirdName);
                data["TeacherNameAndSURNAME"] = $"{teacher.FirstName} {teacher.SecondName}".ToUpper();
            }
            else
            {
                data["TeacherPosition"] = "";
                data["TeacherName"] = "";
                data["TeacherNameAndSURNAME"] = "";
            }

            // Grade counts
            foreach (var mark in marks)
            {
                int count = groupGroup.Count(b => b.Grade >= mark.MinGrade && b.Grade <= mark.MaxGrade);
                data[$"CountOfGrade_{mark.IdMark}"] = count.ToString();
                data[$"Range_{mark.IdMark}"] = $"{mark.MinGrade}-{mark.MaxGrade}";
            }

            var studentGradeList = students.Select(s => new StudentGradeData
            {
                StudentName = FormatNameInitials(s.Student.FirstName, s.Student.SecondName, s.Student.ThirdName),
                ReportCard = s.Student.ReportCard,
                Grade = s.Grade.ToString(),
                GradeName = marks.FirstOrDefault(m => s.Grade >= m.MinGrade && s.Grade <= m.MaxGrade)?.NameOfGrade ?? ""
            }).ToList();

            var gradeSummary = marks.Select(mark => new GradeSummaryData
            {
                Range = $"{mark.MinGrade}-{mark.MaxGrade}",
                Count = groupGroup.Count(b => b.Grade >= mark.MinGrade && b.Grade <= mark.MaxGrade).ToString()
            }).ToList();

            var fileContent = GenerateWordFile(data, studentGradeList, gradeSummary);

            files.Add(new StatementFileDto
            {
                FileName = $"Statement_{group.GroupCode}_{discipline.NameMainDisciplines}.docx",
                Content = fileContent
            });
        }

        return files;
    }

    public async Task<List<StatementFileDto>> GenerateSelectiveDisciplineStatementsAsync(Guid disciplineId)
    {
        var discipline = await _context.SelectiveDisciplines
            .Include(d => d.Department)
                .ThenInclude(dept => dept.Faculty)
            .Include(d => d.DegreeLevel)
            .Include(d => d.TypeOfControl)
            .Include(d => d.BindTeachersSelectives)
                .ThenInclude(bts => bts.Admin)
                    .ThenInclude(a => a.AcademicDegree)
            .FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == disciplineId);

        if (discipline == null) return new List<StatementFileDto>();

        var binds = await _context.BindSelectiveDisciplines
            .Include(b => b.Student)
                .ThenInclude(s => s.Group)
                    .ThenInclude(g => g.EducationalProgram)
                        .ThenInclude(ep => ep.Speciality)
                            .ThenInclude(sp => sp.Department)
                                .ThenInclude(dept => dept.Faculty)
            .Include(b => b.Student)
                .ThenInclude(s => s.Group)
                    .ThenInclude(g => g.EducationalProgram)
                        .ThenInclude(ep => ep.Degree)
            .Where(b => b.SelectiveDisciplineId == disciplineId)
            .ToListAsync();

        var marks = await _context.MarkOfScores.ToListAsync();
        var facultyId = discipline.Department.FacultyId;
        var dean = await GetDeanForFacultyAsync(facultyId);

        var groups = binds.GroupBy(b => b.Student.Group).ToList();
        var files = new List<StatementFileDto>();

        foreach (var groupGroup in groups)
        {
            var group = groupGroup.Key;
            var students = groupGroup.OrderBy(s => s.Student.SecondName).ToList();

            var data = new Dictionary<string, string>
            {
                { "Faculty", group.EducationalProgram.Speciality.Department.Faculty.NameFaculty },
                { "Speciality", group.EducationalProgram.Speciality.Name },
                { "EducationalProgram", group.EducationalProgram.NameEducationalProgram },
                { "DegreeLevel", group.EducationalProgram.Degree.NameEducationalDegree },
                { "Course", group.Course.ToString() },
                { "GroupCode", group.GroupCode },
                { "date", DateTime.Now.ToShortDateString() },
                { "MainDisciplineName", discipline.NameSelectiveDisciplines },
                { "SemestrText", GetSemesterText(groupGroup.First().Semestr) },
                { "FormControl", discipline.TypeOfControl?.Type ?? "" },
                { "Hours", "0" }, 
                { "Loans", groupGroup.First().Loans?.ToString() ?? "0" },
                { "dateNow", DateTime.Now.ToShortDateString() },
                { "DeanOfThisFacultyName", dean != null ? $"{dean.FirstName} {dean.SecondName}".ToUpper() : "" }
            };

            var headTeacherBind = discipline.BindTeachersSelectives.FirstOrDefault(b => b.IsHead);
            if (headTeacherBind != null)
            {
                var teacher = headTeacherBind.Admin;
                data["TeacherPosition"] = teacher.AcademicDegree.AcademicDegreeShortedName;
                data["TeacherName"] = FormatNameInitials(teacher.FirstName, teacher.SecondName, teacher.ThirdName);
                data["TeacherNameAndSURNAME"] = $"{teacher.FirstName} {teacher.SecondName}".ToUpper();
            }
            else
            {
                data["TeacherPosition"] = "";
                data["TeacherName"] = "";
                data["TeacherNameAndSURNAME"] = "";
            }

            var studentGradeList = students.Select(s => new StudentGradeData
            {
                StudentName = FormatNameInitials(s.Student.FirstName, s.Student.SecondName, s.Student.ThirdName),
                ReportCard = s.Student.ReportCard,
                Grade = s.Grade.ToString(),
                GradeName = marks.FirstOrDefault(m => s.Grade >= m.MinGrade && s.Grade <= m.MaxGrade)?.NameOfGrade ?? ""
            }).ToList();

            var gradeSummary = marks.Select(mark => new GradeSummaryData
            {
                Range = $"{mark.MinGrade}-{mark.MaxGrade}",
                Count = groupGroup.Count(b => b.Grade >= mark.MinGrade && b.Grade <= mark.MaxGrade).ToString()
            }).ToList();

            var fileContent = GenerateWordFile(data, studentGradeList, gradeSummary);

            files.Add(new StatementFileDto
            {
                FileName = $"Statement_{group.GroupCode}_{discipline.NameSelectiveDisciplines}.docx",
                Content = fileContent
            });
        }

        return files;
    }

    private async Task<AdminsPersonal?> GetDeanForFacultyAsync(Guid facultyId)
    {
        var deanRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Dean");
        if (deanRole == null) return null;

        var deanUserRole = await _context.UserRoles
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == deanRole.IdRole && ur.FacultyId == facultyId)
            .FirstOrDefaultAsync();

        if (deanUserRole == null) return null;

        return await _context.AdminsPersonals.FirstOrDefaultAsync(a => a.UserId == deanUserRole.UserId);
    }

    private string GetSemesterText(int semester)
    {
        return semester switch
        {
            1 => "Перший",
            2 => "Другий",
            3 => "Третій",
            4 => "Четвертий",
            5 => "П'ятий",
            6 => "Шостий",
            7 => "Сьомий",
            8 => "Восьмий",
            _ => semester.ToString()
        };
    }

    private string FormatNameInitials(string firstName, string? secondName, string? thirdName)
    {
        string res = secondName ?? "";
        if (!string.IsNullOrEmpty(firstName))
        {
            res += $" {firstName[0]}.";
        }
        if (!string.IsNullOrEmpty(thirdName))
        {
            res += $"{thirdName[0]}.";
        }
        return res;
    }

    private byte[] GenerateWordFile(Dictionary<string, string> placeholders, List<StudentGradeData> studentGrades, List<GradeSummaryData> gradeSummary)
    {
        if (!File.Exists(_templatePath))
        {
            throw new FileNotFoundException("Template file not found.", _templatePath);
        }

        byte[] templateBytes = File.ReadAllBytes(_templatePath);
        using var mem = new MemoryStream();
        mem.Write(templateBytes, 0, templateBytes.Length);

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(mem, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;

            // Replace simple placeholders
            foreach (var text in body.Descendants<Text>())
            {
                foreach (var placeholder in placeholders)
                {
                    if (text.Text.Contains($"{{{{{placeholder.Key}}}}}"))
                    {
                        text.Text = text.Text.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                    }
                }
            }

            // Fill students table
            var studentTable = body.Descendants<Table>()
                .FirstOrDefault(t => t.Descendants<Text>().Any(tx => tx.Text.Contains("{{StudentName}}")));

            if (studentTable != null)
            {
                var rowTemplate = studentTable.Descendants<TableRow>()
                    .FirstOrDefault(r => r.Descendants<Text>().Any(tx => tx.Text.Contains("{{StudentName}}")));

                if (rowTemplate != null)
                {
                    foreach (var student in studentGrades)
                    {
                        var newRow = (TableRow)rowTemplate.CloneNode(true);
                        foreach (var text in newRow.Descendants<Text>())
                        {
                            text.Text = text.Text.Replace("{{StudentName}}", student.StudentName);
                            text.Text = text.Text.Replace("{{reportCard}}", student.ReportCard);
                            text.Text = text.Text.Replace("{{Grade}}", student.Grade);
                            text.Text = text.Text.Replace("{{nameOfGrade}}", student.GradeName);
                        }
                        studentTable.AppendChild(newRow);
                    }
                    rowTemplate.Remove();
                }
            }
            
            // Handle Grade Summary Table
            var summaryTable = body.Descendants<Table>()
                .FirstOrDefault(t => t.Descendants<Text>().Any(tx => tx.Text.Contains("{{CountOfGradeByThisRange}}")));

            if (summaryTable != null)
            {
                var rowTemplate = summaryTable.Descendants<TableRow>()
                    .FirstOrDefault(r => r.Descendants<Text>().Any(tx => tx.Text.Contains("{{CountOfGradeByThisRange}}")));

                if (rowTemplate != null)
                {
                    foreach (var markSummary in gradeSummary)
                    {
                        var newRow = (TableRow)rowTemplate.CloneNode(true);
                        foreach (var text in newRow.Descendants<Text>())
                        {
                            text.Text = text.Text.Replace("{{Range}}", markSummary.Range);
                            text.Text = text.Text.Replace("{{CountOfGradeByThisRange}}", markSummary.Count);
                        }
                        summaryTable.AppendChild(newRow);
                    }
                    rowTemplate.Remove();
                }
            }

            wordDoc.MainDocumentPart.Document.Save();
        }

        return mem.ToArray();
    }

    private class StudentGradeData
    {
        public string StudentName { get; set; } = null!;
        public string ReportCard { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string GradeName { get; set; } = null!;
    }

    private class GradeSummaryData
    {
        public string Range { get; set; } = null!;
        public string Count { get; set; } = null!;
    }
}
