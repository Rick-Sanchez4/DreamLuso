using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DreamLuso.Data.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("../DreamLuso.WebAPI/appsettings.Development.json", optional: true)
            .AddJsonFile("../DreamLuso.WebAPI/appsettings.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DreamLusoDB") 
            ?? "Server=localhost;Database=DreamLusoDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

