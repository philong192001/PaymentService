using Payment.App.DTOs;

namespace Payment.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OnePayController : ControllerBase
{
    private readonly ConfigDbContext _configDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSetting _appSetting;
    private IHttpClientFactory _factory;
    private readonly PaymentDbContext _paymentDbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<OnePayController> _logger;

    public OnePayController(IHttpClientFactory factory, AppSetting appSetting, IHttpContextAccessor httpContextAccessor, PaymentDbContext paymentDbContext, ConfigDbContext configDbContext, ILogger<OnePayController> logger)
    {
        _configDbContext = configDbContext;
        _httpContextAccessor = httpContextAccessor;
        _appSetting = appSetting;
        _factory = factory;
        _paymentDbContext = paymentDbContext;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger = logger;
    }

    /// <summary>
    /// Tạo link thanh toán cho bản web
    /// </summary>
    /// <remarks>
    ///  Sample request:
    ///
    ///       {
    ///         "Amount": "6579800",
    ///         "TicketNo": "10.13.2.12",
    ///         "CompanyId": "49",
    ///         "BankId": "vietcombank",
    ///         "PaymentType" : 0,
    ///          "DriverId" : "LPV2"
    ///       }
    /// </remarks>
    /// <param name="onePayRequest"></param>
    /// <returns></returns>
    /// <response code="200">Trả về response theo model</response>
    /// <response code="400">Log exception và response 400</response>
    [HttpPost("PaymentWeb")]
    [ProducesResponseType(typeof(OnePayRequest), 200)]
    [ProducesResponseType(400)]
    public IActionResult CreateUrlPayment(OnePayRequest onePayRequest)
    {
        try
        {
            if (onePayRequest == null)
            {
                return NotFound("REQUEST NULL");
            }

            HttpClient client = _factory.CreateClient();
            //get config DB
            var config = _configDbContext.PaymentConfigs.Where(x => x.CompanyId.Equals(onePayRequest.CompanyId) && x.PaymentType.Equals(onePayRequest.PaymentType)).FirstOrDefault();
            var orderInfo = Constants.OrderInfo(onePayRequest.CompanyId, onePayRequest.DriverId, (int)onePayRequest.PaymentType);
            var ticketNo = onePayRequest.BookId.Substring(0, Math.Min(onePayRequest.BookId.Length, 15));

            var param = $"AgainLink={Constants.VPC_AGAIN_LINK}&Title={Constants.VPC_TITLE}&vpc_AccessCode={config.AccessCode}&vpc_Amount={onePayRequest.Amount}" +
                $"&vpc_Command={Constants.VPC_COMMAND}&vpc_CreateToken=true&vpc_Currency={Constants.VPC_CURRENCY}&vpc_Customer_Id={onePayRequest.DriverId}" +
                $"&vpc_Locale={Constants.VPC_LOCALE}&vpc_MerchTxnRef={EncryptionHelper.Encrypt(onePayRequest.Amount, onePayRequest.DriverId)}&vpc_Merchant={config.Merchant}" +
                $"&vpc_OrderInfo={orderInfo}&vpc_ReturnURL={Constants.VPC_RETURN_URL}&vpc_TicketNo={ticketNo}&vpc_Version={Constants.VPC_VERSION}";

            var vpc_SecureHash = Utils.SHA256OnePay(param, config.HashCode);
            return Ok($"{config.UrlCreateQR}?{param}&vpc_SecureHash={vpc_SecureHash.ToUpper()}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Tạo QRCode thanh toán
    /// </summary>
    /// <remarks>
    /// Here is an example of how to make a purchase request:
    ///
    ///     {
    ///       "Amount": "6579800",
    ///       "CompanyId": "49",
    ///       "PaymentType" : 0,
    ///       "DriverId" : "LPV2",
    ///       "BookId" : "4C38D4C4-38F3-4725-A72F-2727F4398C96"
    ///     }
    /// </remarks>
    /// <param name="onePayRequest"></param>
    /// <returns>đường dẫn ảnh</returns>
    /// <response code="200">Trả về response theo model</response>
    /// <response code="400">Log exception và response 400</response>
    [HttpPost("GetQR")]
    [ProducesResponseType(typeof(OnePayRequest), 200)]
    [ProducesResponseType(400)]
    public IActionResult GetQR(OnePayRequest onePayRequest)
    {
        try
        {
            _logger.LogInformation($"CreateQR : param {onePayRequest.ToString()}");
            var res = new ResponseAppDTO<string>();
            if (onePayRequest == null)
            {
                res.ErrorCode = ErrorCodeEnum.NullRequest;
                res.Data = "";
                res.Message = ErrorCodeEnum.NullRequest.GetDescription();
                return NotFound(res);
            }

            HttpClient client = _factory.CreateClient();
            var config = _configDbContext.PaymentConfigs.Where(x => x.CompanyId.Equals(onePayRequest.CompanyId) && x.PaymentType.Equals(onePayRequest.PaymentType.ToString())).FirstOrDefault();
            var merchTxtRef = EncryptionHelper.Encrypt(onePayRequest.Amount, onePayRequest.DriverId);
            var orderInfo = Constants.OrderInfo(onePayRequest.CompanyId, onePayRequest.DriverId, (int)onePayRequest.PaymentType);
            var ticketNo = onePayRequest.BookId.Substring(0, Math.Min(onePayRequest.BookId.Length, 14));

            var param = $"vpc_AccessCode={config.AccessCode}&vpc_Amount={onePayRequest.Amount}&vpc_Command={Constants.VPC_COMMAND}" +
                $"&vpc_Currency={Constants.VPC_CURRENCY}&vpc_Locale={Constants.VPC_LOCALE}&vpc_MerchTxnRef={merchTxtRef}&vpc_Merchant={config.Merchant}" +
                $"&vpc_OrderInfo={orderInfo}&vpc_ReturnURL={Constants.VPC_RETURN_URL}&vpc_TicketNo={ticketNo}&vpc_Version={Constants.VPC_VERSION}";

            var vpc_SecureHash = Utils.SHA256OnePay(param, config.HashCode);

            // Tạo dữ liệu form-urlencoded
            var formData = new Dictionary<string, string>
            {
                { "vpc_AccessCode", config.AccessCode },
                { "vpc_Amount", onePayRequest.Amount },
                { "vpc_Command", Constants.VPC_COMMAND },
                { "vpc_Currency",Constants.VPC_CURRENCY},
                { "vpc_Locale", Constants.VPC_LOCALE },
                { "vpc_MerchTxnRef", merchTxtRef},
                { "vpc_Merchant", config.Merchant },
                { "vpc_OrderInfo",orderInfo },
                { "vpc_ReturnURL", Constants.VPC_RETURN_URL },
                { "vpc_TicketNo", ticketNo },
                { "vpc_Version", Constants.VPC_VERSION },
                { "vpc_SecureHash", vpc_SecureHash.ToUpper() }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = client.PostAsync(config.UrlCreateQR, content).GetAwaiter().GetResult();
            string query = response.RequestMessage.RequestUri.Query;
            var parsedQuery = QueryHelpers.ParseQuery(query);

            if (parsedQuery.Count == 0)
            {
                res.ErrorCode = ErrorCodeEnum.Error;
                res.Data = "Request error, try again";
                res.Message = ErrorCodeEnum.Error.GetDescription();
                return NotFound(res);
            }
            // Lấy giá trị của tham số id
            string valueId = parsedQuery["id"];
            //đường dẫn ảnh
            var pathImage = CreateQR(valueId, config).Result;
            _logger.LogInformation($" Value ID : {valueId} -- pathImage : {pathImage} -- Query : {query}");

            //Tạo transaction để lưu dưới trạng thái đợi thanh toán
            var trans = new CreateTransactionDTO()
            {
                Amount = onePayRequest.Amount,
                DriverId = onePayRequest.DriverId,
                CompanyId = onePayRequest.CompanyId,
                PaymentType = PaymentType.OnePay,
                PathQR = pathImage,
                BookId = new Guid(onePayRequest.BookId),
            };
            //Lưu transaction
            Utils.InsertTransaction(trans, _paymentDbContext);

            res.Data = pathImage;
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// API xác nhận thanh toán thành công (phía OnePay request về)
    /// </summary>
    /// <remarks>
    /// Here is an example of how to make a purchase request:
    ///
    ///     {
    ///         "vpc_Command": "pay",
    ///         "vpc_Locale": "vn",
    ///         "vpc_CurrencyCode": "VND",
    ///         "vpc_MerchTxnRef": "string",
    ///         "vpc_Merchant": "TOKENONEPAY",
    ///         "vpc_OrderInfo": "49:LPV234:0:0",
    ///         "vpc_Amount": "251000",
    ///         "vpc_TxnResponseCode": "AAAAACV",
    ///         "vpc_TransactionNo": "TETSONEPAYASFJNSF"
    ///     }
    /// </remarks>
    /// <param name="notiOnepayRequest"></param>
    /// <returns></returns>
    /// <response code="200">Trả về response theo model</response>
    /// <response code="400">Log exception và response 400</response>
    [HttpPost("NotiPayment")]
    [ProducesResponseType(typeof(OnePayRequest), 200)]
    [ProducesResponseType(400)]
    public IActionResult NotiPayment(NotiOnepayRequest notiOnepayRequest)
    {
        try
        {
            var res = new ResponseAppDTO<string>();

            HttpClient client = _factory.CreateClient();
            var url = _configDbContext.PaymentConfigs.Where(x => x.Merchant.Equals(notiOnepayRequest.vpc_Merchant) && x.PaymentType.Equals(Constants.ONEPAY)).Select(x => x.UrlCreateQR).FirstOrDefault();
            string[] substrings = notiOnepayRequest.vpc_OrderInfo.Split(':');
            string[] globalVariables = new string[substrings.Length];
            for (int i = 0; i < substrings.Length; i++)
            {
                globalVariables[i] = substrings[i];
            }

            var data = new UpdatePaymentStatusRequest()
            {
                Amount = float.Parse(notiOnepayRequest.vpc_Amount),
                BillNumber = $"{(int)PayType.PayForTrip}{globalVariables[0].PadLeft(3, '0')}{DateTime.Now.ToString("yyMMddhhmmss")}{globalVariables[1]}"
            };
            //Call API HPGO thông báo trạng thái thanh toán
            var response = Utils.SendNotiPayment(client, data, url);
            //Lấy ra record trans đã lưu ở trạng thái đợi
            var record = _paymentDbContext.QRPayTransactions.Where(x => x.CompanyId.Equals(int.Parse(globalVariables[0])) && x.DriverCode.Equals(globalVariables[1]) && x.PaymentType.Equals(int.Parse(globalVariables[2])) && x.Status.Equals(int.Parse(globalVariables[3]))).FirstOrDefault();

            if (record.Amount != double.Parse(notiOnepayRequest.vpc_Amount))
            {
                res.ErrorCode = ErrorCodeEnum.Error;
                res.Data = "Số tiền từ request không khớp nhau";
                res.Message = ErrorCodeEnum.Error.GetDescription();
                return BadRequest(res);
            }
            //Tạo ransaction để lưu dưới trạng thái thanh toán thành công
            record.PartnerTransId = notiOnepayRequest.vpc_TransactionNo;
            record.Status = (int)TransType.Done;

            //Update trans thanh toán thành công vào DB
            Utils.UpdateTransaction(record, _paymentDbContext);

            res.Data = response;
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<string> CreateQR(string invoiceId, PaymentConfig paymentConfig, string id = "vietcombank")
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            HttpClient client = _factory.CreateClient();
            string url = paymentConfig.UrlCreateQR;
            string baseUrl = url.Substring(0, url.LastIndexOf('/'));

            var param = $"/api/v1/invoices/{invoiceId}/qrs";
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
                if (!Directory.Exists(paymentConfig.PathImage))
                {
                    Directory.CreateDirectory(paymentConfig.PathImage);
                }
                Utils.GenerateQRCode(objResponse.data, 300, 300, $"{paymentConfig.PathImage}{nameImage}.png", "ONEPAY QR");
                _logger.LogInformation($"Response string QRCode : {objResponse}");
                return $"{paymentConfig.PathImage}{nameImage}.png";
            }
            else
            {
                // Xử lý phản hồi lỗi
                return $"Lỗi OP return : {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}