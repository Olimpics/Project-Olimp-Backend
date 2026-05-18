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

    public virtual DbSet<AcademicDegree> AcademicDegrees { get; set; }

    public virtual DbSet<AccountingJournal> AccountingJournals { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<AdminsPersonal> AdminsPersonals { get; set; }

    public virtual DbSet<Approval> Approvals { get; set; }

    public virtual DbSet<BindEventStudent> BindEventStudents { get; set; }

    public virtual DbSet<BindExtraActivity> BindExtraActivities { get; set; }

    public virtual DbSet<BindLoansMain> BindLoansMains { get; set; }

    public virtual DbSet<BindMainDiscipline> BindMainDisciplines { get; set; }

    public virtual DbSet<BindRating> BindRatings { get; set; }

    public virtual DbSet<BindSelectiveDiscipline> BindSelectiveDisciplines { get; set; }

    public virtual DbSet<BindSimilaEducationalProgramInGroup> BindSimilaEducationalProgramInGroups { get; set; }

    public virtual DbSet<BindSimilarSelectiveInGroup> BindSimilarSelectiveInGroups { get; set; }

    public virtual DbSet<BindSubdivisionRoleSg> BindSubdivisionRoleSgs { get; set; }

    public virtual DbSet<BindTeacherMain> BindTeacherMains { get; set; }

    public virtual DbSet<BindTeachersSelective> BindTeachersSelectives { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CatalogYear> CatalogYears { get; set; }

    public virtual DbSet<CatalogYearsMain> CatalogYearsMains { get; set; }

    public virtual DbSet<CatalogYearsSelective> CatalogYearsSelectives { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }

    public virtual DbSet<DefaultUniNeed> DefaultUniNeeds { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DeviceTransferSession> DeviceTransferSessions { get; set; }

    public virtual DbSet<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; }

    public virtual DbSet<EducationStatus> EducationStatuses { get; set; }

    public virtual DbSet<EducationalDegree> EducationalDegrees { get; set; }

    public virtual DbSet<EducationalProgram> EducationalPrograms { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<GroupSimilarEducationalProgram> GroupSimilarEducationalPrograms { get; set; }

    public virtual DbSet<GroupSimilarSelective> GroupSimilarSelectives { get; set; }

    public virtual DbSet<InventorySg> InventorySgs { get; set; }

    public virtual DbSet<MainDiscipline> MainDisciplines { get; set; }

    public virtual DbSet<MarkOfScore> MarkOfScores { get; set; }

    public virtual DbSet<MembersOfSg> MembersOfSgs { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Normative> Normatives { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PreKey> PreKeys { get; set; }

    public virtual DbSet<Prerequisite> Prerequisites { get; set; }

    public virtual DbSet<RatingCalculationTime> RatingCalculationTimes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RegulationOnAddPoint> RegulationOnAddPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleInEvent> RoleInEvents { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<RolesInSg> RolesInSgs { get; set; }

    public virtual DbSet<SelectiveDetail> SelectiveDetails { get; set; }

    public virtual DbSet<SelectiveDiscipline> SelectiveDisciplines { get; set; }

    public virtual DbSet<SemestersStart> SemestersStarts { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentGroup> StudentGroups { get; set; }

    public virtual DbSet<StudyForm> StudyForms { get; set; }

    public virtual DbSet<SubDivisionsSg> SubDivisionsSgs { get; set; }

    public virtual DbSet<TypeOfControl> TypeOfControls { get; set; }

    public virtual DbSet<TypeOfDiscipline> TypeOfDisciplines { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=project_olymp_db;Username=postgres;Password=B25824DCABCB88B5;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<AcademicDegree>(entity =>
        {
            entity.HasKey(e => e.IdAcademicDegree).HasName("academicdegree_pk");

            entity.ToTable("AcademicDegree");

            entity.Property(e => e.IdAcademicDegree)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idAcademicDegree");
            entity.Property(e => e.AcademicDegreeName)
                .HasColumnType("character varying")
                .HasColumnName("academicDegreeName");
            entity.Property(e => e.AcademicDegreeShortedName)
                .HasColumnType("character varying")
                .HasColumnName("academicDegreeShortedName");
        });

        modelBuilder.Entity<AccountingJournal>(entity =>
        {
            entity.HasKey(e => e.IdAccountingJournal).HasName("accountingjournal_pk");

            entity.ToTable("AccountingJournal");

            entity.Property(e => e.IdAccountingJournal)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idAccountingJournal");
            entity.Property(e => e.Comment)
                .HasColumnType("character varying")
                .HasColumnName("comment");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.InventorySgid).HasColumnName("InventorySGId");
            entity.Property(e => e.IsBack)
                .HasDefaultValue(false)
                .HasColumnName("isBack");
            entity.Property(e => e.RealBackTime).HasColumnName("realBackTime");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("startDate");

            entity.HasOne(d => d.InventorySg).WithMany(p => p.AccountingJournals)
                .HasForeignKey(d => d.InventorySgid)
                .HasConstraintName("accountingjournal_inventorysg_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.AccountingJournals)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("accountingjournal_student_fk");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.IdAdminLogs).HasName("adminlogs_pk");

            entity.Property(e => e.IdAdminLogs)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idAdminLogs");
            entity.Property(e => e.Action).HasColumnType("character varying");
            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("character varying");
            entity.Property(e => e.NewData).HasColumnType("character varying");
            entity.Property(e => e.OldData).HasColumnType("character varying");
            entity.Property(e => e.TableName).HasColumnType("character varying");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("adminlogs_adminspersonal_fk");
        });

        modelBuilder.Entity<AdminsPersonal>(entity =>
        {
            entity.HasKey(e => e.IdAdmins).HasName("adminspersonal_pk");

            entity.ToTable("AdminsPersonal");

            entity.HasIndex(e => e.UserId, "adminspersonal_unique").IsUnique();

            entity.Property(e => e.IdAdmins)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idAdmins");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.SecondName)
                .HasColumnType("character varying")
                .HasColumnName("second_name");
            entity.Property(e => e.ThirdName)
                .HasColumnType("character varying")
                .HasColumnName("third_name");

            entity.HasOne(d => d.AcademicDegree).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.AcademicDegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminspersonal_academicdegree_fk");

            entity.HasOne(d => d.Department).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminspersonal_department_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminspersonal_faculties_fk");

            entity.HasOne(d => d.User).WithOne(p => p.AdminsPersonal)
                .HasForeignKey<AdminsPersonal>(d => d.UserId)
                .HasConstraintName("adminspersonal_users_fk");
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.HasKey(e => e.IdApproval).HasName("approval_pk");

            entity.ToTable("Approval");

            entity.Property(e => e.IdApproval)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_approval");
            entity.Property(e => e.AppovalStatus)
                .HasColumnType("character varying")
                .HasColumnName("appovalStatus");
            entity.Property(e => e.ApprobalLevel).HasColumnName("approbal_level");

            entity.HasOne(d => d.Role).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("approval_roles_fk");
        });

        modelBuilder.Entity<BindEventStudent>(entity =>
        {
            entity.HasKey(e => e.IdBindEventStudent).HasName("bindeventstudent_pk");

            entity.ToTable("BindEventStudent");

            entity.Property(e => e.IdBindEventStudent)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idBindEventStudent");
            entity.Property(e => e.OtherOption)
                .HasColumnType("character varying")
                .HasColumnName("otherOption");
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
            entity
                .HasNoKey()
                .ToTable("BindExtraActivity");

            entity.Property(e => e.IdBindExtraActivity)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idBindExtraActivity");
            entity.Property(e => e.NameExtraActivity)
                .HasColumnType("character varying")
                .HasColumnName("nameExtraActivity");
            entity.Property(e => e.Points).HasColumnName("points");

            entity.HasOne(d => d.Regulation).WithMany()
                .HasForeignKey(d => d.RegulationId)
                .HasConstraintName("bindextraactivity_regulationonaddpoints_fk");

            entity.HasOne(d => d.Student).WithMany()
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindextraactivity_student_fk");
        });

        modelBuilder.Entity<BindLoansMain>(entity =>
        {
            entity.HasKey(e => e.IdBindLoanMain).HasName("bindloansmain_pk");

            entity.ToTable("BindLoansMain");

            entity.Property(e => e.IdBindLoanMain)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind_loan_main");
            entity.Property(e => e.SelectiveDisciplinesId).HasColumnName("selective_disciplines_id");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("bindloansmain_educationalprogram_fk");

            entity.HasOne(d => d.SelectiveDisciplines).WithMany(p => p.BindLoansMains)
                .HasForeignKey(d => d.SelectiveDisciplinesId)
                .HasConstraintName("bindloansmain_selectivedisciplines_fk");
        });

        modelBuilder.Entity<BindMainDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindMainDisciplines).HasName("bindmaindisciplines_pk");

            entity.Property(e => e.IdBindMainDisciplines)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind_main_disciplines");
            entity.Property(e => e.Grade)
                .HasDefaultValue(0)
                .HasColumnName("grade");
            entity.Property(e => e.IsRedo)
                .HasDefaultValue(false)
                .HasColumnName("isRedo");
            entity.Property(e => e.Semestr).HasColumnName("semestr");
            entity.Property(e => e.YearId).HasColumnName("year_id");

            entity.HasOne(d => d.MainDisciplines).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.MainDisciplinesId)
                .HasConstraintName("bindmaindisciplines_maindisciplines_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindmaindisciplines_student_fk");

            entity.HasOne(d => d.Year).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.YearId)
                .HasConstraintName("bindmaindisciplines_catalogyear_fk");
        });

        modelBuilder.Entity<BindRating>(entity =>
        {
            entity.HasKey(e => e.IdBindRating).HasName("bindrating_pk");

            entity.ToTable("BindRating");

            entity.Property(e => e.IdBindRating)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind_rating");
            entity.Property(e => e.FinalScore).HasColumnName("finalScore");
            entity.Property(e => e.IsEven)
                .HasDefaultValue(false)
                .HasColumnName("is_even");
            entity.Property(e => e.IsRedo)
                .HasDefaultValue(false)
                .HasColumnName("is_redo");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Student).WithMany(p => p.BindRatings)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bindrating_student_fk");
        });

        modelBuilder.Entity<BindSelectiveDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindSelectiveDisciplines).HasName("bindselectivedisciplines_pk");

            entity.Property(e => e.IdBindSelectiveDisciplines)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind_selective_disciplines");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Grade)
                .HasDefaultValue(0)
                .HasColumnName("grade");
            entity.Property(e => e.InProcess)
                .HasDefaultValue(true)
                .HasColumnName("in_process");
            entity.Property(e => e.IsRedo)
                .HasDefaultValue(false)
                .HasColumnName("is_redo");
            entity.Property(e => e.Loans).HasColumnName("loans");
            entity.Property(e => e.NeedReview)
                .HasDefaultValue(false)
                .HasColumnName("need_review");
            entity.Property(e => e.SelectiveDisciplineId).HasColumnName("selective_discipline_id");
            entity.Property(e => e.Semestr).HasColumnName("semestr");
            entity.Property(e => e.StudentAssessment)
                .HasComment("Оцінка дисципліни студентом")
                .HasColumnName("studentAssessment");
            entity.Property(e => e.YearId).HasColumnName("year_id");

            entity.HasOne(d => d.SelectiveDiscipline).WithMany(p => p.BindSelectiveDisciplines)
                .HasForeignKey(d => d.SelectiveDisciplineId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("bindselectivedisciplines_selectivedisciplines_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.BindSelectiveDisciplines)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("bindselectivedisciplines_student_fk");

            entity.HasOne(d => d.Year).WithMany(p => p.BindSelectiveDisciplines)
                .HasForeignKey(d => d.YearId)
                .HasConstraintName("bindselectivedisciplines_catalogyear_fk");
        });

        modelBuilder.Entity<BindSimilaEducationalProgramInGroup>(entity =>
        {
            entity.HasKey(e => e.IdBind).HasName("bindsimilaeducationalprogramingroup_pk");

            entity.ToTable("BindSimilaEducationalProgramInGroup");

            entity.HasIndex(e => e.EducationalProgramId, "bindsimilaeducationalprogramingroup_educationalprogramid_idx");

            entity.HasIndex(e => e.GroupId, "bindsimilaeducationalprogramingroup_groupid_idx");

            entity.Property(e => e.IdBind)
                .ValueGeneratedNever()
                .HasColumnName("idBind");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindSimilaEducationalProgramInGroups)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("bindsimilaeducationalprogramingroup_educationalprogram_fk");

            entity.HasOne(d => d.Group).WithMany(p => p.BindSimilaEducationalProgramInGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("bindsimilaeducationalprogramingroup_groupsimilareducationalprog");
        });

        modelBuilder.Entity<BindSimilarSelectiveInGroup>(entity =>
        {
            entity.HasKey(e => e.IdBind).HasName("bindsimilarselectiveingroup_pk");

            entity.ToTable("BindSimilarSelectiveInGroup");

            entity.Property(e => e.IdBind)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.SelectiveId).HasColumnName("selective_id");

            entity.HasOne(d => d.Group).WithMany(p => p.BindSimilarSelectiveInGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("bindsimilarselectiveingroup_groupsimilarselective_fk");

            entity.HasOne(d => d.Selective).WithMany(p => p.BindSimilarSelectiveInGroups)
                .HasForeignKey(d => d.SelectiveId)
                .HasConstraintName("bindsimilarselectiveingroup_selectivedisciplines_fk");
        });

        modelBuilder.Entity<BindSubdivisionRoleSg>(entity =>
        {
            entity.HasKey(e => e.IdBindSubdivisionRoleSg).HasName("bindsubdivisionrolesg_pk");

            entity.ToTable("BindSubdivisionRoleSG");

            entity.Property(e => e.IdBindSubdivisionRoleSg)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idBindSubdivisionRoleSG");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.RoleInSgid).HasColumnName("RoleInSGId");

            entity.HasOne(d => d.RoleInSg).WithMany(p => p.BindSubdivisionRoleSgs)
                .HasForeignKey(d => d.RoleInSgid)
                .HasConstraintName("bindsubdivisionrolesg_rolesinsg_fk");

            entity.HasOne(d => d.SubDivision).WithMany(p => p.BindSubdivisionRoleSgs)
                .HasForeignKey(d => d.SubDivisionId)
                .HasConstraintName("bindsubdivisionrolesg_subdivisionssg_fk");
        });

        modelBuilder.Entity<BindTeacherMain>(entity =>
        {
            entity.HasKey(e => e.IdBindTeacherMain).HasName("bindteachermain_pk");

            entity.ToTable("BindTeacherMain");

            entity.Property(e => e.IdBindTeacherMain)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idBindTeacherMain");
            entity.Property(e => e.IsHead)
                .HasDefaultValue(false)
                .HasColumnName("isHead");

            entity.HasOne(d => d.Admin).WithMany(p => p.BindTeacherMains)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("bindteachermain_adminspersonal_fk");

            entity.HasOne(d => d.MainDisciplines).WithMany(p => p.BindTeacherMains)
                .HasForeignKey(d => d.MainDisciplinesId)
                .HasConstraintName("bindteachermain_maindisciplines_fk");
        });

        modelBuilder.Entity<BindTeachersSelective>(entity =>
        {
            entity.HasKey(e => e.IdBindTeacherSelective).HasName("bindteachersselective_pk");

            entity.ToTable("BindTeachersSelective");

            entity.Property(e => e.IdBindTeacherSelective)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_bind_teacher_selective");
            entity.Property(e => e.IsHead)
                .HasDefaultValue(true)
                .HasColumnName("is_head");
            entity.Property(e => e.SelectiveDisciplinesId).HasColumnName("selective_disciplines_id");

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
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_branch");
            entity.Property(e => e.Avail)
                .HasDefaultValue(false)
                .HasColumnName("avail");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<CatalogYear>(entity =>
        {
            entity.HasKey(e => e.IdCatalog).HasName("catalogyear_pk");

            entity.ToTable("CatalogYear");

            entity.Property(e => e.IdCatalog)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_catalog");
            entity.Property(e => e.YearEnd).HasColumnName("yearEnd");
            entity.Property(e => e.YearStart).HasColumnName("yearStart");
        });

        modelBuilder.Entity<CatalogYearsMain>(entity =>
        {
            entity.HasKey(e => e.IdCatalogYear).HasName("catalogyears_main_pk");

            entity.ToTable("CatalogYears_Main");

            entity.Property(e => e.IdCatalogYear)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idCatalogYear");
            entity.Property(e => e.IsFormed)
                .HasDefaultValue(false)
                .HasColumnName("isFormed");
            entity.Property(e => e.YearEnd).HasColumnName("yearEnd");
            entity.Property(e => e.YearStart).HasColumnName("yearStart");
        });

        modelBuilder.Entity<CatalogYearsSelective>(entity =>
        {
            entity.HasKey(e => e.IdCatalogYearSelective).HasName("catalogyears_selective_pk");

            entity.ToTable("CatalogYears_Selective");

            entity.Property(e => e.IdCatalogYearSelective)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_catalog_year_selective");
            entity.Property(e => e.IsFormed)
                .HasDefaultValue(false)
                .HasColumnName("is_formed");
            entity.Property(e => e.YearEnd).HasColumnName("yearEnd");
            entity.Property(e => e.YearStart).HasColumnName("yearStart");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.IdConversation).HasName("conversations_pkey");

            entity.HasIndex(e => e.ConversationToken, "conversations_conversation_token_key").IsUnique();

            entity.Property(e => e.IdConversation)
                .ValueGeneratedNever()
                .HasColumnName("idConversation");
            entity.Property(e => e.ConversationToken).HasColumnName("conversationToken");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsAnonymous)
                .HasDefaultValue(false)
                .HasColumnName("isAnonymous");
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => e.IdConversationParticipants).HasName("conversation_participants_pkey");

            entity.Property(e => e.IdConversationParticipants)
                .ValueGeneratedNever()
                .HasColumnName("idConversationParticipants");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.EncryptedParticipant).HasColumnName("encryptedParticipant");
            entity.Property(e => e.IsIdentityRevealed)
                .HasDefaultValue(true)
                .HasColumnName("isIdentityRevealed");
            entity.Property(e => e.Pseudonym)
                .HasColumnType("character varying")
                .HasColumnName("pseudonym");

            entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationParticipants)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("conversation_participants_conversation_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ConversationParticipants)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("conversationparticipants_users_fk");
        });

        modelBuilder.Entity<DefaultUniNeed>(entity =>
        {
            entity.HasKey(e => e.IdUniNeeds).HasName("defaultunineeds_pk");

            entity.Property(e => e.IdUniNeeds)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idUniNeeds");
            entity.Property(e => e.Choice).HasColumnName("choice");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.IsAccelerated)
                .HasDefaultValue(false)
                .HasColumnName("isAccelerated");
            entity.Property(e => e.SemestrIsEven)
                .HasDefaultValue(false)
                .HasColumnName("semestrIsEven");

            entity.HasOne(d => d.EducationalDegree).WithMany(p => p.DefaultUniNeeds)
                .HasForeignKey(d => d.EducationalDegreeId)
                .HasConstraintName("defaultunineeds_educationaldegree_fk");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("department_pk");

            entity.ToTable("Department");

            entity.Property(e => e.IdDepartment)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_department");
            entity.Property(e => e.Abbreviation)
                .HasColumnType("character varying")
                .HasColumnName("abbreviation");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.NameDepartment)
                .HasColumnType("character varying")
                .HasColumnName("nameDepartment");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("department_faculties_fk");
        });

        modelBuilder.Entity<DeviceTransferSession>(entity =>
        {
            entity.HasKey(e => e.IdDeviceTransferSessions).HasName("DeviceTransferSessions_pkey");

            entity.HasIndex(e => e.ExpiresAt, "IxDeviceTransferSessionsExpiresAt");

            entity.HasIndex(e => e.TransferSessionToken, "IxDeviceTransferSessionsTransferSessionToken").IsUnique();

            entity.HasIndex(e => e.UserId, "IxDeviceTransferSessionsUserId");

            entity.HasIndex(e => e.IsCompleted, "devicetransfersessions_iscompleted_idx");

            entity.HasIndex(e => e.IsExpired, "devicetransfersessions_isexpired_idx");

            entity.Property(e => e.IdDeviceTransferSessions)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idDeviceTransferSessions");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.EncryptedTransferPayload).HasColumnName("encryptedTransferPayload");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresAt");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("isCompleted");
            entity.Property(e => e.IsExpired)
                .HasDefaultValue(false)
                .HasColumnName("isExpired");
            entity.Property(e => e.NewDevicePublicKey).HasColumnName("newDevicePublicKey");
            entity.Property(e => e.OldDevicePublicKey).HasColumnName("oldDevicePublicKey");
            entity.Property(e => e.TransferCode)
                .HasColumnType("character varying")
                .HasColumnName("transferCode");
            entity.Property(e => e.TransferSessionToken)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("transferSessionToken");

            entity.HasOne(d => d.NewDevice).WithMany(p => p.DeviceTransferSessionNewDevices)
                .HasForeignKey(d => d.NewDeviceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FkDeviceTransferSessionsNewDevice");

            entity.HasOne(d => d.OldDevice).WithMany(p => p.DeviceTransferSessionOldDevices)
                .HasForeignKey(d => d.OldDeviceId)
                .HasConstraintName("FkDeviceTransferSessionsOldDevice");

            entity.HasOne(d => d.User).WithMany(p => p.DeviceTransferSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FkDeviceTransferSessionsUser");
        });

        modelBuilder.Entity<DisciplineChoicePeriod>(entity =>
        {
            entity.HasKey(e => e.IdDisciplineChoicePeriod).HasName("disciplinechoiceperiod_pk");

            entity.ToTable("DisciplineChoicePeriod");

            entity.Property(e => e.IdDisciplineChoicePeriod)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idDisciplineChoicePeriod");
            entity.Property(e => e.DegreeLevelId).HasColumnName("degreeLevel_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.EndOfCheckPeriod).HasColumnName("endOfCheckPeriod");
            entity.Property(e => e.IsClose)
                .HasDefaultValue(false)
                .HasColumnName("is_close");
            entity.Property(e => e.IsForOnSemestr)
                .HasDefaultValue(true)
                .HasColumnName("isForOnSemestr");
            entity.Property(e => e.PeriodCourse).HasColumnName("periodCourse");
            entity.Property(e => e.PeriodType)
                .HasColumnType("bit(1)")
                .HasColumnName("periodType");
            entity.Property(e => e.StartDate).HasColumnName("startDate");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DegreeLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("disciplinechoiceperiod_educationaldegree_fk");

            entity.HasOne(d => d.Department).WithMany(p => p.DisciplineChoicePeriods)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("disciplinechoiceperiod_department_fk");
        });

        modelBuilder.Entity<EducationStatus>(entity =>
        {
            entity.HasKey(e => e.IdEducationStatus).HasName("educationstatus_pk");

            entity.ToTable("EducationStatus");

            entity.Property(e => e.IdEducationStatus)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(50)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<EducationalDegree>(entity =>
        {
            entity.HasKey(e => e.Ideducationaldegree).HasName("educationaldegree_pk");

            entity.ToTable("EducationalDegree");

            entity.Property(e => e.Ideducationaldegree)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("ideducationaldegree");
            entity.Property(e => e.NameEducationalDegree)
                .HasColumnType("character varying")
                .HasColumnName("nameEducationalDegree");
            entity.Property(e => e.NameInDocuments)
                .HasColumnType("character varying")
                .HasColumnName("nameInDocuments");
        });

        modelBuilder.Entity<EducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdEducationalProgram).HasName("educationalprogram_pk");

            entity.ToTable("EducationalProgram");

            entity.Property(e => e.IdEducationalProgram)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasColumnType("character varying")
                .HasColumnName("accreditationType");
            entity.Property(e => e.Goals)
                .HasColumnType("character varying")
                .HasColumnName("goals");
            entity.Property(e => e.Instrument)
                .HasColumnType("character varying")
                .HasColumnName("instrument");
            entity.Property(e => e.IsAccelerated)
                .HasDefaultValue(false)
                .HasColumnName("is_accelerated");
            entity.Property(e => e.Keys)
                .HasColumnType("character varying[]")
                .HasColumnName("keys");
            entity.Property(e => e.Methodics)
                .HasColumnType("character varying")
                .HasColumnName("methodics");
            entity.Property(e => e.MinUniSelectiveDisciplineBySemestr).HasColumnName("minUniSelectiveDisciplineBySemestr");
            entity.Property(e => e.NameDock)
                .HasColumnType("character varying")
                .HasColumnName("nameDock");
            entity.Property(e => e.NameEducationalProgram)
                .HasColumnType("character varying")
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.NeedFix)
                .HasDefaultValue(false)
                .HasColumnName("need_fix");
            entity.Property(e => e.SelectiveDisciplineBySemestr).HasColumnName("selectiveDisciplineBySemestr");
            entity.Property(e => e.SpecialityId).HasColumnName("speciality_id");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
            entity.Property(e => e.StudyFormId).HasColumnName("study_form_id");
            entity.Property(e => e.StudyTurm)
                .HasColumnType("character varying")
                .HasColumnName("study_turm");
            entity.Property(e => e.Subject)
                .HasColumnType("character varying")
                .HasColumnName("subject");
            entity.Property(e => e.TheoreticalContent)
                .HasColumnType("character varying")
                .HasColumnName("theoreticalContent");

            entity.HasOne(d => d.Catalog).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.CatalogId)
                .HasConstraintName("educationalprogram_catalogyears_main_fk");

            entity.HasOne(d => d.Degree).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.DegreeId)
                .HasConstraintName("educationalprogram_educationaldegree_fk");

            entity.HasOne(d => d.Speciality).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.SpecialityId)
                .HasConstraintName("educationalprogram_speciality_fk");

            entity.HasOne(d => d.Specialization).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.SpecializationId)
                .HasConstraintName("educationalprogram_specialization_fk");

            entity.HasOne(d => d.StudyForm).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.StudyFormId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("educationalprogram_studyform_fk");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("events_pk");

            entity.Property(e => e.IdEvent)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idEvent");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
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
                .OnDelete(DeleteBehavior.ClientSetNull)
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
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_faculty");
            entity.Property(e => e.Abbreviation)
                .HasColumnType("character varying")
                .HasColumnName("abbreviation");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.NameFaculty)
                .HasColumnType("character varying")
                .HasColumnName("nameFaculty");
        });

        modelBuilder.Entity<GroupSimilarEducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("groupsimilareducationalprogram_pk");

            entity.ToTable("GroupSimilarEducationalProgram");

            entity.Property(e => e.IdGroup)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idGroup");
            entity.Property(e => e.GroupName)
                .HasColumnType("character varying")
                .HasColumnName("groupName");
        });

        modelBuilder.Entity<GroupSimilarSelective>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("groupsimilarselective_pk");

            entity.ToTable("GroupSimilarSelective");

            entity.Property(e => e.IdGroup)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_group");
            entity.Property(e => e.CentrallId).HasColumnName("centrall_id");
            entity.Property(e => e.GroupName)
                .HasColumnType("character varying")
                .HasColumnName("groupName");

            entity.HasOne(d => d.Centrall).WithMany(p => p.GroupSimilarSelectives)
                .HasForeignKey(d => d.CentrallId)
                .HasConstraintName("groupsimilarselective_selectivedisciplines_fk");
        });

        modelBuilder.Entity<InventorySg>(entity =>
        {
            entity.HasKey(e => e.IdInventoroy).HasName("inventorysg_pk");

            entity.ToTable("InventorySG");

            entity.Property(e => e.IdInventoroy)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idInventoroy");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
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
            entity.HasKey(e => e.IdMainDisciplines).HasName("maindisciplines_pk");

            entity.Property(e => e.IdMainDisciplines)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idMainDisciplines");
            entity.Property(e => e.CodeMainDisciplines)
                .HasColumnType("character varying")
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.Hours).HasColumnName("hours");
            entity.Property(e => e.Loans).HasColumnName("loans");
            entity.Property(e => e.NameDock)
                .HasColumnType("character varying")
                .HasColumnName("nameDock");
            entity.Property(e => e.NameMainDisciplines)
                .HasColumnType("character varying")
                .HasColumnName("nameMainDisciplines");
            entity.Property(e => e.NeedFix)
                .HasDefaultValue(false)
                .HasColumnName("needFix");
            entity.Property(e => e.Semestr).HasColumnName("semestr");
            entity.Property(e => e.TypeOfControl).HasColumnName("type_of_control");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.MainDisciplines)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("maindisciplines_educationalprogram_fk");

            entity.HasOne(d => d.TypeOfControlNavigation).WithMany(p => p.MainDisciplines)
                .HasForeignKey(d => d.TypeOfControl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("maindisciplines_typeofcontrol_fk");
        });

        modelBuilder.Entity<MarkOfScore>(entity =>
        {
            entity.HasKey(e => e.IdMark).HasName("markofscore_pk");

            entity.ToTable("MarkOfScore");

            entity.Property(e => e.IdMark)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_mark");
            entity.Property(e => e.MaxGrade).HasColumnName("maxGrade");
            entity.Property(e => e.MinGrade).HasColumnName("minGrade");
            entity.Property(e => e.NameOfGrade)
                .HasColumnType("character varying")
                .HasColumnName("nameOfGrade");
        });

        modelBuilder.Entity<MembersOfSg>(entity =>
        {
            entity.HasKey(e => e.IdMembersOfSg).HasName("membersofsg_pk");

            entity.ToTable("MembersOfSG");

            entity.Property(e => e.IdMembersOfSg)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idMembersOfSG");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.BindsubdivisionRoleSgid).HasColumnName("BindsubdivisionRoleSGId");

            entity.HasOne(d => d.BindsubdivisionRoleSg).WithMany(p => p.MembersOfSgs)
                .HasForeignKey(d => d.BindsubdivisionRoleSgid)
                .HasConstraintName("membersofsg_bindsubdivisionrolesg_fk");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MembersOfSgCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("membersofsg_studentcreator_fk");

            entity.HasOne(d => d.Student).WithMany(p => p.MembersOfSgStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("membersofsg_student_fk");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.IdMessage).HasName("messages_pkey");

            entity.HasIndex(e => new { e.ConversationId, e.CreatedAt }, "idx_messages_conversation_created").IsDescending(false, true);

            entity.Property(e => e.IdMessage)
                .ValueGeneratedNever()
                .HasColumnName("idMessage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deliveredAt");
            entity.Property(e => e.EncryptedPayload).HasColumnName("encryptedPayload");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.IsDelivered)
                .HasDefaultValue(false)
                .HasColumnName("isDelivered");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("isRead");
            entity.Property(e => e.Nonce).HasColumnName("nonce");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("readAt");
            entity.Property(e => e.SenderDevicePublicKey).HasColumnName("senderDevicePublicKey");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasPrincipalKey(p => p.ConversationToken)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("messages_conversations_fk");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("messages_users_fk");
        });

        modelBuilder.Entity<Normative>(entity =>
        {
            entity.HasKey(e => e.IdNormative).HasName("normative_pk");

            entity.ToTable("Normative");

            entity.Property(e => e.IdNormative)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idNormative");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.IsFaculty)
                .HasDefaultValue(false)
                .HasColumnName("isFaculty");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.Normatives)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("normative_educationaldegree_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("notifications_pk");

            entity.Property(e => e.IdNotification)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idNotification");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomMessage)
                .HasColumnType("character varying")
                .HasColumnName("customMessage");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("isRead");
            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb")
                .HasColumnName("metadata");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NotificationCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("notification_created_by_fk");

            entity.HasOne(d => d.Template).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("notifications_notificationtemplates_fk");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_users_fk");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.IdNotificationTemplates).HasName("notificationtemplates_pk");

            entity.Property(e => e.IdNotificationTemplates)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idNotificationTemplates");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.Message)
                .HasColumnType("character varying")
                .HasColumnName("message");
            entity.Property(e => e.NotificationType)
                .HasColumnType("character varying")
                .HasColumnName("notificationType");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.IdPermission).HasName("permissions_pk");

            entity.HasIndex(e => e.BitIndex, "Permissions_bitIndex_key").IsUnique();

            entity.HasIndex(e => e.Code, "Permissions_code_key").IsUnique();

            entity.HasIndex(e => e.Code, "permissions_code_key").IsUnique();

            entity.Property(e => e.IdPermission)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idPermission");
            entity.Property(e => e.BitIndex).HasColumnName("bitIndex");
            entity.Property(e => e.Code).HasColumnName("code");
        });

        modelBuilder.Entity<PreKey>(entity =>
        {
            entity.HasKey(e => e.IdPreKeys).HasName("pre_keys_pkey");

            entity.Property(e => e.IdPreKeys)
                .ValueGeneratedNever()
                .HasColumnName("idPreKeys");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("isUsed");
            entity.Property(e => e.PublicPreKey).HasColumnName("publicPreKey");

            entity.HasOne(d => d.Device).WithMany(p => p.PreKeys)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pre_keys_device_id_fkey");
        });

        modelBuilder.Entity<Prerequisite>(entity =>
        {
            entity.HasKey(e => e.IdPrerequisites).HasName("prerequisites_pk");

            entity.Property(e => e.IdPrerequisites)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_prerequisites");
            entity.Property(e => e.SelectiveDisciplineId).HasColumnName("selective_discipline_id");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("prerequisites_educationalprogram_fk");

            entity.HasOne(d => d.SelectiveDiscipline).WithMany(p => p.Prerequisites)
                .HasForeignKey(d => d.SelectiveDisciplineId)
                .HasConstraintName("prerequisites_selectivedisciplines_fk");
        });

        modelBuilder.Entity<RatingCalculationTime>(entity =>
        {
            entity.HasKey(e => e.IdRatingCalculationTime).HasName("ratingcalculationtime_pk");

            entity.ToTable("RatingCalculationTime");

            entity.Property(e => e.IdRatingCalculationTime)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_rating_calculation_time");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnName("date");
            entity.Property(e => e.IsEven)
                .HasDefaultValue(false)
                .HasColumnName("is_even");
            entity.Property(e => e.IsShorted)
                .HasDefaultValue(false)
                .HasColumnName("is_shorted");
            entity.Property(e => e.SpecialityId).HasColumnName("speciality_id");
            entity.Property(e => e.YearId).HasColumnName("year_id");

            entity.HasOne(d => d.Speciality).WithMany(p => p.RatingCalculationTimes)
                .HasForeignKey(d => d.SpecialityId)
                .HasConstraintName("ratingcalculationtime_speciality_fk");

            entity.HasOne(d => d.Year).WithMany(p => p.RatingCalculationTimes)
                .HasForeignKey(d => d.YearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratingcalculationtime_catalogyear_fk");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.IdRefreshTokens).HasName("refresh_tokens_pkey");

            entity.Property(e => e.IdRefreshTokens)
                .ValueGeneratedNever()
                .HasColumnName("idRefreshTokens");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiresAt");
            entity.Property(e => e.Token).HasColumnName("token");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<RegulationOnAddPoint>(entity =>
        {
            entity.HasKey(e => e.IdRegulationOnAddPoints).HasName("regulationonaddpoints_pk");

            entity.HasIndex(e => e.CodeRegulationOnAddPoints, "regulationonaddpoints_coderegulationonaddpoints_idx");

            entity.Property(e => e.IdRegulationOnAddPoints)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idRegulationOnAddPoints");
            entity.Property(e => e.AmountMax).HasColumnName("amountMax");
            entity.Property(e => e.AmountMin).HasColumnName("amountMin");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.CodeRegulationOnAddPoints)
                .HasColumnType("character varying")
                .HasColumnName("codeRegulationOnAddPoints");
            entity.Property(e => e.Notes)
                .HasColumnType("character varying")
                .HasColumnName("notes");
            entity.Property(e => e.SubTypeOfActivitys)
                .HasColumnType("character varying")
                .HasColumnName("subTypeOfActivitys");
            entity.Property(e => e.TypeOfActivitys)
                .HasColumnType("character varying")
                .HasColumnName("typeOfActivitys");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("roles_pk");

            entity.Property(e => e.IdRole)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idRole");
            entity.Property(e => e.IsSystem)
                .HasDefaultValue(true)
                .HasColumnName("is_system");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ParentRoleId).HasColumnName("parentRoleId");
            entity.Property(e => e.PermissionsMask)
                .HasDefaultValue(0L)
                .HasColumnName("permissionsMask");
        });

        modelBuilder.Entity<RoleInEvent>(entity =>
        {
            entity.HasKey(e => e.IdRoleInEvent).HasName("roleinevent_pk");

            entity.ToTable("RoleInEvent");

            entity.Property(e => e.IdRoleInEvent)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idRoleInEvent");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.IdRolePermission).HasName("rolepermissions_pk");

            entity.Property(e => e.IdRolePermission)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idRolePermission");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("rolepermissions_permissions_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("rolepermissions_roles_fk");
        });

        modelBuilder.Entity<RolesInSg>(entity =>
        {
            entity.HasKey(e => e.IdRoleSg).HasName("rolesinsg_pk");

            entity.ToTable("RolesInSG");

            entity.Property(e => e.IdRoleSg)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idRoleSG");
            entity.Property(e => e.NameRole)
                .HasColumnType("character varying")
                .HasColumnName("nameRole");
            entity.Property(e => e.PointsFac).HasColumnName("pointsFac");
            entity.Property(e => e.PointsUni).HasColumnName("pointsUni");
        });

        modelBuilder.Entity<SelectiveDetail>(entity =>
        {
            entity.HasKey(e => e.IdSelectiveDetails).HasName("selectivedetails_pk");

            entity.Property(e => e.IdSelectiveDetails)
                .ValueGeneratedNever()
                .HasColumnName("id_selective_details");
            entity.Property(e => e.DisciplineTopics)
                .HasColumnType("character varying[]")
                .HasColumnName("disciplineTopics");
            entity.Property(e => e.Language)
                .HasColumnType("character varying")
                .HasColumnName("language");
            entity.Property(e => e.NameSelectiveDisciplinesEng)
                .HasColumnType("character varying")
                .HasColumnName("nameSelectiveDisciplinesEng");
            entity.Property(e => e.Prerequisites)
                .HasColumnType("character varying")
                .HasColumnName("prerequisites");
            entity.Property(e => e.Provision)
                .HasColumnType("character varying")
                .HasColumnName("provision");
            entity.Property(e => e.Recommended)
                .HasColumnType("jsonb")
                .HasColumnName("recommended");
            entity.Property(e => e.ResultEducation)
                .HasColumnType("character varying")
                .HasColumnName("resultEducation");
            entity.Property(e => e.Teachers).HasColumnType("character varying");
            entity.Property(e => e.TypesOfTraining)
                .HasColumnType("character varying")
                .HasColumnName("typesOfTraining");
            entity.Property(e => e.UsingIrl)
                .HasColumnType("character varying")
                .HasColumnName("usingIRL");
            entity.Property(e => e.WhyInterestingDetermination)
                .HasColumnType("character varying")
                .HasColumnName("whyInterestingDetermination");

            entity.HasOne(d => d.IdSelectiveDetailsNavigation).WithOne(p => p.SelectiveDetail)
                .HasForeignKey<SelectiveDetail>(d => d.IdSelectiveDetails)
                .HasConstraintName("selectivedetails_selectivedisciplines_fk");
        });

        modelBuilder.Entity<SelectiveDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdSelectiveDisciplines).HasName("selectivedisciplines_pk");

            entity.Property(e => e.IdSelectiveDisciplines)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_selective_disciplines");
            entity.Property(e => e.ApprovalStatusId).HasColumnName("approval_status_id");
            entity.Property(e => e.CatalogId).HasColumnName("catalog_id");
            entity.Property(e => e.CodeSelectiveDisciplines)
                .HasColumnType("character varying")
                .HasColumnName("codeSelectiveDisciplines");
            entity.Property(e => e.Courses).HasColumnName("courses");
            entity.Property(e => e.DegreeLevelId).HasColumnName("degree_level_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Feedback)
                .HasColumnType("jsonb")
                .HasColumnName("feedback");
            entity.Property(e => e.IsEven)
                .HasDefaultValue(false)
                .HasColumnName("is_even");
            entity.Property(e => e.IsFaculty)
                .HasDefaultValue(true)
                .HasColumnName("isFaculty");
            entity.Property(e => e.IsForseChange)
                .HasDefaultValue(false)
                .HasColumnName("isForseChange");
            entity.Property(e => e.Keys)
                .HasColumnType("character varying[]")
                .HasColumnName("keys");
            entity.Property(e => e.MaxCountPeople).HasColumnName("maxCountPeople");
            entity.Property(e => e.MinCountPeople).HasColumnName("minCountPeople");
            entity.Property(e => e.NameDock)
                .HasColumnType("character varying")
                .HasColumnName("nameDock");
            entity.Property(e => e.NameSelectiveDisciplines)
                .HasColumnType("character varying")
                .HasColumnName("nameSelectiveDisciplines");
            entity.Property(e => e.NeedFix)
                .HasDefaultValue(false)
                .HasColumnName("need_fix");
            entity.Property(e => e.RecommendedEp).HasColumnName("recommended_ep");
            entity.Property(e => e.SimilarId).HasColumnName("similar_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeOfControlId).HasColumnName("type_of_control_id");

            entity.HasOne(d => d.ApprovalStatus).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.ApprovalStatusId)
                .HasConstraintName("selectivedisciplines_approval_fk");

            entity.HasOne(d => d.Catalog).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.CatalogId)
                .HasConstraintName("selectivedisciplines_catalogyears_selective_fk");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("selectivedisciplines_educationaldegree_fk");

            entity.HasOne(d => d.Department).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("selectivedisciplines_department_fk");

            entity.HasOne(d => d.Type).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("selectivedisciplines_typeofdiscipline_fk");

            entity.HasOne(d => d.TypeOfControl).WithMany(p => p.SelectiveDisciplines)
                .HasForeignKey(d => d.TypeOfControlId)
                .HasConstraintName("selectivedisciplines_typeofcontrol_fk");
        });

        modelBuilder.Entity<SemestersStart>(entity =>
        {
            entity.HasKey(e => e.IdSemestrStart).HasName("semestersstart_pk");

            entity.ToTable("SemestersStart");

            entity.Property(e => e.IdSemestrStart)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_semestr_start");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity.HasKey(e => e.IdSpeciality).HasName("speciality_pk");

            entity.ToTable("Speciality");

            entity.HasIndex(e => e.IdSpeciality, "speciality_unique").IsUnique();

            entity.Property(e => e.IdSpeciality)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_speciality");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasColumnType("character varying")
                .HasColumnName("accreditationType");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.LicensedVolume).HasColumnName("licensedVolume");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Branch).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("speciality_branch_fk");

            entity.HasOne(d => d.Department).WithMany(p => p.Specialities)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("speciality_department_fk");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.IdSpecialization).HasName("specialization_pk");

            entity.ToTable("Specialization");

            entity.Property(e => e.IdSpecialization)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_specialization");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("student_pk");

            entity.ToTable("Student");

            entity.HasIndex(e => e.UserId, "student_unique").IsUnique();

            entity.Property(e => e.IdStudent)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idStudent");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.EdboCode)
                .HasColumnType("character varying")
                .HasColumnName("edboCode");
            entity.Property(e => e.EducationEnd).HasColumnName("educationEnd");
            entity.Property(e => e.EducationStart).HasColumnName("educationStart");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.IsFunded)
                .HasDefaultValue(true)
                .HasColumnName("isFunded");
            entity.Property(e => e.IsInSg)
                .HasDefaultValue(false)
                .HasColumnName("isInSG");
            entity.Property(e => e.Notes)
                .HasColumnType("character varying")
                .HasColumnName("notes");
            entity.Property(e => e.ReportCard)
                .HasComment("Залікова книга")
                .HasColumnType("character varying")
                .HasColumnName("reportCard");
            entity.Property(e => e.SecondName)
                .HasColumnType("character varying")
                .HasColumnName("second_name");
            entity.Property(e => e.ThirdName)
                .HasColumnType("character varying")
                .HasColumnName("third_name");

            entity.HasOne(d => d.EducationStatus).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationStatusId)
                .HasConstraintName("student_educationstatus_fk");

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("student_studentgroup_fk");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .HasConstraintName("student_users_fk");
        });

        modelBuilder.Entity<StudentGroup>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("studentgroup_pk");

            entity.ToTable("StudentGroup");

            entity.Property(e => e.IdGroup)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idGroup");
            entity.Property(e => e.AdmissionYear).HasColumnName("admissionYear");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.GroupCode)
                .HasColumnType("character varying")
                .HasColumnName("groupCode");
            entity.Property(e => e.IsAccelerated)
                .HasDefaultValue(false)
                .HasColumnName("isAccelerated");

            entity.HasOne(d => d.Admin).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("studentgroup_adminspersonal_fk");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.EducationalProgramId)
                .HasConstraintName("studentgroup_educationalprogram_fk");

            entity.HasOne(d => d.StudyForm).WithMany(p => p.StudentGroups)
                .HasForeignKey(d => d.StudyFormId)
                .HasConstraintName("studentgroup_studyform_fk");
        });

        modelBuilder.Entity<StudyForm>(entity =>
        {
            entity.HasKey(e => e.IdStudyForm).HasName("studyform_pk");

            entity.ToTable("StudyForm");

            entity.Property(e => e.IdStudyForm)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasColumnType("character varying")
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<SubDivisionsSg>(entity =>
        {
            entity.HasKey(e => e.IdSubDivisions).HasName("subdivisionssg_pk");

            entity.ToTable("SubDivisionsSG");

            entity.Property(e => e.IdSubDivisions)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idSubDivisions");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.NameDivision)
                .HasColumnType("character varying")
                .HasColumnName("nameDivision");
        });

        modelBuilder.Entity<TypeOfControl>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfControl).HasName("typeofcontrol_pk");

            entity.ToTable("TypeOfControl");

            entity.Property(e => e.IdTypeOfControl)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_type_of_control");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
        });

        modelBuilder.Entity<TypeOfDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfDiscipline).HasName("typeofdiscipline_pk");

            entity.ToTable("TypeOfDiscipline");

            entity.Property(e => e.IdTypeOfDiscipline)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_type_of_discipline");
            entity.Property(e => e.TypeName)
                .HasColumnType("character varying")
                .HasColumnName("typeName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("Users_pkey");

            entity.Property(e => e.IdUser)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idUser");
            entity.Property(e => e.Avail)
                .HasDefaultValue(true)
                .HasColumnName("avail");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.IsFirstLogin)
                .HasDefaultValue(true)
                .HasColumnName("isFirstLogin");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastLoginAt");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("passwordChangedAt");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.PasswordSalt).HasColumnName("passwordSalt");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.IdUserDevices).HasName("user_devices_pkey");

            entity.Property(e => e.IdUserDevices)
                .ValueGeneratedNever()
                .HasColumnName("idUserDevices");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeviceName)
                .HasColumnType("character varying")
                .HasColumnName("deviceName");
            entity.Property(e => e.IdentityKey).HasColumnName("identityKey");
            entity.Property(e => e.LastSeen)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastSeen");
            entity.Property(e => e.SignedPreKey)
                .HasColumnType("character varying")
                .HasColumnName("signedPreKey");
            entity.Property(e => e.SignedPreKeyId).HasColumnName("signedPreKeyId");
            entity.Property(e => e.SignedPreKeySignature)
                .HasColumnType("character varying")
                .HasColumnName("signedPreKeySignature");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_devices_user_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole).HasName("userroles_pk");

            entity.Property(e => e.IdUserRole)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("idUserRole");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

            entity.HasOne(d => d.Department).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("userroles_department_fk");

            entity.HasOne(d => d.Faculty).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("userroles_faculties_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("userroles_roles_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userroles_users_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
