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

    public virtual DbSet<AccountingJournal> AccountingJournals { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<AdminsPersonal> AdminsPersonals { get; set; }

    public virtual DbSet<Approval> Approvals { get; set; }

    public virtual DbSet<BindEpspecialization> BindEpspecializations { get; set; }

    public virtual DbSet<BindEventStudent> BindEventStudents { get; set; }

    public virtual DbSet<BindExtraActivity> BindExtraActivities { get; set; }

    public virtual DbSet<BindLoansMain> BindLoansMains { get; set; }

    public virtual DbSet<BindMainDiscipline> BindMainDisciplines { get; set; }

    public virtual DbSet<BindRating> BindRatings { get; set; }

    public virtual DbSet<BindSelectiveDiscipline> BindSelectiveDisciplines { get; set; }

    public virtual DbSet<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; }

    public virtual DbSet<BindTeacherMain> BindTeacherMains { get; set; }

    public virtual DbSet<BindTeachersSelective> BindTeachersSelectives { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CatalogYearsMain> CatalogYearsMains { get; set; }

    public virtual DbSet<CatalogYearsSelective> CatalogYearsSelectives { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; }

    public virtual DbSet<EducationStatus> EducationStatuses { get; set; }

    public virtual DbSet<EducationalDegree> EducationalDegrees { get; set; }

    public virtual DbSet<EducationalProgram> EducationalPrograms { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<InventorySg> InventorySgs { get; set; }

    public virtual DbSet<MainDiscipline> MainDisciplines { get; set; }

    public virtual DbSet<MainGrade> MainGrades { get; set; }

    public virtual DbSet<MembersOfSg> MembersOfSgs { get; set; }

    public virtual DbSet<Normative> Normatives { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Prerequisite> Prerequisites { get; set; }

    public virtual DbSet<RegulationOnAddPoint> RegulationOnAddPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<RoleInEvent> RoleInEvents { get; set; }

    public virtual DbSet<RolesInSg> RolesInSgs { get; set; }

    public virtual DbSet<SelectiveDetail> SelectiveDetails { get; set; }

    public virtual DbSet<SelectiveDiscipline> SelectiveDisciplines { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentGroup> StudentGroups { get; set; }

    public virtual DbSet<StudyForm> StudyForms { get; set; }

    public virtual DbSet<SubDivisionsSg> SubDivisionsSgs { get; set; }

    public virtual DbSet<TypeOfControl> TypeOfControls { get; set; }

    public virtual DbSet<TypeOfDiscipline> TypeOfDisciplines { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=project_olymp_db;Username=postgres;Password=B25824DCABCB88B5;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountingJournal>(entity =>
        {
            entity.HasKey(e => e.IdAccountingJournal).HasName("accountingjournal_pk");

            entity.ToTable("AccountingJournal");

            entity.Property(e => e.IdAccountingJournal)
                .ValueGeneratedNever()
                .HasColumnName("idAccountingJournal");
            entity.Property(e => e.Comment)
                .HasColumnType("character varying")
                .HasColumnName("comment");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.InventorySgid).HasColumnName("InventorySGId");
            entity.Property(e => e.IsBack)
                .HasColumnType("bit(1)")
                .HasColumnName("isBack");
            entity.Property(e => e.RealBackTime).HasColumnName("realBackTime");
            entity.Property(e => e.StartDate).HasColumnName("startDate");

            entity.HasOne(d => d.InventorySg).WithMany(p => p.AccountingJournals)
                .HasForeignKey(d => d.InventorySgid)
                .HasConstraintName("accountingjournal_inventorysg_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.AccountingJournals)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("accountingjournal_student_fk");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("adminlogs_pk");

            entity.Property(e => e.LogId).ValueGeneratedNever();
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ChangeTime).HasMaxLength(50);
            entity.Property(e => e.NewData).HasMaxLength(256);
            entity.Property(e => e.OldData).HasMaxLength(256);
            entity.Property(e => e.TableName).HasMaxLength(50);

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("adminlogs_adminspersonal_fk");
        });

        modelBuilder.Entity<AdminsPersonal>(entity =>
        {
            entity.HasKey(e => e.IdAdmins).HasName("adminspersonal_pk");

            entity.ToTable("AdminsPersonal");

            entity.Property(e => e.IdAdmins)
                .ValueGeneratedNever()
                .HasColumnName("idAdmins");
            entity.Property(e => e.NameAdmin)
                .HasMaxLength(50)
                .HasColumnName("nameAdmin");

            entity.HasOne(d => d.Department).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("adminspersonal_department_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("adminspersonal_faculties_fk");

            entity.HasOne(d => d.User).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("adminspersonal_users_fk");
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.HasKey(e => e.IdApproval).HasName("approval_pk");

            entity.ToTable("Approval");

            entity.Property(e => e.IdApproval)
                .ValueGeneratedNever()
                .HasColumnName("idApproval");
            entity.Property(e => e.AppovalStatus)
                .HasColumnType("character varying")
                .HasColumnName("appovalStatus");
        });

        modelBuilder.Entity<BindEpspecialization>(entity =>
        {
            entity.HasKey(e => e.IdBind).HasName("bindepspecialization_pk");

            entity.ToTable("BindEPSpecialization");

            entity.Property(e => e.IdBind)
                .ValueGeneratedNever()
                .HasColumnName("idBind");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindEpspecializations)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("bindepspecialization_educationalprogram_fk");

            entity.HasOne(d => d.Specialization).WithMany(p => p.BindEpspecializations)
                .HasForeignKey(d => d.SpecializationId)
                .HasConstraintName("bindepspecialization_specialization_fk");
        });

        modelBuilder.Entity<BindEventStudent>(entity =>
        {
            entity.HasKey(e => e.IdBindEventStudent).HasName("bindeventstudent_pk");

            entity.ToTable("BindEventStudent");

            entity.Property(e => e.IdBindEventStudent)
                .ValueGeneratedNever()
                .HasColumnName("idBindEventStudent");
            entity.Property(e => e.OtherOpion)
                .HasColumnType("character varying")
                .HasColumnName("otherOpion");
            entity.Property(e => e.Point).HasColumnName("point");

            entity.HasOne(d => d.Event).WithMany(p => p.BindEventStudents)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("bindeventstudent_events_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.BindEventStudents)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("bindeventstudent_roleinevent_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindEventStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindeventstudent_student_fk");
        });

        modelBuilder.Entity<BindExtraActivity>(entity =>
        {
            entity.HasKey(e => e.IdBindExtraActivity).HasName("bindextraactivity_pk");

            entity.ToTable("BindExtraActivity");

            entity.Property(e => e.IdBindExtraActivity)
                .ValueGeneratedNever()
                .HasColumnName("idBindExtraActivity");
            entity.Property(e => e.NameExtraActivity)
                .HasColumnType("character varying")
                .HasColumnName("nameExtraActivity");
            entity.Property(e => e.Points).HasColumnName("points");

            entity.HasOne(d => d.Regulation).WithMany(p => p.BindExtraActivities)
                .HasForeignKey(d => d.RegulationId)
                .HasConstraintName("bindextraactivity_regulationonaddpoints_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindExtraActivities)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindextraactivity_student_fk");
        });

        modelBuilder.Entity<BindLoansMain>(entity =>
        {
            entity.HasKey(e => e.IdBindLoan).HasName("bindloansmain_pk");

            entity.ToTable("BindLoansMain");

            entity.Property(e => e.IdBindLoan)
                .ValueGeneratedNever()
                .HasColumnName("idBindLoan");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("bindloansmain_educationalprogram_fk");

            entity.HasOne(d => d.SelectiveDisciplines).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.SelectiveDisciplinesId)
                .HasConstraintName("bindloansmain_adddisciplines_fk");
        });

        modelBuilder.Entity<BindMainDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindMainDisciplines).HasName("bindadddisciplines_pk_2");

            entity.Property(e => e.IdBindMainDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.Grade)
                .HasMaxLength(50)
                .HasColumnName("grade");
            entity.Property(e => e.IsRedo)
                .HasColumnType("bit(1)")
                .HasColumnName("isRedo");
            entity.Property(e => e.Semestr).HasColumnName("semestr");

            entity.HasOne(d => d.IdBindMainDisciplinesNavigation).WithOne(p => p.BindMainDiscipline)
                .HasForeignKey<BindMainDiscipline>(d => d.IdBindMainDisciplines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bindmaindisciplines_maindisciplines_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindmaindisciplines_student_fk");
        });

        modelBuilder.Entity<BindRating>(entity =>
        {
            entity.HasKey(e => e.IdBindRating).HasName("bindrating_pk");

            entity.ToTable("BindRating");

            entity.Property(e => e.IdBindRating)
                .ValueGeneratedNever()
                .HasColumnName("idBindRating");
            entity.Property(e => e.FinalScore).HasColumnName("finalScore");
            entity.Property(e => e.IsRedo)
                .HasColumnType("bit(1)")
                .HasColumnName("isRedo");
            entity.Property(e => e.Semestr)
                .HasColumnType("bit(1)")
                .HasColumnName("semestr");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Student).WithMany(p => p.BindRatings)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindrating_student_fk");
        });

        modelBuilder.Entity<BindSelectiveDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindSelectiveDisciplines).HasName("bindadddisciplines_pk");

            entity.Property(e => e.IdBindSelectiveDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idBindSelectiveDisciplines");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(50)
                .HasColumnName("createdAt");
            entity.Property(e => e.Grade)
                .HasMaxLength(50)
                .HasColumnName("grade");
            entity.Property(e => e.InProcess)
                .HasColumnType("bit(1)")
                .HasColumnName("inProcess");
            entity.Property(e => e.IsRedo)
                .HasColumnType("bit(1)")
                .HasColumnName("isRedo");
            entity.Property(e => e.Loans).HasColumnName("loans");
            entity.Property(e => e.Semestr).HasColumnName("semestr");

            entity.HasOne(d => d.SelectiveDisciplines).WithMany(p => p.BindSelectiveDisciplines)
                .HasForeignKey(d => d.SelectiveDisciplinesId)
                .HasConstraintName("bindadddisciplines_adddisciplines_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindSelectiveDisciplines)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindadddisciplines_student_fk");
        });

        modelBuilder.Entity<BindSubdivisionRoleSg>(entity =>
        {
            entity.HasKey(e => e.IdBindSubdivisionRoleSg).HasName("members_pk");

            entity.ToTable("BindSubdivisionRoleSG");

            entity.Property(e => e.IdBindSubdivisionRoleSg)
                .ValueGeneratedNever()
                .HasColumnName("idBindSubdivisionRoleSG");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.RoleInSgid).HasColumnName("RoleInSGId");

            entity.HasOne(d => d.RoleInSg).WithMany(p => p.BindSubdivisionRoleSgs)
                .HasForeignKey(d => d.RoleInSgid)
                .HasConstraintName("membersofsg_rolesinsg_fk");

            entity.HasOne(d => d.SubDivision).WithMany(p => p.BindSubdivisionRoleSgs)
                .HasForeignKey(d => d.SubDivisionId)
                .HasConstraintName("membersofsg_subdivisionssg_fk");
        });

        modelBuilder.Entity<BindTeacherMain>(entity =>
        {
            entity.HasKey(e => e.IdBindTeacherMain).HasName("bindteachermain_pk");

            entity.ToTable("BindTeacherMain");

            entity.Property(e => e.IdBindTeacherMain)
                .ValueGeneratedNever()
                .HasColumnName("idBindTeacherMain");

            entity.HasOne(d => d.Admin).WithMany(p => p.BindTeacherMains)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("bindteachermain_adminspersonal_fk");

            entity.HasOne(d => d.MainDisciplines).WithMany(p => p.BindTeacherMains)
                .HasForeignKey(d => d.MainDisciplinesId)
                .HasConstraintName("bindteachermain_maindisciplines_fk");
        });

        modelBuilder.Entity<BindTeachersSelective>(entity =>
        {
            entity.HasKey(e => e.IdBindTeacherSelective).HasName("bindteachermain_pk_1");

            entity.ToTable("BindTeachersSelective");

            entity.Property(e => e.IdBindTeacherSelective)
                .ValueGeneratedNever()
                .HasColumnName("idBindTeacherSelective");

            entity.HasOne(d => d.Admin).WithMany(p => p.BindTeachersSelectives)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("bindteachersselective_adminspersonal_fk");

            entity.HasOne(d => d.SelectiveDisciplines).WithMany(p => p.BindTeachersSelectives)
                .HasForeignKey(d => d.SelectiveDisciplinesId)
                .HasConstraintName("bindteachersselective_selectivedisciplines_fk");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.IdBranch).HasName("branch_pk");

            entity.ToTable("Branch");

            entity.Property(e => e.IdBranch)
                .ValueGeneratedNever()
                .HasColumnName("idBranch");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CatalogYearsMain>(entity =>
        {
            entity.HasKey(e => e.IdCatalogYear).HasName("catalogyears_pk");

            entity.ToTable("CatalogYears_Main");

            entity.Property(e => e.IdCatalogYear)
                .ValueGeneratedNever()
                .HasColumnName("idCatalogYear");
            entity.Property(e => e.IsFormed)
                .HasColumnType("bit(1)")
                .HasColumnName("isFormed");
            entity.Property(e => e.NameCatalog)
                .HasMaxLength(50)
                .HasColumnName("nameCatalog");
        });

        modelBuilder.Entity<CatalogYearsSelective>(entity =>
        {
            entity.HasKey(e => e.IdCatalogYear).HasName("catalogyears_selective_pk");

            entity.ToTable("CatalogYears_Selective");

            entity.Property(e => e.IdCatalogYear)
                .ValueGeneratedNever()
                .HasColumnName("idCatalogYear");
            entity.Property(e => e.IsFormed)
                .HasColumnType("bit(1)")
                .HasColumnName("isFormed");
            entity.Property(e => e.NameCatalog)
                .HasMaxLength(50)
                .HasColumnName("nameCatalog");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("department_pk");

            entity.ToTable("Department");

            entity.Property(e => e.IdDepartment)
                .ValueGeneratedNever()
                .HasColumnName("idDepartment");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(50)
                .HasColumnName("abbreviation");
            entity.Property(e => e.NameDepartment)
                .HasMaxLength(64)
                .HasColumnName("nameDepartment");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("department_faculties_fk");
        });

        modelBuilder.Entity<DisciplineChoicePeriod>(entity =>
        {
            entity.HasKey(e => e.IdDisciplineChoicePeriod).HasName("disciplinechoiceperiod_pk");

            entity.ToTable("DisciplineChoicePeriod");

            entity.Property(e => e.IdDisciplineChoicePeriod)
                .ValueGeneratedNever()
                .HasColumnName("idDisciplineChoicePeriod");
            entity.Property(e => e.EndDate)
                .HasMaxLength(50)
                .HasColumnName("endDate");
            entity.Property(e => e.IsClose)
                .HasColumnType("bit(1)")
                .HasColumnName("isClose");
            entity.Property(e => e.PeriodCourse).HasColumnName("periodCourse");
            entity.Property(e => e.PeriodType)
                .HasColumnType("bit(1)")
                .HasColumnName("periodType");
            entity.Property(e => e.StartDate)
                .HasMaxLength(50)
                .HasColumnName("startDate");

            entity.HasOne(d => d.Department).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("disciplinechoiceperiod_department_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("disciplinechoiceperiod_faculties_fk");
        });

        modelBuilder.Entity<EducationStatus>(entity =>
        {
            entity.HasKey(e => e.IdEducationStatus).HasName("educationstatus_pk");

            entity.ToTable("EducationStatus");

            entity.Property(e => e.IdEducationStatus)
                .ValueGeneratedNever()
                .HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(50)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<EducationalDegree>(entity =>
        {
            entity.HasKey(e => e.IdEducationalDegree).HasName("educationaldegree_pk");

            entity.ToTable("EducationalDegree");

            entity.Property(e => e.IdEducationalDegree)
                .ValueGeneratedNever()
                .HasColumnName("idEducationalDegree");
            entity.Property(e => e.NameEducationalDegree)
                .HasMaxLength(50)
                .HasColumnName("nameEducationalDegree");
        });

        modelBuilder.Entity<EducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdEducationalProgram).HasName("educationalprogram_pk");

            entity.ToTable("EducationalProgram");

            entity.Property(e => e.IdEducationalProgram)
                .ValueGeneratedNever()
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(50)
                .HasColumnName("accreditationType");
            entity.Property(e => e.IsSpecialization)
                .HasColumnType("bit(1)")
                .HasColumnName("isSpecialization");
            entity.Property(e => e.NameEducationalProgram)
                .HasMaxLength(128)
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.SelectiveDisciplineBySemestr).HasColumnName("selectiveDisciplineBySemestr");

            entity.HasOne(d => d.Degree).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.DegreeId)
                .HasConstraintName("educationalprogram_educationaldegree_fk");

            entity.HasOne(d => d.Speciality).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.SpeciaityId)
                .HasConstraintName("educationalprogram_speciality_fk");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("events_pk");

            entity.Property(e => e.IdEvent)
                .ValueGeneratedNever()
                .HasColumnName("idEvent");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Format)
                .HasColumnType("character varying")
                .HasColumnName("format");
            entity.Property(e => e.Location)
                .HasColumnType("character varying")
                .HasColumnName("location");
            entity.Property(e => e.NameEvent)
                .HasColumnType("character varying")
                .HasColumnName("nameEvent");
            entity.Property(e => e.SubdivisionSgid).HasColumnName("SubdivisionSGId");

            entity.HasOne(d => d.Creator).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("events_users_fk");

            entity.HasOne(d => d.Regulation).WithMany(p => p.Events)
                .HasForeignKey(d => d.RegulationId)
                .HasConstraintName("events_regulationonaddpoints_fk");

            entity.HasOne(d => d.SubdivisionSg).WithMany(p => p.Events)
                .HasForeignKey(d => d.SubdivisionSgid)
                .HasConstraintName("events_subdivisionssg_fk");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty).HasName("faculties_pk");

            entity.Property(e => e.IdFaculty)
                .ValueGeneratedNever()
                .HasColumnName("idFaculty");
            entity.Property(e => e.Abbreviation).HasMaxLength(50);
            entity.Property(e => e.Metadata).HasMaxLength(50);
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(64)
                .HasColumnName("nameFaculty");
        });

        modelBuilder.Entity<InventorySg>(entity =>
        {
            entity.HasKey(e => e.IdInventoroy).HasName("inventorysg_pk");

            entity.ToTable("InventorySG");

            entity.Property(e => e.IdInventoroy)
                .ValueGeneratedNever()
                .HasColumnName("idInventoroy");
            entity.Property(e => e.CodeInventory)
                .HasColumnType("character varying")
                .HasColumnName("codeInventory");
            entity.Property(e => e.NameInventory)
                .HasColumnType("character varying")
                .HasColumnName("nameInventory");

            entity.HasOne(d => d.Student).WithMany(p => p.InventorySgs)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("inventorysg_student_fk");
        });

        modelBuilder.Entity<MainDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindMainDisciplines).HasName("maindisciplines_pk");

            entity.Property(e => e.IdBindMainDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.CodeMainDisciplines)
                .HasMaxLength(15)
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.FormControl)
                .HasMaxLength(50)
                .HasColumnName("formControl");
            entity.Property(e => e.Hours).HasColumnName("hours");
            entity.Property(e => e.Loans).HasColumnName("loans");
            entity.Property(e => e.NameBindMainDisciplines)
                .HasMaxLength(50)
                .HasColumnName("nameBindMainDisciplines");
            entity.Property(e => e.Semestr).HasColumnName("semestr");

            entity.HasOne(d => d.Catalog).WithMany(p => p.MainDisciplines)
                .HasForeignKey(d => d.CatalogId)
                .HasConstraintName("maindisciplines_catalogyears_main_fk");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.MainDisciplines)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("maindisciplines_educationalprogram_fk");
        });

        modelBuilder.Entity<MainGrade>(entity =>
        {
            entity.HasKey(e => e.IdMainGrade).HasName("maingrade_pk");

            entity.ToTable("MainGrade");

            entity.Property(e => e.IdMainGrade)
                .ValueGeneratedNever()
                .HasColumnName("idMainGrade");
            entity.Property(e => e.MainGrade1)
                .HasMaxLength(50)
                .HasColumnName("MainGrade");

            entity.HasOne(d => d.MainDisciplines).WithMany(p => p.MainGrades)
                .HasForeignKey(d => d.MainDisciplinesId)
                .HasConstraintName("maingrade_maindisciplines_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.MainGrades)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("maingrade_student_fk");
        });

        modelBuilder.Entity<MembersOfSg>(entity =>
        {
            entity.HasKey(e => e.IdMembersOfSg).HasName("membersofsg_pk");

            entity.ToTable("MembersOfSG");

            entity.Property(e => e.IdMembersOfSg)
                .ValueGeneratedNever()
                .HasColumnName("idMembersOfSG");
            entity.Property(e => e.BindsubdivisionRoleSgid).HasColumnName("BindsubdivisionRoleSGId");

            entity.HasOne(d => d.BindsubdivisionRoleSg).WithMany(p => p.MembersOfSgs)
                .HasForeignKey(d => d.BindsubdivisionRoleSgid)
                .HasConstraintName("membersofsg_bindsubdivisionrolesg_fk");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MembersOfSgCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("created_membersofsg_student_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.MembersOfSgStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("membersofsg_student_fk");
        });

        modelBuilder.Entity<Normative>(entity =>
        {
            entity.HasKey(e => e.IdNormative).HasName("normative_pk");

            entity.ToTable("Normative");

            entity.Property(e => e.IdNormative)
                .ValueGeneratedNever()
                .HasColumnName("idNormative");
            entity.Property(e => e.IsFaculty).HasColumnType("bit(1)");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.Normatives)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("normative_educationaldegree_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("notifications_pk");

            entity.Property(e => e.IdNotification)
                .ValueGeneratedNever()
                .HasColumnName("idNotification");
            entity.Property(e => e.CustomMessage).HasMaxLength(128);
            entity.Property(e => e.IsRead).HasColumnType("bit(1)");
            entity.Property(e => e.Metadata).HasColumnType("jsonb");

            entity.HasOne(d => d.Template).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("notifications_notificationtemplates_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_users_fk");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.IdNotificationTemplates).HasName("notificationtemplates_pk");

            entity.Property(e => e.IdNotificationTemplates)
                .ValueGeneratedNever()
                .HasColumnName("idNotificationTemplates");
            entity.Property(e => e.Message).HasMaxLength(128);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.HasIndex(e => e.BitIndex, "Permissions_bitIndex_key").IsUnique();

            entity.HasIndex(e => e.Code, "Permissions_code_key").IsUnique();

            entity.HasIndex(e => e.Code, "permissions_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('permissions_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.BitIndex).HasColumnName("bit_index");
            entity.Property(e => e.Code).HasColumnName("code");
        });

        modelBuilder.Entity<Prerequisite>(entity =>
        {
            entity.HasKey(e => e.Idprerequisites).HasName("prerequisites_pk");

            entity.Property(e => e.Idprerequisites)
                .ValueGeneratedNever()
                .HasColumnName("idprerequisites");
            entity.Property(e => e.Adddisciplensid).HasColumnName("adddisciplensid");
            entity.Property(e => e.Educationalprogramid).HasColumnName("educationalprogramid");

            entity.HasOne(d => d.Adddisciplens).WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.Adddisciplensid)
                .HasConstraintName("prerequisites_adddisciplines_fk");

            entity.HasOne(d => d.Educationalprogram).WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.Educationalprogramid)
                .HasConstraintName("prerequisites_educationalprogram_fk");
        });

        modelBuilder.Entity<RegulationOnAddPoint>(entity =>
        {
            entity.HasKey(e => e.IdRegulationOnAddPoints).HasName("regulationonaddpoints_pk");

            entity.Property(e => e.IdRegulationOnAddPoints)
                .ValueGeneratedNever()
                .HasColumnName("idRegulationOnAddPoints");
            entity.Property(e => e.AmountMax)
                .HasMaxLength(50)
                .HasColumnName("amountMax");
            entity.Property(e => e.AmountMin)
                .HasMaxLength(50)
                .HasColumnName("amountMin");
            entity.Property(e => e.CodeRegulationOnAddPoints)
                .HasMaxLength(50)
                .HasColumnName("codeRegulationOnAddPoints");
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.SubTypeOfActivitys)
                .HasMaxLength(50)
                .HasColumnName("subTypeOfActivitys");
            entity.Property(e => e.TypeOfActivitys)
                .HasMaxLength(50)
                .HasColumnName("typeOfActivitys");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("roles_pkey");

            entity.Property(e => e.IdRole)
                .HasDefaultValueSql("nextval('roles_id_seq'::regclass)")
                .HasColumnName("id_role");
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
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("role_permissions_role_id_fkey"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("role_permissions_pkey");
                        j.ToTable("RolePermissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<RoleInEvent>(entity =>
        {
            entity.HasKey(e => e.IdRoleInEvent).HasName("roleinevent_pk");

            entity.ToTable("RoleInEvent");

            entity.Property(e => e.IdRoleInEvent)
                .ValueGeneratedNever()
                .HasColumnName("idRoleInEvent");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<RolesInSg>(entity =>
        {
            entity.HasKey(e => e.IdRoleSg).HasName("rolesinsg_pk");

            entity.ToTable("RolesInSG");

            entity.Property(e => e.IdRoleSg)
                .ValueGeneratedNever()
                .HasColumnName("idRoleSG");
            entity.Property(e => e.NameRole)
                .HasMaxLength(50)
                .HasColumnName("nameRole");
            entity.Property(e => e.PointsFac)
                .HasMaxLength(50)
                .HasColumnName("points_fac");
            entity.Property(e => e.PointsUni)
                .HasColumnType("character varying")
                .HasColumnName("points_uni");
        });

        modelBuilder.Entity<SelectiveDetail>(entity =>
        {
            entity.HasKey(e => e.IdSelectiveDetails).HasName("adddetails_pk");

            entity.Property(e => e.IdSelectiveDetails)
                .ValueGeneratedNever()
                .HasColumnName("idSelectiveDetails");
            entity.Property(e => e.DisciplineTopics).HasMaxLength(1024);
            entity.Property(e => e.Idtypeofcontroll).HasColumnName("idtypeofcontroll");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Prerequisites).HasMaxLength(50);
            entity.Property(e => e.Provision).HasMaxLength(256);
            entity.Property(e => e.Recomend).HasMaxLength(64);
            entity.Property(e => e.ResultEducation).HasMaxLength(64);
            entity.Property(e => e.Teachers).HasMaxLength(50);
            entity.Property(e => e.TypesOfTraining).HasMaxLength(50);
            entity.Property(e => e.UsingIrl)
                .HasMaxLength(50)
                .HasColumnName("UsingIRL");
            entity.Property(e => e.WhyInterestingDetermination).HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany(p => p.SelectiveDetails)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("adddetails_department_fk");

            entity.HasOne(d => d.IdSelectiveDetailsNavigation).WithOne(p => p.SelectiveDetail)
                .HasForeignKey<SelectiveDetail>(d => d.IdSelectiveDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adddetails_adddisciplines_fk");

            entity.HasOne(d => d.IdtypeofcontrollNavigation).WithMany(p => p.SelectiveDetails)
                .HasForeignKey(d => d.Idtypeofcontroll)
                .HasConstraintName("adddetails_typeofcontroll_fk");
        });

        modelBuilder.Entity<SelectiveDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdSelectiveDisciplines).HasName("adddisciplines_pk");

            entity.Property(e => e.IdSelectiveDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idSelectiveDisciplines");
            entity.Property(e => e.CodeSelectiveDisciplines)
                .HasMaxLength(50)
                .HasColumnName("codeSelectiveDisciplines");
            entity.Property(e => e.Feedback)
                .HasColumnType("jsonb")
                .HasColumnName("feedback");
            entity.Property(e => e.IdCatalog).HasColumnName("idCatalog");
            entity.Property(e => e.Idsimilar).HasColumnName("idsimilar");
            entity.Property(e => e.IsFaculty).HasColumnName("isFaculty");
            entity.Property(e => e.MaxCountPeople).HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse).HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople).HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse).HasColumnName("minCourse");
            entity.Property(e => e.NameSelectiveDisciplines)
                .HasMaxLength(300)
                .HasColumnName("nameSelectiveDisciplines");
            entity.Property(e => e.Recomendet)
                .HasColumnType("jsonb")
                .HasColumnName("recomendet");

            entity.HasOne(d => d.ApprovalStatus).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.ApprovalStatusId)
                .HasConstraintName("selectivedisciplines_approval_fk");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("adddisciplines_educationaldegree_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("adddisciplines_faculties_fk");

            entity.HasOne(d => d.IdCatalogNavigation).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.IdCatalog)
                .HasConstraintName("adddisciplines_catalogyears_selective_fk");

            entity.HasOne(d => d.Type).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("adddisciplines_typeofdiscipline_fk");
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity.HasKey(e => e.IdSpeciality).HasName("speciality_pk");

            entity.ToTable("Speciality");

            entity.HasIndex(e => e.IdSpeciality, "speciality_unique").IsUnique();

            entity.Property(e => e.IdSpeciality)
                .ValueGeneratedNever()
                .HasColumnName("idSpeciality");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(50)
                .HasColumnName("accreditationType");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IdBranch).HasColumnName("idBranch");
            entity.Property(e => e.IdDepartment).HasColumnName("idDepartment");
            entity.Property(e => e.LicensedVolume).HasColumnName("licensedVolume");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.IdBranchNavigation).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.IdBranch)
                .HasConstraintName("speciality_branch_fk");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("speciality_department_fk");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.IdSpecialization).HasName("specialization_pk");

            entity.ToTable("Specialization");

            entity.Property(e => e.IdSpecialization)
                .ValueGeneratedNever()
                .HasColumnName("idSpecialization");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IdSpeciality).HasColumnName("idSpeciality");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("student_pk");

            entity.ToTable("Student");

            entity.Property(e => e.IdStudent)
                .ValueGeneratedNever()
                .HasColumnName("idStudent");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.DepartmentId).HasDefaultValue(1);
            entity.Property(e => e.EducationEnd).HasColumnName("educationEnd");
            entity.Property(e => e.EducationStart).HasColumnName("educationStart");
            entity.Property(e => e.IdFav).HasColumnName("idFav");
            entity.Property(e => e.IsFunded)
                .HasColumnType("bit(1)")
                .HasColumnName("isFunded");
            entity.Property(e => e.IsInSg)
                .HasColumnType("bit(1)")
                .HasColumnName("isInSG");
            entity.Property(e => e.IsShort).HasColumnName("isShort");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");
            entity.Property(e => e.ReportCard)
                .HasColumnType("character varying")
                .HasColumnName("reportCard");

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("student_department_fk");

            entity.HasOne(d => d.EducationStatus).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_educationstatus_fk");

            entity.HasOne(d => d.EducationalDegree).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationalDegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_educationaldegree_fk");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_educationalprogram_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_faculties_fk");

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_group_fk");

            entity.HasOne(d => d.StudyForm).WithMany(p => p.Students)
                .HasForeignKey(d => d.StudyFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_studyform_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_users_fk");
        });

        modelBuilder.Entity<StudentGroup>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("group_pk");

            entity.ToTable("StudentGroup");

            entity.Property(e => e.IdGroup)
                .ValueGeneratedNever()
                .HasColumnName("idGroup");
            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.Admissionyear).HasColumnName("admissionyear");
            entity.Property(e => e.GroupCode).HasMaxLength(45);
            entity.Property(e => e.IdEducationalProgram).HasColumnName("idEducationalProgram");
            entity.Property(e => e.IdStudyForm).HasColumnName("idStudyForm");
            entity.Property(e => e.IsAccelerated).HasColumnType("bit(1)");

            entity.HasOne(d => d.Admin).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("group_adminspersonal_fk");

            entity.HasOne(d => d.Degree).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.DegreeId)
                .HasConstraintName("group_educationaldegree_fk");

            entity.HasOne(d => d.IdEducationalProgramNavigation).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.IdEducationalProgram)
                .HasConstraintName("studentgroup_educationalprogram_fk");

            entity.HasOne(d => d.IdStudyFormNavigation).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.IdStudyForm)
                .HasConstraintName("group_studyform_fk");
        });

        modelBuilder.Entity<StudyForm>(entity =>
        {
            entity.HasKey(e => e.IdStudyForm).HasName("studyform_pk");

            entity.ToTable("StudyForm");

            entity.Property(e => e.IdStudyForm)
                .ValueGeneratedNever()
                .HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasMaxLength(50)
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<SubDivisionsSg>(entity =>
        {
            entity.HasKey(e => e.IdSubDivision).HasName("subdivisionssg_pk");

            entity.ToTable("SubDivisionsSG");

            entity.Property(e => e.IdSubDivision)
                .ValueGeneratedNever()
                .HasColumnName("idSubDivision");
            entity.Property(e => e.NameDivision).HasMaxLength(50);
        });

        modelBuilder.Entity<TypeOfControl>(entity =>
        {
            entity.HasKey(e => e.Idtypeofcontroll).HasName("typeofcontroll_pk");

            entity.ToTable("TypeOfControl");

            entity.Property(e => e.Idtypeofcontroll)
                .ValueGeneratedNever()
                .HasColumnName("idtypeofcontroll");
            entity.Property(e => e.Type).HasColumnType("character varying");
        });

        modelBuilder.Entity<TypeOfDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfDiscipline).HasName("typeofdiscipline_pk");

            entity.ToTable("TypeOfDiscipline");

            entity.Property(e => e.IdTypeOfDiscipline)
                .ValueGeneratedNever()
                .HasColumnName("idTypeOfDiscipline");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("typeName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pk");

            entity.HasIndex(e => e.IdUser, "users_idusers_idx");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("idUser");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.IsFirstLogin)
                .HasColumnType("bit(1)")
                .HasColumnName("isFirstLogin");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastLoginAt");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("passwordChangedAt");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.PasswordSalt).HasColumnName("passwordSalt");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
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
                        j.ToTable("UserRoles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
