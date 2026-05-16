using StarLight_Core.Enum;

namespace StarLight_Core.Models.Authentication;

/// <summary>
/// 离线账户类
/// </summary>
public class OfflineAccount : BaseAccount
{
    public override AuthType Type => AuthType.Offline;
}