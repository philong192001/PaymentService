namespace Payment.App.Models;

public class PaymentConfig
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string UrlGetToken { get; set; }
    public string PaymentType { get; set; }
    public string UrlCreateQR { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PathImage { get; set; }
    public string HashCode { get; set; }
    public string AccessCode { get; set; }
    public string Merchant { get; set; }
    public string UrlDriverCompany { get; set; }
}