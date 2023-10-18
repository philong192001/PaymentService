using Payment.App.Common.Enums;
using Payment.App.Models.VnPay;
using Payment.App.Models;

namespace Payment.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VnPayController : ControllerBase
{
    private readonly PaymentDbContext _dbContext;
    private readonly AppSetting _appSetting;
    private IHttpClientFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public VnPayController(IHttpClientFactory factory, AppSetting appSetting, PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
        _appSetting = appSetting;
        _factory = factory;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [HttpPost("GetQR")]
    public IActionResult GetQR(VnPayQrRequest payQrRequest)
    {
        try
        {
            HttpClient client = _factory.CreateClient();
            client.BaseAddress = new Uri(_appSetting.VnPay.UrlCreateQR);
            //kiểm tra key dictionary nếu có thì bỏ qua chưa có thì add vào 
            if (!TokenStore.TokenVNP.ContainsKey(Constants.KEY_TOKEN_DIC))
            {
                TokenStore.TokenVNP.Add(Constants.KEY_TOKEN_DIC, "");
            }
            //param request api
            string queryString = $"?Amount={payQrRequest.Amount}&MerchantCode={payQrRequest.MerchantCode}&ImageFormat={payQrRequest.ImageFormat}" +
                $"&ImageType={payQrRequest.ImageType}&TerminalID={payQrRequest.TerminalID}&Height={payQrRequest.Height}" +
                $"&Width={payQrRequest.Width}&BillNumber={payQrRequest.BillNumber}";
            //response từ API
            var response = client.GetAsync(queryString).Result;
            // Thêm thông tin xác thực vào header "Authorization"
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStore.TokenVNP[Constants.KEY_TOKEN_DIC]);
            // Kiểm tra mã trạng thái của res API
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Gọi lại phương thức getToken() để lấy token mới
                GetToken();
                // Cập nhật lại header "Authorization" với token mới
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStore.TokenVNP[Constants.KEY_TOKEN_DIC]);
                // Thực hiện cuộc gọi API lần nữa với token mới
                response = client.GetAsync(queryString).Result;
            }
            string jsonData = response.Content.ReadAsStringAsync().Result;
            var data = JsonSerializer.Deserialize<VnPayQrResponse>(jsonData, _jsonSerializerOptions);
            var pathImage = Utils.Base64ToImage(data.Data.Image, Guid.NewGuid().ToString(), _appSetting.VnPay.PathImage);
            return Ok(pathImage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("MerchantPayment")]
    public IActionResult MerchantPayment(MerchantRequest request)
    {
        var res = new MerchantResponse
        {
            Code = "00",
            Message = "Success",
        };

        try
        {
            var temp = ValidateTrans(request); //connect HUNGPHAT
            if (temp.Code != "00")
            {
                return Ok(temp);
            }

      
            return Ok();
        }
        catch (FormatException)
        {
            //FileHelper.WriteLog("", "Loi update tran vnpay K ĐÚNG ĐỊNH DẠNG TK HÃNG (QR TĨNH)" + fmEx.ToString(), "LogETran", "LogETran", true);
            return BadRequest(new MerchantResponse
            {
                Code = "04",
                Message = "Record not found",
                TxnData = new TxnData { TxnId = request.TxnId }
            });
        }
        catch (Exception ex)
        {
            //FileHelper.WriteLog("", "Loi update tran vnpay " + ex.ToString(), "LogETran", "LogETran", true);
            return BadRequest( new MerchantResponse
            {
                Code = "04",
                Message = "Exception",
                TxnData = new TxnData { TxnId = request.TxnId }
            });
        }
    }

    private void GetToken()
    {
        HttpClient client = _factory.CreateClient();
        client.BaseAddress = new Uri(_appSetting.VnPay.UrlGetToken);
        string queryString = Constants.QUERY_STRING_TOKEN;

        // Thêm xác thực cơ bản vào header yêu cầu
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appSetting.VnPay.UserName}:{_appSetting.VnPay.Password}"));
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
        topTrans.ErrorCode = Convert.ToInt32(res.Code);
        topTrans.Message = res.Message;
        _dbContext.VnPayTrans.Attach(topTrans);
        // Đánh dấu chỉ cập nhật các trường cần thay đổi
        _dbContext.Entry(topTrans).Property(e => e.ErrorCode).IsModified = true;
        _dbContext.Entry(topTrans).Property(e => e.Message).IsModified = true;
        _dbContext.SaveChanges();
        return res;
    }
}
