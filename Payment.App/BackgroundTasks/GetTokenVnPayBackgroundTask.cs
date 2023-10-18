namespace Payment.App.BackgroundTasks;

public class GetTokenVnPayBackgroundTask : BackgroundService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<GetTokenVnPayBackgroundTask> _logger;
    private IHttpClientFactory _factory;
    public GetTokenVnPayBackgroundTask(IHttpClientFactory factory, ILogger<GetTokenVnPayBackgroundTask> logger)
    {
        _logger = logger;
        _factory = factory;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
        //await Task.Factory.StartNew(async () =>
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            HttpClient client = _factory.CreateClient();
        //            client.BaseAddress = new Uri(Constants.URL_VNPAY);
        //            string queryString = Constants.QUERY_STRING_TOKEN;

        //            // Thêm xác thực cơ bản vào tiêu đề yêu cầu
        //            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Constants.USERNAME}:{Constants.PASSWORD}"));
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        //            // Tạo nội dung yêu cầu
        //            var content = new StringContent("", Encoding.UTF8, "application/json");
        //            var response = client.PostAsync(queryString, content).Result;
        //            string jsonData = response.Content.ReadAsStringAsync().Result;
        //            var data = JsonSerializer.Deserialize<VnPayToken>(jsonData, _jsonSerializerOptions);

        //            if (!TokenStore.Token.ContainsKey(Constants.KEY_TOKEN_DIC))
        //            {
        //                TokenStore.Token.Add(Constants.KEY_TOKEN_DIC, data.Access_token);
        //            }
        //            else
        //            {
        //                TokenStore.Token.Remove(Constants.KEY_TOKEN_DIC);
        //                TokenStore.Token.Add(Constants.KEY_TOKEN_DIC, data.Access_token);
        //            }

        //            _logger.LogInformation("Token refreshed successfully");

        //            // Wait for 25 minutes before refreshing the token again
        //            await Task.Delay(TimeSpan.FromMinutes(25), stoppingToken);
        //        }
        //        catch (Exception ex)
        //        {

        //            _logger.LogError(ex, "An error occurred while refreshing the token");
        //        }

        //    }
        //}, stoppingToken);
    }
}
