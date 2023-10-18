namespace Payment.App.Models
{
    public class QRPayTerminal
    {
        public int Id { get; set; }
        public string DriverCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MasterMerchant { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantType { get; set; }
        public string TerminalName { get; set; }
        public string TerminalCode { get; set; }
        public int CompanyId { get; set; }
        public string DriverName { get; set; }
        public int TerminalType { get; set; }
        public string MomoPartnerCode { get; set; }
        public string MomoRequestType { get; set; }
        public string MomoAccessKey { get; set; }
        public string MomoSecretKey { get; set; }
        public string MomoApiUrl { get; set; }
    }
}
