namespace Payment.App.Models.VnPay;

public class MerchantRequest
{
    [JsonPropertyName("code")]
    public string Code { set; get; }

    [JsonPropertyName("message")]
    public string Message { set; get; }

    [JsonPropertyName("msgType")]
    public string MsgType { set; get; }

    [JsonPropertyName("txnId")]
    public string TxnId { set; get; }

    [JsonPropertyName("qrTrace")]
    public string QRTrace { set; get; }

    [JsonPropertyName("bankCode")]
    public string BankCode { set; get; }

    [JsonPropertyName("mobile")]
    public string Mobile { set; get; }

    [JsonPropertyName("accountNo")]
    public string AccountNo { set; get; }

    [JsonPropertyName("amount")]
    public string Amount { set; get; }

    [JsonPropertyName("payDate")]
    public string PayDate { set; get; }

    [JsonPropertyName("merchantCode")]
    public string MerchantCode { set; get; }

    [JsonPropertyName("terminalId")]
    public string TerminalId { set; get; }

    [JsonPropertyName("name")]
    public string Name { set; get; }

    [JsonPropertyName("phone")]
    public string Phone { set; get; }

    [JsonPropertyName("province_id")]
    public string ProvinceId { set; get; }

    [JsonPropertyName("district_id")]
    public string DistrictId { set; get; }

    [JsonPropertyName("address")]
    public string Address { set; get; }

    [JsonPropertyName("email")]
    public string Email { set; get; }

    [JsonPropertyName("checksum")]
    public string CheckSum { set; get; }
}