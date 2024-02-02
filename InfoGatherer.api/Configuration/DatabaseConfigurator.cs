using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InfoGatherer.api.Data;

public static class DatabaseConfigurator
{
    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("dbConnection") ?? throw new InvalidOperationException("Connection String error"));
        });

    }
}
