using System.ComponentModel;

namespace Payment.App.DTOs;

public class ResponseAppDTO<T>
{
    /// <summary>
    /// mã lỗi
    /// </summary>
    [JsonPropertyName("ErrorCode")]
    public ErrorCodeEnum ErrorCode { get; set; } = ErrorCodeEnum.Success;

    /// <summary>
    /// thông báo lỗi
    /// </summary>
    [JsonPropertyName("Message")]
    public string Message { get; set; } = ErrorCodeEnum.Success.GetDescription();

    /// <summary>
    /// dữ liệu trả về
    /// </summary>
    [JsonPropertyName("Data")]
    public T? Data { get; set; }
}

public enum ErrorCodeEnum
{
    [Description("Thành công")]
    Success = 0,

    [Description("Lỗi: gửi tham số null")]
    NullRequest = 1,

    [Description("Error")]
    Error = 2,
}


