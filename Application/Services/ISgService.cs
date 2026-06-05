using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO.Sg;

namespace OlimpBack.Application.Services;

public interface ISgService
{
    // SubDivisions
    Task<List<SubDivisionUserDto>> GetUserSubDivisionsAsync(Guid userId);
    
    // Events
    Task<List<EventDto>> GetEventsAsync(Guid subDivisionId, Guid? facultyId, Guid catalogYearId, bool isEven, string? search);
    Task<EventDto?> GetEventByIdAsync(Guid id);
    Task<EventDto> CreateEventAsync(EventCreateUpdateDto dto, Guid creatorId);
    Task<bool> UpdateEventAsync(Guid id, EventCreateUpdateDto dto);
    Task<bool> DeleteEventAsync(Guid id);

    // BindEventStudent
    Task<List<BindEventStudentDto>> GetEventStudentsAsync(Guid eventId, string? search);
    Task<bool> AddStudentToEventAsync(BindEventStudentCreateUpdateDto dto);
    Task<bool> UpdateStudentInEventAsync(Guid id, BindEventStudentCreateUpdateDto dto);
    Task<bool> DeleteStudentFromEventAsync(Guid id);

    // MembersOfSg
    Task<List<StudentSgDto>> GetStudentsInSgAsync(Guid subDivisionId, Guid? facultyId, string? search);
    Task<bool> AddStudentToSubDivisionAsync(MemberSgCreateUpdateDto dto);
    Task<bool> UpdateMemberSgAsync(Guid id, MemberSgCreateUpdateDto dto);
    Task<bool> DeleteMemberSgAsync(Guid id);

    // InventorySg
    Task<List<InventorySgDto>> GetInventoryAsync(string? search);
    Task<bool> AddInventoryItemAsync(InventorySgCreateUpdateDto dto);
    Task<bool> UpdateInventoryItemAsync(Guid id, InventorySgCreateUpdateDto dto);
    Task<bool> DeleteInventoryItemAsync(Guid id);

    // AccountingJournal
    Task<List<AccountingJournalDto>> GetJournalEntriesAsync(Guid inventoryId, string? search);
    Task<bool> AddJournalEntryAsync(AccountingJournalCreateUpdateDto dto);
    Task<bool> UpdateJournalEntryAsync(Guid id, AccountingJournalCreateUpdateDto dto);
    Task<bool> DeleteJournalEntryAsync(Guid id);
    Task<bool> ConfirmReturnAsync(Guid id, Guid currentUserId);
}
