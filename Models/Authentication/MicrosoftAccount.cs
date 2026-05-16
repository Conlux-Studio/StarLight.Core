using StarLight_Core.Enum;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 微软账户类
/// </summary>
public class MicrosoftAccount : BaseAccount
{
    public override AuthType Type => AuthType.Microsoft;

    // 刷新令牌
    public string RefreshToken { get; set; }

    // 皮肤地址
    public string SkinUrl { get; set; }

    //获取时间
    public DateTime DateTime { get; set; }

    public bool IsNewAccount { get; set; } = false;
}