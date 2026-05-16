using StarLight_Core.Enum;

namespace StarLight_Core.Models.Utilities;

/// <summary>
/// 游戏核心信息
/// </summary>
public class GameCoreInfo
{
    public string Id { get; set; }

    public string Version { get; set; }

    public string Type { get; set; }

    public int JavaVersion { get; set; }

    public string InheritsFrom { get; set; }

    public string MainClass { get; set; }

    public string ReleaseTime { get; set; }

    public bool IsNewVersion { get; set; }

    public string Time { get; set; }

    public string root { get; set; }

    public string Assets { get; set; }

    public string MinecraftArguments { get; set; }

    public LoaderType LoaderType { get; set; }

    public ArgumentsJson Arguments { get; set; }

    public Exception Exception { get; set; }
}