using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OlimpBack.Application.DTO;
using System.Text;

namespace OlimpBack.Application.Services;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

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

                // Находим все таблицы в документе
                var tables = body.Descendants<Table>();

                foreach (var table in tables)
                {
                    foreach (var row in table.Descendants<TableRow>())
                    {
                        var cells = row.Descendants<TableCell>().ToList();

                        // Нам нужны только строки, где есть минимум 2 ячейки (Ключ - Значение)
                        if (cells.Count >= 2)
                        {
                            string key = GetCellText(cells[0]).ToLower();
                            string value = GetCellText(cells[1]);

                            if (string.IsNullOrWhiteSpace(value)) continue;

                            // Маппинг по ключевым словам из первой колонки
                            if (key.Contains("код та назва")) content.CodeAndName = value;
                            else if (key.Contains("рекомендується для галузі")) content.RecommendedForFields = value;
                            else if (key.Contains("кафедра")) content.Department = value;
                            else if (key.Contains("п.і.п. нпп") || key.Contains("викладач")) content.Instructor = value;
                            else if (key.Contains("рівень во")) content.EducationLevel = value;
                            else if (key.Contains("курс, семестр")) content.CourseAndSemester = value;
                            else if (key.Contains("мова викладання")) content.LanguageOfInstruction = value;
                            else if (key.Contains("пререквізити")) content.Prerequisites = value;
                            else if (key.Contains("чому це цікаво")) content.WhyStudyThisCourse = value;
                            else if (key.Contains("перелік тем")) content.TopicList = value;
                            else if (key.Contains("як можна користуватися")) content.CompetenciesGained = value;
                            else if (key.Contains("очікувані результати")) content.ExpectedLearningOutcomes = value;
                            else if (key.Contains("інформаційне забезпечення")) content.InformationResources = value;
                            else if (key.Contains("види навчальних занять")) content.TypesOfLearningActivities = value;
                            else if (key.Contains("вид семестрового контролю")) content.SemesterControlType = value;
                            else if (key.Contains("максимальна кількість")) content.MaxMinStudents = value;
                        }
                    }
                }

                // Специальная обработка для поля "Перелік тем", если оно идет списком после таблицы
                if (string.IsNullOrWhiteSpace(content.TopicList))
                {
                    content.TopicList = ExtractTopicsAfterTable(body);
                }
            }
        }
        catch (Exception ex)
        {
            // Здесь добавь логгер: _logger.LogError(ex, "Error reading word file");
        }

        return content;
    }

    // Вспомогательный метод для чистого извлечения текста из ячейки
    private string GetCellText(TableCell cell)
    {
        var text = new StringBuilder();
        foreach (var para in cell.Descendants<Paragraph>())
        {
            foreach (var run in para.Descendants<Run>())
            {
                text.Append(run.InnerText);
            }
            text.Append(" "); // Добавляем пробел между параграфами в одной ячейке
        }
        return text.ToString().Trim();
    }

    public async Task<EducationalProgramWordContentDto> ExtractEducationalProgramContentAsync(string filePath)
    {
        var content = new EducationalProgramWordContentDto();

        try
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document.Body;
                if (body == null) return content;

                var tables = body.Descendants<Table>().ToList();
                if (tables.Count == 0) return content;

                // 1. Process main information (usually first tables)
                foreach (var table in tables)
                {
                    foreach (var row in table.Descendants<TableRow>())
                    {
                        var cells = row.Descendants<TableCell>().ToList();
                        if (cells.Count >= 2)
                        {
                            string key = GetCellText(cells[0]);
                            string value = GetCellText(cells[1]);

                            if (key.Contains("Офіційна назва", StringComparison.OrdinalIgnoreCase)) content.NameEducationalProgram = value;
                            else if (key.Contains("Ступінь вищої освіти", StringComparison.OrdinalIgnoreCase)) content.Degree = value;
                            else if (key.Contains("Форми навчання", StringComparison.OrdinalIgnoreCase)) content.StudyForm = value;
                            else if (key.Contains("Мета освітньої програми", StringComparison.OrdinalIgnoreCase)) content.Goals = value;
                            else if (key.Contains("Кваліфікація в дипломі", StringComparison.OrdinalIgnoreCase)) content.SpecialityAndSpecializationWithDetails = value;
                            else if (key.Contains("Основний фокус", StringComparison.OrdinalIgnoreCase)) content.Subject = value;
                        }
                    }
                }

                // 2. Process disciplines table
                // Find table after "2 Перелік компонентів" or "Перелік компонентів ОП"
                Table? componentsTable = null;
                bool foundHeader = false;

                foreach (var element in body.Elements())
                {
                    if (element is Paragraph p)
                    {
                        var text = p.InnerText;
                        if (text.Contains("Перелік компонентів", StringComparison.OrdinalIgnoreCase))
                        {
                            foundHeader = true;
                        }
                    }
                    else if (element is Table t && foundHeader)
                    {
                        componentsTable = t;
                        break;
                    }
                }

                if (componentsTable != null)
                {
                    bool isMandatory = true;
                    foreach (var row in componentsTable.Descendants<TableRow>())
                    {
                        var cells = row.Descendants<TableCell>().ToList();
                        var rowText = string.Join(" ", cells.Select(GetCellText));

                        if (rowText.Contains("Вибіркові компоненти", StringComparison.OrdinalIgnoreCase))
                        {
                            isMandatory = false;
                            continue;
                        }
                        if (rowText.Contains("Обов'язкові компоненти", StringComparison.OrdinalIgnoreCase))
                        {
                            isMandatory = true;
                            continue;
                        }

                        if (cells.Count >= 5)
                        {
                            var discipline = new DisciplineRowDto
                            {
                                Code = GetCellText(cells[0]),
                                Name = GetCellText(cells[1]),
                                Loans = GetCellText(cells[2]),
                                Control = GetCellText(cells[3]),
                                Semester = GetCellText(cells[4])
                            };

                            if (string.IsNullOrWhiteSpace(discipline.Name) || discipline.Name.Contains("Освітній компонент", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (isMandatory) content.MainDisciplines.Add(discipline);
                            else content.SelectiveDisciplines.Add(discipline);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log error
        }

        return content;
    }

    // Если темы не в таблице, а просто текстом после заголовка
    private string? ExtractTopicsAfterTable(Body body)
    {
        var allText = body.InnerText;
        var marker = "Перелік тем з дисципліни";
        int index = allText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (index != -1)
        {
            // Берем текст после маркера, Gemini сам разберется где конец
            return allText.Substring(index + marker.Length).Trim();
        }
        return null;
    }
}
