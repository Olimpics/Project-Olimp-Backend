using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public class ExcelProcessingService : IExcelProcessingService
{
    public async Task<List<GroupExcelRowDto>> ExtractGroupsAsync(IFormFile file)
    {
        var rows = new List<GroupExcelRowDto>();

        using (var stream = file.OpenReadStream())
        {
            using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart == null) return rows;

                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null) return rows;

                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null) return rows;

                var sharedStringTablePart = workbookPart.SharedStringTablePart;

                // Skip header row
                var excelRows = sheetData.Elements<Row>().Skip(1);

                foreach (var row in excelRows)
                {
                    var cells = row.Elements<Cell>().ToList();
                    if (cells.Count < 6) continue;

                    var dto = new GroupExcelRowDto
                    {
                        StartOfStudy = GetCellValue(cells[0], sharedStringTablePart),
                        FormOfStudy = GetCellValue(cells[1], sharedStringTablePart),
                        TermOfStudy = GetCellValue(cells[2], sharedStringTablePart),
                        EducationalProgram = GetCellValue(cells[3], sharedStringTablePart),
                        Course = GetCellValue(cells[4], sharedStringTablePart),
                        GroupCode = GetCellValue(cells[5], sharedStringTablePart)
                    };

                    if (!string.IsNullOrWhiteSpace(dto.GroupCode))
                    {
                        rows.Add(dto);
                    }
                }
            }
        }

        return rows;
    }

    public async Task<List<StudentExcelRowDto>> ExtractStudentsAsync(IFormFile file)
    {
        var rows = new List<StudentExcelRowDto>();

        using (var stream = file.OpenReadStream())
        {
            using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart == null) return rows;

                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null) return rows;

                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null) return rows;

                var sharedStringTablePart = workbookPart.SharedStringTablePart;

                // Skip header row
                var excelRows = sheetData.Elements<Row>().Skip(1);

                foreach (var row in excelRows)
                {
                    var cells = row.Elements<Cell>().ToList();
                    if (cells.Count < 9) continue;

                    var dto = new StudentExcelRowDto
                    {
                        EdboCode = GetCellValue(cells[0], sharedStringTablePart),
                        NameStudent = GetCellValue(cells[1], sharedStringTablePart),
                        EducationStart = GetCellValue(cells[2], sharedStringTablePart),
                        EducationEnd = GetCellValue(cells[3], sharedStringTablePart),
                        GroupCode = GetCellValue(cells[4], sharedStringTablePart),
                        EducationStatus = GetCellValue(cells[5], sharedStringTablePart),
                        IsFunded = GetCellValue(cells[6], sharedStringTablePart),
                        Email = GetCellValue(cells[7], sharedStringTablePart),
                        ReportCard = GetCellValue(cells[8], sharedStringTablePart)
                    };

                    if (!string.IsNullOrWhiteSpace(dto.EdboCode))
                    {
                        rows.Add(dto);
                    }
                }
            }
        }

        return rows;
    }

    private string? GetCellValue(Cell cell, SharedStringTablePart? sharedStringTablePart)
    {
        if (cell == null) return null;

        string value = cell.InnerText;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            if (sharedStringTablePart != null)
            {
                value = sharedStringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
            }
        }

        return value;
    }
}
