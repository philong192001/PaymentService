namespace Payment.App.Models.OnePay;

public class OnePayRequest
{
    /// <summary>
    /// Tổng số tiền cần thanh toán (2 số cuối phải là số 0)
    /// <example> 1353500</example>>
    /// </summary>
    public string Amount
    {
        get { return Amount; }
        set
        {
            Amount = value.PadLeft(value.Length + 2, '0');
        }
    }

    /// <summary>
    /// ID Lái xe
    /// </summary>
    public string DriverId { get; set; }

    /// <summary>
    /// Id công ty
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Id chuyến
    /// </summary>
    public string BookId { get; set; }

    /// <summary>
    /// Loại ví thanh toán 
    /// </summary>
    public PaymentType PaymentType { get; set; }
}