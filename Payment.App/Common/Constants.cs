namespace Payment.App.Common;

public static class Constants
{
    //VNPAY
    public const string KEY_TOKEN_DIC = "VNPayToken";

    public const string QUERY_STRING_TOKEN = "?grant_type=client_credentials&scope=read";
    public const string IMAGE_FORMAT = "png";
    public const string IMAGE_TYPE = "binary";
    public const string HEIGHT = "2048";
    public const string WIDTH = "2048";
    public const string VNPAY = "VNPay";

    //Onepay
    public const string KEY_ID_DIC = "id";

    public const string KEY_LOCALE_DIC = "locale";
    public const string ONEPAY = "OnePay";
    public const string VPC_COMMAND = "pay";
    public const string VPC_CURRENCY = "VND";
    public const string VPC_LOCALE = "vn";
    public const string VPC_VERSION = "2";
    public const string VPC_RETURN_URL = "https://bagps.com";
    public const string VPC_AGAIN_LINK = "https://bagps.com";
    public const string VPC_TITLE = "Thanh toán qua OnePay";

    //MOMO
    public const string URL_MOMO = "two";
}