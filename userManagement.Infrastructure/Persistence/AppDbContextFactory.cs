using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


using System.IO;

namespace userManagement.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {

        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(basePath, "../userManagement.Api"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
       
        var connectionString = configuration.GetConnectionString("Default");
        var solutionRoot = Path.GetFullPath(Path.Combine(basePath, ".."));
        var certPath = Path.Combine(solutionRoot, "Certs", "ca.pem");
        
        connectionString = connectionString.Replace("${CERT_PATH}", certPath);
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new AppDbContext(optionsBuilder.Options);
    }
}