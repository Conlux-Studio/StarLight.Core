using System.Text.Json.Serialization;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 设备代码信息
/// </summary>
/// <a href="https://wiki.conlux.studio/Authentication/Microsoft.html#详细-retrievedevicecode-定义">查看文档</a>
public class RetrieveDeviceCode
{
    /// <summary>
    /// 设备代码
    /// </summary>
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; set; }

    /// <summary>
    /// 用户代码
    /// </summary>
    [JsonPropertyName("user_code")]
    public string UserCode { get; set; }

    /// <summary>
    /// 客户端 Id
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// 验证地址
    /// </summary>
    [JsonPropertyName("verification_uri")]
    public string VerificationUri { get; set; }

    /// <summary>
    /// 提示信息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}