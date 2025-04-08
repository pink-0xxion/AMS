
using AMS.Interfaces;
using AMS.Models;
using AMS.Repository;
using CRM.Data;

public static class ServiceConfiguration
{
    public static IServiceCollection AddServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {

        // SERVICES
        services.AddSession();

        // DATA
        services.AddSingleton<DapperContext>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


        // REPOSITORIES
        services.AddScoped<IAdminRepository, AdminRepository>();


        return services;
    }
}