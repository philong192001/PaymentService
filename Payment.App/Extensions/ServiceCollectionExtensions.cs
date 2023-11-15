using Serilog;

namespace Payment.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        var appSetting = AppSetting.MapValue(configuration);
        services.AddSingleton(appSetting);
        services.AddDbContext<PaymentDbContext>(x => x.UseSqlServer(appSetting.ConnectionString22, options =>
        {
            options.EnableRetryOnFailure();
        }));
        services.AddDbContext<ConfigDbContext>(x => x.UseSqlServer(appSetting.ConnectionString3, options =>
        {
            options.EnableRetryOnFailure();
        }));
        services.AddHttpContextAccessor();

      

        return services;
    }
}