using StarLight_Core.Models.Authentication;
using StarLight_Core.Utilities;

namespace StarLight_Core.Authentication;

/// <summary>
/// 外置验证类
/// </summary>
/// <a href="https://wiki.conlux.studio/Authentication/Yggdrasil.html">查看文档</a>
public class YggdrasilAuthenticator : BaseAuthentication
{
    /// <summary>
    /// 外置验证器
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <a href="https://wiki.conlux.studio/Authentication/Yggdrasil.html">查看文档</a>
    public YggdrasilAuthenticator(string email, string password)
    {
        Url = "https://littleskin.cn/api/yggdrasil";
        Email = email;
        Password = password;
        ClientToken = Guid.NewGuid().ToString("D");
    }
    
    /// <summary>
    /// 外置验证器
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="clientToken"></param>
    /// <a href="https://wiki.conlux.studio/Authentication/Yggdrasil.html">查看文档</a>
    public YggdrasilAuthenticator(string email, string password, string clientToken)
    {
        Url = "https://littleskin.cn/api/yggdrasil";
        Email = email;
        Password = password;
        ClientToken = clientToken;
    }
    
    /// <summary>
    /// 外置验证器
    /// </summary>
    /// <param name="url"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="clientToken"></param>
    /// <a href="https://wiki.conlux.studio/Authentication/Yggdrasil.html">查看文档</a>
    public YggdrasilAuthenticator(string url, string email, string password, string? clientToken = null)
    {
        Url = url == "littleskin.cn" ? "https://littleskin.cn/api/yggdrasil" : url;
        Email = email;
        Password = password;
        ClientToken = clientToken ?? Guid.NewGuid().ToString("D");
    }
    

    private string Url { get; }

    private string Email { get; }

    private string Password { get; }

    /// <summary>
    /// 异步验证方法
    /// </summary>
    /// <returns></returns>
    /// <a href="https://wiki.conlux.studio/authentication/yggdrasil.html">查看文档</a>
    public async ValueTask<IEnumerable<YggdrasilAccount>> YggdrasilAuthAsync()
    {
        var requestJson = new
        {
            clientToken = IsValidUuid(ClientToken) ? ClientToken : Guid.NewGuid().ToString("D"),
            username = Email,
            password = Password,
            requestUser = false,
            agent = new
            {
                name = "Minecraft",
                version = 1
            }
        };

        var baseUrl = string.IsNullOrEmpty(Url) ? "https://littleskin.cn/api/yggdrasil" : Url;
        var requestUrl = $"{baseUrl}/authserver/authenticate";

        var postResponseContent =
            await HttpUtil.SendHttpPostRequest(requestUrl, requestJson.Serialize(), "application/json");

        var accountMessage = postResponseContent.ToJsonEntry<YggdrasilResponse>();
        if (accountMessage != null)
            return accountMessage.UserAccounts.Select(userAccount => new YggdrasilAccount
                {
                    AccessToken = accountMessage.AccessToken,
                    ClientToken = accountMessage.ClientToken,
                    Name = userAccount.Name,
                    Uuid = Guid.Parse(userAccount.Uuid).ToString(),
                    ServerUrl = Url,
                    Email = Email,
                    Password = Password
                })
                .ToList();
        // TODO: 错误处理机制
        throw new InvalidOperationException();
    }

    /// <summary>
    /// 异步刷新方法，用于刷新 AccessToken
    /// </summary>
    /// <param name="account">Yggdrasil 账户信息</param>
    /// <returns>刷新后的 Yggdrasil 账号列表</returns>
    /// <exception cref="InvalidOperationException">刷新失败时抛出，并包含服务器返回的错误信息</exception>
    public async ValueTask<IEnumerable<YggdrasilAccount>> YggdrasilRefreshAsync(YggdrasilAccount account)
    {
        var requestJson = new
        {
            accessToken = account.AccessToken,
            clientToken = account.ClientToken,
            requestUser = false
        };
        
        var baseUrl = string.IsNullOrEmpty(Url) ? "https://littleskin.cn/api/yggdrasil" : Url;
        var requestUrl = $"{baseUrl}/authserver/refresh";
        var postResponseContent = await HttpUtil.SendHttpPostRequest(requestUrl, requestJson.Serialize(), "application/json");
        var refreshedResponse = postResponseContent.ToJsonEntry<YggdrasilResponse>();

        if (refreshedResponse == null)
        {
            throw new InvalidOperationException("刷新失败：服务器返回的响应为空或无效");
        }

        // 5. 将响应数据映射到业务模型列表，处理方式与认证方法保持一致
        return refreshedResponse.UserAccounts.Select(userAccount => new YggdrasilAccount
            {
                AccessToken = refreshedResponse.AccessToken,
                ClientToken = refreshedResponse.ClientToken,
                Name = userAccount.Name,
                Uuid = Guid.Parse(userAccount.Uuid).ToString(),
                ServerUrl = Url,
                Email = Email,
                Password = Password
            })
            .ToList();
    }
}