namespace Payment.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        var appSetting = AppSetting.MapValue(configuration);
        services.AddHostedService<GetTokenVnPayBackgroundTask>();
        services.AddSingleton(appSetting);
        services.AddDbContext<PaymentDbContext>( x => x.UseSqlServer(appSetting.ConnectionString, options =>
        {
            options.EnableRetryOnFailure();
        }));
        services.AddHttpContextAccessor();
        return services;
    }
}
