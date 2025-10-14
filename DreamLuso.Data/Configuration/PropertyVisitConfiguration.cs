using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class PropertyVisitConfiguration : IEntityTypeConfiguration<PropertyVisit>
{
    public void Configure(EntityTypeBuilder<PropertyVisit> builder)
    {
        builder.ToTable("PropertyVisit");

        // Primary Key
        builder.HasKey(pv => pv.Id);

        // Basic Properties
        builder.Property(pv => pv.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(pv => pv.ClientFeedback)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(pv => pv.CancellationReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(pv => pv.ConfirmationToken)
            .HasMaxLength(100)
            .IsRequired();

        // Enum Configurations
        builder.Property(pv => pv.TimeSlot)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pv => pv.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(pv => pv.Property)
            .WithMany(p => p.PropertyVisits)
            .HasForeignKey(pv => pv.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pv => pv.Client)
            .WithMany(c => c.PropertyVisits)
            .HasForeignKey(pv => pv.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pv => pv.RealEstateAgent)
            .WithMany(a => a.PropertyVisits)
            .HasForeignKey(pv => pv.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(pv => pv.PropertyId);
        builder.HasIndex(pv => pv.ClientId);
        builder.HasIndex(pv => pv.RealEstateAgentId);
        builder.HasIndex(pv => pv.Status);
        builder.HasIndex(pv => pv.VisitDate);
        builder.HasIndex(pv => pv.ConfirmationToken).IsUnique();

        // Auditable Entity Configuration
        builder.Property(pv => pv.CreatedAt).IsRequired();
        builder.Property(pv => pv.UpdatedAt).IsRequired(false);
        builder.Property(pv => pv.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(pv => pv.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

