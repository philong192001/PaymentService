namespace Payment.App.Settings;

public class AppSetting
{
    public string ConnectionString3 { get; set; }
    public string ConnectionString22 { get; set; }

    public static AppSetting MapValue(IConfiguration configuration)
    {
        var setting = new AppSetting
        {
            ConnectionString3 = configuration[nameof(ConnectionString3)],
            ConnectionString22 = configuration[nameof(ConnectionString22)]
        };
        return setting;
    }
}