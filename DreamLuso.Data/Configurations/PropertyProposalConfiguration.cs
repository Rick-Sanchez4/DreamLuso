using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configurations;

public class PropertyProposalConfiguration : IEntityTypeConfiguration<PropertyProposal>
{
    public void Configure(EntityTypeBuilder<PropertyProposal> builder)
    {
        builder.ToTable("PropertyProposals");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProposalNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ProposedValue)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(p => p.AdditionalNotes)
            .HasMaxLength(2000);

        builder.Property(p => p.RejectionReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(p => p.Property)
            .WithMany()
            .HasForeignKey(p => p.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Client)
            .WithMany()
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Negotiations)
            .WithOne(n => n.Proposal)
            .HasForeignKey(n => n.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.ProposalNumber)
            .IsUnique();
        builder.HasIndex(p => p.PropertyId);
        builder.HasIndex(p => p.ClientId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
    }
}

