using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace StarLight_Core.Authentication;

/// <summary>
/// 验证基类
/// </summary>
public abstract class BaseAuthentication
{
    /// <summary>
    /// 客户端令牌
    /// </summary>
    protected string ClientToken { get; set; } = string.Empty;

    /// <summary>
    /// 验证 Uuid 是否合法
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    protected static bool IsValidUuid(string uuid) => Guid.TryParseExact(uuid, "D", out _) || Guid.TryParseExact(uuid, "N", out _);
    
    /// <summary>
    /// 生成 UUID
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    protected static string GenerateNameUuid(string characterName)
    {
        var input = "OfflinePlayer:" + characterName;
        var inputBytes = Encoding.UTF8.GetBytes(input);
        
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(inputBytes);

        // RFC 4122
        hashBytes[6] &= 0x0f;
        hashBytes[6] |= 0x30;
        hashBytes[8] &= 0x3f;
        hashBytes[8] |= 0x80;

        return new Guid(hashBytes).ToString();
    }
}