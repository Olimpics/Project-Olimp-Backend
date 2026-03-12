using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IGroupService
{
    Task<IEnumerable<GroupFilterDto>> GetGroupsAsync(GroupListQueryDto queryDto);

    Task<GroupDto?> GetGroupAsync(int id);

    Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateGroupAsync(int id, UpdateGroupDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteGroupAsync(int id);
}

