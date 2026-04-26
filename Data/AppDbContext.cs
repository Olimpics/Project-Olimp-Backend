using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;

namespace OlimpBack.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adddetail> Adddetails { get; set; }

    public virtual DbSet<Adddiscipline> Adddisciplines { get; set; }

    public virtual DbSet<Adminlog> Adminlogs { get; set; }

    public virtual DbSet<Adminspersonal> Adminspersonals { get; set; }

    public virtual DbSet<Bindadddiscipline> Bindadddisciplines { get; set; }

    public virtual DbSet<Bindevent> Bindevents { get; set; }

    public virtual DbSet<Bindextraactivity> Bindextraactivities { get; set; }

    public virtual DbSet<Bindloansmain> Bindloansmains { get; set; }

    public virtual DbSet<Bindrolepermission> Bindrolepermissions { get; set; }

    public virtual DbSet<Bindstudentsfavouritediscipline> Bindstudentsfavouritedisciplines { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Catalogyear> Catalogyears { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Disciplinechoiceperiod> Disciplinechoiceperiods { get; set; }

    public virtual DbSet<Educationaldegree> Educationaldegrees { get; set; }

    public virtual DbSet<Educationalprogram> Educationalprograms { get; set; }

    public virtual DbSet<Educationstatus> Educationstatuses { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Maindiscipline> Maindisciplines { get; set; }

    public virtual DbSet<Maingrade> Maingrades { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Normative> Normatives { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Notificationtemplate> Notificationtemplates { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Prerequisite> Prerequisites { get; set; }

    public virtual DbSet<Regulationonaddpoint> Regulationonaddpoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Role1> Roles1 { get; set; }

    public virtual DbSet<Rolesinsg> Rolesinsgs { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Studyform> Studyforms { get; set; }

    public virtual DbSet<Subdivisionssg> Subdivisionssgs { get; set; }

    public virtual DbSet<Typeofdiscipline> Typeofdisciplines { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=project_olymp_db;Username=postgres;Password=B25824DCABCB88B5;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adddetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("adddetails");

            entity.Property(e => e.DisciplineTopics).HasMaxLength(1024);
            entity.Property(e => e.IdAddDetails).HasColumnName("idAddDetails");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Prerequisites).HasMaxLength(50);
            entity.Property(e => e.Provision).HasMaxLength(256);
            entity.Property(e => e.Recomend).HasMaxLength(64);
            entity.Property(e => e.ResultEducation).HasMaxLength(64);
            entity.Property(e => e.Teachers).HasMaxLength(50);
            entity.Property(e => e.TypeOfControll).HasMaxLength(50);
            entity.Property(e => e.TypesOfTraining).HasMaxLength(50);
            entity.Property(e => e.UsingIrl)
                .HasMaxLength(50)
                .HasColumnName("UsingIRL");
            entity.Property(e => e.WhyInterestingDetermination).HasMaxLength(50);
        });

        modelBuilder.Entity<Adddiscipline>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("adddisciplines");

            entity.Property(e => e.CodeAddDisciplines)
                .HasMaxLength(50)
                .HasColumnName("codeAddDisciplines");
            entity.Property(e => e.IdAddDisciplines).HasColumnName("idAddDisciplines");
            entity.Property(e => e.IdCatalog).HasColumnName("idCatalog");
            entity.Property(e => e.MaxCountPeople).HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse).HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople).HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse).HasColumnName("minCourse");
            entity.Property(e => e.NameAddDisciplines)
                .HasMaxLength(300)
                .HasColumnName("nameAddDisciplines");
        });

        modelBuilder.Entity<Adminlog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("adminlogs");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ChangeTime).HasMaxLength(50);
            entity.Property(e => e.NewData).HasMaxLength(256);
            entity.Property(e => e.OldData).HasMaxLength(256);
            entity.Property(e => e.TableName).HasMaxLength(50);
        });

        modelBuilder.Entity<Adminspersonal>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("adminspersonal");

            entity.Property(e => e.IdAdmins).HasColumnName("idAdmins");
            entity.Property(e => e.NameAdmin)
                .HasMaxLength(50)
                .HasColumnName("nameAdmin");
            entity.Property(e => e.Photo).HasMaxLength(50);
        });

        modelBuilder.Entity<Bindadddiscipline>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindadddisciplines");

            entity.Property(e => e.CreatedAt).HasMaxLength(50);
            entity.Property(e => e.Grade).HasMaxLength(50);
            entity.Property(e => e.IdBindAddDisciplines).HasColumnName("idBindAddDisciplines");
            entity.Property(e => e.InProcess).HasColumnName("inProcess");
        });

        modelBuilder.Entity<Bindevent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindevents");

            entity.Property(e => e.EventId)
                .HasMaxLength(50)
                .HasColumnName("EventID");
            entity.Property(e => e.IdBindEvent)
                .HasMaxLength(50)
                .HasColumnName("idBindEvent");
            entity.Property(e => e.Points)
                .HasMaxLength(50)
                .HasColumnName("points");
            entity.Property(e => e.StudentId).HasMaxLength(50);
        });

        modelBuilder.Entity<Bindextraactivity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindextraactivity");

            entity.Property(e => e.IdBindExtraActivity)
                .HasMaxLength(50)
                .HasColumnName("idBindExtraActivity");
            entity.Property(e => e.Points)
                .HasMaxLength(50)
                .HasColumnName("points");
            entity.Property(e => e.RefulationId).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasMaxLength(50);
        });

        modelBuilder.Entity<Bindloansmain>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindloansmain");

            entity.Property(e => e.IdBindLoan).HasColumnName("idBindLoan");
        });

        modelBuilder.Entity<Bindrolepermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindrolepermission");

            entity.Property(e => e.IdBindRolePermission).HasColumnName("idBindRolePermission");
        });

        modelBuilder.Entity<Bindstudentsfavouritediscipline>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bindstudentsfavouritedisciplines");

            entity.Property(e => e.IdAddDiscipline).HasColumnName("idAddDiscipline");
            entity.Property(e => e.IdBindStudentsFavouriteDisciplines).HasColumnName("idBindStudentsFavouriteDisciplines");
            entity.Property(e => e.IdStudent).HasColumnName("idStudent");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("branch");

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.IdBranch).HasColumnName("idBranch");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Catalogyear>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("catalogyears");

            entity.Property(e => e.IdCatalogYear).HasColumnName("idCatalogYear");
            entity.Property(e => e.IsFormed).HasColumnName("isFormed");
            entity.Property(e => e.NameCatalog).HasMaxLength(50);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("department");

            entity.Property(e => e.Abbreviation).HasMaxLength(50);
            entity.Property(e => e.AdminId).HasMaxLength(50);
            entity.Property(e => e.IdDepartment).HasColumnName("idDepartment");
            entity.Property(e => e.Metadata).HasMaxLength(50);
            entity.Property(e => e.NameDepartment)
                .HasMaxLength(64)
                .HasColumnName("nameDepartment");
        });

        modelBuilder.Entity<Disciplinechoiceperiod>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("disciplinechoiceperiod");

            entity.Property(e => e.EndDate).HasMaxLength(50);
            entity.Property(e => e.IdDisciplineChoicePeriod).HasColumnName("idDisciplineChoicePeriod");
            entity.Property(e => e.StartDate).HasMaxLength(50);
        });

        modelBuilder.Entity<Educationaldegree>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("educationaldegree");

            entity.Property(e => e.IdEducationalDegree).HasColumnName("idEducationalDegree");
            entity.Property(e => e.NameEducationalDegreec)
                .HasMaxLength(50)
                .HasColumnName("nameEducationalDegreec");
        });

        modelBuilder.Entity<Educationalprogram>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("educationalprogram");

            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(50)
                .HasColumnName("accreditationType");
            entity.Property(e => e.CountAddSemestr3).HasColumnName("countAddSemestr3");
            entity.Property(e => e.CountAddSemestr4).HasColumnName("countAddSemestr4");
            entity.Property(e => e.CountAddSemestr5).HasColumnName("countAddSemestr5");
            entity.Property(e => e.CountAddSemestr6).HasColumnName("countAddSemestr6");
            entity.Property(e => e.CountAddSemestr7).HasColumnName("countAddSemestr7");
            entity.Property(e => e.CountAddSemestr8).HasColumnName("countAddSemestr8");
            entity.Property(e => e.DegreeId).HasColumnName("degreeId");
            entity.Property(e => e.IdEducationalProgram).HasColumnName("idEducationalProgram");
            entity.Property(e => e.NameEducationalProgram)
                .HasMaxLength(128)
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.Speciality)
                .HasMaxLength(128)
                .HasColumnName("speciality");
            entity.Property(e => e.SpecialityCode)
                .HasColumnType("character varying")
                .HasColumnName("specialityCode");
            entity.Property(e => e.StudentsAmount).HasColumnName("studentsAmount");
        });

        modelBuilder.Entity<Educationstatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("educationstatus");

            entity.Property(e => e.IdEducationStatus).HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(50)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("events");

            entity.Property(e => e.AmountPeople).HasMaxLength(50);
            entity.Property(e => e.Date).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.IdEvent)
                .HasMaxLength(50)
                .HasColumnName("idEvent");
            entity.Property(e => e.LinkToRegistration).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.NameEvent)
                .HasMaxLength(50)
                .HasColumnName("nameEvent");
            entity.Property(e => e.RegulationId).HasMaxLength(50);
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("faculties");

            entity.Property(e => e.Abbreviation).HasMaxLength(50);
            entity.Property(e => e.IdFaculty).HasColumnName("idFaculty");
            entity.Property(e => e.Metadata).HasMaxLength(50);
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(64)
                .HasColumnName("nameFaculty");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Group");

            entity.Property(e => e.GroupCode).HasMaxLength(45);
            entity.Property(e => e.IdEducationalProgram).HasColumnName("idEducationalProgram");
            entity.Property(e => e.IdGroup).HasColumnName("idGroup");
            entity.Property(e => e.IdSpeciality).HasColumnName("idSpeciality");
            entity.Property(e => e.IdSpecialization).HasColumnName("idSpecialization");
            entity.Property(e => e.IdStudyForm).HasColumnName("idStudyForm");
        });

        modelBuilder.Entity<Maindiscipline>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("maindisciplines");

            entity.Property(e => e.CodeMainDisciplines)
                .HasMaxLength(50)
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.FormControll).HasMaxLength(50);
            entity.Property(e => e.IdBindMainDisciplines).HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.Idcataloog).HasColumnName("idcataloog");
            entity.Property(e => e.NameBindMainDisciplines)
                .HasMaxLength(50)
                .HasColumnName("nameBindMainDisciplines");
            entity.Property(e => e.Teachers).HasMaxLength(50);
        });

        modelBuilder.Entity<Maingrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("maingrade");

            entity.Property(e => e.IdMainGrade)
                .HasMaxLength(50)
                .HasColumnName("idMainGrade");
            entity.Property(e => e.MainDisciplinesId).HasMaxLength(50);
            entity.Property(e => e.MainGrade1)
                .HasMaxLength(50)
                .HasColumnName("MainGrade");
            entity.Property(e => e.StudentId).HasMaxLength(50);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("members");

            entity.Property(e => e.IdMember)
                .HasMaxLength(50)
                .HasColumnName("idMember");
            entity.Property(e => e.RoleInSgid)
                .HasMaxLength(50)
                .HasColumnName("RoleInSGID");
            entity.Property(e => e.StudentId).HasMaxLength(50);
            entity.Property(e => e.SubDivisionId).HasMaxLength(50);
        });

        modelBuilder.Entity<Normative>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("normative");

            entity.Property(e => e.IdNormative).HasColumnName("idNormative");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("notifications");

            entity.Property(e => e.CreatedAt).HasMaxLength(50);
            entity.Property(e => e.CustomMessage).HasMaxLength(128);
            entity.Property(e => e.IdNotification).HasColumnName("idNotification");
            entity.Property(e => e.Metadata).HasColumnType("jsonb");
        });

        modelBuilder.Entity<Notificationtemplate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("notificationtemplates");

            entity.Property(e => e.IdNotificationTemplates).HasColumnName("idNotificationTemplates");
            entity.Property(e => e.Message).HasMaxLength(128);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Code, "permissions_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BitIndex).HasColumnName("bit_index");
            entity.Property(e => e.Code).HasColumnName("code");
        });

        modelBuilder.Entity<Prerequisite>(entity =>
        {
            entity.HasKey(e => e.Idprerequisites).HasName("prerequisites_pk");

            entity.ToTable("prerequisites");

            entity.Property(e => e.Idprerequisites)
                .ValueGeneratedNever()
                .HasColumnName("idprerequisites");
            entity.Property(e => e.Adddisciplensid).HasColumnName("adddisciplensid");
            entity.Property(e => e.Educationalprogramid).HasColumnName("educationalprogramid");
        });

        modelBuilder.Entity<Regulationonaddpoint>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("regulationonaddpoints");

            entity.Property(e => e.AmountMax)
                .HasMaxLength(50)
                .HasColumnName("amountMax");
            entity.Property(e => e.AmountMin)
                .HasMaxLength(50)
                .HasColumnName("amountMin");
            entity.Property(e => e.CodeRegulationOnAddPoints)
                .HasMaxLength(50)
                .HasColumnName("codeRegulationOnAddPoints");
            entity.Property(e => e.IdRegulationOnAddPoints)
                .HasMaxLength(50)
                .HasColumnName("idRegulationOnAddPoints");
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.TypeOfActivitys)
                .HasMaxLength(50)
                .HasColumnName("typeOfActivitys");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Role");

            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.IdRole).HasColumnName("idRole");
            entity.Property(e => e.NameRole)
                .HasMaxLength(45)
                .HasColumnName("nameRole");
        });

        modelBuilder.Entity<Role1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ParentRoleId).HasColumnName("parent_role_id");
            entity.Property(e => e.PermissionsMask)
                .HasDefaultValue(0L)
                .HasColumnName("permissions_mask");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("role_permissions_permission_id_fkey"),
                    l => l.HasOne<Role1>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("role_permissions_role_id_fkey"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("role_permissions_pkey");
                        j.ToTable("role_permissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Rolesinsg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("rolesinsg");

            entity.Property(e => e.IdRoleSg)
                .HasMaxLength(50)
                .HasColumnName("idRoleSG");
            entity.Property(e => e.NameRole)
                .HasMaxLength(50)
                .HasColumnName("nameRole");
            entity.Property(e => e.Points)
                .HasMaxLength(50)
                .HasColumnName("points");
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("speciality");

            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(50)
                .HasColumnName("accreditationType");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IdBranch).HasColumnName("idBranch");
            entity.Property(e => e.IdDepartment)
                .HasMaxLength(50)
                .HasColumnName("idDepartment");
            entity.Property(e => e.IdFaculty)
                .HasMaxLength(50)
                .HasColumnName("idFaculty");
            entity.Property(e => e.IdSpeciality).HasColumnName("idSpeciality");
            entity.Property(e => e.LicensedVolume).HasColumnName("licensedVolume");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("specialization");

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IdSpeciality).HasColumnName("idSpeciality");
            entity.Property(e => e.IdSpecialization).HasColumnName("idSpecialization");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("student");

            entity.Property(e => e.IdStudent).HasColumnName("idStudent");
            entity.Property(e => e.IsInSg).HasColumnName("IsInSG");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");
        });

        modelBuilder.Entity<Studyform>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("studyform");

            entity.Property(e => e.IdStudyForm).HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasMaxLength(50)
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<Subdivisionssg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("subdivisionssg");

            entity.Property(e => e.IdSubDivision)
                .HasMaxLength(50)
                .HasColumnName("idSubDivision");
            entity.Property(e => e.NameDivision).HasMaxLength(50);
        });

        modelBuilder.Entity<Typeofdiscipline>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("typeofdiscipline");

            entity.Property(e => e.IdTypeOfDiscipline).HasColumnName("idTypeOfDiscipline");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pk");

            entity.ToTable("users");

            entity.HasIndex(e => e.IdUser, "users_idusers_idx");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.Isfirstlogin).HasColumnName("isfirstlogin");
            entity.Property(e => e.Lastloginat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastloginat");
            entity.Property(e => e.Passwordchangedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("passwordchangedat");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Passwordsalt).HasColumnName("passwordsalt");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role1>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_role_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
