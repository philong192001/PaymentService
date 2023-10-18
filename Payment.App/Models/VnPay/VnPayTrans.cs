using Payment.App.Common.Enums;

namespace Payment.App.Models.VnPay;

public class VnPayTrans
{
    public long Id { get; set; }
    public string TxnId { get; set; }
    public TransStateEnum State { get; set; }
    public string Amount { get; set; }
    public string TerminalId { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ErrorCode { get; set; }
    public string Message { get; set; }
}
