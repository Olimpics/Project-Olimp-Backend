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

    public virtual DbSet<AddDiscipline> AddDisciplines { get; set; }

    public virtual DbSet<BindAddDiscipline> BindAddDisciplines { get; set; }

    public virtual DbSet<BindMainDiscipline> BindMainDisciplines { get; set; }

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
        => optionsBuilder.UseMySql("host=testname-olimp.k.aivencloud.com;port=19136;database=defaultdb;username=avnadmin;password=AVNS_lCKNlJ8n9Gd_5OWtg-M;sslmode=Required", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.35-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AddDiscipline>(entity =>
        {
            entity.HasKey(e => e.idAddDisciplines).HasName("PRIMARY");

            entity.Property(e => e.idAddDisciplines).HasColumnName("idAddDisciplines");
            entity.Property(e => e.AddSemestr).HasColumnType("enum('Парний','Непарний')");
            entity.Property(e => e.CodeAddDisciplines)
                .HasMaxLength(45)
                .HasColumnName("codeAddDisciplines");
            entity.Property(e => e.DegreeLevel).HasColumnType("enum('Бакалавр','Магістр')");
            entity.Property(e => e.Department).HasMaxLength(45);
            entity.Property(e => e.Faculty).HasMaxLength(45);
            entity.Property(e => e.MaxCountPeople).HasColumnName("maxCountPeople");
            entity.Property(e => e.MaxCourse).HasColumnName("maxCourse");
            entity.Property(e => e.MinCountPeople).HasColumnName("minCountPeople");
            entity.Property(e => e.MinCourse).HasColumnName("minCourse");
            entity.Property(e => e.NameAddDisciplines)
                .HasMaxLength(45)
                .HasColumnName("nameAddDisciplines");
            entity.Property(e => e.Prerequisites).HasMaxLength(200);
            entity.Property(e => e.Recomend).HasMaxLength(200);
            entity.Property(e => e.Teacher).HasMaxLength(200);
        });

        modelBuilder.Entity<BindAddDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindAddDisciplines).HasName("PRIMARY");

            entity.HasIndex(e => e.AddDisciplinesId, "Bind_AddCourse_idx");

            entity.HasIndex(e => e.StudentId, "Bind_Students_idx");

            entity.Property(e => e.IdBindAddDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idBindAddDisciplines");
            entity.Property(e => e.Loans).HasDefaultValueSql("'5'");

            entity.HasOne(d => d.Student).WithMany(p => p.BindAddDisciplines)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_Student");

            entity.HasOne(d => d.AddDiscipline).WithMany()
                .HasForeignKey(d => d.AddDisciplinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bind_AddCourse");
        });

        modelBuilder.Entity<BindMainDiscipline>(entity =>
        {
            entity.HasKey(e => e.IdBindMainDisciplines).HasName("PRIMARY");

            entity.HasIndex(e => e.EducationalProgramId, "BindMainDisciplines_EducationalProgram_idx");

            entity.Property(e => e.IdBindMainDisciplines)
                .ValueGeneratedNever()
                .HasColumnName("idBindMainDisciplines");
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

            entity.Property(e => e.IdEducationalProgram)
                .ValueGeneratedNever()
                .HasColumnName("idEducationalProgram");
            entity.Property(e => e.Accreditation).HasColumnName("accreditation");
            entity.Property(e => e.AccreditationType)
                .HasMaxLength(45)
                .HasColumnName("accreditationType");
            entity.Property(e => e.CountAddSemestr3).HasColumnName("countAddSemestr3");
            entity.Property(e => e.CountAddSemestr4).HasColumnName("countAddSemestr4");
            entity.Property(e => e.CountAddSemestr5).HasColumnName("countAddSemestr5");
            entity.Property(e => e.CountAddSemestr6).HasColumnName("countAddSemestr6");
            entity.Property(e => e.CountAddSemestr7).HasColumnName("countAddSemestr7");
            entity.Property(e => e.CountAddSemestr8).HasColumnName("countAddSemestr8");
            entity.Property(e => e.Degree)
                .HasMaxLength(45)
                .HasColumnName("degreeId");
            entity.Property(e => e.NameEducationalProgram)
                .HasMaxLength(45)
                .HasColumnName("nameEducationalProgram");
            entity.Property(e => e.Speciality)
                .HasMaxLength(45)
                .HasColumnName("speciality");
            entity.Property(e => e.StudentsAmount).HasColumnName("studentsAmount");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty).HasName("PRIMARY");

            entity.Property(e => e.IdFaculty)
                .ValueGeneratedNever()
                .HasColumnName("idFaculty");
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(45)
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

            entity.HasIndex(e => e.StatusId, "Students_EducationStatus_idx");

            entity.HasIndex(e => e.EducationalDegreeId, "Students_EducationalDegree_idx");

            entity.HasIndex(e => e.EducationalProgramId, "Students_EducationalProgram_idx");

            entity.HasIndex(e => e.FacultyId, "Students_Faculties_idx");

            entity.HasIndex(e => e.StudyFormId, "Students_StudyForm_idx");

            entity.HasIndex(e => e.UserId, "UserId_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdStudents, "idStudents_UNIQUE").IsUnique();

            entity.Property(e => e.IdStudents)
                .ValueGeneratedNever()
                .HasColumnName("idStudents");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(200)
                .HasColumnName("nameStudent");

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

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
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

            entity.Property(e => e.IdUsers)
                .ValueGeneratedNever()
                .HasColumnName("idUsers");
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
