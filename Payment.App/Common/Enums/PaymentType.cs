namespace Payment.App.Common.Enums;

/// <summary>
/// Loại ví thanh toán (OnePay = 0, VnPay = 1, Momo = 2)
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// OnePay = 0
    /// </summary>
    OnePay = 0,
    /// <summary>
    /// VNPay = 1
    /// </summary>
    VNPay = 1,
    /// <summary>
    /// MoMo = 2
    /// </summary>
    Momo = 2,
}