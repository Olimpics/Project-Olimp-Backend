using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindStudentFavoriteDisciline
{
    public Guid IdBindStudentFavoriteDiscipline { get; set; }

    public Guid StudentId { get; set; }

    public Guid SelectiveDisciplineId { get; set; }

    public virtual SelectiveDiscipline SelectiveDiscipline { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
