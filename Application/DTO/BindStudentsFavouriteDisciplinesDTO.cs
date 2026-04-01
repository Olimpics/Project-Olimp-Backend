using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    /// <summary>
    /// One selected discipline in admin list (with bind id and confirmation state).
    /// </summary>
    public class StudentFavouriteDisciplineDto
    {
        public int IdBindAddDisciplines { get; set; }
        public int IdStudent { get; set; }
        public int IdAddDisciplines { get; set; }
        public string NameAddDisciplines { get; set; } = null!;
        public string CodeAddDisciplines { get; set; } = null!;
    }

    public class AddFavoriteDisciplineDto
    {
        public int StudentId { get; set; }
        public int DisciplineId { get; set; }
    }
}