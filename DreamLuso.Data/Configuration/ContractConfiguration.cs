using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contract");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Basic Properties
        builder.Property(c => c.TermsAndConditions)
            .HasMaxLength(5000)
            .IsRequired(false);

        builder.Property(c => c.PaymentTerms)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(c => c.TerminationClauses)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(c => c.DocumentPath)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(c => c.InsuranceDetails)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(c => c.GuarantorInfo)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Decimal precision configuration
        builder.Property(c => c.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.MonthlyRent)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(c => c.SecurityDeposit)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(c => c.Commission)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(c => c.AdditionalFees)
            .HasPrecision(18, 2)
            .IsRequired(false);

        // Enum Configurations
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.PaymentFrequency)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired(false);

        // Navigation Properties
        builder.HasOne(c => c.Property)
            .WithMany(p => p.Contracts)
            .HasForeignKey(c => c.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.RealEstateAgent)
            .WithMany(a => a.Contracts)
            .HasForeignKey(c => c.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.PropertyId);
        builder.HasIndex(c => c.ClientId);
        builder.HasIndex(c => c.RealEstateAgentId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.StartDate);
        builder.HasIndex(c => c.EndDate);

        // Auditable Entity Configuration
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired(false);
        builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

