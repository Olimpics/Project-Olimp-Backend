namespace OlimpBack.DTO
{
    public class DisciplineChoicePeriodDto
    {
        public int Id { get; set; }
        public int PeriodType { get; set; }
        public int LevelType { get; set; }
        public int? FacultyId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateDisciplineChoicePeriodDto
    {
        public int PeriodType { get; set; }
        public int LevelType { get; set; }
        public int? FacultyId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateDisciplineChoicePeriodDto
    {
        public int Id { get; set; }
        public int PeriodType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
