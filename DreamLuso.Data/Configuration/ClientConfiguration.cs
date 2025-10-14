using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Basic Properties
        builder.Property(c => c.Nif)
            .HasMaxLength(9)
            .IsRequired(false);

        builder.Property(c => c.CitizenCard)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(c => c.PreferredContactMethod)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(c => c.PropertyPreferences)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Enum Configuration
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(c => c.User)
            .WithOne(u => u.Client)
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.PropertyVisits)
            .WithOne(pv => pv.Client)
            .HasForeignKey(pv => pv.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Contracts)
            .WithOne(ct => ct.Client)
            .HasForeignKey(ct => ct.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.FavoriteProperties)
            .WithMany(p => p.InterestedClients)
            .UsingEntity(j => j.ToTable("ClientFavoriteProperties"));

        // Indexes
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.Nif)
            .IsUnique()
            .HasFilter("[Nif] IS NOT NULL");

        // Auditable Entity Configuration
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired(false);
        builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

