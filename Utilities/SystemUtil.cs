using Microsoft.Win32;

namespace StarLight_Core.Utilities;

public class SystemUtil
{
    /// <summary> 判断是否为 Windows 10 及以上版本 </summary>
    /// <returns> 是 Windows 10 及以上版本则为 True，否则为 False </returns>
    public static bool IsOperatingSystemGreaterThanWin10()
    {
        const string subkey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        using (var key = Registry.LocalMachine.OpenSubKey(subkey))
        {
            if (key != null)
            {
                var currentValue = key.GetValue("CurrentMajorVersionNumber");
                if (currentValue is int majorVersion) return majorVersion >= 10;
            }
        }

        return false;
    }

    /// <summary> 判断操作系统是否为 64 位 </summary>
    /// <returns> 如果操作系统是 64 位则返回 True，否则返回 False </returns>
    public static bool IsOperatingSystem64Bit()
    {
        return Environment.Is64BitOperatingSystem;
    }

    /// <summary>
    /// 获取操作系统位数
    /// </summary>
    /// <returns>操作系统位数</returns>
    public static int GetOperatingSystemBit()
    {
        return Environment.Is64BitOperatingSystem ? 64 : 32;
    }
}