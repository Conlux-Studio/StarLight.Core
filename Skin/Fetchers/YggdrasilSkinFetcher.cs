using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Skin;
using StarLight_Core.Utilities;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;

namespace StarLight_Core.Skin.Fetchers;

/// <summary>
/// 外置皮肤获取器
/// </summary>
public class YggdrasilSkinFetcher
{
    /// <summary>
    /// 获取外置皮肤
    /// </summary>
    /// <param name="account">外置账户</param>
    /// <returns>皮肤图片字节信息</returns>
    public static async Task<byte[]> GetYggdrasilSkinAsync(YggdrasilAccount account)
    {
        return await GetYggdrasilSkinAsync(account.ServerUrl, account.Uuid);
    }

    /// <summary>
    /// 获取外置皮肤
    /// </summary>
    /// <param name="serverUrl">服务器 Url</param>
    /// <param name="uuid">外置账户 Uuid</param>
    /// <returns>皮肤图片字节信息</returns>
    public static async Task<byte[]> GetYggdrasilSkinAsync(string serverUrl, string uuid)
    {
        var baseUrl = serverUrl.TrimEnd("/");
        uuid = uuid.Replace("-", "");
        var skinJson = await HttpUtil.GetStringAsync($"{baseUrl}/sessionserver/session/minecraft/profile/{uuid}");
        var skinUrl =
            Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        skinJson.ToJsonEntry<ProfileJsonEntity>().Properties.First().Value))
                .ToJsonEntry<SkinJsonEntity>()
                .Textures.Skin.Url;
        using var httpClient = new HttpClient();
        return await httpClient.GetByteArrayAsync(skinUrl);
    }
}