using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Student
{
    public string FirstName { get; set; } = null!;

    public DateOnly EducationStart { get; set; }

    public DateOnly EducationEnd { get; set; }

    /// <summary>
    /// Залікова книга
    /// </summary>
    public string ReportCard { get; set; } = null!;

    public string? Notes { get; set; }

    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public Guid EducationStatusId { get; set; }

    public Guid IdStudent { get; set; }

    public bool IsInSg { get; set; }

    public List<Guid>? FavId { get; set; }

    public string? EdboCode { get; set; }

    public bool IsFunded { get; set; }

    public bool Avail { get; set; }

    public string? SecondName { get; set; }

    public string? ThirdName { get; set; }

    public virtual ICollection<AccountingJournal> AccountingJournals { get; set; } = new List<AccountingJournal>();

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();

    public virtual ICollection<BindMainDiscipline> BindMainDisciplines { get; set; } = new List<BindMainDiscipline>();

    public virtual ICollection<BindRating> BindRatings { get; set; } = new List<BindRating>();

    public virtual ICollection<BindSelectiveDiscipline> BindSelectiveDisciplines { get; set; } = new List<BindSelectiveDiscipline>();

    public virtual EducationStatus EducationStatus { get; set; } = null!;

    public virtual StudentGroup Group { get; set; } = null!;

    public virtual ICollection<InventorySg> InventorySgs { get; set; } = new List<InventorySg>();

    public virtual ICollection<MembersOfSg> MembersOfSgCreatedByNavigations { get; set; } = new List<MembersOfSg>();

    public virtual ICollection<MembersOfSg> MembersOfSgStudents { get; set; } = new List<MembersOfSg>();

    public virtual User User { get; set; } = null!;
}
