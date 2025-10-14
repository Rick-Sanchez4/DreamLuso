using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamLuso.Data.Configuration;

internal class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Property");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Basic Properties
        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.LicenseNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.RegistryNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.TaxId)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.Amenities)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(p => p.EnergyRating)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(p => p.Orientation)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.ViewType)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.NearbyPoints)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(p => p.VideoUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.VirtualTourUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        // Decimal precision configuration
        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PricePerSqm)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(p => p.Condominium)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(p => p.Imt)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(p => p.Imi)
            .HasPrecision(18, 2)
            .IsRequired(false);

        // Enum Configurations
        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Address Value Object Configuration
        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200).IsRequired();
            address.Property(a => a.Number).HasColumnName("Number").HasMaxLength(20).IsRequired();
            address.Property(a => a.Parish).HasColumnName("Parish").HasMaxLength(100).IsRequired();
            address.Property(a => a.Municipality).HasColumnName("Municipality").HasMaxLength(100).IsRequired();
            address.Property(a => a.District).HasColumnName("District").HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(10).IsRequired();
            address.Property(a => a.Complement).HasColumnName("Complement").HasMaxLength(100).IsRequired(false);
            address.Ignore(a => a.FullAddress);
        });

        // Navigation Properties
        builder.HasOne(p => p.RealEstateAgent)
            .WithMany(a => a.Properties)
            .HasForeignKey(p => p.RealEstateAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Images)
            .WithOne(img => img.Property)
            .HasForeignKey(img => img.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PropertyVisits)
            .WithOne(pv => pv.Property)
            .HasForeignKey(pv => pv.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Contracts)
            .WithOne(ct => ct.Property)
            .HasForeignKey(ct => ct.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better performance
        builder.HasIndex(p => p.RealEstateAgentId);
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransactionType);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.IsFeatured);
        builder.HasIndex(p => p.IsActive);

        // Composite indexes for common queries
        builder.HasIndex(p => new { p.Type, p.Status, p.IsActive })
            .HasDatabaseName("IX_Property_TypeStatusActive");

        // Auditable Entity Configuration
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired(false);
        builder.Property(p => p.CreatedBy).HasMaxLength(100).IsRequired(false);
        builder.Property(p => p.UpdatedBy).HasMaxLength(100).IsRequired(false);
    }
}

