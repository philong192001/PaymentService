using System.ComponentModel.DataAnnotations;

namespace Payment.App.Models.OnePay;

public class NotiOnepayRequest
{
    /// <summary>
    /// Loại thanh toán
    /// </summary>
    [MaxLength(3)]
    public string vpc_Command { get; set; }

    /// <summary>
    /// Ngôn ngữ thanh toán
    /// </summary>
    [MaxLength(5)]
    public string vpc_Locale { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    [MaxLength(3)]
    public string vpc_CurrencyCode { get; set; }

    /// <summary>
    /// Mã hóa đơn giao dịch của người gửi (unique)
    /// </summary>
    [MaxLength(40)]
    public string vpc_MerchTxnRef { get; set; }

    /// <summary>
    /// Mã của OnePay cung cấp
    /// </summary>
    [MaxLength(12)]
    public string vpc_Merchant { get; set; }

    /// <summary>
    /// Thông tin đơn hàng
    /// </summary> 
    [MaxLength(34)]
    public string vpc_OrderInfo { get; set; }

    /// <summary>
    /// Số tiền thanh toán đã nhân 100
    /// </summary> 
    [MaxLength(36)]
    public string vpc_Amount { get; set; }

    /// <summary>
    /// Code trả về
    /// </summary> 
    [MaxLength(3)]
    public string vpc_TxnResponseCode { get; set; }

    /// <summary>
    /// Mã hóa đơn giao dịch của OnePay
    /// </summary>
    [MaxLength(12)]
    public string vpc_TransactionNo { get; set; }

    /// <summary>
    /// Thông tin về giao dịch
    /// </summary>
    [MaxLength(200)]
    public string vpc_Message { get; set; }

    /// <summary>
    /// Thông tin thẻ , hoặc mobile banking thanh toán
    /// </summary>
    [MaxLength(6)]
    public string vpc_Card { get; set; }

    /// <summary>
    /// Kênh thanh toán
    /// </summary>
    [MaxLength(12)]
    public string vpc_PayChannel { get; set; }

    /// <summary>
    /// Mã hóa request thanh toán
    /// </summary>
    [MaxLength(64)]
    public string vpc_SecureHash { get; set; }
}