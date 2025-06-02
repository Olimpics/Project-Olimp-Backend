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

    public virtual DbSet<AddDetail> AddDetails { get; set; }

    public virtual DbSet<AddDiscipline> AddDisciplines { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<AdminsPersonal> AdminsPersonals { get; set; }

    public virtual DbSet<BindAddDiscipline> BindAddDisciplines { get; set; }

    public virtual DbSet<BindLoansMain> BindLoansMains { get; set; }

    public virtual DbSet<BindMainDiscipline> BindMainDisciplines { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<EducationStatus> EducationStatuses { get; set; }

    public virtual DbSet<EducationalDegree> EducationalDegrees { get; set; }

    public virtual DbSet<EducationalProgram> EducationalPrograms { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudyForm> StudyForms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("host=185.237.207.78;port=3306;database=defaultdb;username=remote;password=P@ssw0rd", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AddDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.DepartmentId, "AddDetails_Department_idx");

            entity.HasIndex(e => e.IdAddDetails, "idAddDetails_UNIQUE").IsUnique();

            entity.Property(e => e.AdditionaLiterature).HasMaxLength(800);
            entity.Property(e => e.Determination).HasMaxLength(800);
            entity.Property(e => e.IdAddDetails).HasColumnName("idAddDetails");
            entity.Property(e => e.Language).HasMaxLength(200);
            entity.Property(e => e.Prerequisites).HasMaxLength(800);
            entity.Property(e => e.Recomend).HasMaxLength(800);
            entity.Property(e => e.ResultEducation).HasMaxLength(800);
            entity.Property(e => e.Teacher).HasMaxLength(800);
            entity.Property(e => e.TypeOfControll).HasMaxLength(100);
            entity.Property(e => e.TypesOfTraining).HasMaxLength(200);
            entity.Property(e => e.UsingIrl)
                .HasMaxLength(800)
                .HasColumnName("UsingIRL");
            entity.Property(e => e.WhyInterestingDetermination).HasMaxLength(800);

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDetails_Department");

            entity.HasOne(d => d.IdAddDetailsNavigation).WithOne()
                .HasForeignKey<AddDetail>(d => d.IdAddDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AddDetails_AddDisciples");
        });

        modelBuilder.Entity<AddDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdAddDisciplines).HasName("PRIMARY");

            entity.HasIndex(e => e.DegreeLevelId, "AddDisciplines_EducationalDegree_idx");

            entity.HasIndex(e => e.IdAddDisciplines, "idAddDisciplines_UNIQUE").IsUnique();

            entity.Property(e => e.IdAddDisciplines).HasColumnName("idAddDisciplines");
            entity.Property(e => e.CodeAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("codeAddDisciplines");
            entity.Property(e => e.Faculty).HasMaxLength(200);
            entity.Property(e => e.MaxCountPeople).HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse).HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople).HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse).HasColumnName("minCourse");
            entity.Property(e => e.NameAddDisciplines)
                .HasMaxLength(200)
                .HasColumnName("nameAddDisciplines");

            entity.HasOne(d => d.DegreeLevel).WithMany(p => p.AddDisciplines)
                .HasForeignKey(d => d.DegreeLevelId)
                .HasConstraintName("AddDisciplines_EducationalDegree");
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => new { e.LogId, e.ChangeTime })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasIndex(e => e.AdminId, "idx_AdminLogs_AdminId");

            entity.HasIndex(e => e.ChangeTime, "idx_AdminLogs_Time");

            entity.Property(e => e.LogId).ValueGeneratedOnAdd();
            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Action).HasColumnType("enum('INSERT','UPDATE','DELETE')");
            entity.Property(e => e.AdminId).HasComment("idUsers who performed action");
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

            entity.ToTable("AdminsPersonal");

            entity.HasIndex(e => e.DepartmentId, "AdminsPersonal_Deopartment_idx");

            entity.HasIndex(e => e.FacultyId, "AdminsPersonal_Faculties_idx");

            entity.HasIndex(e => e.UserId, "AdminsPersonal_Users_idx");

            entity.Property(e => e.IdAdmins).HasColumnName("idAdmins");
            entity.Property(e => e.NameAdmin)
                .HasMaxLength(200)
                .HasColumnName("nameAdmin");
            entity.Property(e => e.Photo).HasColumnType("blob");

            entity.HasOne(d => d.Department).WithMany(p => p.AdminsPersonals)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("AdminsPersonal_Deopartment");

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

            entity.HasIndex(e => e.AddDisciplinesId, "Bind_AddCourse_idx");

            entity.HasIndex(e => e.StudentId, "Bind_Student_idx");

            entity.Property(e => e.IdBindAddDisciplines).HasColumnName("idBindAddDisciplines");
            entity.Property(e => e.InProcess).HasColumnName("inProcess");
            entity.Property(e => e.Loans).HasDefaultValueSql("'5'");

            entity.HasOne(d => d.AddDisciplines).WithMany(p => p.BindAddDisciplines)
                .HasForeignKey(d => d.AddDisciplinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_AddDisciple");

            entity.HasOne(d => d.Student).WithMany(p => p.BindAddDisciplines)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_Student");
        });

        modelBuilder.Entity<BindLoansMain>(entity =>
        {
            entity.HasKey(e => e.IdBindLoan).HasName("PRIMARY");

            entity.ToTable("BindLoansMain");

            entity.HasIndex(e => e.AddDisciplinesId, "BindLoansMain_AddDisciplines_idx");

            entity.HasIndex(e => e.EducationalProgramId, "BindLoansMain_EducationalProgram_idx");

            entity.Property(e => e.IdBindLoan).HasColumnName("idBindLoan");

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

            entity.HasIndex(e => e.EducationalProgramId, "BindMainDisciplines_EducationalProgram_idx");

            entity.Property(e => e.IdBindMainDisciplines).HasColumnName("idBindMainDisciplines");
            entity.Property(e => e.CodeMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("codeMainDisciplines");
            entity.Property(e => e.FormControll).HasColumnType("enum('Залік','Екзамен','Диференційований Залік')");
            entity.Property(e => e.NameBindMainDisciplines)
                .HasMaxLength(45)
                .HasColumnName("nameBindMainDisciplines");

            entity.HasOne(d => d.EducationalProgram).WithMany(p => p.BindMainDisciplines)
                .HasForeignKey(d => d.EducationalProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BindMainDisciplines_EducationalProgram");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment).HasName("PRIMARY");

            entity.ToTable("Department");

            entity.HasIndex(e => e.FacultyId, "Department_Faculties_idx");

            entity.Property(e => e.IdDepartment)
                .ValueGeneratedNever()
                .HasColumnName("idDepartment");
            entity.Property(e => e.Abbreviation).HasMaxLength(200);
            entity.Property(e => e.NameDepartment)
                .HasMaxLength(200)
                .HasColumnName("nameDepartment");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Department_Faculties");
        });

        modelBuilder.Entity<EducationStatus>(entity =>
        {
            entity.HasKey(e => e.IdEducationStatus).HasName("PRIMARY");

            entity.ToTable("EducationStatus");

            entity.HasIndex(e => e.IdEducationStatus, "idEducationStatus_UNIQUE").IsUnique();

            entity.Property(e => e.IdEducationStatus)
                .ValueGeneratedNever()
                .HasColumnName("idEducationStatus");
            entity.Property(e => e.NameEducationStatus)
                .HasMaxLength(45)
                .HasColumnName("nameEducationStatus");
        });

        modelBuilder.Entity<EducationalDegree>(entity =>
        {
            entity.HasKey(e => e.IdEducationalDegree).HasName("PRIMARY");

            entity.ToTable("EducationalDegree");

            entity.Property(e => e.IdEducationalDegree)
                .ValueGeneratedNever()
                .HasColumnName("idEducationalDegree");
            entity.Property(e => e.NameEducationalDegreec)
                .HasMaxLength(45)
                .HasColumnName("nameEducationalDegreec");
        });

        modelBuilder.Entity<EducationalProgram>(entity =>
        {
            entity.HasKey(e => e.IdEducationalProgram).HasName("PRIMARY");

            entity.ToTable("EducationalProgram");

            entity.HasIndex(e => e.DegreeId, "EducationalProgram_EducationalDegree_idx");

            entity.Property(e => e.IdEducationalProgram).HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(400)
                .HasColumnName("accreditationType");
            entity.Property(e => e.CountAddSemestr3).HasColumnName("countAddSemestr3");
            entity.Property(e => e.CountAddSemestr4).HasColumnName("countAddSemestr4");
            entity.Property(e => e.CountAddSemestr5).HasColumnName("countAddSemestr5");
            entity.Property(e => e.CountAddSemestr6).HasColumnName("countAddSemestr6");
            entity.Property(e => e.CountAddSemestr7).HasColumnName("countAddSemestr7");
            entity.Property(e => e.CountAddSemestr8).HasColumnName("countAddSemestr8");
            entity.Property(e => e.DegreeId).HasColumnName("degreeId");
            entity.Property(e => e.NameEducationalProgram)
                .HasMaxLength(200)
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.Speciality)
                .HasMaxLength(400)
                .HasColumnName("speciality");
            entity.Property(e => e.SpecialityCode)
                .HasMaxLength(45)
                .HasColumnName("specialityCode");
            entity.Property(e => e.StudentsAmount).HasColumnName("studentsAmount");

            entity.HasOne(d => d.Degree).WithMany(p => p.EducationalPrograms)
                .HasForeignKey(d => d.DegreeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EducationalProgram_EducationalDegree");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty).HasName("PRIMARY");

            entity.Property(e => e.IdFaculty).HasColumnName("idFaculty");
            entity.Property(e => e.Abbreviation).HasMaxLength(45);
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(200)
                .HasColumnName("nameFaculty");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.HasIndex(e => e.IdRole, "idRole_UNIQUE").IsUnique();

            entity.HasIndex(e => e.NameRole, "nameRole_UNIQUE").IsUnique();

            entity.Property(e => e.IdRole)
                .ValueGeneratedNever()
                .HasColumnName("idRole");
            entity.Property(e => e.NameRole)
                .HasMaxLength(45)
                .HasColumnName("nameRole");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudents).HasName("PRIMARY");

            entity.ToTable("Student");

            entity.HasIndex(e => e.DepartmentId, "Students_Department_idx");

            entity.HasIndex(e => e.StatusId, "Students_EducationStatus_idx");

            entity.HasIndex(e => e.EducationalDegreeId, "Students_EducationalDegree_idx");

            entity.HasIndex(e => e.EducationalProgramId, "Students_EducationalProgram_idx");

            entity.HasIndex(e => e.FacultyId, "Students_Faculties_idx");

            entity.HasIndex(e => e.StudyFormId, "Students_StudyForm_idx");

            entity.HasIndex(e => e.UserId, "Students_Users_idx");

            entity.HasIndex(e => e.IdStudents, "idStudents_UNIQUE").IsUnique();

            entity.Property(e => e.IdStudents).HasColumnName("idStudents");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");
            entity.Property(e => e.Photo).HasColumnType("blob");

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

            entity.ToTable("StudyForm");

            entity.Property(e => e.IdStudyForm)
                .ValueGeneratedNever()
                .HasColumnName("idStudyForm");
            entity.Property(e => e.NameStudyForm)
                .HasMaxLength(45)
                .HasColumnName("nameStudyForm");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers).HasName("PRIMARY");

            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.RoleId, "Users_Roles_idx");

            entity.HasIndex(e => e.IdUsers, "idUsers_UNIQUE").IsUnique();

            entity.Property(e => e.IdUsers).HasColumnName("idUsers");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(200);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
