using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Password).HasMaxLength(300);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasIndex(t => t.TokenHash).IsUnique();

        builder.HasOne(t => t.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StudySession> builder)
    {
        builder.HasIndex(s => new { s.UserId, s.Date }).IsUnique();

        builder.HasOne(s => s.User)
            .WithMany(u => u.StudySessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserProgress> builder)
    {
        builder.HasIndex(p => new { p.UserId, p.TopicId }).IsUnique();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Progress)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Topic)
            .WithMany()
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}