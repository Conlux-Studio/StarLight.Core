using System.Text;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Skin;
using StarLight_Core.Utilities;

namespace StarLight_Core.Skin.Fetchers;

/// <summary>
/// 微软皮肤获取器
/// </summary>
public class MicrosoftSkinFetcher
{
    /// <summary>
    /// 获取微软皮肤
    /// </summary>
    /// <param name="account">皮肤图片字节信息</param>
    /// <returns>皮肤图片字节信息</returns>
    public static async Task<byte[]> GetMicrosoftSkinAsync(MicrosoftAccount account)
    {
        return await GetMicrosoftSkinAsync(account.Uuid);
    }

    /// <summary>
    /// 获取微软皮肤
    /// </summary>
    /// <param name="uuid">微软账户 Uuid</param>
    /// <returns>皮肤图片字节信息</returns>
    public static async Task<byte[]> GetMicrosoftSkinAsync(string uuid)
    {
        const string baseUrl = "https://sessionserver.mojang.com/session/minecraft/profile/";
        uuid = uuid.Replace("-", "");
        var skinJson = await HttpUtil.GetStringAsync(baseUrl + uuid);
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