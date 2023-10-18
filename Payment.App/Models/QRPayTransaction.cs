namespace Payment.App.Models;

public class QRPayTransaction
{
    public long TransId { get; set; }
    public Guid BookId { get; set; }
    public string PartnerTransId { get; set; }
    public string VehiclePlate { get; set; }
    public string PrivateCode { get; set; }
    public double Amount { get; set; }
    public double Price { get; set; }
    public string GroupName { get; set; }
    public string OperatorPriceName { get; set; }
    public int CompanyId { get; set; }
    public int PaymentType { get; set; }
    public string Token { get; set; }
    public DateTime DateCreateQR { get; set; }
    public DateTime DatePay { get; set; }
    public string Mobile { get; set; }
    public DateTime FeeMonth { get; set; }
    public string DriverName { get; set; }
    public string DriverCode { get; set; }
    public string CustomerName { get; set; }
    public string QRCode { get; set; }
    public DateTime ExpiredFee { get; set; }
}
