namespace Payment.App.Models.VnPay;

//class tạo QR
public class VnPayQrRequest
{
    /// <summary>
    /// Giá tiền cần thanh toán
    /// </summary>
    public string Amount { get; set; }

    /// <summary>
    /// Id chuyến của khách hàng
    /// </summary>
    public string BookId { get; set; }

    /// <summary>
    /// ID chuyến của lái xe
    /// </summary>
    public string TripId { get; set; }

    /// <summary>
    /// ID Công ty
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// ID Lái xe
    /// </summary>
    public string DriverId { get; set; }
}