namespace Payment.App.Models;

public class QRPayTransaction
{
    public QRPayTransaction()
    {
    }

    public long TransId { get; set; }

    #region Khách hàng thanh toán tiền

    /// <summary>
    /// Id thanh toán cước di chuyển
    /// </summary>
    public Guid BookId { get; set; }

    /// <summary>
    /// Tên khách hàng
    /// </summary>
    public string? CustomerName { get; set; }

    #endregion Khách hàng thanh toán tiền

    #region Lái xe đóng phí hàng tháng
    /// <summary>
    /// BKS lái xe (trường hợp nạp tiền vào ví lái xe)
    /// </summary>
    public string TripId { get; set; }

    /// <summary>
    /// BKS lái xe (trường hợp nạp tiền vào ví lái xe)
    /// </summary>
    public string? VehiclePlate { get; set; }

    /// <summary>
    /// Số hiệu xe của hãng
    /// </summary>
    public string? PrivateCode { get; set; }

    /// <summary>
    /// Phí dịch vụ hàng tháng của lái xe
    /// </summary>
    public DateTime? FeeMonth { get; set; } = DateTime.Now;

    /// <summary>
    /// Tên lái xe
    /// </summary>
    public string? DriverName { get; set; }

    /// <summary>
    /// Mã lái xe
    /// </summary>
    public string DriverCode { get; set; }

    #endregion Lái xe đóng phí hàng tháng

    /// <summary>
    /// TxnId - TransactionNo
    /// </summary>
    public string? PartnerTransId { get; set; }

    /// <summary>
    /// Số tiền
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Id Công ty
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Phương thức thanh toán
    /// </summary>
    public int PaymentType { get; set; }

    /// <summary>
    /// Ngày tạo QR
    /// </summary>
    public DateTime DateCreateQR { get; set; } = DateTime.Now;

    /// <summary>
    /// Ngày giờ thanh toán
    /// </summary>
    public DateTime? DatePay { get; set; } = DateTime.Now;

    /// <summary>
    /// SDT người thanh toán (vnpay res)
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// link ảnh
    /// </summary>
    public string? QRCode { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DateTime? ExpiredFee { get; set; } = DateTime.Now;

    /// <summary>
    /// Trạng thái thanh toán
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Nội dung thanh toán
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Mã lỗi
    /// </summary>
    public int? ErrorCode { get; set; }
}