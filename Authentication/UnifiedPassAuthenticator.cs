using System.Text.Json;
using StarLight_Core.Models;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Utilities;

namespace StarLight_Core.Authentication;

/// <summary>
/// 统一通行证验证类
/// </summary>
public class UnifiedPassAuthenticator
{
    private const string UnifiedPassBaseUrl = "https://auth.mc-user.com:233/";

    /// <summary>
    /// 统一通行证验证器
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="serverId"></param>
    /// <param name="baseUrl"></param>
    public UnifiedPassAuthenticator(string username, string password, string serverId,
        string baseUrl = UnifiedPassBaseUrl)
    {
        Username = username;
        Password = password;
        ServerId = serverId;
        BaseUrl = baseUrl;
    }

    private string Username { get; }

    private string Password { get; }

    private string ServerId { get; }

    private string BaseUrl { get; }

    /// <summary>
    /// 统一通行证异步验证方法
    /// </summary>
    /// <returns>统一通行证账户信息</returns>
    /// <exception cref="ApplicationException">身份验证时出错</exception>
    public async Task<UnifiedPassAccount> UnifiedPassAuthAsync()
    {
        var authenticateUri = new Uri(new Uri(BaseUrl), ServerId + "/authserver/authenticate");

        var requestData = new
        {
            agent = new { name = "StarLight.Core", version = StarLightInfo.Version },
            username = Username,
            password = Password,
            clientToken = null as string,
            requestUser = true
        };

        var jsonData = JsonSerializer.Serialize(requestData);

        string response;
        try
        {
            response = await HttpUtil.SendHttpPostRequest(authenticateUri.ToString(), jsonData, "application/json");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("身份验证时出错: ", ex);
        }

        var authResponse = JsonSerializer.Deserialize<UnifiedPassResponse>(response);
        if (authResponse == null) throw new InvalidOperationException();
        
        return new UnifiedPassAccount
        {
            Name = authResponse.SelectedProfile.Name,
            Uuid = authResponse.SelectedProfile.Uuid,
            AccessToken = authResponse.AccessToken,
            ClientToken = authResponse.ClientToken,
            ServerId = ServerId
        };
    }

    /// <summary>
    /// 刷新统一通行证令牌
    /// </summary>
    /// <param name="clientToken">客户端令牌</param>
    /// <param name="accessToken">资源令牌</param>
    /// <returns>统一通行证账户信息</returns>
    /// <exception cref="ApplicationException">刷新令牌时出错</exception>
    public async Task<UnifiedPassAccount> RefreshUnifiedPassAsync(string clientToken, string accessToken)
    {
        var refreshPostData = new
        {
            accessToken,
            clientToken,
            requestUser = true
        };
        
        string response;
        try
        {
            response = await HttpUtil.SendHttpPostRequest(BaseUrl + "authserver/refresh", refreshPostData.Serialize(), "application/json");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("刷新令牌时出错: ", ex);
        }

        var authResponse = response.ToJsonEntry<UnifiedPassRefreshResponse>();
        if (authResponse == null) throw new InvalidOperationException();
        
        return new UnifiedPassAccount
        {
            Name = authResponse.SelectedProfile.Name,
            Uuid = authResponse.SelectedProfile.Uuid,
            AccessToken = authResponse.AccessToken,
            ClientToken = authResponse.ClientToken,
            ServerId = ServerId
        };
    }
    
    /// <summary>
    /// 销毁当前令牌
    /// </summary>
    /// <param name="accessToken">资源令牌</param>
    /// <param name="clientToken">客户端令牌</param>
    public static async Task InvalidateUnifiedPass(string accessToken, string clientToken)
    {
        var invalidatePostData = new
        {
            accessToken,
            clientToken
        };
        
        await HttpUtil.SendHttpPostRequest(UnifiedPassBaseUrl + "authserver/invalidate", invalidatePostData.Serialize(), "application/json");
    }
    
    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public static async Task SignOutUnifiedPass(string username, string password)
    {
        var signOutPostData = new
        {
            username,
            password
        };
        
        await HttpUtil.SendHttpPostRequest(UnifiedPassBaseUrl + "authserver/signout", signOutPostData.Serialize(), "application/json");
    }

    /// <summary>
    /// 获取玩家皮肤信息
    /// </summary>
    /// <param name="baseUrl">基础 Url</param>
    /// <param name="profileId">账户 Uuid</param>
    /// <returns></returns>
    [Obsolete("此方法已弃用,请使用 StarLight_Core.Skin 中的方法进行获取")]
    public static Dictionary<string, string> RetrieveSkinInfo(string baseUrl, string profileId)
    {
        return new Dictionary<string, string>();
    }
}