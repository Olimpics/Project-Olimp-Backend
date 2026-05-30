using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IUserHierarchyAssignmentService
{
    Task<IReadOnlyList<UserHierarchyAssignmentDto>> GetByUserIdAsync(Guid userId);
    Task<UserHierarchyAssignmentDto?> GetByIdAsync(Guid idAssignment);
    Task<(UserHierarchyAssignmentDto? dto, int? statusCode, string? errorMessage)> CreateAsync(
        Guid granterUserId,
        CreateUserHierarchyAssignmentDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(
        Guid granterUserId,
        Guid idAssignment,
        UpdateUserHierarchyAssignmentDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(
        Guid granterUserId,
        Guid idAssignment);
}

public class UserHierarchyAssignmentService : IUserHierarchyAssignmentService
{
    private readonly AppDbContext _context;
    private readonly IHierarchyAuthorizationService _hierarchyAuthorization;

    public UserHierarchyAssignmentService(
        AppDbContext context,
        IHierarchyAuthorizationService hierarchyAuthorization)
    {
        _context = context;
        _hierarchyAuthorization = hierarchyAuthorization;
    }

    public async Task<IReadOnlyList<UserHierarchyAssignmentDto>> GetByUserIdAsync(Guid userId)
    {
        return await GetQuery()
            .Where(assignment => assignment.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserHierarchyAssignmentDto?> GetByIdAsync(Guid idAssignment)
    {
        return await GetQuery()
            .FirstOrDefaultAsync(assignment => assignment.IdAssignment == idAssignment);
    }

    public async Task<(UserHierarchyAssignmentDto? dto, int? statusCode, string? errorMessage)> CreateAsync(
        Guid granterUserId,
        CreateUserHierarchyAssignmentDto dto)
    {
        var validation = await ValidateReferencesAsync(
            dto.UserId,
            dto.HierarchyNodeId,
            dto.FacultyId,
            dto.DepartmentId,
            dto.GroupId);

        if (validation != null)
            return (null, validation.Value.statusCode, validation.Value.errorMessage);

        if (!await _hierarchyAuthorization.CanAssignHierarchyNodeAsync(
                granterUserId,
                dto.HierarchyNodeId,
                dto.FacultyId,
                dto.DepartmentId,
                dto.GroupId))
        {
            return (null, StatusCodes.Status403Forbidden,
                "You cannot assign this hierarchy node in the selected scope.");
        }

        if (await AssignmentExistsAsync(dto.UserId, dto.HierarchyNodeId, dto.FacultyId, dto.DepartmentId, dto.GroupId))
        {
            return (null, StatusCodes.Status400BadRequest,
                "This hierarchy assignment already exists for the selected scope.");
        }

        var entity = new UserHierarchyAssignment
        {
            IdAssignment = Guid.NewGuid(),
            UserId = dto.UserId,
            HierarchyNodeId = dto.HierarchyNodeId,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId,
            GroupId = dto.GroupId,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.UserHierarchyAssignments.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.IdAssignment), null, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(
        Guid granterUserId,
        Guid idAssignment,
        UpdateUserHierarchyAssignmentDto dto)
    {
        var entity = await _context.UserHierarchyAssignments
            .FirstOrDefaultAsync(assignment => assignment.IdAssignment == idAssignment);

        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Hierarchy assignment not found");

        var validation = await ValidateReferencesAsync(
            entity.UserId,
            dto.HierarchyNodeId,
            dto.FacultyId,
            dto.DepartmentId,
            dto.GroupId);

        if (validation != null)
            return (false, validation.Value.statusCode, validation.Value.errorMessage);

        if (!await _hierarchyAuthorization.CanAssignHierarchyNodeAsync(
                granterUserId,
                dto.HierarchyNodeId,
                dto.FacultyId,
                dto.DepartmentId,
                dto.GroupId))
        {
            return (false, StatusCodes.Status403Forbidden,
                "You cannot assign this hierarchy node in the selected scope.");
        }

        if (await AssignmentExistsAsync(
                entity.UserId,
                dto.HierarchyNodeId,
                dto.FacultyId,
                dto.DepartmentId,
                dto.GroupId,
                idAssignment))
        {
            return (false, StatusCodes.Status400BadRequest,
                "This hierarchy assignment already exists for the selected scope.");
        }

        entity.HierarchyNodeId = dto.HierarchyNodeId;
        entity.FacultyId = dto.FacultyId;
        entity.DepartmentId = dto.DepartmentId;
        entity.GroupId = dto.GroupId;

        await _context.SaveChangesAsync();
        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(
        Guid granterUserId,
        Guid idAssignment)
    {
        var entity = await _context.UserHierarchyAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(assignment => assignment.IdAssignment == idAssignment);

        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Hierarchy assignment not found");

        if (!await _hierarchyAuthorization.CanAssignHierarchyNodeAsync(
                granterUserId,
                entity.HierarchyNodeId,
                entity.FacultyId,
                entity.DepartmentId,
                entity.GroupId))
        {
            return (false, StatusCodes.Status403Forbidden,
                "You cannot remove this hierarchy assignment.");
        }

        _context.UserHierarchyAssignments.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, StatusCodes.Status204NoContent, null);
    }

    private IQueryable<UserHierarchyAssignmentDto> GetQuery()
    {
        return _context.UserHierarchyAssignments
            .AsNoTracking()
            .Select(assignment => new UserHierarchyAssignmentDto
            {
                IdAssignment = assignment.IdAssignment,
                UserId = assignment.UserId,
                UserEmail = assignment.User.Email,
                HierarchyNodeId = assignment.HierarchyNodeId,
                HierarchyNodeName = assignment.HierarchyNode.Name,
                ManagementWeight = assignment.HierarchyNode.ManagementWeight,
                FacultyId = assignment.FacultyId,
                DepartmentId = assignment.DepartmentId,
                GroupId = assignment.GroupId,
                CreatedAt = assignment.CreatedAt
            });
    }

    private async Task<(int statusCode, string errorMessage)?> ValidateReferencesAsync(
        Guid userId,
        Guid hierarchyNodeId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId)
    {
        if (!await _context.Users.AsNoTracking().AnyAsync(user => user.IdUser == userId))
            return (StatusCodes.Status400BadRequest, "User not found");

        if (!await _context.HierarchyNodes.AsNoTracking().AnyAsync(node => node.IdNode == hierarchyNodeId))
            return (StatusCodes.Status400BadRequest, "Hierarchy node not found");

        if (facultyId.HasValue &&
            !await _context.Faculties.AsNoTracking().AnyAsync(faculty => faculty.IdFaculty == facultyId.Value))
        {
            return (StatusCodes.Status400BadRequest, "Faculty not found");
        }

        if (departmentId.HasValue)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .Where(item => item.IdDepartment == departmentId.Value)
                .Select(item => new { item.FacultyId })
                .FirstOrDefaultAsync();

            if (department == null)
                return (StatusCodes.Status400BadRequest, "Department not found");

            if (facultyId.HasValue && department.FacultyId != facultyId.Value)
                return (StatusCodes.Status400BadRequest, "Department does not belong to the selected faculty");
        }

        if (groupId.HasValue)
        {
            var group = await _context.StudentGroups
                .AsNoTracking()
                .Where(item => item.IdGroup == groupId.Value)
                .Select(item => new
                {
                    DepartmentId = item.EducationalProgram.Speciality.DepartmentId,
                    FacultyId = item.EducationalProgram.Speciality.Department.FacultyId
                })
                .FirstOrDefaultAsync();

            if (group == null)
                return (StatusCodes.Status400BadRequest, "Group not found");

            if (departmentId.HasValue && group.DepartmentId != departmentId.Value)
                return (StatusCodes.Status400BadRequest, "Group does not belong to the selected department");

            if (facultyId.HasValue && group.FacultyId != facultyId.Value)
                return (StatusCodes.Status400BadRequest, "Group does not belong to the selected faculty");
        }

        return null;
    }

    private async Task<bool> AssignmentExistsAsync(
        Guid userId,
        Guid hierarchyNodeId,
        Guid? facultyId,
        Guid? departmentId,
        Guid? groupId,
        Guid? excludeIdAssignment = null)
    {
        var query = _context.UserHierarchyAssignments
            .AsNoTracking()
            .Where(assignment =>
                assignment.UserId == userId &&
                assignment.HierarchyNodeId == hierarchyNodeId &&
                assignment.FacultyId == facultyId &&
                assignment.DepartmentId == departmentId &&
                assignment.GroupId == groupId);

        if (excludeIdAssignment.HasValue)
            query = query.Where(assignment => assignment.IdAssignment != excludeIdAssignment.Value);

        return await query.AnyAsync();
    }
}
