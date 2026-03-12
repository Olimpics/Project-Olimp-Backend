namespace OlimpBack.Application.DTO
{
    public class DisciplineChoicePeriodDto
    {
        public int Id { get; set; }
        public int PeriodType { get; set; }
        public int PeriodCourse { get; set; }
        public int DegreeLevelId { get; set; }

        public int isShort { get; set; }
        public int IsClose { get; set; }
        public int? FacultyId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateDisciplineChoicePeriodDto
    {
        public int PeriodType { get; set; }
        public int PeriodCourse { get; set; }
        public int DegreeLevelId { get; set; }
        public int isShort { get; set; }
        public int IsClose { get; set; }
        public int? FacultyId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateDisciplineChoicePeriodDto
    {
        public int Id { get; set; }
        public int PeriodType { get; set; }
        public int PeriodCourse { get; set; }
        public int DegreeLevelId { get; set; }
        public int isShort { get; set; }
        public int IsClose { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateDisciplineChoicePeriodAfterStartDto
    {
        public int Id { get; set; }
        public int IsClose { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class UpdateDisciplineChoicePeriodOpenOrCloseDto
    {
        public int Id { get; set; }
        public int IsClose { get; set; }
    }

    /// <summary>
    /// Query parameters for DisciplineChoicePeriod GetAll.
    /// </summary>
    public class GetDisciplineChoicePeriodsQueryDto
    {
        public int? FacultyId { get; set; }
        public int? DegreeLevelId { get; set; }
        public int isShort { get; set; }
        public int? PeriodType { get; set; }
        public int? IsClose { get; set; }
        public int? PeriodCourse { get; set; }
    }
}
