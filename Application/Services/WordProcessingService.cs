using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OlimpBack.Application.DTO;
using System.Text;

namespace OlimpBack.Application.Services;

public class WordProcessingService : IWordProcessingService
{
    public async Task<SelectiveDisciplineWordContentDto> ExtractContentAsync(string filePath)
    {
        var content = new SelectiveDisciplineWordContentDto();
        
        try
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document.Body;
                if (body == null) return content;

                var fullText = new StringBuilder();
                foreach (var text in body.Descendants<Text>())
                {
                    fullText.AppendLine(text.Text);
                }

                // Generic extraction: Since Word files for syllabi often have tables or specific headers,
                // we'll try to find keywords and extract text until the next keyword.
                // However, a more robust way for Gemini is to just give it the whole text 
                // and let it find these fields. 
                // BUT the user wants a JSON format for the batch.
                
                string textContent = fullText.ToString();
                
                content.CodeAndName = ExtractSection(textContent, new[] { "Код і назва", "Назва дисципліни", "Discipline name" });
                content.RecommendedForFields = ExtractSection(textContent, new[] { "Рекомендовано для", "Recommended for" });
                content.Department = ExtractSection(textContent, new[] { "Кафедра", "Department" });
                content.Instructor = ExtractSection(textContent, new[] { "Викладач", "Instructor", "Lecturer" });
                content.EducationLevel = ExtractSection(textContent, new[] { "Рівень освіти", "Education level" });
                content.CourseAndSemester = ExtractSection(textContent, new[] { "Курс та семестр", "Course and semester" });
                content.LanguageOfInstruction = ExtractSection(textContent, new[] { "Мова викладання", "Language of instruction" });
                content.Prerequisites = ExtractSection(textContent, new[] { "Пререквізити", "Prerequisites" });
                content.WhyStudyThisCourse = ExtractSection(textContent, new[] { "Чому варто вивчати", "Why study this course" });
                content.TopicList = ExtractSection(textContent, new[] { "Перелік тем", "Topic list", "Content" });
                content.CompetenciesGained = ExtractSection(textContent, new[] { "Набуті компетенції", "Competencies" });
                content.ExpectedLearningOutcomes = ExtractSection(textContent, new[] { "Очікувані результати", "Learning outcomes" });
                content.InformationResources = ExtractSection(textContent, new[] { "Інформаційні ресурси", "Resources" });
                content.TypesOfLearningActivities = ExtractSection(textContent, new[] { "Види навчальних занять", "Types of learning activities" });
                content.SemesterControlType = ExtractSection(textContent, new[] { "Вид семестрового контролю", "Semester control" });
                content.MaxMinStudents = ExtractSection(textContent, new[] { "Макс/Мін студентів", "Number of students" });
                
                // If everything is empty, just put all text in CodeAndName as a fallback
                if (string.IsNullOrWhiteSpace(content.CodeAndName) && string.IsNullOrWhiteSpace(content.Department))
                {
                    content.CodeAndName = textContent.Length > 2000 ? textContent.Substring(0, 2000) : textContent;
                }
            }
        }
        catch (Exception)
        {
            // Log or handle
        }

        return content;
    }

    private string? ExtractSection(string text, string[] keywords)
    {
        foreach (var keyword in keywords)
        {
            int index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                // Find next keyword or end of text
                // This is a very rough approximation. In practice, syllabus files are complex.
                // For now, let's take some characters after the keyword.
                int start = index + keyword.Length;
                int end = text.Length;
                
                // Try to find if there's a colon or newline right after keyword
                while (start < text.Length && (text[start] == ':' || char.IsWhiteSpace(text[start])))
                {
                    start++;
                }

                // Take until a large gap or a known next keyword header (simplified)
                // Actually, let's just take a chunk of text, Gemini will clean it up.
                int length = Math.Min(500, text.Length - start); 
                return text.Substring(start, length).Trim();
            }
        }
        return null;
    }
}
