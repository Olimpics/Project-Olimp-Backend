using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Specialization
{
    public int IdSpecialization { get; set; }

    /// <summary>
    /// Код спеціалізації (наприклад, 014.01)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Назва спеціалізації
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Зовнішній ключ на Спеціальність
    /// </summary>
    public int IdSpeciality { get; set; }

    /// <summary>
    /// Опис спеціалізації
    /// </summary>
    public string? Description { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Speciality IdSpecialityNavigation { get; set; } = null!;
}
