using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IGroupService
{
    Task<IEnumerable<GroupFilterDto>> GetGroupsAsync(GroupListQueryDto queryDto);

    Task<GroupDto?> GetGroupAsync(Guid id);
    Task<GroupDetailsDto?> GetGroupDetailsAsync(Guid id);
    Task<IReadOnlyList<GroupStudentDto>> GetStudentsByGroupIdAsync(Guid groupId);

    Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateGroupAsync(Guid id, UpdateGroupDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteGroupAsync(Guid id);
    Task<GroupCurriculumDTO?> GetGroupCurriculumAsync(Guid groupId);
}

