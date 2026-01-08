using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ThiSmartUI.Models;

public partial class ExamSystemContext : DbContext
{
    public ExamSystemContext()
    {
    }

    public ExamSystemContext(DbContextOptions<ExamSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<ExamSession> ExamSessions { get; set; }

    public virtual DbSet<ProctoringLog> ProctoringLogs { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<StudentAttempt> StudentAttempts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=exam_system;User Id=sa;Password=123;TrustServerCertificate=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83FC9886B27");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_categories_parent");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Categories)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_categories_teacher");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__exams__3213E83F361A29AA");

            entity.ToTable("exams");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AntiCheatSettings).HasColumnName("anti_cheat_settings");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsRandom)
                .HasDefaultValue(false)
                .HasColumnName("is_random");
            entity.Property(e => e.PassScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("pass_score");
            entity.Property(e => e.SnapshotUrl).HasColumnName("snapshot_url");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Exams)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_exams_teacher");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => new { e.ExamId, e.QuestionId }).HasName("PK__exam_que__1E605ABDADC18F90");

            entity.ToTable("exam_questions");

            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK_exam_questions_exam");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_exam_questions_question");
        });

        modelBuilder.Entity<ExamSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__exam_ses__3213E83FD397FDEA");

            entity.ToTable("exam_sessions");

            entity.HasIndex(e => e.AccessCode, "UQ__exam_ses__D304327196FD30D6").IsUnique();

            entity.HasIndex(e => e.AccessCode, "idx_exam_access_code");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("access_code");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK_sessions_exam");
        });

        modelBuilder.Entity<ProctoringLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__proctori__3213E83F6795B251");

            entity.ToTable("proctoring_logs");

            entity.HasIndex(e => e.AttemptId, "idx_proctoring_attempt");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("event_type");
            entity.Property(e => e.LogTime)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("log_time");

            entity.HasOne(d => d.Attempt).WithMany(p => p.ProctoringLogs)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK_logs_attempt");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__question__3213E83F9C6A1D79");

            entity.ToTable("questions");

            entity.HasIndex(e => e.CategoryId, "idx_question_category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("level");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("MultipleChoice")
                .HasColumnName("question_type");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_questions_category");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_questions_teacher");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__question__3213E83FCCB8D8B4");

            entity.ToTable("question_options");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false)
                .HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_options_question");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F572C2184");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B1775C2742").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__student___3213E83FCEA8D736");

            entity.ToTable("student_answers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.EssayAnswer).HasColumnName("essay_answer");
            entity.Property(e => e.IsFlagged)
                .HasDefaultValue(false)
                .HasColumnName("is_flagged");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SelectedOptionId).HasColumnName("selected_option_id");

            entity.HasOne(d => d.Attempt).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK_answers_attempt");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_answers_question");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .HasConstraintName("FK_answers_option");
        });

        modelBuilder.Entity<StudentAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__student___3213E83F3E6CFBF9");

            entity.ToTable("student_attempts");

            entity.HasIndex(e => e.SessionId, "idx_student_attempt_session");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("In-Progress")
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.TotalScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_score");

            entity.HasOne(d => d.Session).WithMany(p => p.StudentAttempts)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK_attempts_session");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAttempts)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_attempts_student");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FA4AE5AF3");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164CB76162D").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC572CA51E503").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_users_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
