using System.Text.Json;
using StarLight_Core.Downloader;
using StarLight_Core.Enum;
using StarLight_Core.Models.Installer;
using StarLight_Core.Models.Utilities;
using StarLight_Core.Utilities;

namespace StarLight_Core.Installer;

/// <summary>
/// Fabric 安装器
/// </summary>
/// <a href="https://wiki.conlux.studio/Installer/Fabric.html">查看文档</a>
public class FabricInstaller : InstallerBase
{
    /// <summary>
    /// Fabric 安装器
    /// </summary>
    /// <param name="gameVersion">游戏版本</param>
    /// <param name="fabricVersion">Fabric 版本</param>
    /// <param name="root">安装目录</param>
    /// <param name="onSpeedChanged">速度报告</param>
    /// <param name="onProgressChanged">进度报告</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <a href="https://wiki.conlux.studio/Installer/Fabric.html">查看文档</a>
    public FabricInstaller(string gameVersion, string fabricVersion, string root = ".minecraft",
        Action<string>? onSpeedChanged = null, Action<string, int>? onProgressChanged = null,
        CancellationToken cancellationToken = default)
    {
        GameVersion = gameVersion;
        FabricVersion = fabricVersion;
        OnSpeedChanged = onSpeedChanged;
        OnProgressChanged = onProgressChanged;
        CancellationToken = cancellationToken;
        Root = FileUtil.IsAbsolutePath(root)
            ? Path.Combine(root)
            : Path.Combine(FileUtil.GetCurrentExecutingDirectory(), root);
    }

    /// <summary>
    /// Fabric 安装器
    /// </summary>
    /// <param name="gameVersion">游戏版本</param>
    /// <param name="fabricVersion">Fabric 版本</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <a href="https://wiki.conlux.studio/Installer/Fabric.html">查看文档</a>
    public FabricInstaller(string gameVersion, string fabricVersion, CancellationToken cancellationToken = default)
    {
        GameVersion = gameVersion;
        FabricVersion = fabricVersion;
        CancellationToken = cancellationToken;
        Root = Path.Combine(FileUtil.GetCurrentExecutingDirectory(), ".minecraft");
    }

    private string GameVersion { get; }

    private string FabricVersion { get; }

    private CancellationToken CancellationToken { get; }

    /// <summary>
    /// 异步安装方法
    /// </summary>
    /// <param name="customId">自定义版本名称</param>
    /// <a href="https://wiki.conlux.studio/Installer/Fabric.html#installasync-异步安装">查看文档</a>
    /// <returns></returns>
    public async Task<FabricInstallResult> InstallAsync(string? customId = null)
    {
        var versionId = customId ?? $"{GameVersion}-fabric-loader_{FabricVersion}";
        var varPath = Path.Combine(Root, "versions", versionId);
        var jsonPath = Path.Combine(varPath, versionId + ".json");
        FileUtil.IsDirectory(varPath, true);

        try
        {
            OnProgressChanged?.Invoke("开始安装 Fabric 加载器", 0);

            if (FileUtil.IsFile(jsonPath))
                return new FabricInstallResult(Status.Failed, GameVersion, FabricVersion, versionId,
                    new Exception("版本已存在"));

            OnProgressChanged?.Invoke("下载加载器索引文件", 20);
            if (CancellationToken != default)
                CancellationToken.ThrowIfCancellationRequested();

            string fabricLoaderJson;
            try
            {
                fabricLoaderJson = await HttpUtil.GetStringAsync(DownloadAPIs.Current.FabricRoot +
                                                               $"/v2/versions/loader/{GameVersion}/{FabricVersion}/profile/json");
            }
            catch (Exception e)
            {
                OnProgressChanged?.Invoke("下载加载器索引文件错误: " + e.Message, 0);
                return new FabricInstallResult(Status.Failed, GameVersion, FabricVersion, customId, e);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var gameCoreData = JsonSerializer.Deserialize<Dictionary<string, object>>(fabricLoaderJson, options);

            if (gameCoreData != null && gameCoreData.ContainsKey("id"))
                gameCoreData["id"] = versionId;

            var modifiedJson = JsonSerializer.Serialize(gameCoreData, options);
            await File.WriteAllTextAsync(jsonPath, modifiedJson, CancellationToken);

            var fabricLoaderEntity = JsonSerializer.Deserialize<GameDownloadJsonEntity>(fabricLoaderJson);

            OnProgressChanged?.Invoke("下载加载器核心文件", 60);
            if (CancellationToken != default)
                CancellationToken.ThrowIfCancellationRequested();

            var librariesDownloader = new MultiFileDownloader();
            var downloadList = new List<DownloadItem>();
            var failedList = new List<DownloadItem>();

            foreach (var versionDownload in fabricLoaderEntity.Libraries)
            {
                var basePath =
                    MinecraftInstallerModel.BuildFromName(versionDownload.Name, Path.DirectorySeparatorChar.ToString());
                var jarFilePath = Path.Combine(Root, "libraries") + basePath;
                var jarDownloadPath = DownloadAPIs.Current.FabricMaven + basePath.Replace("\\", "/");

                if (!FileUtil.IsFile(jarFilePath))
                    downloadList.Add(new DownloadItem(jarDownloadPath, jarFilePath));

                librariesDownloader.OnSpeedChanged = speed =>
                {
                    OnSpeedChanged?.Invoke(CalcMemoryMensurableUnit(speed));
                };

                librariesDownloader.ProgressChanged = (downloaded, total) =>
                {
                    OnProgressChanged?.Invoke($"下载加载器核心文件: {downloaded}/{total}", 60);
                };

                librariesDownloader.DownloadFailed = item => { failedList.Add(item); };

                await librariesDownloader.DownloadFiles(downloadList, CancellationToken);
                librariesDownloader.Dispose();

                OnProgressChanged?.Invoke("补全加载器文件", 80);

                if (CancellationToken != default)
                    CancellationToken.ThrowIfCancellationRequested();

                var assetsDownloader = new MultiFileDownloader
                {
                    OnSpeedChanged = speed =>
                        OnSpeedChanged?.Invoke(CalcMemoryMensurableUnit(speed))
                };

                await assetsDownloader.DownloadFiles(failedList, CancellationToken);
                assetsDownloader.Dispose();

                OnProgressChanged?.Invoke("安装已完成", 100);
                return new FabricInstallResult(Status.Succeeded, GameVersion, FabricVersion, customId);
            }
        }
        catch (OperationCanceledException)
        {
            OnProgressChanged?.Invoke("已取消安装", 0);
            return new FabricInstallResult(Status.Cancel, GameVersion, FabricVersion, customId);
        }
        catch (Exception e)
        {
            OnProgressChanged?.Invoke("安装 Fabric 加载器错误: " + e.Message, 0);
            return new FabricInstallResult(Status.Failed, GameVersion, FabricVersion, customId, e);
        }

        OnProgressChanged?.Invoke("Fabric 加载器安装完成", 100);
        return new FabricInstallResult(Status.Succeeded, GameVersion, FabricVersion, customId);
    }

    /// <summary>
    /// 获取指定 Minecraft 版本的所有 Fabric 版本列表
    /// </summary>
    /// <param name="version">Minecraft 版本</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="Exception"></exception>
    public static async Task<IEnumerable<FabricVersionEntity>> FetchFabricVersionsAsync(string version)
    {
        try
        {
            var json = await HttpUtil.GetStringAsync(DownloadAPIs.Current.FabricRoot + $"/v2/versions/loader/{version}");
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("版本列表为空");

            var result = json.ToJsonEntry<IEnumerable<FabricVersionJsonEntity>>().Select(manifest =>
                new FabricVersionEntity
                {
                    Intermediary = manifest.Intermediary,
                    Loader = manifest.Loader,
                    GameVersion = manifest.Intermediary.Version,
                    Version = manifest.Loader.Version
                }).ToList();

            return result;
        }
        catch (JsonException je)
        {
            throw new Exception("版本列表解析失败：" + je.Message, je);
        }
        catch (HttpRequestException hre)
        {
            throw new Exception("下载版本列表失败：" + hre.Message, hre);
        }
        catch (Exception e)
        {
            throw new Exception("获取版本列表时发生未知错误：" + e.Message, e);
        }
    }
}