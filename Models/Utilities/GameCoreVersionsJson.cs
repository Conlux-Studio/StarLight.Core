using System.Text.Json.Serialization;
using StarLight_Core.Models.Installer;

namespace StarLight_Core.Models.Utilities;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
public class GameCoreVersionsJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("mainClass")]
    public string MainClass { get; set; }

    [JsonPropertyName("inheritsFrom")]
    public string InheritsFrom { get; set; }

    [JsonPropertyName("releaseTime")]
    public string ReleaseTime { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("minecraftArguments")]
    public string MinecraftArguments { get; set; }

    [JsonPropertyName("assets")]
    public string Assets { get; set; }

    [JsonPropertyName("clientVersion")]
    public string ClientVersion { get; set; }

    [JsonPropertyName("arguments")]
    public ArgumentsJson Arguments { get; set; }

    [JsonPropertyName("downloads")]
    public DownloadsJsonEntity Downloads { get; set; }

    [JsonPropertyName("javaVersion")]
    public JavaVersionJsonEntity JavaVersion { get; set; } = new()
    {
        MajorVersion = 8
    };
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释