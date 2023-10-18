namespace Payment.App.Models.OnePay;

public class OnePayQrRequest
{
    public OnePayQrRequest()
    {
    }

    public OnePayQrRequest(Application app, Device device, string platform)
    {
        this.app = app;
        this.device = device;
        this.platform = platform;
    }

    public Application app { get; set; }
    public Device device { get; set; }
    public string platform { get; set; }
}

public class Application
{
    public Application()
    {
    }

    public string id { get; set; }
}

public class Device
{
    public string user_agent { get; set; }
}