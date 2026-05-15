using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class AdminsPersonal
{
    public string NameAdmin { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid AcademicDegreeId { get; set; }

    public bool Avail { get; set; }

    public Guid IdAdmins { get; set; }

    public List<Guid>? SubDegreesId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid FacultyId { get; set; }

    public virtual AcademicDegree AcademicDegree { get; set; } = null!;

    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    public virtual ICollection<BindTeacherMain> BindTeacherMains { get; set; } = new List<BindTeacherMain>();

    public virtual ICollection<BindTeachersSelective> BindTeachersSelectives { get; set; } = new List<BindTeachersSelective>();

    public virtual Department Department { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual User User { get; set; } = null!;
}
