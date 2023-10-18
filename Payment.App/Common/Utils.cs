using SkiaSharp;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.QrCode;

namespace Payment.App.Common;

public class Utils
{
    public static string MD5Hash(string input)
    {
        using (var md5 = MD5.Create())
        {
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            return Encoding.ASCII.GetString(result);
        }
    }

    public static string Sha256(string data)
    {
        using (var sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

            // Convert byte array to a string   
            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    public static string GetIpAddress()
    {
        string ipAddress;
        try
        {
            ipAddress = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ipAddress = "Invalid IP:" + ex.Message;
        }

        return ipAddress;
    }

    static byte[] StringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static string SHA256OnePay(string text, string key)
    {
        byte[] keyBytes = StringToByteArray(key);

        text = SortQueryString(text);

        using (var hmacSha256 = new HMACSHA256(keyBytes))
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacSha256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }

    static string SortQueryString(string queryString)
    {
        var queryParams = HttpUtility.ParseQueryString(queryString);

        var parameters = queryParams.AllKeys
        .Where(key => key.StartsWith("vpc_") || key.StartsWith("user_"))
        .Select(key => new { Key = key, Value = queryParams[key] });

        var sortedQueryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        return sortedQueryString;
    }

    //func convert từ res api của vnpay
    public static string Base64ToImage(string base64String, string nameImage, string pathImage)
    {
        // Convert base 64 string to byte[]
        byte[] imageBytes = Convert.FromBase64String(base64String);
        // Convert byte[] to Image
        // Lưu byte array thành hình ảnh
        using (var stream = new MemoryStream(imageBytes))
        {
            using (var originalImage = SixLabors.ImageSharp.Image.Load(stream))
            {
                // Create new image with white background
                using (var newImage = new Image<Rgba32>(originalImage.Width, originalImage.Height))
                {
                    newImage.Mutate(x => x.BackgroundColor(new Rgba32(255, 255, 255)).DrawImage(originalImage, 1f));

                    // Save the new image with white background
                    using (var outputStream = new MemoryStream())
                    {
                        newImage.Save(outputStream, new PngEncoder());
                        using (var fileStream = new FileStream($"{pathImage}{nameImage}.png", FileMode.Create))
                        {
                            outputStream.Seek(0, SeekOrigin.Begin);
                            outputStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        return $"{pathImage}{nameImage}.png";
    }

    public static string GetUserInfo(HttpContext context)
    {
        // Lấy User Agent từ header của yêu cầu
        string userAgent = context.Request.Headers["User-Agent"];
        return userAgent;
    }

    public static string GetPlatformFromUserAgent(string userAgent)
    {
        if (userAgent.Contains("Windows"))
        {
            Regex regex = new Regex(@"(WOW64|Win64|amd64|x64)");
            Match match = regex.Match(userAgent);
            if (match.Success)
            {
                return "Win64";
            }
            else
            {
                return "Win32";
            }
        }
        else if (userAgent.Contains("Macintosh") || userAgent.Contains("Mac OS"))
        {
            return "Mac OS";
        }
        else if (userAgent.Contains("Linux"))
        {
            return "Linux";
        }
        else if (userAgent.Contains("Android"))
        {
            return "Android";
        }
        else if (userAgent.Contains("iOS"))
        {
            return "iOS";
        }

        return "Unknown";
    }

    //func dùng cho res api onepay
    public static void GenerateQRCode(string content, int width, int height, string filePath, string text)
    {
        var qrCodeData = new QRCodeWriter().encode(content, BarcodeFormat.QR_CODE, width, height);
        var qrCodeBitmap = new SKBitmap(qrCodeData.Width, qrCodeData.Height);

        using (var canvas = new SKCanvas(qrCodeBitmap))
        {
            canvas.Clear(SKColors.White);
            using (var paint = new SKPaint { Color = SKColors.Black })
            {
                for (var y = 0; y < qrCodeData.Height; y++)
                {
                    for (var x = 0; x < qrCodeData.Width; x++)
                    {
                        if (qrCodeData[x, y])
                        {
                            canvas.DrawRect(x, y, 1, 1, paint);
                        }
                    }
                }

                using (var textPaint = new SKPaint { Color = SKColors.Red, TextSize = 20 })
                {
                    var textWidth = textPaint.MeasureText(text);
                    var textHeight = textPaint.FontMetrics.Descent - textPaint.FontMetrics.Ascent;
                    var textX = (qrCodeData.Width - textWidth) / 2;
                    var textY = qrCodeData.Height + textHeight + 10;
                    canvas.DrawText(text, textX, textY, textPaint);
                }
            }
        }

        using (var image = SKImage.FromBitmap(qrCodeBitmap))
        using (var data = image.Encode())
        {
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }
}
