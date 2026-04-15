using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Speciality
{
    public int IdSpeciality { get; set; }

    /// <summary>
    /// Код спеціальності (наприклад, 121)
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Назва спеціальності
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Зовнішній ключ на Галузь
    /// </summary>
    public int? IdBranch { get; set; }

    /// <summary>
    /// Зовнішній ключ на Факультет
    /// </summary>
    public int? IdFaculty { get; set; }

    /// <summary>
    /// Зовнішній ключ на Кафедру
    /// </summary>
    public int? IdDepartment { get; set; }

    /// <summary>
    /// Акредитація (0 або 1)
    /// </summary>
    public bool? Accreditation { get; set; }

    /// <summary>
    /// Тип акредитації (текст зі скріншоту)
    /// </summary>
    public string? AccreditationType { get; set; }

    /// <summary>
    /// Ліцензійний обсяг (studentsAmount)
    /// </summary>
    public int? LicensedVolume { get; set; }

    /// <summary>
    /// Опис спеціальності
    /// </summary>
    public string? Description { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Branch? IdBranchNavigation { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; }

    public virtual Faculty? IdFacultyNavigation { get; set; }

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
}
