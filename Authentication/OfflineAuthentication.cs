using System.Security.Cryptography;
using System.Text;
using StarLight_Core.Models.Authentication;

namespace StarLight_Core.Authentication;

/// <summary>
/// 离线验证类
/// </summary>
/// <a href="https://wiki.conlux.studio/Authentication/Offline.html">查看文档</a>
public class OfflineAuthentication : BaseAuthentication
{
    /// <summary>
    /// 离线验证器
    /// </summary>
    /// <param name="username">用户名</param>
    /// <a href="https://wiki.conlux.studio/Authentication/Offline.html#构造函数">查看文档</a>
    public OfflineAuthentication(string username)
    {
        AccessToken = Guid.NewGuid().ToString("N");
        ClientToken = Guid.NewGuid().ToString("N");
        Name = username;
        Uuid = GenerateNameUuid(username);
    }

    /// <summary>
    /// 离线验证器
    /// 推荐使用用户名生成 Uuid 构造函数
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="uuid">用户 UUID</param>
    /// <a href="https://wiki.conlux.studio/Authentication/Offline.html#构造函数">查看文档</a>
    public OfflineAuthentication(string username, string uuid)
    {
        AccessToken = Guid.NewGuid().ToString("N");
        ClientToken = Guid.NewGuid().ToString("N");
        Name = username;
        Uuid = uuid;
    }

    /// <summary>
    /// 离线验证器
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="clientToken">客户端令牌</param>
    /// <a href="https://wiki.conlux.studio/Authentication/Offline.html#构造函数">查看文档</a>
    public OfflineAuthentication(string username, string accessToken, string clientToken)
    {
        AccessToken = accessToken;
        ClientToken = clientToken;
        Name = username;
        Uuid = GenerateNameUuid(username);
    }
    
    /// <summary>
    /// 离线验证器
    /// 推荐使用用户名生成 Uuid 构造函数
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="uuid">用户 UUID</param>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="clientToken">客户端令牌</param>
    /// <a href="https://wiki.conlux.studio/Authentication/Offline.html#构造函数">查看文档</a>
    public OfflineAuthentication(string username, string uuid, string accessToken, string clientToken)
    {
        AccessToken = accessToken;
        ClientToken = clientToken;
        Name = username;
        Uuid = uuid;
    }

    private string AccessToken { get; }
    private string Name { get; }
    private string Uuid { get; set; }

    /// <summary>
    /// 验证方法
    /// </summary>
    /// <returns></returns>
    /// <a href="https://wiki.conlux.studio/Authentication/Offline.html#offlineauth-验证方法">查看文档</a>
    public OfflineAccount OfflineAuth()
    {
        if (!IsValidUuid(Uuid)) Uuid = Guid.NewGuid().ToString("N");

        var result = new OfflineAccount
        {
            Name = Name,
            Uuid = Uuid,
            AccessToken = AccessToken,
            ClientToken = ClientToken
        };

        return result;
    }
}