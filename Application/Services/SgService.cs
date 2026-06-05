using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.Application.DTO.Sg;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class SgService : ISgService
{
    private readonly ISgRepository _sgRepository;
    private readonly IMapper _mapper;

    public SgService(ISgRepository sgRepository, IMapper mapper)
    {
        _sgRepository = sgRepository;
        _mapper = mapper;
    }

    public async Task<List<SubDivisionUserDto>> GetUserSubDivisionsAsync(Guid userId)
    {
        var members = await _sgRepository.GetUserSubDivisionsAsync(userId);
        return _mapper.Map<List<SubDivisionUserDto>>(members);
    }

    public async Task<List<EventDto>> GetEventsAsync(Guid subDivisionId, Guid? facultyId, Guid catalogYearId, bool isEven, string? search)
    {
        var events = await _sgRepository.GetEventsAsync(subDivisionId, facultyId, catalogYearId, isEven, search);
        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        var @event = await _sgRepository.GetEventByIdAsync(id);
        return _mapper.Map<EventDto>(@event);
    }

    public async Task<EventDto> CreateEventAsync(EventCreateUpdateDto dto, Guid creatorId)
    {
        var @event = _mapper.Map<Event>(dto);
        @event.IdEvent = Guid.NewGuid();
        @event.CreatorId = creatorId;
        @event.Avail = true;
        
        await _sgRepository.AddEventAsync(@event);
        await _sgRepository.SaveChangesAsync();
        
        return _mapper.Map<EventDto>(@event);
    }

    public async Task<bool> UpdateEventAsync(Guid id, EventCreateUpdateDto dto)
    {
        var @event = await _sgRepository.GetEventByIdAsync(id);
        if (@event == null) return false;

        _mapper.Map(dto, @event);
        _sgRepository.UpdateEvent(@event);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var @event = await _sgRepository.GetEventByIdAsync(id);
        if (@event == null) return false;

        _sgRepository.DeleteEvent(@event);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<BindEventStudentDto>> GetEventStudentsAsync(Guid eventId, string? search)
    {
        var binds = await _sgRepository.GetEventStudentsAsync(eventId, search);
        return _mapper.Map<List<BindEventStudentDto>>(binds);
    }

    public async Task<bool> AddStudentToEventAsync(BindEventStudentCreateUpdateDto dto)
    {
        var bind = _mapper.Map<BindEventStudent>(dto);
        bind.IdBindEventStudent = Guid.NewGuid();
        
        await _sgRepository.AddBindEventStudentAsync(bind);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStudentInEventAsync(Guid id, BindEventStudentCreateUpdateDto dto)
    {
        var bind = await _sgRepository.GetBindEventStudentByIdAsync(id);
        if (bind == null) return false;

        _mapper.Map(dto, bind);
        _sgRepository.UpdateBindEventStudent(bind);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentFromEventAsync(Guid id)
    {
        var bind = await _sgRepository.GetBindEventStudentByIdAsync(id);
        if (bind == null) return false;

        _sgRepository.DeleteBindEventStudent(bind);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<StudentSgDto>> GetStudentsInSgAsync(Guid subDivisionId, Guid? facultyId, string? search)
    {
        var members = await _sgRepository.GetStudentsInSgAsync(subDivisionId, facultyId, search);
        return _mapper.Map<List<StudentSgDto>>(members);
    }

    public async Task<bool> AddStudentToSubDivisionAsync(MemberSgCreateUpdateDto dto)
    {
        var member = _mapper.Map<MembersOfSg>(dto);
        member.IdMembersOfSg = Guid.NewGuid();
        member.Avail = true;

        await _sgRepository.AddMemberSgAsync(member);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMemberSgAsync(Guid id, MemberSgCreateUpdateDto dto)
    {
        var member = await _sgRepository.GetMemberSgByIdAsync(id);
        if (member == null) return false;

        _mapper.Map(dto, member);
        _sgRepository.UpdateMemberSg(member);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMemberSgAsync(Guid id)
    {
        var member = await _sgRepository.GetMemberSgByIdAsync(id);
        if (member == null) return false;

        _sgRepository.DeleteMemberSg(member);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<InventorySgDto>> GetInventoryAsync(string? search)
    {
        var inventory = await _sgRepository.GetInventoryAsync(search);
        return _mapper.Map<List<InventorySgDto>>(inventory);
    }

    public async Task<bool> AddInventoryItemAsync(InventorySgCreateUpdateDto dto)
    {
        var item = _mapper.Map<InventorySg>(dto);
        item.IdInventory = Guid.NewGuid();
        item.Avail = true;

        await _sgRepository.AddInventoryAsync(item);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateInventoryItemAsync(Guid id, InventorySgCreateUpdateDto dto)
    {
        var item = await _sgRepository.GetInventoryByIdAsync(id);
        if (item == null) return false;

        _mapper.Map(dto, item);
        _sgRepository.UpdateInventory(item);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInventoryItemAsync(Guid id)
    {
        var item = await _sgRepository.GetInventoryByIdAsync(id);
        if (item == null) return false;

        _sgRepository.DeleteInventory(item);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<AccountingJournalDto>> GetJournalEntriesAsync(Guid inventoryId, string? search)
    {
        var entries = await _sgRepository.GetJournalEntriesAsync(inventoryId, search);
        return _mapper.Map<List<AccountingJournalDto>>(entries);
    }

    public async Task<bool> AddJournalEntryAsync(AccountingJournalCreateUpdateDto dto)
    {
        // Validation: StartDate not earlier than creation time
        if (dto.StartDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Exception("StartDate cannot be earlier than current date.");
        }

        // Validation: Overlap
        if (await _sgRepository.HasOverlapAsync(dto.InventorySgid, dto.StartDate, dto.EndDate))
        {
            throw new Exception("The range between StartDate and EndDate overlaps with another record.");
        }

        var entry = _mapper.Map<AccountingJournal>(dto);
        entry.IdAccountingJournal = Guid.NewGuid();
        entry.IsBack = false;

        await _sgRepository.AddJournalEntryAsync(entry);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateJournalEntryAsync(Guid id, AccountingJournalCreateUpdateDto dto)
    {
        var entry = await _sgRepository.GetJournalEntryByIdAsync(id);
        if (entry == null) return false;

        // Validation: Overlap
        if (await _sgRepository.HasOverlapAsync(dto.InventorySgid, dto.StartDate, dto.EndDate, id))
        {
            throw new Exception("The range between StartDate and EndDate overlaps with another record.");
        }

        _mapper.Map(dto, entry);
        _sgRepository.UpdateJournalEntry(entry);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteJournalEntryAsync(Guid id)
    {
        var entry = await _sgRepository.GetJournalEntryByIdAsync(id);
        if (entry == null) return false;

        _sgRepository.DeleteJournalEntry(entry);
        await _sgRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConfirmReturnAsync(Guid id, Guid currentUserId)
    {
        var entry = await _sgRepository.GetJournalEntryByIdAsync(id);
        if (entry == null) return false;

        // Validation: Only Watchman can confirm
        // I need to check if currentUserId belongs to the student who is the Watchman of this inventory item.
        // I'll check it via the InventorySg relation.
        if (entry.InventorySg.Watchman.UserId != currentUserId)
        {
            throw new Exception("Only the watchman of this inventory item can confirm the return.");
        }

        entry.IsBack = true;
        entry.RealBackTime = DateOnly.FromDateTime(DateTime.Now);
        
        _sgRepository.UpdateJournalEntry(entry);
        await _sgRepository.SaveChangesAsync();
        return true;
    }
}
