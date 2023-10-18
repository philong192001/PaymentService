namespace Payment.App.Models.VnPay;

public class VnPayQrResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public DataImage Data { get; set; }
}

public class DataImage
{
    public string Type { get; set; }
    public string Image { get; set; }
    public string PayLoad { get; set; }
}
