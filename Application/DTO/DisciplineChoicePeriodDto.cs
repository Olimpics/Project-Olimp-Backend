using System;

namespace OlimpBack.Application.DTO
{
    public class DisciplineChoicePeriodDto
    {
        public Guid Id { get; set; }
        public sbyte PeriodType { get; set; }
        public sbyte PeriodCourse { get; set; }
        public Guid DegreeLevelId { get; set; }
        public bool isShort { get; set; }
        public bool IsClose { get; set; }
        public Guid? FacultyId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateDisciplineChoicePeriodDto
    {
        public sbyte PeriodType { get; set; }
        public sbyte PeriodCourse { get; set; }
        public Guid DegreeLevelId { get; set; }
        public bool isShort { get; set; }
        public bool IsClose { get; set; }
        public Guid? FacultyId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateDisciplineChoicePeriodDto
    {
        public Guid Id { get; set; }
        public sbyte PeriodType { get; set; }
        public sbyte PeriodCourse { get; set; }
        public Guid DegreeLevelId { get; set; }
        public bool isShort { get; set; }
        public bool IsClose { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateDisciplineChoicePeriodAfterStartDto
    {
        public Guid Id { get; set; }
        public bool IsClose { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class UpdateDisciplineChoicePeriodOpenOrCloseDto
    {
        public Guid Id { get; set; }
        public bool IsClose { get; set; }
    }

    /// <summary>
    /// Query parameters for DisciplineChoicePeriod GetAll.
    /// </summary>
    public class GetDisciplineChoicePeriodsQueryDto
    {
        public Guid? FacultyId { get; set; }
        public Guid? DegreeLevelId { get; set; }
        public bool isShort { get; set; }
        public sbyte? PeriodType { get; set; }
        public bool? IsClose { get; set; }
        public sbyte? PeriodCourse { get; set; }
    }
}
