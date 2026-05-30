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
                    var rows = table.Descendants<TableRow>().ToList();
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var cells = rows[i].Descendants<TableCell>().ToList();
                        if (cells.Count >= 1)
                        {
                            string key = GetCellText(cells[0]);

                            if (key.Contains("Офіційна назва", StringComparison.OrdinalIgnoreCase))
                            {
                                if (string.IsNullOrEmpty(content.NameEducationalProgram))
                                    content.NameEducationalProgram = cells.Count >= 2 ? GetCellText(cells[1]) : "";
                            }
                            else if (key.Contains("Повна назва вищого навчального закладу", StringComparison.OrdinalIgnoreCase) || 
                                     key.Contains("Повна назва ВНЗ", StringComparison.OrdinalIgnoreCase))
                            {
                                content.InstitutionAndStructuralProgram = cells.Count >= 2 ? GetCellText(cells[1]) : "";
                            }
                            else if (key.Contains("Ступінь вищої освіти", StringComparison.OrdinalIgnoreCase)) content.Degree = GetCellText(cells.Count >= 2 ? cells[1] : cells[0]);
                            else if (key.Contains("Форми навчання", StringComparison.OrdinalIgnoreCase)) content.StudyForm = GetCellText(cells.Count >= 2 ? cells[1] : cells[0]);
                            else if (key.Contains("Мета освітньої програми", StringComparison.OrdinalIgnoreCase))
                            {
                                if (i + 1 < rows.Count)
                                {
                                    var nextRowCells = rows[i + 1].Descendants<TableCell>().ToList();
                                    if (nextRowCells.Count >= 1)
                                    {
                                        content.Goals = GetCellText(nextRowCells[0]);
                                    }
                                }
                            }
                            else if (key.Contains("Кваліфікація в дипломі", StringComparison.OrdinalIgnoreCase))
                            {
                                if (string.IsNullOrEmpty(content.SpecialityAndSpecializationWithDetails))
                                    content.SpecialityAndSpecializationWithDetails = cells.Count >= 2 ? GetCellText(cells[1]) : "";
                            }
                            else if (key.Contains("Основний фокус", StringComparison.OrdinalIgnoreCase)) content.Subject = GetCellText(cells.Count >= 2 ? cells[1] : cells[0]);
                        }
                    }
                }

                // 2. Process disciplines table(s)
                bool foundHeader = false;
                bool isMandatory = true;
                bool firstTable = true;
                bool stopParsing = false;

                foreach (var element in body.Elements())
                {
                    if (stopParsing) break;

                    if (element is Paragraph p)
                    {
                        var text = p.InnerText;
                        if (text.Contains("ЗАГАЛЬНИЙ ОБСЯГ ОСВІТНЬОЇ", StringComparison.OrdinalIgnoreCase))
                        {
                            stopParsing = true;
                            break;
                        }

                        if (text.Contains("Перелік компонент", StringComparison.OrdinalIgnoreCase))
                        {
                            foundHeader = true;
                            continue;
                        }

                        if (foundHeader)
                        {
                            if (text.Contains("Вибіркові компоненти", StringComparison.OrdinalIgnoreCase) || 
                                text.Contains("Вибіркові освітні компоненти", StringComparison.OrdinalIgnoreCase) ||
                                text.Contains("Цикл вибіркових", StringComparison.OrdinalIgnoreCase))
                            {
                                isMandatory = false;
                            }
                            else if (text.Contains("Обов'язкові компоненти", StringComparison.OrdinalIgnoreCase) ||
                                     text.Contains("Обов'язкові освітні компоненти", StringComparison.OrdinalIgnoreCase) ||
                                     text.Contains("Цикл обов'язкових", StringComparison.OrdinalIgnoreCase))
                            {
                                isMandatory = true;
                            }
                        }
                    }
                    else if (element is Table t && foundHeader)
                    {
                        var rows = t.Descendants<TableRow>().ToList();
                        int startRow = 0;
                        
                        // Skip first two rows only for the very first table after main header
                        if (firstTable)
                        {
                            startRow = 2;
                            firstTable = false;
                        }

                        for (int i = startRow; i < rows.Count; i++)
                        {
                            var row = rows[i];
                            var cells = row.Descendants<TableCell>().ToList();
                            var rowText = string.Join(" ", cells.Select(c => GetCellText(c))).Trim();

                            if (string.IsNullOrWhiteSpace(rowText)) continue;

                            if (rowText.Contains("ЗАГАЛЬНИЙ ОБСЯГ ОСВІТНЬОЇ", StringComparison.OrdinalIgnoreCase))
                            {
                                stopParsing = true;
                                break;
                            }

                            // Check for section headers within table rows
                            if (rowText.Contains("Вибіркові компоненти", StringComparison.OrdinalIgnoreCase) || 
                                rowText.Contains("Вибіркові освітні компоненти", StringComparison.OrdinalIgnoreCase) ||
                                rowText.Contains("Цикл вибіркових", StringComparison.OrdinalIgnoreCase))
                            {
                                isMandatory = false;
                                continue;
                            }
                            if (rowText.Contains("Обов'язкові компоненти", StringComparison.OrdinalIgnoreCase) ||
                                rowText.Contains("Обов'язкові освітні компоненти", StringComparison.OrdinalIgnoreCase) ||
                                rowText.Contains("Цикл обов'язкових", StringComparison.OrdinalIgnoreCase))
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

                                if (string.IsNullOrWhiteSpace(discipline.Name) || 
                                    discipline.Name.Contains("Освітній компонент", StringComparison.OrdinalIgnoreCase) ||
                                    discipline.Name.Equals("Назва", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                if (isMandatory)
                                {
                                    // NEW LOGIC: Check if first word of Name is in Loans
                                    string nameText = discipline.Name ?? "";
                                    string firstWordOfName = nameText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                                    string loansText = (discipline.Loans ?? "").ToLower();

                                    if (!string.IsNullOrEmpty(firstWordOfName) && loansText.Contains(firstWordOfName.ToLower()))
                                    {
                                        string fullRow = string.Join(" | ", cells.Select(c => GetCellText(c)));
                                        content.MainDisciplinesNeedFix.Add(fullRow);
                                    }
                                    else
                                    {
                                        content.MainDisciplines.Add(discipline);
                                    }
                                }
                                else
                                {
                                    content.SelectiveDisciplines.Add(discipline);
                                }
                            }
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
