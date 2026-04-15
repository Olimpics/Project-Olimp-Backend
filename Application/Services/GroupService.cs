using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GroupFilterDto>> GetGroupsAsync(GroupListQueryDto queryDto) =>
        await _repository.GetFilteredGroupsAsync(queryDto);

    public async Task<GroupDto?> GetGroupAsync(int id) =>
        await _repository.GetDtoByIdAsync(id);

    public async Task<GroupDetailsDto?> GetGroupDetailsAsync(int id) =>
        await _repository.GetDetailsByIdAsync(id);

    public async Task<IReadOnlyList<GroupStudentDto>> GetStudentsByGroupIdAsync(int groupId) =>
        await _repository.GetStudentsByGroupIdAsync(groupId);

    public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
    {
        var group = _mapper.Map<Group>(dto);
        await _repository.AddAsync(group);
        await _repository.SaveChangesAsync();

        return _mapper.Map<GroupDto>(group);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateGroupAsync(int id, UpdateGroupDto dto)
    {
        var group = await _repository.GetEntityByIdAsync(id);
        if (group == null)
            return (false, StatusCodes.Status404NotFound, "Group not found");

        _mapper.Map(dto, group);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Group not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteGroupAsync(int id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Group not found");

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<GroupCurriculumDTO?> GetGroupCurriculumAsync(int groupId) =>
        await _repository.GetCurriculumByGroupIdAsync(groupId);
}