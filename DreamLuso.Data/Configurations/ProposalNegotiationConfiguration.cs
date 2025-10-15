using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configurations;

public class ProposalNegotiationConfiguration : IEntityTypeConfiguration<ProposalNegotiation>
{
    public void Configure(EntityTypeBuilder<ProposalNegotiation> builder)
    {
        builder.ToTable("ProposalNegotiations");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.CounterOffer)
            .HasColumnType("decimal(18,2)");

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(n => n.SentAt)
            .IsRequired();

        // Relationships
        builder.HasOne(n => n.Proposal)
            .WithMany(p => p.Negotiations)
            .HasForeignKey(n => n.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Sender)
            .WithMany()
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(n => n.ProposalId);
        builder.HasIndex(n => n.SentAt);
    }
}

