using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface ISgRepository
{
    // SubDivisions
    Task<List<MembersOfSg>> GetUserSubDivisionsAsync(Guid userId);
    
    // Events
    Task<List<Event>> GetEventsAsync(Guid subDivisionId, Guid? facultyId, Guid catalogYearId, bool isEven, string? search);
    Task<Event?> GetEventByIdAsync(Guid id);
    Task AddEventAsync(Event @event);
    void UpdateEvent(Event @event);
    void DeleteEvent(Event @event);

    // BindEventStudent
    Task<List<BindEventStudent>> GetEventStudentsAsync(Guid eventId, string? search);
    Task<BindEventStudent?> GetBindEventStudentByIdAsync(Guid id);
    Task AddBindEventStudentAsync(BindEventStudent bind);
    void UpdateBindEventStudent(BindEventStudent bind);
    void DeleteBindEventStudent(BindEventStudent bind);

    // MembersOfSg
    Task<List<MembersOfSg>> GetStudentsInSgAsync(Guid subDivisionId, Guid? facultyId, string? search);
    Task<MembersOfSg?> GetMemberSgByIdAsync(Guid id);
    Task AddMemberSgAsync(MembersOfSg member);
    void UpdateMemberSg(MembersOfSg member);
    void DeleteMemberSg(MembersOfSg member);

    // InventorySg
    Task<List<InventorySg>> GetInventoryAsync(string? search);
    Task<InventorySg?> GetInventoryByIdAsync(Guid id);
    Task AddInventoryAsync(InventorySg inventory);
    void UpdateInventory(InventorySg inventory);
    void DeleteInventory(InventorySg inventory);

    // AccountingJournal
    Task<List<AccountingJournal>> GetJournalEntriesAsync(Guid inventoryId, string? search);
    Task<AccountingJournal?> GetJournalEntryByIdAsync(Guid id);
    Task AddJournalEntryAsync(AccountingJournal journal);
    void UpdateJournalEntry(AccountingJournal journal);
    void DeleteJournalEntry(AccountingJournal journal);
    Task<bool> HasOverlapAsync(Guid inventoryId, DateOnly startDate, DateOnly endDate, Guid? excludeId = null);

    Task SaveChangesAsync();
}

public class SgRepository : ISgRepository
{
    private readonly AppDbContext _context;

    public SgRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MembersOfSg>> GetUserSubDivisionsAsync(Guid userId)
    {
        return await _context.MembersOfSgs
            .Include(m => m.BindSubdivisionRoleInSg)
                .ThenInclude(b => b.SubDivision)
            .Include(m => m.Faculty)
            .Where(m => m.Student.UserId == userId && m.Avail)
            .ToListAsync();
    }

    public async Task<List<Event>> GetEventsAsync(Guid subDivisionId, Guid? facultyId, Guid catalogYearId, bool isEven, string? search)
    {
        var query = _context.Events
            .Where(e => e.SubdivisionSgid == subDivisionId && e.CatalogYearId == catalogYearId && e.IsEven == isEven && e.Avail);

        if (facultyId.HasValue)
        {
            query = query.Where(e => e.FacultyId == facultyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.NameEvent.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(Guid id)
    {
        return await _context.Events.FirstOrDefaultAsync(e => e.IdEvent == id);
    }

    public async Task AddEventAsync(Event @event)
    {
        await _context.Events.AddAsync(@event);
    }

    public void UpdateEvent(Event @event)
    {
        _context.Events.Update(@event);
    }

    public void DeleteEvent(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public async Task<List<BindEventStudent>> GetEventStudentsAsync(Guid eventId, string? search)
    {
        var query = _context.BindEventStudents
            .Include(b => b.Student)
            .Include(b => b.Role)
            .Where(b => b.EventId == eventId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Student.FirstName.Contains(search) || b.Student.SecondName.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<BindEventStudent?> GetBindEventStudentByIdAsync(Guid id)
    {
        return await _context.BindEventStudents.FirstOrDefaultAsync(b => b.IdBindEventStudent == id);
    }

    public async Task AddBindEventStudentAsync(BindEventStudent bind)
    {
        await _context.BindEventStudents.AddAsync(bind);
    }

    public void UpdateBindEventStudent(BindEventStudent bind)
    {
        _context.BindEventStudents.Update(bind);
    }

    public void DeleteBindEventStudent(BindEventStudent bind)
    {
        _context.BindEventStudents.Remove(bind);
    }

    public async Task<List<MembersOfSg>> GetStudentsInSgAsync(Guid subDivisionId, Guid? facultyId, string? search)
    {
        var query = _context.MembersOfSgs
            .Include(m => m.Student)
                .ThenInclude(s => s.Group)
            .Include(m => m.BindSubdivisionRoleInSg)
                .ThenInclude(b => b.RoleInSg)
            .Include(m => m.Faculty)
            .Where(m => m.BindSubdivisionRoleInSg.SubDivisionId == subDivisionId && m.Avail);

        if (facultyId.HasValue)
        {
            query = query.Where(m => m.FacultyId == facultyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(m => m.Student.FirstName.Contains(search) || m.Student.SecondName.Contains(search) || m.Student.Group.GroupCode.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<MembersOfSg?> GetMemberSgByIdAsync(Guid id)
    {
        return await _context.MembersOfSgs.FirstOrDefaultAsync(m => m.IdMembersOfSg == id);
    }

    public async Task AddMemberSgAsync(MembersOfSg member)
    {
        await _context.MembersOfSgs.AddAsync(member);
    }

    public void UpdateMemberSg(MembersOfSg member)
    {
        _context.MembersOfSgs.Update(member);
    }

    public void DeleteMemberSg(MembersOfSg member)
    {
        _context.MembersOfSgs.Remove(member);
    }

    public async Task<List<InventorySg>> GetInventoryAsync(string? search)
    {
        var query = _context.InventorySgs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.InventoryName.Contains(search) || i.InventoryCode.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<InventorySg?> GetInventoryByIdAsync(Guid id)
    {
        return await _context.InventorySgs.FirstOrDefaultAsync(i => i.IdInventory == id);
    }

    public async Task AddInventoryAsync(InventorySg inventory)
    {
        await _context.InventorySgs.AddAsync(inventory);
    }

    public void UpdateInventory(InventorySg inventory)
    {
        _context.InventorySgs.Update(inventory);
    }

    public void DeleteInventory(InventorySg inventory)
    {
        _context.InventorySgs.Remove(inventory);
    }

    public async Task<List<AccountingJournal>> GetJournalEntriesAsync(Guid inventoryId, string? search)
    {
        var query = _context.AccountingJournals
            .Include(j => j.Student)
            .Where(j => j.InventorySgid == inventoryId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j => j.Student.FirstName.Contains(search) || j.Student.SecondName.Contains(search) || j.Comment.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<AccountingJournal?> GetJournalEntryByIdAsync(Guid id)
    {
        return await _context.AccountingJournals
            .Include(j => j.InventorySg)
                .ThenInclude(i => i.Watchman)
            .FirstOrDefaultAsync(j => j.IdAccountingJournal == id);
    }

    public async Task AddJournalEntryAsync(AccountingJournal journal)
    {
        await _context.AccountingJournals.AddAsync(journal);
    }

    public void UpdateJournalEntry(AccountingJournal journal)
    {
        _context.AccountingJournals.Update(journal);
    }

    public void DeleteJournalEntry(AccountingJournal journal)
    {
        _context.AccountingJournals.Remove(journal);
    }

    public async Task<bool> HasOverlapAsync(Guid inventoryId, DateOnly startDate, DateOnly endDate, Guid? excludeId = null)
    {
        var query = _context.AccountingJournals
            .Where(j => j.InventorySgid == inventoryId && !j.IsBack);

        if (excludeId.HasValue)
        {
            query = query.Where(j => j.IdAccountingJournal != excludeId.Value);
        }

        return await query.AnyAsync(j => (startDate <= j.EndDate && endDate >= j.StartDate));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
