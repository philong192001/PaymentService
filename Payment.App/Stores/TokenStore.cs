namespace Payment.App.Stores;

public class TokenStore
{
    //Token của Vnpay
    public static Dictionary<string, string> TokenVNP = new Dictionary<string, string>();

    //Dic Info Transaction Onepay
    public static Dictionary<int, string> TransVNP = new Dictionary<int, string>();

    //Id của OnePay để generate QRCode
    //public static Dictionary<string,string> IdInvoiceOP = new Dictionary<string,string>();
}