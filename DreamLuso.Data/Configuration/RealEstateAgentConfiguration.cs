using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class RealEstateAgentConfiguration : IEntityTypeConfiguration<RealEstateAgent>
{
    public void Configure(EntityTypeBuilder<RealEstateAgent> builder)
    {
        builder.ToTable("RealEstateAgent");

        // Primary Key
        builder.HasKey(a => a.Id);

        // Basic Properties
        builder.Property(a => a.LicenseNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.OfficeEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.OfficePhone)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(a => a.Specialization)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.Bio)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Collections stored as JSON (simplified approach)
        builder.Property(a => a.Certifications)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.LanguagesSpoken)
            .HasConversion(
                v => string.Join(',', v.Select(l => (int)l)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => (Language)int.Parse(s)).ToList())
            .HasMaxLength(200)
            .IsRequired(false);

        // Navigation Properties
        builder.HasOne(a => a.User)
            .WithOne(u => u.RealEstateAgent)
            .HasForeignKey<RealEstateAgent>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Properties)
            .WithOne(p => p.RealEstateAgent)
            .HasForeignKey(p => p.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.PropertyVisits)
            .WithOne(pv => pv.RealEstateAgent)
            .HasForeignKey(pv => pv.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Contracts)
            .WithOne(ct => ct.RealEstateAgent)
            .HasForeignKey(ct => ct.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.UserId).IsUnique();
        builder.HasIndex(a => a.LicenseNumber)
            .IsUnique()
            .HasFilter("[LicenseNumber] IS NOT NULL");

        // Auditable Entity Configuration
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired(false);
        builder.Property(a => a.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(a => a.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

