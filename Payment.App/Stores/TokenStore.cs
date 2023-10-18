namespace Payment.App.Stores;

public static class TokenStore
{
    //Token của Vnpay
    public static Dictionary<string,string> TokenVNP = new Dictionary<string,string>();
    //Id của OnePay để generate QRCode
    public static Dictionary<string,string> IdInvoiceOP = new Dictionary<string,string>();
}
