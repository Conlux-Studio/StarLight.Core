using System.Text.Json.Serialization;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 令牌信息
/// </summary>
/// <a href="https://wiki.conlux.studio/Authentication/Microsoft.html#详细-gettokenresponse-定义">查看文档</a>
public class GetTokenResponse
{
    /// <summary>
    /// 验证令牌
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// 客户端 Id
    /// </summary>
    [JsonIgnore]
    public string ClientId { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}