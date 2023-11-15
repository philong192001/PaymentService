using Payment.App.DTOs;

namespace Payment.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VnPayController : ControllerBase
{
    private readonly ConfigDbContext _configDbContext;
    private readonly PaymentDbContext _dbContext;
    private readonly AppSetting _appSetting;
    private IHttpClientFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public VnPayController(IHttpClientFactory factory, AppSetting appSetting, PaymentDbContext dbContext, ConfigDbContext configDbContext)
    {
        _configDbContext = configDbContext;
        _dbContext = dbContext;
        _appSetting = appSetting;
        _factory = factory;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Tạo QRCode VNPay để thanh toán
    /// </summary>
    /// <param name="payQrRequest">Object</param>
    /// <returns> Đường dẫn chứa ảnh QRCode trên server</returns>
    [HttpPost("GetQR")]
    public IActionResult GetQR(VnPayQrRequest payQrRequest)
    {
        try
        {
            var res = new ResponseAppDTO<string>();

            if (payQrRequest == null)
            {
                res.ErrorCode = ErrorCodeEnum.NullRequest;
                res.Data = "Request Null";
                res.Message = ErrorCodeEnum.NullRequest.GetDescription();
                return NotFound(res);
            }

            HttpClient client = _factory.CreateClient();
            var config = _configDbContext.PaymentConfigs.Where(x => x.CompanyId.Equals(payQrRequest.CompanyId) && x.PaymentType.Equals(Constants.VNPAY)).FirstOrDefault();

            client.BaseAddress = new Uri(config.UrlCreateQR);
            //kiểm tra key dictionary nếu có thì bỏ qua chưa có thì add vào
            if (!TokenStore.TokenVNP.ContainsKey(Constants.KEY_TOKEN_DIC))
            {
                TokenStore.TokenVNP.Add(Constants.KEY_TOKEN_DIC, "");
            }
            //param request api
            string queryString = $"?Amount={payQrRequest.Amount}&MerchantCode={config.Merchant}&ImageFormat={Constants.IMAGE_FORMAT}" +
                $"&ImageType={Constants.IMAGE_TYPE}&TerminalID={config.AccessCode}&Height={Constants.HEIGHT}" +
                $"&Width={Constants.WIDTH}&BillNumber={payQrRequest.BookId}";
            //response từ API
            var response = client.GetAsync(queryString).Result;
            // Thêm thông tin xác thực vào header "Authorization"
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStore.TokenVNP[Constants.KEY_TOKEN_DIC]);
            // Kiểm tra mã trạng thái của res API
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Gọi lại phương thức getToken() để lấy token mới
                GetToken(config);
                // Cập nhật lại header "Authorization" với token mới
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStore.TokenVNP[Constants.KEY_TOKEN_DIC]);
                // Thực hiện cuộc gọi API lần nữa với token mới
                response = client.GetAsync(queryString).Result;
            }
            string jsonData = response.Content.ReadAsStringAsync().Result;
            var data = JsonSerializer.Deserialize<VnPayQrResponse>(jsonData, _jsonSerializerOptions);
            //đường dẫn ảnh
            var pathImage = Utils.Base64ToImage(data.Data.Image, Guid.NewGuid().ToString(), config.PathImage);

            //Tạo transaction để lưu dưới trạng thái đợi thanh toán
            var trans = new CreateTransactionDTO()
            {
                Amount = payQrRequest.Amount,
                DriverId = payQrRequest.DriverId,
                CompanyId = payQrRequest.CompanyId,
                PaymentType = PaymentType.VNPay,
                PathQR = pathImage,
                BookId = new Guid(payQrRequest.BookId),
            };

            //lưu giao dịch dưới trạng thái chờ thanh toán
            Utils.InsertTransaction(trans, _dbContext);

            res.Data = pathImage;
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("MerchantPayment")]
    public IActionResult MerchantPayment(MerchantRequest request)
    {
        try
        {

            var temp = ValidateTrans(request);
            if (temp.Code != "00")
            {
                return Ok(temp);
            }

            HttpClient client = _factory.CreateClient();
            var url = _configDbContext.PaymentConfigs.Where(x => x.PaymentType.Equals(Constants.VNPAY) && x.Merchant.Equals(request.MerchantCode) && x.AccessCode.Equals(request.TerminalId)).Select(x => x.UrlCreateQR).FirstOrDefault();

            // Phân biệt QR tĩnh hay động qua Terminal có bằng Terminal của Bình Anh không
            // Chỉ có phần thanh toán cuốc
            // request.TerminalId != WebConfig.BATerminal)
            // 2020/05/12, kiểm tra TERMINAL_ID có thuộc danh sách TERMINAL của các hãng không
            // => Không phải TERMINAL hãng thì sẽ là TERMINAL của từng lái xe
            if (_dbContext.QRPayTerminals.Any(t => t.TerminalCode == request.TerminalId))
            {
                try
                {
                    // 2020/05/18, không cần tách thông tin nữa, login mã TerminalCode trong bảng [QRPay.Terminals] để tìm ra driverCode cần cập nhật giao dịch
                    var terminal = _dbContext.QRPayTerminals.Where(x => x.TerminalCode == request.TerminalId).FirstOrDefault();
                    var driverCode = "";
                    var companyId = 0;
                    if (terminal != null)
                    {
                        driverCode = terminal.DriverCode;
                        companyId = terminal.CompanyId;
                    }
                    //FileHelper.WriteLog("", "Update trans static Comp " + companyId + " dvCode " + driverCode + " amount " + request.Amount, "LogTranStatic", "LogTranStatic", true);

                    if (TokenStore.TransVNP.ContainsKey(companyId))
                    {
                        //update
                        var data = new UpdatePaymentStatusRequest()
                        {
                            Amount = float.Parse(request.Amount),
                            BillNumber = request.TxnId
                        };
                        Utils.SendNotiPayment(client, data, url);

                    }
                }
                catch (FormatException fmEx)
                {
                    //FileHelper.WriteLog("", "Loi update tran vnpay K ĐÚNG ĐỊNH DẠNG TK HÃNG (QR TĨNH)" + fmEx.ToString(), "LogETran", "LogETran", true);
                    return BadRequest(new MerchantResponse { Code = "04", Message = "Record not found", TxnData = new TxnData { TxnId = request.TxnId } });
                }
                catch (Exception ex)
                {
                    //FileHelper.WriteLog("", "Loi update tran vnpay " + ex.ToString(), "LogETran", "LogETran", true);
                    return BadRequest(new MerchantResponse { Code = "04", Message = "Exception", TxnData = new TxnData { TxnId = request.TxnId } });
                }
            }
            else
            {
                // Gồm cả thanh toán cuốc và LX thanh toán phí đàm
                try
                {
                    // Tách thông tin từ billNumber
                    var payType = Convert.ToInt32(request.TxnId.Substring(0, 1));
                    var companyId = Convert.ToInt32(request.TxnId.Substring(1, 3));
                    string monthFeeStr = "";
                    string vehiclePlate = "";

                    if (payType == 1)
                    {
                        monthFeeStr = request.TxnId.Substring(4, 4);
                        vehiclePlate = request.TxnId.Substring(8);
                    }
                    else if (payType == 2)
                    {
                        monthFeeStr = request.TxnId.Substring(4, 12);
                        vehiclePlate = request.TxnId.Substring(16);
                    }
                    else if (payType == 3)
                    {
                        vehiclePlate = request.TxnId.Substring(16);
                    }
                    else
                    {
                        vehiclePlate = request.TxnId.Substring(4);
                    }

                    if (TokenStore.TransVNP.ContainsKey(companyId))
                    {
                        var data = new UpdatePaymentStatusRequest()
                        {
                            Amount = float.Parse(request.Amount),
                            BillNumber = request.TxnId
                        };
                        Utils.SendNotiPayment(client, data, url);
                    }
                    else
                    {
                        return NotFound(new MerchantResponse
                        {
                            Code = "07",
                            Message = "Invalid Money",
                            TxnData = new TxnData { TxnId = request.TxnId }
                        });
                    }
                }
                catch (FormatException fmEx)
                {
                    var record = _dbContext.VnPayTrans.Where(x => x.TxnId == request.TxnId).FirstOrDefault();
                    record.State = TransStateEnum.Fail;

                    _dbContext.VnPayTrans.Update(record);
                    _dbContext.SaveChanges();

                    //FileHelper.WriteLog("", "Loi update tran vnpay K ĐÚNG ĐỊNH DẠNG " + fmEx.ToString(), "LogETran", "LogETran", true);
                    return BadRequest(new MerchantResponse
                    {
                        Code = "04",
                        Message = "Record not found",
                        TxnData = new TxnData { TxnId = request.TxnId }
                    });
                }
                catch (Exception ex)
                {
                    var record = _dbContext.VnPayTrans.Where(x => x.TxnId == request.TxnId).FirstOrDefault();
                    record.State = TransStateEnum.Fail;

                    _dbContext.VnPayTrans.Update(record);
                    _dbContext.SaveChanges();

                    //FileHelper.WriteLog("", "Loi update tran vnpay " + ex.ToString(), "LogETran", "LogETran", true);
                    return BadRequest(new MerchantResponse
                    {
                        Code = "04",
                        Message = "Exception",
                        TxnData = new TxnData { TxnId = request.TxnId }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
            return BadRequest(new MerchantResponse
            {
                Code = "04",
                Message = "Exception",
                TxnData = new TxnData { TxnId = request.TxnId }
            });
        }

        return Ok(new MerchantResponse
        {
            Code = "00",
            Message = "Success",
            TxnData = new TxnData { TxnId = request.TxnId }
        });
        //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);

    }

    private void GetToken(PaymentConfig config)
    {
        HttpClient client = _factory.CreateClient();
        client.BaseAddress = new Uri(config.UrlGetToken);
        string queryString = Constants.QUERY_STRING_TOKEN;

        // Thêm xác thực cơ bản vào header yêu cầu
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.UserName}:{config.Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        // Tạo nội dung yêu cầu
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = client.PostAsync(queryString, content).Result;
        string jsonData = response.Content.ReadAsStringAsync().Result;
        var data = JsonSerializer.Deserialize<VnPayToken>(jsonData, _jsonSerializerOptions);

        if (!TokenStore.TokenVNP.ContainsKey(Constants.KEY_TOKEN_DIC))
        {
            TokenStore.TokenVNP.Add(Constants.KEY_TOKEN_DIC, data.Access_token);
        }
        else
        {
            TokenStore.TokenVNP.Remove(Constants.KEY_TOKEN_DIC);
            TokenStore.TokenVNP.Add(Constants.KEY_TOKEN_DIC, data.Access_token);
        }
    }

    private MerchantResponse ValidateTrans(MerchantRequest request)
    {
        var res = new MerchantResponse
        {
            Code = "00",
            Message = "Success",
        };

        // cjNaVAhET34Oysae là secretKey
        var strToMD5 = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", request.Code, request.MsgType, request.TxnId, request.QRTrace, request.BankCode, request.Mobile, request.AccountNo, request.Amount, request.PayDate, request.MerchantCode, "cjNaVAhET34Oysae");
        var strMD5 = Utils.MD5Hash(strToMD5);
        //FileHelper.WriteLog("", "Current Checksum " + strMD5 + " FromString " + strToMD5, "LogUpdateTrans", "LogUpdateTrans", true);

        // Kiểm tra txnId
        var topTrans = _dbContext.VnPayTrans.Where(x => x.TxnId == request.TxnId).FirstOrDefault();
        if (strMD5 != request.CheckSum)
        {
            res.Code = "06";
            res.Message = "Sai thông tin xác thực";
            //res.TxnData = new TxnData { TxnId = request.TxnId };
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
        }
        else if (topTrans == null)
        {
            res.Code = "04";
            res.Message = "Sai mã giao dịch";
            //res.TxnData = new TxnData { TxnId = request.TxnId };
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
        }
        else if (topTrans.State == TransStateEnum.Success || topTrans.State == TransStateEnum.Fail)
        {
            res.Code = "03";
            res.Message = "Giao dich da thuc hien";
            res.TxnData = new TxnData { TxnId = request.TxnId };
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
        }
        else if (topTrans.Amount != request.Amount)
        {
            res.Code = "07";
            res.Message = "Số tiền không chính xác";
            res.TxnData = new TxnData { Amount = topTrans.Amount };
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
        }
        // Check thông tin ngày tạo mã - so sánh với hiện tại quá x ngày thì response thanh toán thất bại
        else if ((DateTime.Now - topTrans.CreatedDate).TotalMinutes > 15)
        {
            res.Code = "09";
            res.Message = "QR hết hạn thanh toán";
            res.TxnData = new TxnData { Amount = topTrans.Amount };
            //FileHelper.WriteLog("", "RES " + JsonConvert.SerializeObject(res), "LogUpdateTrans", "LogUpdateTrans", true);
        }
        //topTrans.ErrorCode = int.Parse(res.Code);
        topTrans.Message = res.Message;
        _dbContext.VnPayTrans.Attach(topTrans);
        // Đánh dấu chỉ cập nhật các trường cần thay đổi
        _dbContext.Entry(topTrans).Property(e => e.ErrorCode).IsModified = true;
        _dbContext.Entry(topTrans).Property(e => e.Message).IsModified = true;
        _dbContext.SaveChanges();
        return res;
    }
}