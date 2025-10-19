using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Context;

public class ApplicationDbContext : DbContext
{
    // Existing DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<RealEstateAgent> RealEstateAgents { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<PropertyVisit> PropertyVisits { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    
    // New DbSets
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PropertyProposal> PropertyProposals { get; set; }
    public DbSet<ProposalNegotiation> ProposalNegotiations { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar configurações
        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        // Schema padrão (apenas para SQL Server)
        // PostgreSQL usa "public" por padrão
        var provider = Database.ProviderName;
        if (provider != null && provider.Contains("SqlServer"))
        {
            modelBuilder.HasDefaultSchema("DreamLuso");
        }
        else if (provider != null && provider.Contains("PostgreSQL"))
        {
            modelBuilder.HasDefaultSchema("public");
        }

        base.OnModelCreating(modelBuilder);
    }
}

