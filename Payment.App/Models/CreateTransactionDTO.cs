namespace Payment.App.Models;

public class CreateTransactionDTO
{
    /// <summary>
    /// Tổng số tiền cần thanh toán
    /// </summary>
    public string Amount { get; set; }

    /// <summary>
    /// ID Lái xe
    /// </summary>
    public string DriverId { get; set; }

    /// <summary>
    /// Id công ty
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Mã đơn hàng , mã chuyến
    /// </summary>
    public Guid BookId { get; set; }

    /// <summary>
    /// Loại ngân hàng thanh toán
    /// </summary>
    public string BankId { get; set; }

    /// <summary>
    /// Ví thanh toán
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Đường dẫn QRCode
    /// </summary>
    public string PathQR { get; set; }
}