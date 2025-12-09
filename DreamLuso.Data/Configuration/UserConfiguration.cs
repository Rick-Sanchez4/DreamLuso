using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        // Primary Key
        builder.HasKey(u => u.Id);

        // Name Value Object Configuration
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired();
            
            // Ignore computed property
            name.Ignore(n => n.FullName);
        });

        // Basic Properties
        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(u => u.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.PasswordHash)
            .IsRequired(false);

        builder.Property(u => u.PasswordSalt)
            .IsRequired(false);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500)
            .IsRequired(false);

        // Enum Configuration
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(u => u.Client)
            .WithOne(c => c.User)
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.RealEstateAgent)
            .WithOne(a => a.User)
            .HasForeignKey<RealEstateAgent>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Auditable Entity Configuration
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired(false);
        builder.Property(u => u.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(u => u.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

