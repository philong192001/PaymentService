namespace Payment.App.Models.OnePay;

public class UpdatePaymentStatusRequest
{
    /// <summary>
    /// Sô tiền
    /// </summary>
    public float Amount { set; get; }

    /// <summary>
    /// Mã giao dịch theo cấu trúc : Id Công ty + (id chuyến hoặc id lái xe) + năm, tháng , ngày , giờ , phút , giây = 20 số
    ///<example>123333231109008054</example>
    /// </summary>
    public string BillNumber { set; get; }
}