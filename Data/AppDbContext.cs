using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

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

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<AddDetail> AddDetails { get; set; }

    public virtual DbSet<AddDiscipline> AddDisciplines { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<AdminsPersonal> AdminsPersonals { get; set; }

    public virtual DbSet<BindAddDiscipline> BindAddDisciplines { get; set; }

    public virtual DbSet<BindEvent> BindEvents { get; set; }

    public virtual DbSet<BindExtraActivity> BindExtraActivities { get; set; }

    public virtual DbSet<BindLoansMain> BindLoansMains { get; set; }

    public virtual DbSet<BindMainDiscipline> BindMainDisciplines { get; set; }

    public virtual DbSet<BindRolePermission> BindRolePermissions { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; }

    public virtual DbSet<EducationStatus> EducationStatuses { get; set; }

    public virtual DbSet<EducationalDegree> EducationalDegrees { get; set; }

    public virtual DbSet<EducationalProgram> EducationalPrograms { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<MainGrade> MainGrades { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Normative> Normatives { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RegulationOnAddPoint> RegulationOnAddPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesInSg> RolesInSgs { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudyForm> StudyForms { get; set; }

    public virtual DbSet<SubDivisionsSg> SubDivisionsSgs { get; set; }

    public virtual DbSet<TypeOfDiscipline> TypeOfDisciplines { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("host=127.0.0.1;port=3307;database=DNUProjectDb;username=user_dnupr;password=B25824DCABCB88B5", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.11-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.IdAchievement).HasName("PRIMARY");

            entity
                .ToTable("Achievement")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdAchievement)
                .HasColumnType("int(11)")
                .HasColumnName("idAchievement");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Photo).HasColumnType("blob");
        });

        modelBuilder.Entity<AddDetail>(entity =>
        {
            entity.HasKey(e => e.IdAddDetails).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.DepartmentId, "AddDetails_Departments");

            entity.HasIndex(e => e.IdAddDetails, "idAddDeteils_UNIQUE").IsUnique();

            entity.Property(e => e.IdAddDetails)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("idAddDetails");
            entity.Property(e => e.AdditionaLiterature).HasMaxLength(800);
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.Determination).HasMaxLength(800);
            entity.Property(e => e.Language).HasMaxLength(200);
            entity.Property(e => e.Prerequisites).HasMaxLength(800);
            entity.Property(e => e.Recomend).HasMaxLength(800);
            entity.Property(e => e.ResultEducation).HasMaxLength(800);
            entity.Property(e => e.Teachers).HasColumnType("json");
            entity.Property(e => e.TypeOfControll).HasMaxLength(100);
            entity.Property(e => e.TypesOfTraining).HasMaxLength(200);
            entity.Property(e => e.UsingIrl)
                .HasMaxLength(800)
                .HasColumnName("UsingIRL");
            entity.Property(e => e.WhyInterestingDetermination).HasMaxLength(800);

            entity.HasOne(d => d.Department).WithMany(p => p.AddDetails)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("AddDetails_Departments");

            entity.HasOne(d => d.IdAddDetailsNavigation).WithOne(p => p.AddDetail)
                .HasForeignKey<AddDetail>(d => d.IdAddDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDeteils_AddDisciplines");
        });

        modelBuilder.Entity<AddDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdAddDisciplines).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.FacultyId, "AddDisciplines_ Faculties_idx");

            entity.HasIndex(e => e.DegreeLevelId, "AddDisciplines_EducationalDegree_idx");

            entity.HasIndex(e => e.TypeId, "AddDisciplines_Type_idx");

            entity.HasIndex(e => e.IdAddDisciplines, "idAddDisciplines_UNIQUE").IsUnique();

            entity.Property(e => e.IdAddDisciplines)
                .HasColumnType("int(11)")
                .HasColumnName("idAddDisciplines");
            entity.Property(e => e.AddSemestr).HasColumnType("tinyint(4)");
            entity.Property(e => e.CodeAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("codeAddDisciplines");
            entity.Property(e => e.DegreeLevelId).HasColumnType("int(11)");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.MaxCountPeople)
                .HasColumnType("int(11)")
                .HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse)
                .HasColumnType("int(11)")
                .HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople)
                .HasColumnType("int(11)")
                .HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse)
                .HasColumnType("int(11)")
                .HasColumnName("minCourse");
            entity.Property(e => e.NameAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("nameAddDisciplines");
            entity.Property(e => e.TypeId).HasColumnType("int(11)");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("AddDisciplines_EducationalDegree");

            entity.HasOne(d => d.Faculty).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDisciplines_ Faculties");

            entity.HasOne(d => d.Type).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("AddDisciplines_Type");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => new { e.LogId, e.ChangeTime })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "idx_AdminLogs_AdminId");

            entity.HasIndex(e => e.ChangeTime, "idx_AdminLogs_Time");

            entity.Property(e => e.LogId)
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint(20)");
            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Action).HasColumnType("enum('INSERT','UPDATE','DELETE')");
            entity.Property(e => e.AdminId)
                .HasComment("idUsers who performed action")
                .HasColumnType("int(11)");
            entity.Property(e => e.KeyValue)
                .HasMaxLength(255)
                .HasComment("Primary key of affected row");
            entity.Property(e => e.NewData).HasColumnType("json");
            entity.Property(e => e.OldData).HasColumnType("json");
            entity.Property(e => e.TableName).HasMaxLength(64);
        });

        modelBuilder.Entity<AdminsPersonal>(entity =>
        {
            entity.HasKey(e => e.IdAdmins).HasName("PRIMARY");

            entity
                .ToTable("AdminsPersonal")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.DepartmentId, "AdminsPersonal_Department_idx");

            entity.HasIndex(e => e.FacultyId, "AdminsPersonal_Faculties_idx");

            entity.HasIndex(e => e.UserId, "AdminsPersonal_Users_idx");

            entity.Property(e => e.IdAdmins)
                .HasColumnType("int(11)")
                .HasColumnName("idAdmins");
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.NameAdmin)
                .HasMaxLength(200)
                .HasColumnName("nameAdmin");
            entity.Property(e => e.Photo).HasColumnType("blob");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Department).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("AdminsPersonal_Department");

            entity.HasOne(d => d.Faculty).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("AdminsPersonal_Faculties");

            entity.HasOne(d => d.User).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminsPersonal_Users");
        });

        modelBuilder.Entity<BindAddDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindAddDisciplines).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AddDisciplinesId, "Bind_AddCourse_idx");

            entity.HasIndex(e => e.StudentId, "Bind_Student_idx");

            entity.Property(e => e.IdBindAddDisciplines)
                .HasColumnType("int(11)")
                .HasColumnName("idBindAddDisciplines");
            entity.Property(e => e.AddDisciplinesId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Grade).HasColumnType("int(11)");
            entity.Property(e => e.InProcess)
                .HasColumnType("tinyint(4)")
                .HasColumnName("inProcess");
            entity.Property(e => e.Loans)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Semestr).HasColumnType("int(11)");
            entity.Property(e => e.StudentId).HasColumnType("int(11)");

            entity.HasOne(d => d.AddDisciplines).WithMany(p => p.BindAddDisciplines)
                .HasForeignKey(d => d.AddDisciplinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_AddDisciple");

            entity.HasOne(d => d.Student).WithMany(p => p.BindAddDisciplines)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_Student");
        });

        modelBuilder.Entity<BindEvent>(entity =>
        {
            entity.HasKey(e => e.IdBindEvent).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.EventId, "EventID");

            entity.HasIndex(e => e.StudentId, "StudentId");

            entity.Property(e => e.IdBindEvent)
                .HasColumnType("int(11)")
                .HasColumnName("idBindEvent");
            entity.Property(e => e.EventId)
                .HasColumnType("int(11)")
                .HasColumnName("EventID");
            entity.Property(e => e.Points)
                .HasColumnType("int(11)")
                .HasColumnName("points");
            entity.Property(e => e.StudentId).HasColumnType("int(11)");

            entity.HasOne(d => d.Event).WithMany(p => p.BindEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindEvent_Event");

            entity.HasOne(d => d.Student).WithMany(p => p.BindEvents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindEvents_Student");
        });

        modelBuilder.Entity<BindExtraActivity>(entity =>
        {
            entity.HasKey(e => e.IdBindExtraActivity).HasName("PRIMARY");

            entity
                .ToTable("BindExtraActivity")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.RefulationId, "BindExtraActivity_Regulation_idx");

            entity.HasIndex(e => e.StudentId, "BindExtraActivity_Student");

            entity.Property(e => e.IdBindExtraActivity)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("idBindExtraActivity");
            entity.Property(e => e.Points)
                .HasColumnType("int(11)")
                .HasColumnName("points");
            entity.Property(e => e.RefulationId).HasColumnType("int(11)");
            entity.Property(e => e.StudentId).HasColumnType("int(11)");

            entity.HasOne(d => d.Refulation).WithMany(p => p.BindExtraActivities)
                .HasForeignKey(d => d.RefulationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindExtraActivity_Regulation");

            entity.HasOne(d => d.Student).WithMany(p => p.BindExtraActivities)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindExtraActivity_Student");
        });

        modelBuilder.Entity<BindLoansMain>(entity =>
        {
            entity.HasKey(e => e.IdBindLoan).HasName("PRIMARY");

            entity
                .ToTable("BindLoansMain")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AddDisciplinesId, "BindLoansMain_AddDisciplines_idx");

            entity.HasIndex(e => e.EducationalProgramId, "BindLoansMain_EducationalProgram_idx");

            entity.Property(e => e.IdBindLoan)
                .HasColumnType("int(11)")
                .HasColumnName("idBindLoan");
            entity.Property(e => e.AddDisciplinesId).HasColumnType("int(11)");
            entity.Property(e => e.EducationalProgramId).HasColumnType("int(11)");

            entity.HasOne(d => d.AddDisciplines).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.AddDisciplinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindLoansMain_AddDisciplines");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindLoansMain_EducationalProgram");
        });

        modelBuilder.Entity<BindMainDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindMainDisciplines).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.EducationalProgramId, "BindMainDisciplines_EducationalProgram_idx");

            entity.Property(e => e.IdBindMainDisciplines)
                .HasColumnType("int(11)")
                .HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.CodeMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.EducationalProgramId).HasColumnType("int(11)");
            entity.Property(e => e.FormControll).HasColumnType("enum('Залік','Екзамен','Диференційований Залік')");
            entity.Property(e => e.Loans).HasColumnType("int(11)");
            entity.Property(e => e.NameBindMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("nameBindMainDisciplines");
            entity.Property(e => e.Semestr).HasColumnType("int(11)");
            entity.Property(e => e.Teachers).HasColumnType("json");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindMainDisciplines_EducationalProgram");
        });

        modelBuilder.Entity<BindRolePermission>(entity =>
        {
            entity.HasKey(e => e.IdBindRolePermission).HasName("PRIMARY");

            entity
                .ToTable("BindRolePermission")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.PermissionId, "BindRolePermission_Permission_idx");

            entity.HasIndex(e => e.RoleId, "BindRolePermission_Role_idx");

            entity.Property(e => e.IdBindRolePermission)
                .HasColumnType("int(11)")
                .HasColumnName("idBindRolePermission");
            entity.Property(e => e.PermissionId).HasColumnType("int(11)");
            entity.Property(e => e.RoleId).HasColumnType("int(11)");

            entity.HasOne(d => d.Permission).WithMany(p => p.BindRolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindRolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.BindRolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindRolePermission_Role");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("PRIMARY");

            entity
                .ToTable("Department")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "Department_Admin_idx");

            entity.HasIndex(e => e.FacultyId, "Department_Faculties_idx");

            entity.Property(e => e.IdDepartment)
                .HasColumnType("int(11)")
                .HasColumnName("idDepartment");
            entity.Property(e => e.Abbreviation).HasMaxLength(200);
            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.NameDepartment)
                .HasMaxLength(200)
                .HasColumnName("nameDepartment");

            entity.HasOne(d => d.Admin).WithMany(p => p.Departments)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("Department_Admin");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Department_Faculties");
        });

        modelBuilder.Entity<DisciplineChoicePeriod>(entity =>
        {
            entity.HasKey(e => e.IdDisciplineChoicePeriod).HasName("PRIMARY");

            entity
                .ToTable("DisciplineChoicePeriod")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.DepartmentId, "fk_dcp_department");

            entity.HasIndex(e => e.FacultyId, "fk_dcp_faculty");

            entity.Property(e => e.IdDisciplineChoicePeriod)
                .HasColumnType("int(11)")
                .HasColumnName("idDisciplineChoicePeriod");
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.IsClose).HasColumnType("tinyint(4)");
            entity.Property(e => e.PeriodCourse).HasColumnType("tinyint(4)");
            entity.Property(e => e.PeriodType).HasColumnType("tinyint(4)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("fk_dcp_department");

            entity.HasOne(d => d.Faculty).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("fk_dcp_faculty");
        });

        modelBuilder.Entity<EducationStatus>(entity =>
        {
            entity.HasKey(e => e.IdEducationStatus).HasName("PRIMARY");

            entity
                .ToTable("EducationStatus")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdEducationStatus, "idEducationStatus_UNIQUE").IsUnique();

            entity.Property(e => e.IdEducationStatus)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(45)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<EducationalDegree>(entity =>
        {
            entity.HasKey(e => e.IdEducationalDegree).HasName("PRIMARY");

            entity
                .ToTable("EducationalDegree")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdEducationalDegree)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("idEducationalDegree");
            entity.Property(e => e.NameEducationalDegreec)
                .HasMaxLength(45)
                .HasColumnName("nameEducationalDegreec");
        });

        modelBuilder.Entity<EducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdEducationalProgram).HasName("PRIMARY");

            entity
                .ToTable("EducationalProgram")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.DegreeId, "EducationalProgram_EducationalDegree_idx");

            entity.Property(e => e.IdEducationalProgram)
                .HasColumnType("int(11)")
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation)
                .HasColumnType("tinyint(4)")
                .HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(400)
                .HasColumnName("accreditationType");
            entity.Property(e => e.CountAddSemestr3)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr3");
            entity.Property(e => e.CountAddSemestr4)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr4");
            entity.Property(e => e.CountAddSemestr5)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr5");
            entity.Property(e => e.CountAddSemestr6)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr6");
            entity.Property(e => e.CountAddSemestr7)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr7");
            entity.Property(e => e.CountAddSemestr8)
                .HasColumnType("int(11)")
                .HasColumnName("countAddSemestr8");
            entity.Property(e => e.DegreeId)
                .HasColumnType("int(11)")
                .HasColumnName("degreeId");
            entity.Property(e => e.NameEducationalProgram)
                .HasMaxLength(200)
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.Speciality)
                .HasMaxLength(400)
                .HasColumnName("speciality");
            entity.Property(e => e.SpecialityCode)
                .HasMaxLength(45)
                .HasColumnName("specialityCode");
            entity.Property(e => e.StudentsAmount)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("studentsAmount");

            entity.HasOne(d => d.Degree).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.DegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EducationalProgram_EducationalDegree");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.RegulationId, "Event_Regultion_idx");

            entity.Property(e => e.IdEvent)
                .HasColumnType("int(11)")
                .HasColumnName("idEvent");
            entity.Property(e => e.AmountPeople).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.LinkToRegistration).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.NameEvent)
                .HasMaxLength(255)
                .HasColumnName("nameEvent");
            entity.Property(e => e.RegulationId).HasColumnType("int(11)");

            entity.HasOne(d => d.Regulation).WithMany(p => p.Events)
                .HasForeignKey(d => d.RegulationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Regultion");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "Faculties_Admin_idx");

            entity.Property(e => e.IdFaculty)
                .HasColumnType("int(11)")
                .HasColumnName("idFaculty");
            entity.Property(e => e.Abbreviation).HasMaxLength(45);
            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(200)
                .HasColumnName("nameFaculty");

            entity.HasOne(d => d.Admin).WithMany(p => p.Faculties)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("Faculties_Admin");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("PRIMARY");

            entity
                .ToTable("Group")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.AdminId, "Group_Admin_idx");

            entity.HasIndex(e => e.DepartmentId, "Group_Department");

            entity.HasIndex(e => e.DegreeId, "Group_Department_idx");

            entity.HasIndex(e => e.FacultyId, "Group_Faculties");

            entity.Property(e => e.IdGroup)
                .HasColumnType("int(11)")
                .HasColumnName("idGroup");
            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.Course).HasColumnType("int(11)");
            entity.Property(e => e.DegreeId).HasColumnType("int(11)");
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.GroupCode).HasMaxLength(45);
            entity.Property(e => e.NumberOfStudents).HasColumnType("int(11)");

            entity.HasOne(d => d.Admin).WithMany(p => p.Groups)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("Group_Admin");

            entity.HasOne(d => d.Degree).WithMany(p => p.Groups)
                .HasForeignKey(d => d.DegreeId)
                .HasConstraintName("Group_Degree");

            entity.HasOne(d => d.Department).WithMany(p => p.Groups)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("Group_Department");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Groups)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("Group_Faculties");
        });

        modelBuilder.Entity<MainGrade>(entity =>
        {
            entity.HasKey(e => e.IdMainGrade).HasName("PRIMARY");

            entity
                .ToTable("MainGrade")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.StudentId, "FK_MainGrade_Student");

            entity.HasIndex(e => e.MainDisciplinesId, "MainGrade_MainDisciplines_idx");

            entity.Property(e => e.IdMainGrade)
                .HasColumnType("int(11)")
                .HasColumnName("idMainGrade");
            entity.Property(e => e.MainDisciplinesId).HasColumnType("int(11)");
            entity.Property(e => e.MainGrade1)
                .HasColumnType("int(11)")
                .HasColumnName("MainGrade");
            entity.Property(e => e.StudentId).HasColumnType("int(11)");

            entity.HasOne(d => d.MainDisciplines).WithMany(p => p.MainGrades)
                .HasForeignKey(d => d.MainDisciplinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MainGrade_MainDisciplines");

            entity.HasOne(d => d.Student).WithMany(p => p.MainGrades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MainGrade_Student");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.IdMember).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.RoleInSgid, "RoleInSGID");

            entity.HasIndex(e => e.StudentId, "StudentId");

            entity.HasIndex(e => e.SubDivisionId, "SubDivisionId");

            entity.Property(e => e.IdMember)
                .HasColumnType("int(11)")
                .HasColumnName("idMember");
            entity.Property(e => e.RoleInSgid)
                .HasColumnType("int(11)")
                .HasColumnName("RoleInSGID");
            entity.Property(e => e.StudentId).HasColumnType("int(11)");
            entity.Property(e => e.SubDivisionId).HasColumnType("int(11)");

            entity.HasOne(d => d.RoleInSg).WithMany(p => p.Members)
                .HasForeignKey(d => d.RoleInSgid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Members_RoleInSG");

            entity.HasOne(d => d.Student).WithMany(p => p.Members)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Members_Students");

            entity.HasOne(d => d.SubDivision).WithMany(p => p.Members)
                .HasForeignKey(d => d.SubDivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Members_SubDivisionsSG");
        });

        modelBuilder.Entity<Normative>(entity =>
        {
            entity.HasKey(e => e.IdNormative).HasName("PRIMARY");

            entity
                .ToTable("Normative")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdNormative)
                .HasColumnType("int(11)")
                .HasColumnName("idNormative");
            entity.Property(e => e.Count).HasColumnType("int(11)");
            entity.Property(e => e.DegreeLevelId).HasColumnType("int(11)");
            entity.Property(e => e.IsFaculty).HasColumnType("tinyint(2)");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TemplateId, "Notification_NotificationTemplate_idx");

            entity.HasIndex(e => e.UserId, "Notification_Users_idx");

            entity.Property(e => e.IdNotification)
                .HasColumnType("int(11)")
                .HasColumnName("idNotification");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CustomMessage).HasMaxLength(1000);
            entity.Property(e => e.CustomTitle).HasMaxLength(255);
            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.TemplateId).HasColumnType("int(11)");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Template).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("Notification_NotificationTemplate");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notification_Users");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.IdNotificationTemplates).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdNotificationTemplates)
                .HasColumnType("int(11)")
                .HasColumnName("idNotificationTemplates");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.NotificationType).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.IdPermissions).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdPermissions)
                .HasColumnType("int(11)")
                .HasColumnName("idPermissions");
            entity.Property(e => e.TableName).HasMaxLength(45);
            entity.Property(e => e.TypePermission).HasMaxLength(45);
        });

        modelBuilder.Entity<RegulationOnAddPoint>(entity =>
        {
            entity.HasKey(e => e.IdRegulationOnAddPoints).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdRegulationOnAddPoints)
                .HasColumnType("int(11)")
                .HasColumnName("idRegulationOnAddPoints");
            entity.Property(e => e.AmountMax)
                .HasColumnType("int(11)")
                .HasColumnName("amountMax");
            entity.Property(e => e.AmountMin)
                .HasColumnType("int(11)")
                .HasColumnName("amountMin");
            entity.Property(e => e.CodeRegulationOnAddPoints)
                .HasMaxLength(45)
                .HasColumnName("codeRegulationOnAddPoints");
            entity.Property(e => e.Notes).HasMaxLength(100);
            entity.Property(e => e.TypeOfActivitys)
                .HasMaxLength(500)
                .HasColumnName("typeOfActivitys");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PRIMARY");

            entity
                .ToTable("Role")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdRole, "idRole_UNIQUE").IsUnique();

            entity.HasIndex(e => e.NameRole, "nameRole_UNIQUE").IsUnique();

            entity.Property(e => e.IdRole)
                .HasColumnType("int(11)")
                .HasColumnName("idRole");
            entity.Property(e => e.NameRole)
                .HasMaxLength(45)
                .HasColumnName("nameRole");
        });

        modelBuilder.Entity<RolesInSg>(entity =>
        {
            entity.HasKey(e => e.IdRoleSg).HasName("PRIMARY");

            entity
                .ToTable("RolesInSG")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdRoleSg)
                .HasColumnType("int(11)")
                .HasColumnName("idRoleSG");
            entity.Property(e => e.NameRole)
                .HasMaxLength(100)
                .HasColumnName("nameRole");
            entity.Property(e => e.Points)
                .HasColumnType("int(11)")
                .HasColumnName("points");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("PRIMARY");

            entity
                .ToTable("Student")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.DepartmentId, "Students_Department_idx");

            entity.HasIndex(e => e.StatusId, "Students_EducationStatus_idx");

            entity.HasIndex(e => e.EducationalDegreeId, "Students_EducationalDegree_idx");

            entity.HasIndex(e => e.EducationalProgramId, "Students_EducationalProgram_idx");

            entity.HasIndex(e => e.FacultyId, "Students_Faculties_idx");

            entity.HasIndex(e => e.GroupId, "Students_Group_idx");

            entity.HasIndex(e => e.StudyFormId, "Students_StudyForm_idx");

            entity.HasIndex(e => e.UserId, "Students_Users_idx");

            entity.HasIndex(e => e.IdStudent, "idStudent_UNIQUE").IsUnique();

            entity.Property(e => e.IdStudent)
                .HasColumnType("int(11)")
                .HasColumnName("idStudent");
            entity.Property(e => e.Achievement).HasColumnType("json");
            entity.Property(e => e.Course).HasColumnType("int(11)");
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.EducationalDegreeId).HasColumnType("int(11)");
            entity.Property(e => e.EducationalProgramId).HasColumnType("int(11)");
            entity.Property(e => e.FacultyId).HasColumnType("int(11)");
            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.IsInSg).HasColumnName("IsInSG");
            entity.Property(e => e.IsShort).HasColumnType("tinyint(4)");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");
            entity.Property(e => e.Photo).HasColumnType("blob");
            entity.Property(e => e.StatusId).HasColumnType("int(11)");
            entity.Property(e => e.StudyFormId).HasColumnType("int(11)");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.EducationalDegree).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationalDegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_EducationalDegree");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_EducationalProgram");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_Faculties");

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_Group");

            entity.HasOne(d => d.Status).WithMany(p => p.Students)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_EducationStatus");

            entity.HasOne(d => d.StudyForm).WithMany(p => p.Students)
                .HasForeignKey(d => d.StudyFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_StudyForm");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_Users");
        });

        modelBuilder.Entity<StudyForm>(entity =>
        {
            entity.HasKey(e => e.IdStudyForm).HasName("PRIMARY");

            entity
                .ToTable("StudyForm")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdStudyForm)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasMaxLength(45)
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<SubDivisionsSg>(entity =>
        {
            entity.HasKey(e => e.IdSubDivision).HasName("PRIMARY");

            entity
                .ToTable("SubDivisionsSG")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdSubDivision)
                .HasColumnType("int(11)")
                .HasColumnName("idSubDivision");
            entity.Property(e => e.NameDivision).HasMaxLength(100);
        });

        modelBuilder.Entity<TypeOfDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfDiscipline).HasName("PRIMARY");

            entity
                .ToTable("TypeOfDiscipline")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdTypeOfDiscipline)
                .HasColumnType("int(11)")
                .HasColumnName("idTypeOfDiscipline");
            entity.Property(e => e.TypeName).HasMaxLength(45);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.RoleId, "Users_Roles_idx");

            entity.HasIndex(e => e.IdUsers, "idUsers_UNIQUE").IsUnique();

            entity.Property(e => e.IdUsers)
                .HasColumnType("int(11)")
                .HasColumnName("idUsers");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IsFirstLogin)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            entity.Property(e => e.PasswordChangedAt).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnType("int(11)");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
