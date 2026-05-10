using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Models;

public partial class GameQuestDbContext : DbContext
{
    public GameQuestDbContext()
    {
    }

    public GameQuestDbContext(DbContextOptions<GameQuestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advert> Adverts { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<UserGameProfile> UserGameProfiles { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GameQuestDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adverts__3214EC07D765F94A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RequiredRank).HasMaxLength(50);
            entity.Property(e => e.RequiredRole).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Game).WithMany(p => p.Adverts)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Adverts_Games");

            entity.HasOne(d => d.User).WithMany(p => p.Adverts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Adverts_Users");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC0774D1430F");

            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);

            entity.HasOne(d => d.Advert).WithMany(p => p.Applications)
                .HasForeignKey(d => d.AdvertId)
                .HasConstraintName("FK_Applications_Adverts");

            entity.HasOne(d => d.Applicant).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07A5BA2B5D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Games__3214EC0708ACB650");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Games)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Games_Categories");
        });

        modelBuilder.Entity<UserGameProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PreferredRole).HasMaxLength(50);
            entity.Property(e => e.Rank).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.UserGameProfiles)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserGameProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Country).HasMaxLength(60);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TimeZone).HasMaxLength(50);

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C2DF58AF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscordProfile).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IdentityUserId).HasMaxLength(450);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasIndex(e => e.IdentityUserId)
                .IsUnique()
                .HasFilter("[IdentityUserId] IS NOT NULL");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
