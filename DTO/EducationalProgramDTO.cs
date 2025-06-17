namespace OlimpBack.DTO
{
    public class EducationalProgramDto
    {
        public int IdEducationalProgram { get; set; }
        public string NameEducationalProgram { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public int DegreeId { get; set; }
        public string Speciality { get; set; } = null!;
        public uint StudentsAmount { get; set; }
        public int StudentsCount { get; set; }
        public int DisciplinesCount { get; set; }
    }
    public class EducationalProgramFullDto
    {
        public int IdEducationalProgram { get; set; }
        public string NameEducationalProgram { get; set; } = null!;
        public int? CountAddSemestr3 { get; set; }
        public int? CountAddSemestr4 { get; set; }
        public int? CountAddSemestr5 { get; set; }
        public int? CountAddSemestr6 { get; set; }
        public int? CountAddSemestr7 { get; set; }
        public int? CountAddSemestr8 { get; set; }
        public string Degree { get; set; } = null!;
        public int DegreeId { get; set; }
        public string Speciality { get; set; } = null!;
        public sbyte Accreditation { get; set; }
        public string AccreditationType { get; set; } = null!;
        public uint StudentsAmount { get; set; }
        public int StudentsCount { get; set; }
        public int DisciplinesCount { get; set; }
    }
    public class CreateEducationalProgramDto
    {
        public string NameEducationalProgram { get; set; } = null!;
        public int? CountAddSemestr3 { get; set; }
        public int? CountAddSemestr4 { get; set; }
        public int? CountAddSemestr5 { get; set; }
        public int? CountAddSemestr6 { get; set; }
        public int? CountAddSemestr7 { get; set; }
        public int? CountAddSemestr8 { get; set; }
        public string Degree { get; set; } = null!;
        public string Speciality { get; set; } = null!;
        public sbyte Accreditation { get; set; }
        public string AccreditationType { get; set; } = null!;
        public uint StudentsAmount { get; set; }
    }

    public class UpdateEducationalProgramDto : CreateEducationalProgramDto
    {
        public int IdEducationalProgram { get; set; }
    }

}
