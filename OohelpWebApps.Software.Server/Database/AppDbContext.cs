using Microsoft.EntityFrameworkCore;
using OohelpWebApps.Software.Server.Database.DataConverters;
using OohelpWebApps.Software.Server.Database.DTO;

namespace OohelpWebApps.Software.Server.Database;

public class AppDbContext : DbContext
{
    public DbSet<ApplicationInfoDto> Applications { get; set; }
    public DbSet<ApplicationReleaseDto> Releases { get; set; }
    public DbSet<ReleaseDetailDto> Details { get; set; }
    public DbSet<ReleaseFileDto> Files { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>()
          .HaveConversion<DateOnlyConverter, DateOnlyComparer>()
          .HaveColumnType("date");
        //base.ConfigureConventions(configurationBuilder);
    }

}
