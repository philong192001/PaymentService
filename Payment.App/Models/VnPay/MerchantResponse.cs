namespace Payment.App.Models.VnPay;

public class MerchantResponse
{
    public MerchantResponse()
    {
    }


    [JsonPropertyName("code")]
    public string Code { set; get; }
    [JsonPropertyName("message")]
    public string Message { set; get; }
    [JsonPropertyName("data")]
    public TxnData TxnData { set; get; }
}

public class TxnData
{
    [JsonPropertyName( "txnId")]
    public string TxnId { set; get; }
    [JsonPropertyName("amount")]
    public string Amount { set; get; }
}