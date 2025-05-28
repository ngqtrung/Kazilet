using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class PRN231_Kazilet_v2Context : DbContext
    {
        public PRN231_Kazilet_v2Context()
        {
        }

        public PRN231_Kazilet_v2Context(DbContextOptions<PRN231_Kazilet_v2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Folder> Folders { get; set; } = null!;
        public virtual DbSet<Gameplay> Gameplays { get; set; } = null!;
        public virtual DbSet<GameplayAnswer> GameplayAnswers { get; set; } = null!;
        public virtual DbSet<GameplaySetting> GameplaySettings { get; set; } = null!;
        public virtual DbSet<LearningHistory> LearningHistories { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .HasColumnName("content");

                entity.Property(e => e.IsCorrect).HasColumnName("is_correct");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__Answers__questio__3B75D760");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CoursePassword)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("course_password");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .HasColumnName("description");

                entity.Property(e => e.IsPublic).HasColumnName("isPublic");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Courses__created__3C69FB99");

                entity.HasMany(d => d.Folders)
                    .WithMany(p => p.Courses)
                    .UsingEntity<Dictionary<string, object>>(
                        "FolderCourse",
                        l => l.HasOne<Folder>().WithMany().HasForeignKey("FolderId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__FolderCou__folde__778AC167"),
                        r => r.HasOne<Course>().WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__FolderCou__cours__76969D2E"),
                        j =>
                        {
                            j.HasKey("CourseId", "FolderId").HasName("PK__FolderCo__2F1AA7DF7C6E5754");

                            j.ToTable("FolderCourse");

                            j.IndexerProperty<int>("CourseId").HasColumnName("course_id");

                            j.IndexerProperty<int>("FolderId").HasColumnName("folder_id");
                        });
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Folders)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Folders__created__3F466844");
            });

            modelBuilder.Entity<Gameplay>(entity =>
            {
                entity.ToTable("Gameplay");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("avatar");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("code")
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsGetResult).HasColumnName("is_get_result");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.Streak).HasColumnName("streak");

                entity.Property(e => e.Turn).HasColumnName("turn");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.CodeNavigation)
                    .WithMany(p => p.Gameplays)
                    .HasPrincipalKey(p => p.Code)
                    .HasForeignKey(d => d.Code)
                    .HasConstraintName("FK__Gameplay__code__403A8C7D");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Gameplays)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Gameplay__user_i__4222D4EF");
            });

            modelBuilder.Entity<GameplayAnswer>(entity =>
            {
                entity.HasKey(e => new { e.GameplayId, e.QuestionId })
                    .HasName("PK__Gameplay__653945255F6933D9");

                entity.ToTable("GameplayAnswer");

                entity.Property(e => e.GameplayId).HasColumnName("gameplay_id");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.Property(e => e.PlayerAnswer).HasColumnName("player_answer");

                entity.HasOne(d => d.Gameplay)
                    .WithMany(p => p.GameplayAnswers)
                    .HasForeignKey(d => d.GameplayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameplayA__gamep__6FE99F9F");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.GameplayAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameplayA__quest__70DDC3D8");
            });

            modelBuilder.Entity<GameplaySetting>(entity =>
            {
                entity.ToTable("GameplaySetting");

                entity.HasIndex(e => e.Code, "UQ__Gameplay__357D4CF9017EDB1A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("code")
                    .IsFixedLength();

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsCompleted).HasColumnName("is_completed");

                entity.Property(e => e.IsHostPlay).HasColumnName("is_host_play");

                entity.Property(e => e.IsSkillEnabled).HasColumnName("is_skill_enabled");

                entity.Property(e => e.IsStarted).HasColumnName("is_started");

                entity.Property(e => e.NoQuestion).HasColumnName("no_question");

                entity.Property(e => e.TimeLimit).HasColumnName("time_limit");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.GameplaySettings)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK__GameplayS__cours__440B1D61");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.GameplaySettings)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__GameplayS__creat__44FF419A");
            });

            modelBuilder.Entity<LearningHistory>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CourseId })
                    .HasName("PK__Learning__414FD8751EF5695D");

                entity.ToTable("LearningHistory");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.LearningDate)
                    .HasColumnType("date")
                    .HasColumnName("learning_date");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.LearningHistories)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LearningH__cours__02FC7413");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LearningHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LearningH__user___02084FDA");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasMaxLength(100)
                    .HasColumnName("content");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Link)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("link");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notificat__user___47DBAE45");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .HasColumnName("content");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.IsMarked).HasColumnName("is_marked");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK__Questions__cours__48CFD27E");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__AB6E61646A12B12B")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Gid)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("gid");

                entity.Property(e => e.Password)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Role)
                    .HasConstraintName("FK__Users__role__4AB81AF0");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
