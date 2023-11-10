namespace Payment.App.Models.OnePay;

public class NotiOnepayRequest
{
    /// <summary>
    /// Loại thanh toán
    /// </summary>
    public string vpc_Command { get; set; }

    /// <summary>
    /// Ngôn ngữ thanh toán
    /// </summary>
    public string vpc_Locale { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    public string vpc_CurrencyCode { get; set; }

    /// <summary>
    /// Mã hóa đơn giao dịch của người gửi (unique)
    /// </summary>
    public string vpc_MerchTxnRef { get; set; }

    /// <summary>
    /// Mã của OnePay cung cấp
    /// </summary>
    public string vpc_Merchant { get; set; }

    /// <summary>
    /// Thông tin đơn hàng
    /// </summary>
    public string vpc_OrderInfo { get; set; }

    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public string vpc_Amount { get; set; }

    /// <summary>
    /// Code trả về
    /// </summary>
    public string vpc_TxnResponseCode { get; set; }

    /// <summary>
    /// Mã hóa đơn giao dịch của OnePay
    /// </summary>
    public string vpc_TransactionNo { get; set; }

    /// <summary>
    /// Thông tin về giao dịch
    /// </summary>
    public string vpc_Message { get; set; }

    /// <summary>
    /// Thông tin thẻ , hoặc mobile banking thanh toán
    /// </summary>
    public string vpc_Card { get; set; }

    /// <summary>
    /// Kênh thanh toán
    /// </summary>
    public string vpc_PayChannel { get; set; }

    /// <summary>
    /// Mã hóa request thanh toán
    /// </summary>
    public string vpc_SecureHash { get; set; }
}