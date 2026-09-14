using System.Diagnostics;
using StarLight_Core.Enum;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;

namespace StarLight_Core.Launch;

public class MinecraftLauncher
{
    public MinecraftLauncher(LaunchConfig launchConfig)
    {
        GameWindowConfig = launchConfig.GameWindowConfig;
        GameCoreConfig = launchConfig.GameCoreConfig;
        JavaConfig = launchConfig.JavaConfig;
        BaseAccount = launchConfig.Account.BaseAccount;
    }

    public BaseAccount BaseAccount { get; set; }

    public GameWindowConfig GameWindowConfig { get; set; }

    public GameCoreConfig GameCoreConfig { get; set; }

    public JavaConfig JavaConfig { get; set; }

    // TODO: 更为完善统一的进度报告
    /// <summary>
    /// 启动游戏
    /// </summary>
    /// <param name="onProgressChanged">进度报告</param>
    /// <returns></returns>
    public async Task<LaunchResponse> LaunchAsync(Action<ProgressReport> onProgressChanged)
    {
        var stopwatch = new Stopwatch();
        var process = new Process();
        var progressReport = new ProgressReport();

        progressReport.Description = "检查启动配置";
        progressReport.Percentage = 20;
        onProgressChanged?.Invoke(progressReport);

        if (GameCoreConfig == null)
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception("未配置游戏核心信息"));
        if (JavaConfig == null) 
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception("未配置Java"));
        if (BaseAccount == null) 
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception("未配置账户信息"));
        if (GameCoreUtil.GetGameCore(GameCoreConfig.Version, GameCoreConfig.Root) == null)
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception("游戏核心不存在或游戏核心已损坏"));
        if (JavaUtil.GetJavaInfo(JavaConfig.JavaPath) == null)
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception("Java 不存在或 Java 已损坏"));

        try
        {
            progressReport.Description = "解压游戏资源";
            progressReport.Percentage = 50;
            onProgressChanged?.Invoke(progressReport);
            await FileUtil.DecompressionNatives(GameCoreConfig);
        }
        catch (Exception e)
        {
            return new LaunchResponse(Status.Failed, stopwatch, process, new Exception($"解压游戏资源错误 : {e}"));
        }

        try
        {
            string optionsFilePath;
            if (GameCoreConfig.IsVersionIsolation)
                optionsFilePath = FileUtil.IsAbsolutePath(GameCoreConfig.Root)
                    ? Path.Combine(GameCoreConfig.Root, "versions", GameCoreConfig.Version, "options.txt")
                    : Path.Combine(FileUtil.GetCurrentExecutingDirectory(), GameCoreConfig.Root, "versions",
                        GameCoreConfig.Version, "options.txt");
            else
                optionsFilePath = FileUtil.IsAbsolutePath(GameCoreConfig.Root)
                    ? Path.Combine(GameCoreConfig.Root, "options.txt")
                    : Path.Combine(FileUtil.GetCurrentExecutingDirectory(), GameCoreConfig.Root, "options.txt");
            FileUtil.ModifyLangValue(optionsFilePath, GameCoreConfig.Version, GameCoreConfig.Root,
                GameCoreConfig.Language);

            progressReport.Description = "构建启动参数";
            progressReport.Percentage = 70;

            onProgressChanged?.Invoke(progressReport);
            try
            {
                var arguments = await new ArgumentsBuildUtil(GameWindowConfig, GameCoreConfig, JavaConfig, BaseAccount)
                    .Build();

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = JavaConfig.JavaPath,
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = GameCoreConfig.Root
                    },
                    EnableRaisingEvents = true
                };

                stopwatch.Start();

                progressReport.Description = "进程启动中...";
                progressReport.Percentage = 90;
                onProgressChanged?.Invoke(progressReport);

                return new LaunchResponse(Status.Succeeded, stopwatch, process, arguments, new Exception());
            }
            catch (Exception e)
            {
                return new LaunchResponse(Status.Failed, stopwatch, process, e);
            }
        }
        catch (Exception e)
        {
            return new LaunchResponse(Status.Failed, stopwatch, process, e);
        }
    }
}