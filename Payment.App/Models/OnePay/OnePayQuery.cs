namespace Payment.App.Models.OnePay;

public class OnePayQuery
{
    public string vpc_Command { get; set; } = "queryDR";
    public string vpc_Version { get; set; } = "2";
    public string vpc_MerchTxnRef { get; set; }
    public string vpc_Merchant { get; set; }
    public string vpc_AccessCode { get; set; }
    public string vpc_User { get; set; }
    public string vpc_Password { get; set; }
    public string vpc_SecureHash { get; set; }
}
