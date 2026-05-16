using System.Text.Json;
using System.Text.Json.Serialization;
using StarLight_Core.Models.Utilities;

namespace StarLight_Core.Utilities;

/// <summary>
/// Modrinth 工具类
/// </summary>
public static class ModrinthUtil
{
    /// <summary>
    /// 获取指定数量的随机 Mod
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <a href="https://wiki.conlux.studio/Utilities/Modrinth.html">查看文档</a>
    public static async Task<IEnumerable<ModrinthInfo>?> GetRandomMod(int count)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // 自动转换 snake_case
            Converters = { new JsonStringEnumConverter() }
        };
        try
        {
            var response = await HttpUtil.SendHttpGetRequest($"https://api.modrinth.com/v2/projects_random?count={count}");
            return response.ToJsonEntry<IEnumerable<ModrinthInfo>>(options);
        }
        catch (Exception ex)
        {
            throw new Exception("获取 Modrinth Mod 时发生错误", ex);
        }
    }
}