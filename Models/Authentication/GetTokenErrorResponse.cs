using System.Text.Json.Serialization;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 令牌错误信息
/// </summary>
public class GetTokenErrorResponse
{
    /// <summary>
    /// 验证令牌
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }
}