using StarLight_Core.Enum;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// Yggdrasil 账户类
/// </summary>
public class YggdrasilAccount : BaseAccount
{
    /// <summary>
    /// 账户类型
    /// </summary>
    public override AuthType Type => AuthType.Yggdrasil;

    /// <summary>
    /// 服务器地址
    /// </summary>
    public string ServerUrl { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}