using Microsoft.AspNetCore.WebUtilities;

namespace Payment.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OnePayController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSetting _appSetting;
    private IHttpClientFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public OnePayController(IHttpClientFactory factory, AppSetting appSetting, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSetting = appSetting;
        _factory = factory;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [HttpPost("PaymentWeb")]
    public IActionResult CreateTokenPayment(OnePayRequest onePayRequest)
    {
		try
		{
            HttpClient client = _factory.CreateClient();
            client.BaseAddress = new Uri(_appSetting.OnePay.Url);
            var param = $"AgainLink={onePayRequest.AgainLink}&Title={onePayRequest.Title}&vpc_AccessCode={_appSetting.OnePay.AccessCode}&vpc_Amount={onePayRequest.vpc_Amount}" +
                $"&vpc_Command={onePayRequest.vpc_Command}&vpc_CreateToken={onePayRequest.vpc_CreateToken}&vpc_Currency={onePayRequest.vpc_Currency}&vpc_Customer_Id={onePayRequest.vpc_Customer_Id}" +
                $"&vpc_Locale={onePayRequest.vpc_Locale}&vpc_MerchTxnRef={onePayRequest.vpc_MerchTxnRef}&vpc_Merchant={_appSetting.OnePay.Merchant}" +
                $"&vpc_OrderInfo={onePayRequest.vpc_OrderInfo}&vpc_ReturnURL={onePayRequest.vpc_ReturnURL}&vpc_TicketNo={onePayRequest.vpc_TicketNo}&vpc_Version={onePayRequest.vpc_Version}";
         
            onePayRequest.vpc_SecureHash = Utils.SHA256OnePay(param , _appSetting.OnePay.HashCode);
            return Ok($"{_appSetting.OnePay.Url}?{param}&vpc_SecureHash={onePayRequest.vpc_SecureHash.ToUpper()}");
        }
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
    }

    [HttpPost("CreateInvoiceCode")]
    public IActionResult CreateInvoiceCode(OnePayRequest onePayRequest)
    {
        try
        {
            var id = Constants.KEY_ID_DIC;
            var locale = Constants.KEY_LOCALE_DIC;
            HttpClient client = _factory.CreateClient();
            var param = $"vpc_AccessCode={_appSetting.OnePay.AccessCode}&vpc_Amount={onePayRequest.vpc_Amount}&vpc_Command={onePayRequest.vpc_Command}" +
                $"&vpc_Currency={onePayRequest.vpc_Currency}&vpc_Locale={onePayRequest.vpc_Locale}&vpc_MerchTxnRef={onePayRequest.vpc_MerchTxnRef}&vpc_Merchant={_appSetting.OnePay.Merchant}" +
                $"&vpc_OrderInfo={onePayRequest.vpc_OrderInfo}&vpc_ReturnURL={onePayRequest.vpc_ReturnURL}&vpc_TicketNo={onePayRequest.vpc_TicketNo}&vpc_Version={onePayRequest.vpc_Version}";

            onePayRequest.vpc_SecureHash = Utils.SHA256OnePay(param, _appSetting.OnePay.HashCode);

            // Tạo dữ liệu form-urlencoded
            var formData = new Dictionary<string, string>
            {
                { "vpc_AccessCode", _appSetting.OnePay.AccessCode },
                { "vpc_Amount", onePayRequest.vpc_Amount },
                { "vpc_Command", onePayRequest.vpc_Command },
                { "vpc_Currency",onePayRequest.vpc_Currency},
                { "vpc_Locale", onePayRequest.vpc_Locale },
                { "vpc_MerchTxnRef", onePayRequest.vpc_MerchTxnRef },
                { "vpc_Merchant", _appSetting.OnePay.Merchant },
                { "vpc_OrderInfo", onePayRequest.vpc_OrderInfo },
                { "vpc_ReturnURL", onePayRequest.vpc_ReturnURL },
                { "vpc_TicketNo", onePayRequest.vpc_TicketNo },
                { "vpc_Version", onePayRequest.vpc_Version },
                { "vpc_SecureHash", onePayRequest.vpc_SecureHash.ToUpper() }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = client.PostAsync(_appSetting.OnePay.Url, content).GetAwaiter().GetResult();
            string query = response.RequestMessage.RequestUri.Query;
            var parsedQuery = QueryHelpers.ParseQuery(query);
            // Lấy giá trị của tham số id
            string valueId = parsedQuery[id];
            // Lấy giá trị của tham số locale
            string valueLocale = parsedQuery[locale];

            //chưa có key thì add vào dic , có rồi thì xóa key đi rồi add lại value mới vì key dic unique
            if (!TokenStore.IdInvoiceOP.ContainsKey(id) && !TokenStore.IdInvoiceOP.ContainsKey(locale))
            {
                TokenStore.IdInvoiceOP.Add(id, valueId);
                TokenStore.IdInvoiceOP.Add(locale, valueLocale);
            }
            else
            {
                TokenStore.IdInvoiceOP.Remove(id);
                TokenStore.IdInvoiceOP.Remove(locale);
                TokenStore.IdInvoiceOP.Add(id, valueId);
                TokenStore.IdInvoiceOP.Add(locale, valueLocale);
            }
            return Ok($"Id : {valueId} & locale : {valueLocale}");

        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    [HttpGet("CreateQR")]
    public async Task<IActionResult> CreateQR(string id = "vietcombank")
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            HttpClient client = _factory.CreateClient();
            string url = _appSetting.OnePay.Url;
            string baseUrl = url.Substring(0, url.LastIndexOf('/'));

            var param = $"/api/v1/invoices/{TokenStore.IdInvoiceOP[Constants.KEY_ID_DIC]}/qrs";
            var data = new OnePayQrRequest
            {
                app = new Application() { id = id },
                device = new Device() { user_agent = Utils.GetUserInfo(httpContext) },
                platform = Utils.GetPlatformFromUserAgent(Utils.GetUserInfo(httpContext))
            };
            // Chuyển đối tượng data thành chuỗi JSON
            var jsonData = JsonSerializer.Serialize(data);
            // Tạo nội dung của yêu cầu POST
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUrl + param, content);

            // Kiểm tra phản hồi từ API
            if (response.IsSuccessStatusCode)
            {
                // Xử lý phản hồi thành công
                var responseBody = await response.Content.ReadAsStringAsync();
                var objResponse = JsonSerializer.Deserialize<OnePayQrResponse>(responseBody, _jsonSerializerOptions);
                var nameImage = Guid.NewGuid();
                Utils.GenerateQRCode(objResponse.data, 300, 300, _appSetting.VnPay.PathImage + $"{nameImage}.png", "ONEPAY QR");
                return Ok(_appSetting.VnPay.PathImage + $"{nameImage}.png");
            }
            else
            {
                // Xử lý phản hồi lỗi
                return BadRequest($"Lỗi OP return : {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
