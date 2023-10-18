namespace Payment.App.Models.VnPay;

//class tạo QR 
public class VnPayQrRequest
{
    public string Amount { get; set; }
    public string MerchantCode { get; set; }
    public string ImageFormat { get; set; }
    public string ImageType { get; set; }
    public string TerminalID { get; set; }
    public string Height { get; set; }
    public string Width { get; set; }
    public string BillNumber { get; set; }
}
