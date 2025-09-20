using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Utilities;

#pragma warning disable CA1822
namespace StarLight_Core.Authentication;

/// <summary>
/// 微软验证类
/// </summary>
/// <a href="https://mohen.wiki/Authentication/Microsoft.html">查看文档</a>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public class MicrosoftAuthentication
{
    /// <summary>
    /// 微软验证器
    /// </summary>
    /// <param name="clientId">客户端令牌</param>
    /// <a href="https://mohen.wiki/Authentication/Microsoft.html#构造函数">查看文档</a>
    public MicrosoftAuthentication(string clientId)
    {
        ClientId = clientId;
    }

    private static string[] Scopes => new[] { "XboxLive.signin", "offline_access", "openid", "profile", "email" };

    /// <summary>
    /// 客户端令牌
    /// </summary>
    /// <a href="https://mohen.wiki/Authentication/Microsoft.html#构造函数">查看文档</a>
    public string ClientId { get; init; }

    /// <summary>
    /// 获取用户代码
    /// </summary>
    /// <returns>设备代码信息</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <a href="https://mohen.wiki/Authentication/Microsoft.html#retrievedevicecodeinfo-获取设备代码">查看文档</a>
    public async ValueTask<RetrieveDeviceCode> RetrieveDeviceCodeInfo()
    {
        if (string.IsNullOrEmpty(ClientId))
            throw new ArgumentNullException(nameof(ClientId), "ClientId为空");

        var postData = $"client_id={ClientId}&scope={string.Join(" ", Scopes)}";
        const string deviceCodeUrl = "https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode";
        var responseJson = await HttpUtil.SendHttpPostRequest(deviceCodeUrl, postData);

        var responseDict = JsonSerializer.Deserialize<RetrieveDeviceCode>(responseJson);

        var resultDict = new RetrieveDeviceCode
        {
            ClientId = ClientId,
            UserCode = responseDict.UserCode,
            DeviceCode = responseDict.DeviceCode,
            VerificationUri = responseDict.VerificationUri,
            Message = responseDict.Message
        };

        return resultDict;
    }

    /// <summary>
    /// 轮询获取 Token
    /// </summary>
    /// <returns>令牌信息</returns>
    /// <param name="deviceCodeInfo"></param>
    /// <exception cref="TimeoutException"></exception>
    /// <a href="https://mohen.wiki/Authentication/Microsoft.html#gettokenresponse-轮询获取-token">查看文档</a>
    public async ValueTask<GetTokenResponse> GetTokenResponse(RetrieveDeviceCode deviceCodeInfo)
    {
        using var client = new HttpClient();
        const string tenant = "/consumers";
        var pollingInterval = TimeSpan.FromSeconds(5);
        var codeExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15);

        while (DateTimeOffset.UtcNow < codeExpiresOn)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
                ["device_code"] = deviceCodeInfo.DeviceCode,
                ["client_id"] = deviceCodeInfo.ClientId,
                ["tenant"] = tenant
            });

            var tokenRes = await client.PostAsync("https://login.microsoftonline.com/consumers/oauth2/v2.0/token",
                content);
            var tokenJson = await tokenRes.Content.ReadAsStringAsync();

            if (tokenRes.IsSuccessStatusCode)
            {
                var tokenData = JsonSerializer.Deserialize<GetTokenResponse>(tokenJson);
                if (!string.IsNullOrEmpty(tokenData.AccessToken))
                    return new GetTokenResponse
                    {
                        ExpiresIn = tokenData.ExpiresIn,
                        RefreshToken = tokenData.RefreshToken,
                        AccessToken = tokenData.AccessToken,
                        ClientId = deviceCodeInfo.ClientId
                    };
            }

            await Task.Delay(pollingInterval);
        }

        throw new TimeoutException("登录已超时,请重试");
    }

    /// <summary>
    /// 异步验证方法
    /// </summary>
    /// <returns>微软账户信息</returns> 
    /// <param name="tokenInfo"></param>
    /// <param name="action"></param>
    /// <param name="refreshToken"></param>
    /// <exception cref="Exception"></exception>
    /// <a href="https://mohen.wiki/Authentication/Microsoft.html#microsoftauthasync-异步验证方法">查看文档</a>
    public async ValueTask<MicrosoftAccount> MicrosoftAuthAsync(GetTokenResponse tokenInfo, Action<string> action,
        string? refreshToken = null)
    {
        // 刷新令牌
        if (refreshToken != null)
        {
            action("正在刷新令牌");

            var refreshPostData = $"client_id={ClientId}&refresh_token={refreshToken}&grant_type=refresh_token";
            string refreshResponse;

            try
            {
                refreshResponse = await HttpUtil.SendHttpPostRequest("https://login.live.com/oauth20_token.srf",
                    refreshPostData);
            }
            catch (Exception e)
            {
                throw new Exception("刷新令牌错误: " + e.Message);
            }

            var refreshResponseData = JsonSerializer.Deserialize<TokenResponse>(refreshResponse);

            tokenInfo.AccessToken = refreshResponseData.AccessToken;
            tokenInfo.RefreshToken = refreshResponseData.RefreshToken;
            tokenInfo.ExpiresIn = refreshResponseData.ExpiresIn;
        }

        return await GetMicrosoftAuthInfo(tokenInfo, action);
    }

    /// <summary>
    /// 获取账户信息
    /// </summary>
    /// <param name="tokenInfo"></param>
    /// <param name="action"></param>
    /// <param name="isNewXbl"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public async ValueTask<MicrosoftAccount> GetMicrosoftAuthInfo(GetTokenResponse tokenInfo, Action<string> action,
        bool isNewXbl = true)
    {
        action("开始获取 Microsoft 账号信息");

        // 获取XBL令牌
        action("正在获取 XBL 令牌");
        var rpsTicketValue = isNewXbl ? "d=" : "";

        var xboxLoginContent = new
        {
            Properties = new
            {
                AuthMethod = "RPS",
                SiteName = "user.auth.xboxlive.com",
                RpsTicket = $"{rpsTicketValue}{tokenInfo.AccessToken}"
            },
            RelyingParty = "http://auth.xboxlive.com",
            TokenType = "JWT"
        };

        var xblLoginContentString = JsonSerializer.Serialize(xboxLoginContent);

        string xboxResponseString;

        try
        {
            xboxResponseString = await HttpUtil.SendHttpPostRequest("https://user.auth.xboxlive.com/user/authenticate",
                xblLoginContentString, "application/json");
        }
        catch (Exception e)
        {
            throw new Exception("获取 XBL 令牌错误: " + e.Message);
        }

        var xboxResponseData = JsonSerializer.Deserialize<XboxResponse>(xboxResponseString);
        var xblAuthToken = xboxResponseData.AuthToken;
        var userHash = xboxResponseData.DisplayClaims.Xui[0].UserHash;

        // 获取XSTS令牌
        action("正在获取 XSTS 令牌");
        var getXstsJsonData = new
        {
            Properties = new
            {
                SandboxId = "RETAIL",
                UserTokens = new[] { xblAuthToken }
            },
            RelyingParty = "rp://api.minecraftservices.com/",
            TokenType = "JWT"
        };

        var xstsPostData = JsonSerializer.Serialize(getXstsJsonData);

        var xstsResponse = await HttpUtil.SendHttpPostRequest("https://xsts.auth.xboxlive.com/xsts/authorize",
            xstsPostData, "application/json");

        var xstsResponseData = JsonSerializer.Deserialize<XboxResponse>(xstsResponse);
        var xstsToken = xstsResponseData.AuthToken;

        // Minecraft 身份验证
        action("正在获取 Minecraft 账户信息");
        const string url = "https://api.minecraftservices.com/authentication/login_with_xbox";
        var accountResponseData = new
        {
            identityToken = $"XBL3.0 x={userHash};{xstsToken}"
        };

        var accountPostData = JsonSerializer.Serialize(accountResponseData);

        var accountResponse = await HttpUtil.SendHttpPostRequest(url, accountPostData, "application/json");
        var accountData = JsonSerializer.Deserialize<MinecraftAccountData>(accountResponse);
        var accessToken = accountData.AccessToken;

        // 检查游戏所有权
        action("正在检查游戏所有权");
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync("https://api.minecraftservices.com/entitlements/mcstore");
        var ownTheGame = false;

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var gameAccountJsonData = JsonDocument.Parse(responseContent);

            if (gameAccountJsonData.RootElement.TryGetProperty("items", out var itemsArray))
                ownTheGame = itemsArray.GetArrayLength() > 0;
        }
        else
        {
            throw new Exception("请求失败: " + response.StatusCode);
        }

        if (!ownTheGame) throw new Exception("未购买 Minecraft!");
        action("开始获取玩家档案");
        var profileContent = "null";
        var profileHttpClient = new HttpClient();
        profileHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var profileResponse = await httpClient.GetAsync("https://api.minecraftservices.com/minecraft/profile");

        if (profileResponse.IsSuccessStatusCode) profileContent = await profileResponse.Content.ReadAsStringAsync();

        var jsonObject = JsonDocument.Parse(profileContent);

        var minecraftProfile = JsonSerializer.Deserialize<MinecraftProfile>(profileContent);
        var uuid = minecraftProfile.Uuid;
        var name = minecraftProfile.Name;
        var skinUrl = minecraftProfile.Skins[0].Url;

        action("微软登录完成");

        return new MicrosoftAccount
        {
            Uuid = uuid,
            Name = name,
            ClientToken = Guid.NewGuid().ToString("N"),
            AccessToken = accessToken,
            RefreshToken = tokenInfo.RefreshToken,
            SkinUrl = skinUrl,
            DateTime = DateTime.Now
        };

        throw new Exception("验证失败");
    }
}
#pragma warning restore CA1822