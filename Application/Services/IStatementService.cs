using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IStatementService
{
    Task<List<GroupShortDto>> GetGroupsByMainDisciplineIdAsync(Guid disciplineId);
    Task<List<GroupShortDto>> GetGroupsBySelectiveDisciplineIdAsync(Guid disciplineId);
    Task<List<StatementFileDto>> GenerateMainDisciplineStatementsAsync(Guid disciplineId);
    Task<List<StatementFileDto>> GenerateSelectiveDisciplineStatementsAsync(Guid disciplineId);
}
