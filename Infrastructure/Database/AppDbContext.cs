using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Database;

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
    public virtual DbSet<BindStudentsFavouriteDiscipline> BindStudentsFavouriteDisciplines { get; set; }
    public virtual DbSet<Branch> Branches { get; set; }
    public virtual DbSet<CatalogYear> CatalogYears { get; set; }
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
    public virtual DbSet<Speciality> Specialities { get; set; }
    public virtual DbSet<Specialization> Specializations { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<StudyForm> StudyForms { get; set; }
    public virtual DbSet<SubDivisionsSg> SubDivisionsSgs { get; set; }
    public virtual DbSet<TypeOfDiscipline> TypeOfDisciplines { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.IdAchievement);

            entity.ToTable("achievement");

            entity.Property(e => e.IdAchievement)
                .HasColumnType("integer")
                .HasColumnName("idAchievement");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Photo).HasColumnType("bytea");
        });

        modelBuilder.Entity<AddDetail>(entity =>
        {
            entity.HasKey(e => e.IdAddDetails);

            entity.ToTable("adddetails");

            entity.HasIndex(e => e.DepartmentId, "AddDetails_Departments");

            entity.HasIndex(e => e.IdAddDetails, "idAddDeteils_UNIQUE").IsUnique();

            entity.Property(e => e.IdAddDetails)
                .ValueGeneratedNever()
                .HasColumnType("integer")
                .HasColumnName("idAddDetails");
            entity.Property(e => e.DepartmentId).HasColumnType("integer");
            entity.Property(e => e.DisciplineTopics).HasMaxLength(800);
            entity.Property(e => e.Language).HasMaxLength(200);
            entity.Property(e => e.Prerequisites).HasMaxLength(800);
            entity.Property(e => e.Provision).HasMaxLength(800);
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
            entity.HasKey(e => e.IdAddDisciplines);

            entity.ToTable("adddisciplines");

            entity.HasIndex(e => e.FacultyId, "AddDisciplines_ Faculties_idx");

            entity.HasIndex(e => e.IdCatalog, "AddDisciplines_CatalogYears_FK");

            entity.HasIndex(e => e.DegreeLevelId, "AddDisciplines_EducationalDegree_idx");

            entity.HasIndex(e => e.TypeId, "AddDisciplines_Type_idx");

            entity.HasIndex(e => e.IdAddDisciplines, "idAddDisciplines_UNIQUE").IsUnique();

            entity.Property(e => e.IdAddDisciplines)
                .HasColumnType("integer")
                .HasColumnName("idAddDisciplines");
            entity.Property(e => e.CodeAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("codeAddDisciplines");
            entity.Property(e => e.DegreeLevelId).HasColumnType("integer");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
            entity.Property(e => e.IdCatalog)
                .HasDefaultValueSql("1")
                .HasColumnType("integer")
                .HasColumnName("idCatalog");
            entity.Property(e => e.IsEven).HasColumnType("smallint");
            entity.Property(e => e.IsFaculty).HasColumnType("smallint");
            entity.Property(e => e.IsForseChange).HasColumnType("smallint");
            entity.Property(e => e.MaxCountPeople)
                .HasColumnType("integer")
                .HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse)
                .HasColumnType("integer")
                .HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople)
                .HasColumnType("integer")
                .HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse)
                .HasColumnType("integer")
                .HasColumnName("minCourse");
            entity.Property(e => e.NameAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("nameAddDisciplines");
            entity.Property(e => e.TypeId).HasColumnType("integer");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("AddDisciplines_EducationalDegree");

            entity.HasOne(d => d.Faculty).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDisciplines_ Faculties");

            entity.HasOne(d => d.IdCatalogNavigation).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.IdCatalog)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDisciplines_CatalogYears_FK");

            entity.HasOne(d => d.Type).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("AddDisciplines_Type");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => new { e.LogId, e.ChangeTime });

            entity.ToTable("adminlogs");

            entity.HasIndex(e => e.AdminId, "idx_AdminLogs_AdminId");

            entity.HasIndex(e => e.ChangeTime, "idx_AdminLogs_Time");

            entity.Property(e => e.LogId)
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");
            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Action).HasColumnType("text");
            entity.Property(e => e.AdminId)
                .HasComment("idUsers who performed action")
                .HasColumnType("integer");
            entity.Property(e => e.KeyValue)
                .HasMaxLength(255)
                .HasComment("Primary key of affected row");
            entity.Property(e => e.NewData).HasColumnType("json");
            entity.Property(e => e.OldData).HasColumnType("json");
            entity.Property(e => e.TableName).HasMaxLength(64);
        });

        modelBuilder.Entity<AdminsPersonal>(entity =>
        {
            entity.HasKey(e => e.IdAdmins);

            entity.ToTable("adminspersonal");

            entity.HasIndex(e => e.DepartmentId, "AdminsPersonal_Department_idx");

            entity.HasIndex(e => e.FacultyId, "AdminsPersonal_Faculties_idx");

            entity.HasIndex(e => e.UserId, "AdminsPersonal_Users_idx");

            entity.Property(e => e.IdAdmins)
                .HasColumnType("integer")
                .HasColumnName("idAdmins");
            entity.Property(e => e.DepartmentId).HasColumnType("integer");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
            entity.Property(e => e.NameAdmin)
                .HasMaxLength(200)
                .HasColumnName("nameAdmin");
            entity.Property(e => e.Photo).HasColumnType("bytea");
            entity.Property(e => e.UserId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdBindAddDisciplines);

            entity.ToTable("bindadddisciplines");

            entity.HasIndex(e => e.AddDisciplinesId, "Bind_AddCourse_idx");

            entity.HasIndex(e => e.StudentId, "Bind_Student_idx");

            entity.Property(e => e.IdBindAddDisciplines)
                .HasColumnType("integer")
                .HasColumnName("idBindAddDisciplines");
            entity.Property(e => e.AddDisciplinesId).HasColumnType("integer");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Grade).HasColumnType("integer");
            entity.Property(e => e.InProcess)
                .HasColumnType("smallint")
                .HasColumnName("inProcess");
            entity.Property(e => e.Loans)
                .HasDefaultValueSql("5")
                .HasColumnType("integer");
            entity.Property(e => e.Semestr).HasColumnType("integer");
            entity.Property(e => e.StudentId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdBindEvent);

            entity.ToTable("bindevents");

            entity.HasIndex(e => e.EventId, "EventID");

            entity.HasIndex(e => e.StudentId, "StudentId");

            entity.Property(e => e.IdBindEvent)
                .HasColumnType("integer")
                .HasColumnName("idBindEvent");
            entity.Property(e => e.EventId)
                .HasColumnType("integer")
                .HasColumnName("EventID");
            entity.Property(e => e.Points)
                .HasColumnType("integer")
                .HasColumnName("points");
            entity.Property(e => e.StudentId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdBindExtraActivity);

            entity.ToTable("bindextraactivity");

            entity.HasIndex(e => e.RefulationId, "BindExtraActivity_Regulation_idx");

            entity.HasIndex(e => e.StudentId, "BindExtraActivity_Student");

            entity.Property(e => e.IdBindExtraActivity)
                .ValueGeneratedNever()
                .HasColumnType("integer")
                .HasColumnName("idBindExtraActivity");
            entity.Property(e => e.Points)
                .HasColumnType("integer")
                .HasColumnName("points");
            entity.Property(e => e.RefulationId).HasColumnType("integer");
            entity.Property(e => e.StudentId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdBindLoan);

            entity.ToTable("bindloansmain");

            entity.HasIndex(e => e.AddDisciplinesId, "BindLoansMain_AddDisciplines_idx");

            entity.HasIndex(e => e.EducationalProgramId, "BindLoansMain_EducationalProgram_idx");

            entity.Property(e => e.IdBindLoan)
                .HasColumnType("integer")
                .HasColumnName("idBindLoan");
            entity.Property(e => e.AddDisciplinesId).HasColumnType("integer");
            entity.Property(e => e.EducationalProgramId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdBindMainDisciplines);

            entity.ToTable("bindmaindisciplines");

            entity.HasIndex(e => e.EducationalProgramId, "BindMainDisciplines_EducationalProgram_idx");

            entity.Property(e => e.IdBindMainDisciplines)
                .HasColumnType("integer")
                .HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.CodeMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.EducationalProgramId).HasColumnType("integer");
            entity.Property(e => e.FormControll).HasColumnType("text");
            entity.Property(e => e.Hours)
                .HasComment("Години")
                .HasColumnType("integer");
            entity.Property(e => e.Loans).HasColumnType("integer");
            entity.Property(e => e.NameBindMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("nameBindMainDisciplines");
            entity.Property(e => e.Semestr).HasColumnType("integer");
            entity.Property(e => e.Teachers).HasColumnType("json");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindMainDisciplines_EducationalProgram");
        });

        modelBuilder.Entity<BindRolePermission>(entity =>
        {
            entity.HasKey(e => e.IdBindRolePermission);

            entity.ToTable("bindrolepermission");

            entity.HasIndex(e => e.PermissionId, "BindRolePermission_Permission_idx");

            entity.HasIndex(e => e.RoleId, "BindRolePermission_Role_idx");

            entity.Property(e => e.IdBindRolePermission)
                .HasColumnType("integer")
                .HasColumnName("idBindRolePermission");
            entity.Property(e => e.PermissionId).HasColumnType("integer");
            entity.Property(e => e.RoleId).HasColumnType("integer");

            entity.HasOne(d => d.Permission).WithMany(p => p.BindRolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindRolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.BindRolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindRolePermission_Role");
        });

        modelBuilder.Entity<BindStudentsFavouriteDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindStudentsFavouriteDisciplines);

            entity.ToTable("bindstudentsfavouritedisciplines");

            entity.HasIndex(e => e.IdAddDiscipline, "BindStudentsFavouriteDisciplines_AddDisciplines_FK");

            entity.HasIndex(e => e.IdStudent, "BindStudentsFavouriteDisciplines_Student_FK");

            entity.Property(e => e.IdBindStudentsFavouriteDisciplines)
                .HasColumnType("integer")
                .HasColumnName("idBindStudentsFavouriteDisciplines");
            entity.Property(e => e.IdAddDiscipline)
                .HasColumnType("integer")
                .HasColumnName("idAddDiscipline");
            entity.Property(e => e.IdStudent)
                .HasColumnType("integer")
                .HasColumnName("idStudent");

            entity.HasOne(d => d.IdAddDisciplineNavigation).WithMany(p => p.BindStudentsFavouriteDisciplines)
                .HasForeignKey(d => d.IdAddDiscipline)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("BindStudentsFavouriteDisciplines_AddDisciplines_FK");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.BindStudentsFavouriteDisciplines)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("BindStudentsFavouriteDisciplines_Student_FK");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.IdBranch);

            entity.ToTable("branch");

            entity.Property(e => e.IdBranch)
                .HasColumnType("integer")
                .HasColumnName("idBranch");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasComment("Код галузі (наприклад, 12)")
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Назва галузі (наприклад, Інформаційні технології)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<CatalogYear>(entity =>
        {
            entity.HasKey(e => e.IdCatalogYear);

            entity.ToTable("catalogyears", tb => tb.HasComment("Катлог за навчальним роками."));

            entity.Property(e => e.IdCatalogYear)
                .HasColumnType("integer")
                .HasColumnName("idCatalogYear");
            entity.Property(e => e.IsFormed)
                .HasColumnType("smallint")
                .HasColumnName("isFormed");
            entity.Property(e => e.NameCatalog).HasColumnType("text");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment);

            entity.ToTable("department");

            entity.HasIndex(e => e.AdminId, "Department_Admin_idx");

            entity.HasIndex(e => e.FacultyId, "Department_Faculties_idx");

            entity.Property(e => e.IdDepartment)
                .HasColumnType("integer")
                .HasColumnName("idDepartment");
            entity.Property(e => e.Abbreviation).HasMaxLength(200);
            entity.Property(e => e.AdminId)
                .HasDefaultValueSql("1")
                .HasColumnType("integer");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
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
            entity.HasKey(e => e.IdDisciplineChoicePeriod);

            entity.ToTable("disciplinechoiceperiod");

            entity.HasIndex(e => e.DegreeLevelId, "fk_dcp_degreeLevel_idx");

            entity.HasIndex(e => e.DepartmentId, "fk_dcp_department");

            entity.HasIndex(e => e.FacultyId, "fk_dcp_faculty");

            entity.Property(e => e.IdDisciplineChoicePeriod)
                .HasColumnType("integer")
                .HasColumnName("idDisciplineChoicePeriod");
            entity.Property(e => e.DegreeLevelId).HasColumnType("integer");
            entity.Property(e => e.DepartmentId).HasColumnType("integer");
            entity.Property(e => e.EndDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
            entity.Property(e => e.IsClose).HasColumnType("smallint");
            entity.Property(e => e.PeriodCourse).HasColumnType("smallint");
            entity.Property(e => e.PeriodType)
                .HasComment("Для всіх = 0, Перевибір = 1")
                .HasColumnType("smallint");
            entity.Property(e => e.StartDate).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DegreeLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dcp_degreeLevel");

            entity.HasOne(d => d.Department).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("fk_dcp_department");

            entity.HasOne(d => d.Faculty).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("fk_dcp_faculty");
        });

        modelBuilder.Entity<EducationStatus>(entity =>
        {
            entity.HasKey(e => e.IdEducationStatus);

            entity.ToTable("educationstatus");

            entity.HasIndex(e => e.IdEducationStatus, "idEducationStatus_UNIQUE").IsUnique();

            entity.Property(e => e.IdEducationStatus)
                .ValueGeneratedNever()
                .HasColumnType("integer")
                .HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(45)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<EducationalDegree>(entity =>
        {
            entity.HasKey(e => e.IdEducationalDegree);

            entity.ToTable("educationaldegree");

            entity.Property(e => e.IdEducationalDegree)
                .ValueGeneratedNever()
                .HasColumnType("integer")
                .HasColumnName("idEducationalDegree");
            entity.Property(e => e.NameEducationalDegreec)
                .HasMaxLength(45)
                .HasColumnName("nameEducationalDegreec");
        });

        modelBuilder.Entity<EducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdEducationalProgram);

            entity.ToTable("educationalprogram");

            entity.HasIndex(e => e.DegreeId, "EducationalProgram_EducationalDegree_idx");

            entity.Property(e => e.IdEducationalProgram)
                .HasColumnType("integer")
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation)
                .HasColumnType("smallint")
                .HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(400)
                .HasColumnName("accreditationType");
            entity.Property(e => e.CountAddSemestr3)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr3");
            entity.Property(e => e.CountAddSemestr4)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr4");
            entity.Property(e => e.CountAddSemestr5)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr5");
            entity.Property(e => e.CountAddSemestr6)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr6");
            entity.Property(e => e.CountAddSemestr7)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr7");
            entity.Property(e => e.CountAddSemestr8)
                .HasColumnType("integer")
                .HasColumnName("countAddSemestr8");
            entity.Property(e => e.DegreeId)
                .HasColumnType("integer")
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
                .HasColumnType("integer")
                .HasColumnName("studentsAmount");

            entity.HasOne(d => d.Degree).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.DegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EducationalProgram_EducationalDegree");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent);

            entity.ToTable("events");

            entity.HasIndex(e => e.RegulationId, "Event_Regultion_idx");

            entity.Property(e => e.IdEvent)
                .HasColumnType("integer")
                .HasColumnName("idEvent");
            entity.Property(e => e.AmountPeople).HasColumnType("integer");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.LinkToRegistration).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.NameEvent)
                .HasMaxLength(255)
                .HasColumnName("nameEvent");
            entity.Property(e => e.RegulationId).HasColumnType("integer");

            entity.HasOne(d => d.Regulation).WithMany(p => p.Events)
                .HasForeignKey(d => d.RegulationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Regultion");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty);

            entity.ToTable("faculties");

            entity.HasIndex(e => e.AdminId, "Faculties_Admin_idx");

            entity.Property(e => e.IdFaculty)
                .HasColumnType("integer")
                .HasColumnName("idFaculty");
            entity.Property(e => e.Abbreviation).HasMaxLength(45);
            entity.Property(e => e.AdminId).HasColumnType("integer");
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
            entity.HasKey(e => e.IdGroup);

            entity.ToTable("Group");

            entity.HasIndex(e => e.IdEducationalProgram, "FK_Group_EducationalProgram");

            entity.HasIndex(e => e.IdSpeciality, "FK_Group_Speciality");

            entity.HasIndex(e => e.IdSpecialization, "FK_Group_Specialization");

            entity.HasIndex(e => e.IdStudyForm, "FK_Group_StudyForm");

            entity.HasIndex(e => e.AdminId, "Group_Admin_idx");

            entity.HasIndex(e => e.DepartmentId, "Group_Department");

            entity.HasIndex(e => e.DegreeId, "Group_Department_idx");

            entity.HasIndex(e => e.FacultyId, "Group_Faculties");

            entity.Property(e => e.IdGroup)
                .HasColumnType("integer")
                .HasColumnName("idGroup");
            entity.Property(e => e.AdminId).HasColumnType("integer");
            entity.Property(e => e.AdmissionYear)
                .HasComment("Рік вступу")
                .HasColumnType("integer");
            entity.Property(e => e.Course).HasColumnType("integer");
            entity.Property(e => e.DegreeId).HasColumnType("integer");
            entity.Property(e => e.DepartmentId).HasColumnType("integer");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
            entity.Property(e => e.GroupCode).HasMaxLength(45);
            entity.Property(e => e.IdEducationalProgram)
                .HasComment("Освітня програма")
                .HasColumnType("integer")
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.IdSpeciality)
                .HasComment("Спеціальність")
                .HasColumnType("integer")
                .HasColumnName("idSpeciality");
            entity.Property(e => e.IdSpecialization)
                .HasComment("Спеціалізація")
                .HasColumnType("integer")
                .HasColumnName("idSpecialization");
            entity.Property(e => e.IdStudyForm)
                .HasComment("Форма навчання")
                .HasColumnType("integer")
                .HasColumnName("idStudyForm");
            entity.Property(e => e.IsAccelerated).HasComment("Чи прискорений (0 - ні, 1 - так)");
            entity.Property(e => e.NumberOfStudents).HasColumnType("integer");

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

            entity.HasOne(d => d.IdEducationalProgramNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdEducationalProgram)
                .HasConstraintName("FK_Group_EducationalProgram");

            entity.HasOne(d => d.IdSpecialityNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdSpeciality)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Group_Speciality");

            entity.HasOne(d => d.IdSpecializationNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdSpecialization)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Group_Specialization");

            entity.HasOne(d => d.IdStudyFormNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdStudyForm)
                .HasConstraintName("FK_Group_StudyForm");
        });

        modelBuilder.Entity<MainGrade>(entity =>
        {
            entity.HasKey(e => e.IdMainGrade);

            entity.ToTable("maingrade");

            entity.HasIndex(e => e.StudentId, "FK_MainGrade_Student");

            entity.HasIndex(e => e.MainDisciplinesId, "MainGrade_MainDisciplines_idx");

            entity.Property(e => e.IdMainGrade)
                .HasColumnType("integer")
                .HasColumnName("idMainGrade");
            entity.Property(e => e.MainDisciplinesId).HasColumnType("integer");
            entity.Property(e => e.MainGrade1)
                .HasColumnType("integer")
                .HasColumnName("MainGrade");
            entity.Property(e => e.StudentId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdMember);

            entity.ToTable("members");

            entity.HasIndex(e => e.RoleInSgid, "RoleInSGID");

            entity.HasIndex(e => e.StudentId, "StudentId");

            entity.HasIndex(e => e.SubDivisionId, "SubDivisionId");

            entity.Property(e => e.IdMember)
                .HasColumnType("integer")
                .HasColumnName("idMember");
            entity.Property(e => e.RoleInSgid)
                .HasColumnType("integer")
                .HasColumnName("RoleInSGID");
            entity.Property(e => e.StudentId).HasColumnType("integer");
            entity.Property(e => e.SubDivisionId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdNormative);

            entity.ToTable("normative");

            entity.HasIndex(e => e.DegreeLevelId, "Normative_Degree_idx");

            entity.Property(e => e.IdNormative)
                .HasColumnType("integer")
                .HasColumnName("idNormative");
            entity.Property(e => e.Count).HasColumnType("integer");
            entity.Property(e => e.DegreeLevelId).HasColumnType("integer");
            entity.Property(e => e.IsFaculty).HasColumnType("smallint");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.Normatives)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("Normative_Degree");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification);

            entity.ToTable("notifications");

            entity.HasIndex(e => e.TemplateId, "Notification_NotificationTemplate_idx");

            entity.HasIndex(e => e.UserId, "Notification_Users_idx");

            entity.Property(e => e.IdNotification)
                .HasColumnType("integer")
                .HasColumnName("idNotification");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.CustomMessage).HasMaxLength(1000);
            entity.Property(e => e.CustomTitle).HasMaxLength(255);
            entity.Property(e => e.Metadata).HasColumnType("json");
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.TemplateId).HasColumnType("integer");
            entity.Property(e => e.UserId).HasColumnType("integer");

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
            entity.HasKey(e => e.IdNotificationTemplates);

            entity.ToTable("notificationtemplates");

            entity.Property(e => e.IdNotificationTemplates)
                .HasColumnType("integer")
                .HasColumnName("idNotificationTemplates");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.NotificationType).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.IdPermissions);

            entity.ToTable("permissions");

            entity.Property(e => e.IdPermissions)
                .HasColumnType("integer")
                .HasColumnName("idPermissions");
            entity.Property(e => e.BitIndex)
                .HasColumnType("integer")
                .HasColumnName("bit_index");
            entity.Property(e => e.TableName).HasMaxLength(45);
            entity.Property(e => e.TypePermission).HasMaxLength(45);
        });

        modelBuilder.Entity<RegulationOnAddPoint>(entity =>
        {
            entity.HasKey(e => e.IdRegulationOnAddPoints);

            entity.ToTable("regulationonaddpoints");

            entity.Property(e => e.IdRegulationOnAddPoints)
                .HasColumnType("integer")
                .HasColumnName("idRegulationOnAddPoints");
            entity.Property(e => e.AmountMax)
                .HasColumnType("integer")
                .HasColumnName("amountMax");
            entity.Property(e => e.AmountMin)
                .HasColumnType("integer")
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
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.HasIndex(e => e.IdRole, "idRole_UNIQUE").IsUnique();

            entity.HasIndex(e => e.NameRole, "nameRole_UNIQUE").IsUnique();

            entity.Property(e => e.IdRole)
                .HasColumnType("integer")
                .HasColumnName("idRole");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.IsSystem)
                .HasDefaultValueSql("0")
                .HasColumnType("smallint");
            entity.Property(e => e.NameRole)
                .HasMaxLength(45)
                .HasColumnName("nameRole");
            entity.Property(e => e.ParentRoleId)
                .HasColumnType("integer")
                .HasColumnName("parent_role_id");
            entity.Property(e => e.PermissionsMask)
                .HasColumnType("bigint")
                .HasColumnName("permissions_mask");

            entity.HasOne(d => d.ParentRole)
                .WithMany(p => p.InverseParentRole)
                .HasForeignKey(d => d.ParentRoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Role_ParentRole");
        });

        modelBuilder.Entity<RolesInSg>(entity =>
        {
            entity.HasKey(e => e.IdRoleSg);

            entity.ToTable("rolesinsg");

            entity.Property(e => e.IdRoleSg)
                .HasColumnType("integer")
                .HasColumnName("idRoleSG");
            entity.Property(e => e.NameRole)
                .HasMaxLength(100)
                .HasColumnName("nameRole");
            entity.Property(e => e.Points)
                .HasColumnType("integer")
                .HasColumnName("points");
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity.HasKey(e => e.IdSpeciality);

            entity.ToTable("speciality");

            entity.HasIndex(e => e.IdBranch, "FK_Speciality_Branch");

            entity.HasIndex(e => e.IdDepartment, "FK_Speciality_Department");

            entity.HasIndex(e => e.IdFaculty, "FK_Speciality_Faculty");

            entity.Property(e => e.IdSpeciality)
                .HasColumnType("integer")
                .HasColumnName("idSpeciality");
            entity.Property(e => e.Accreditation)
                .HasDefaultValueSql("0")
                .HasComment("Акредитація (0 або 1)")
                .HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(255)
                .HasComment("Тип акредитації (текст зі скріншоту)")
                .HasColumnName("accreditationType");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasComment("Код спеціальності (наприклад, 121)")
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasComment("Опис спеціальності")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IdBranch)
                .HasComment("Зовнішній ключ на Галузь")
                .HasColumnType("integer")
                .HasColumnName("idBranch");
            entity.Property(e => e.IdDepartment)
                .HasComment("Зовнішній ключ на Кафедру")
                .HasColumnType("integer")
                .HasColumnName("idDepartment");
            entity.Property(e => e.IdFaculty)
                .HasComment("Зовнішній ключ на Факультет")
                .HasColumnType("integer")
                .HasColumnName("idFaculty");
            entity.Property(e => e.LicensedVolume)
                .HasDefaultValueSql("0")
                .HasComment("Ліцензійний обсяг (studentsAmount)")
                .HasColumnType("integer")
                .HasColumnName("licensedVolume");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Назва спеціальності")
                .HasColumnName("name");

            entity.HasOne(d => d.IdBranchNavigation).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.IdBranch)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Speciality_Branch");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.IdDepartment)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Speciality_Department");

            entity.HasOne(d => d.IdFacultyNavigation).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.IdFaculty)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Speciality_Faculty");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.IdSpecialization);

            entity.ToTable("specialization");

            entity.HasIndex(e => e.IdSpeciality, "FK_Specialization_Speciality");

            entity.Property(e => e.IdSpecialization)
                .HasColumnType("integer")
                .HasColumnName("idSpecialization");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasComment("Код спеціалізації (наприклад, 014.01)")
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasComment("Опис спеціалізації")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IdSpeciality)
                .HasComment("Зовнішній ключ на Спеціальність")
                .HasColumnType("integer")
                .HasColumnName("idSpeciality");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasComment("Назва спеціалізації")
                .HasColumnName("name");

            entity.HasOne(d => d.IdSpecialityNavigation).WithMany(p => p.Specializations)
                .HasForeignKey(d => d.IdSpeciality)
                .HasConstraintName("FK_Specialization_Speciality");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent);

            entity.ToTable("student");

            entity.HasIndex(e => e.DepartmentId, "Students_Department_idx");

            entity.HasIndex(e => e.EducationStatusId, "Students_EducationStatus_idx");

            entity.HasIndex(e => e.EducationalDegreeId, "Students_EducationalDegree_idx");

            entity.HasIndex(e => e.EducationalProgramId, "Students_EducationalProgram_idx");

            entity.HasIndex(e => e.FacultyId, "Students_Faculties_idx");

            entity.HasIndex(e => e.GroupId, "Students_Group_idx");

            entity.HasIndex(e => e.StudyFormId, "Students_StudyForm_idx");

            entity.HasIndex(e => e.UserId, "Students_Users_idx");

            entity.HasIndex(e => e.IdStudent, "idStudent_UNIQUE").IsUnique();

            entity.Property(e => e.IdStudent)
                .HasColumnType("integer")
                .HasColumnName("idStudent");
            entity.Property(e => e.Achievement).HasColumnType("json");
            entity.Property(e => e.Course).HasColumnType("integer");
            entity.Property(e => e.DepartmentId).HasColumnType("integer");
            entity.Property(e => e.EducationStatusId).HasColumnType("integer");
            entity.Property(e => e.EducationalDegreeId).HasColumnType("integer");
            entity.Property(e => e.EducationalProgramId).HasColumnType("integer");
            entity.Property(e => e.FacultyId).HasColumnType("integer");
            entity.Property(e => e.GroupId).HasColumnType("integer");
            entity.Property(e => e.IsInSg).HasColumnName("IsInSG");
            entity.Property(e => e.IsShort).HasColumnType("smallint");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");
            entity.Property(e => e.Photo).HasColumnType("bytea");
            entity.Property(e => e.StudyFormId).HasColumnType("integer");
            entity.Property(e => e.UserId).HasColumnType("integer");

            entity.HasOne(d => d.EducationStatus).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_EducationStatus");

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
            entity.HasKey(e => e.IdStudyForm);

            entity.ToTable("studyform");

            entity.Property(e => e.IdStudyForm)
                .ValueGeneratedNever()
                .HasColumnType("integer")
                .HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasMaxLength(45)
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<SubDivisionsSg>(entity =>
        {
            entity.HasKey(e => e.IdSubDivision);

            entity.ToTable("subdivisionssg");

            entity.Property(e => e.IdSubDivision)
                .HasColumnType("integer")
                .HasColumnName("idSubDivision");
            entity.Property(e => e.NameDivision).HasMaxLength(100);
        });

        modelBuilder.Entity<TypeOfDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfDiscipline);

            entity.ToTable("typeofdiscipline");

            entity.Property(e => e.IdTypeOfDiscipline)
                .HasColumnType("integer")
                .HasColumnName("idTypeOfDiscipline");
            entity.Property(e => e.TypeName).HasMaxLength(45);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers);

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.RoleId, "Users_Roles_idx");

            entity.HasIndex(e => e.IdUsers, "idUsers_UNIQUE").IsUnique();

            entity.Property(e => e.IdUsers)
                .HasColumnType("integer")
                .HasColumnName("idUsers");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IsFirstLogin)
                .IsRequired()
                .HasDefaultValueSql("1");
            entity.Property(e => e.LastLoginAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PasswordChangedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnType("integer");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}