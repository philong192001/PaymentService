using System.ComponentModel.DataAnnotations;

namespace Payment.App.Models.VnPay;

public class MerchantRequest
{
    [JsonPropertyName("code")]
    [Required]
    [MaxLength(10)]
    public string Code { set; get; }

    [JsonPropertyName("message")]
    [Required]
    [MaxLength(100)]
    public string Message { set; get; }

    [JsonPropertyName("msgType")]
    [Required]
    [MaxLength(10)]
    public string MsgType { set; get; }

    [JsonPropertyName("txnId")]
    [Required]
    [MaxLength(20)]
    public string TxnId { set; get; }

    [JsonPropertyName("qrTrace")]
    [Required]
    [MaxLength(10)]
    public string QRTrace { set; get; }

    [JsonPropertyName("bankCode")]
    [Required]
    [MaxLength(10)]
    public string BankCode { set; get; }

    [JsonPropertyName("mobile")]
    [MaxLength(20)]
    public string Mobile { set; get; }

    [JsonPropertyName("accountNo")]
    [MaxLength(30)]
    public string AccountNo { set; get; }

    [JsonPropertyName("amount")]
    [Required]
    [MaxLength(13)]
    public string Amount { set; get; }

    [JsonPropertyName("payDate")]
    [Required]
    [MaxLength(14)]
    public string PayDate { set; get; }

    [JsonPropertyName("merchantCode")]
    [Required]
    [MaxLength(20)]
    public string MerchantCode { set; get; }

    [JsonPropertyName("terminalId")]
    [Required]
    [MaxLength(8)]
    public string TerminalId { set; get; }

    [JsonPropertyName("name")]
    [MaxLength(100)]
    public string Name { set; get; }

    [JsonPropertyName("phone")]
    [MaxLength(20)]
    public string Phone { set; get; }

    [JsonPropertyName("province_id")]
    [MaxLength(14)]
    public string ProvinceId { set; get; }

    [MaxLength(14)]
    [JsonPropertyName("district_id")]
    public string DistrictId { set; get; }

    [JsonPropertyName("address")]
    [MaxLength(100)]
    public string Address { set; get; }

    [JsonPropertyName("email")]
    [MaxLength(100)]
    public string Email { set; get; }

    [JsonPropertyName("checksum")]
    [Required]
    [MaxLength(32)]
    public string CheckSum { set; get; }
}