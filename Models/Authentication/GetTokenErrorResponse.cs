using System.Text.Json.Serialization;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 令牌错误信息
/// </summary>
/// <a href="https://wiki.conlux.studio/Authentication/Microsoft.html#详细-gettokenresponse-定义">查看文档</a>
public class GetTokenErrorResponse
{
    /// <summary>
    /// 错误
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }
}