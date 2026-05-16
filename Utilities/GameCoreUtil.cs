using System.Text.Json;
using StarLight_Core.Enum;
using StarLight_Core.Models.Utilities;

namespace StarLight_Core.Utilities;

/// <summary>
/// 游戏核心工具类
/// </summary>
public class GameCoreUtil
{
    /// <summary>
    /// 获取游戏核心信息
    /// </summary>
    /// <param name="root"></param>
    /// <returns>指定路径的游戏列表</returns>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<GameCoreInfo> GetGameCores(string root = ".minecraft")
    {
        var rootPath = "null";

        FileUtil.IsDirectory(root, true);

        rootPath = FileUtil.IsAbsolutePath(root)
            ? Path.Combine(root, "versions")
            : Path.Combine(FileUtil.GetCurrentExecutingDirectory(), root, "versions");

        FileUtil.IsDirectory(rootPath, true);

        var versions = Directory.GetDirectories(rootPath);
        var gameCores = new List<GameCoreInfo>();

        foreach (var version in versions)
        {
            var versionFolder = new DirectoryInfo(version);
            var versionFile = Path.Combine(versionFolder.FullName, versionFolder.Name + ".json");

            if (!File.Exists(versionFile)) continue;
            try
            {
                var jsonData = File.ReadAllText(versionFile);
                var gameCore = jsonData.ToJsonEntry<GameCoreVersionsJson>();
                var gameCoreInfo = new GameCoreInfo();

                if (gameCore != null)
                    gameCoreInfo = new GameCoreInfo
                    {
                        Id = gameCore.Id,
                        Type = gameCore.Type,
                        JavaVersion = gameCore.JavaVersion?.MajorVersion ?? 0,
                        MainClass = gameCore.MainClass,
                        InheritsFrom = gameCore.InheritsFrom,
                        ReleaseTime = gameCore.ReleaseTime,
                        IsNewVersion = gameCore.Arguments?.Game != null,
                        Time = gameCore.Time,
                        root = rootPath + Path.DirectorySeparatorChar + gameCore.Id,
                        Version = gameCore.ClientVersion,
                        Assets = gameCore.Assets,
                        LoaderType = GetLoader(gameCore.MainClass)
                    };

                if (gameCoreInfo.InheritsFrom != null && gameCoreInfo.InheritsFrom != "null")
                    gameCoreInfo.Version = gameCoreInfo.InheritsFrom;

                gameCoreInfo.Version ??= gameCoreInfo.Assets;
                gameCoreInfo.Version ??= "0.0.0";

                gameCores.Add(gameCoreInfo);
            }
            catch (Exception ex)
            {
                gameCores.Add(new GameCoreInfo { Exception = ex });
            }
        }

        return gameCores;
    }
    
    /// <summary>
    /// 获取指定版本的游戏核心信息
    /// </summary>
    /// <param name="versionId">版本名称</param>
    /// <param name="root">游戏根目录</param>
    /// <exception cref="Exception"></exception>
    /// <returns>指定版本的游戏核心信息</returns>
    public static GameCoreInfo GetGameCore(string versionId, string root = ".minecraft")
    {
        FileUtil.IsDirectory(root, true);

        var rootPath = FileUtil.IsAbsolutePath(root)
            ? Path.Combine(root, "versions")
            : Path.Combine(FileUtil.GetCurrentExecutingDirectory(), root, "versions");

        var versions = Directory.GetDirectories(rootPath);

        foreach (var version in versions)
        {
            var versionFolder = new DirectoryInfo(version);
            var versionFile = Path.Combine(versionFolder.FullName, versionFolder.Name + ".json");

            if (!File.Exists(versionFile)) continue;
            try
            {
                var jsonData = File.ReadAllText(versionFile);
                var gameCore = JsonSerializer.Deserialize<GameCoreVersionsJson>(jsonData);

                if (gameCore == null || gameCore.Id != versionId) continue;
                var gameCoreInfo = new GameCoreInfo
                {
                    Id = gameCore.Id,
                    Type = gameCore.Type,
                    JavaVersion = gameCore.JavaVersion?.MajorVersion ?? 0,
                    MainClass = gameCore.MainClass,
                    InheritsFrom = gameCore.InheritsFrom,
                    ReleaseTime = gameCore.ReleaseTime,
                    Time = gameCore.Time,
                    IsNewVersion = gameCore.Arguments?.Game != null,
                    root = rootPath + "\\" + gameCore.Id,
                    Assets = gameCore.Assets,
                    MinecraftArguments = gameCore.MinecraftArguments,
                    Version = gameCore.ClientVersion,
                    LoaderType = GetLoader(gameCore.MainClass)
                };

                try
                {
                    if (gameCoreInfo.InheritsFrom != null)
                        gameCoreInfo.Assets = GetGameCore(gameCoreInfo.InheritsFrom, root).Assets;
                }
                catch (Exception e)
                {
                    _ = e;
                }

                if (gameCoreInfo.InheritsFrom != null && gameCoreInfo.InheritsFrom != "null")
                    gameCoreInfo.Version = gameCoreInfo.InheritsFrom;

                gameCoreInfo.Version ??= gameCoreInfo.Assets;
                gameCoreInfo.Version ??= "0.0.0";

                try
                {
                    if (gameCore.Arguments != null)
                        gameCoreInfo.Arguments = gameCore.Arguments;
                    else
                        gameCoreInfo.MinecraftArguments = gameCore.MinecraftArguments;
                }
                catch (Exception e)
                {
                    gameCoreInfo.IsNewVersion = false;
                }

                return gameCoreInfo;
            }
            catch (JsonException ex)
            {
                throw new Exception($"版本 JSON 解析错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"发生错误: {ex.Message}");
            }
        }

        return null!;
    }

    private static LoaderType GetLoader(string mainClass)
    {
        return mainClass switch
        {
            "net.fabricmc.loader.impl.launch.knot.KnotClient" => LoaderType.Fabric,
            "net.minecraft.launchwrapper.Launch" => LoaderType.Forge,
            _ => LoaderType.Vanilla
        };
    }

    internal static int GetMajorVersion(string version)
    {
        var parts = version.Split('.');
        return parts.Length > 1 ? int.Parse(parts[1]) : 0;
    }
}