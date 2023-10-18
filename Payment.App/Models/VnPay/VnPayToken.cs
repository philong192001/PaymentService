namespace Payment.App.Models.VnPay;

public class VnPayToken
{
    public string Access_token { get; set; }
    public string Token_type { get; set; }
    public int Expires_in { get; set; }
    public string Scope { get; set; }
    public string MerchantCode { get; set; }
    public string Iss { get; set; }
    public string Jti { get; set; }
}
