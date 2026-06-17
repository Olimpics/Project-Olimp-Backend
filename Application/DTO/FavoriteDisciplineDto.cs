using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO
{
    public class FavoriteDisciplineDto
    {
        public Guid Id { get; set; }
        public Guid DisciplineId { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public string? DepartmentName { get; set; }
        public int CatalogYearStart { get; set; }
        public int CatalogYearEnd { get; set; }

        /// <summary>Analogous disciplines from newer catalogs (cross-catalog recommendations).</summary>
        public List<SimilarDisciplineDto> Similar { get; set; } = new();
    }

    public class AddFavoriteDisciplineDto
    {
        public Guid StudentId { get; set; }
        public Guid DisciplineId { get; set; }
    }
}
