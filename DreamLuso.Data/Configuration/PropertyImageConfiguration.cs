using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("PropertyImage");

        // Primary Key
        builder.HasKey(pi => pi.Id);

        // Basic Properties
        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pi => pi.Title)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(pi => pi.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        // Enum Configuration
        builder.Property(pi => pi.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(pi => pi.Property)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pi => pi.PropertyId);
        builder.HasIndex(pi => new { pi.PropertyId, pi.IsPrimary });

        // Auditable Entity Configuration
        builder.Property(pi => pi.CreatedAt).IsRequired();
        builder.Property(pi => pi.UpdatedAt).IsRequired(false);
        builder.Property(pi => pi.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(pi => pi.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

