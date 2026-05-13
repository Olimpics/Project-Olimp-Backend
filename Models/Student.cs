using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Student
{
    public int IdStudent { get; set; }

    public string? NameStudent { get; set; }

    public int EducationStatusId { get; set; }

    public DateOnly EducationStart { get; set; }

    public DateOnly EducationEnd { get; set; }

    public short IsShort { get; set; }

    public int Course { get; set; }

    public int GroupId { get; set; }

    public BitArray IsInSg { get; set; } = null!;

    public List<int>? FavId { get; set; }

    /// <summary>
    /// Залікова книга
    /// </summary>
    public string? ReportCard { get; set; }

    public BitArray? IsFunded { get; set; }

    public string? Notes { get; set; }

    public bool? Avail { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<AccountingJournal> AccountingJournals { get; set; } = new List<AccountingJournal>();

    public virtual ICollection<BindEventStudent> BindEventStudents { get; set; } = new List<BindEventStudent>();

    public virtual ICollection<BindExtraActivity> BindExtraActivities { get; set; } = new List<BindExtraActivity>();

    public virtual ICollection<BindMainDiscipline> BindMainDisciplines { get; set; } = new List<BindMainDiscipline>();

    public virtual ICollection<BindRating> BindRatings { get; set; } = new List<BindRating>();

    public virtual ICollection<BindSelectiveDiscipline> BindSelectiveDisciplines { get; set; } = new List<BindSelectiveDiscipline>();

    public virtual EducationStatus EducationStatus { get; set; } = null!;

    public virtual StudentGroup Group { get; set; } = null!;

    public virtual ICollection<InventorySg> InventorySgs { get; set; } = new List<InventorySg>();

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();

    public virtual ICollection<MembersOfSg> MembersOfSgCreatedByNavigations { get; set; } = new List<MembersOfSg>();

    public virtual ICollection<MembersOfSg> MembersOfSgStudents { get; set; } = new List<MembersOfSg>();

    public virtual User? User { get; set; }
}
