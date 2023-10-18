namespace Payment.App.Settings;

public class AppSetting
{
    public VnPaySetting VnPay { get; set; }
    public OnePaySetting OnePay { get; set; }
    public string ConnectionString { get; set; }

    public static AppSetting MapValue(IConfiguration configuration)
    {
        var setting = new AppSetting
        {
            VnPay = configuration.GetSection(nameof(VnPay)).Get<VnPaySetting>(),
            OnePay = configuration.GetSection(nameof(OnePay)).Get<OnePaySetting>(),
            ConnectionString = configuration[nameof(ConnectionString)]
        };
        return setting;
    }
}
