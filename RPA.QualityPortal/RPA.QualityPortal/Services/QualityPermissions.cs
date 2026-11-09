using System.Security.Claims;

namespace RPA.QualityPortal.Services;

public sealed class QualityPermissions
{
    public const string BasicAccess = "Quality Checks: Basic Access";
    public const string BankMember = "Quality Checks: BA Team Member";
    public const string BankAdmin = "Quality Checks: BA Admin";
    public const string BankLeader = "Quality Checks: BA Team Leader";
    public const string OutboundMember = "Quality Checks: OC Team Member";
    public const string OutboundManager = "Quality Checks: OC Team Manager";
    public const string OutboundAdmin = "Quality Checks: OC Admin";
    public const string OutboundAccess = "QualityChecks.OutboundAccess";
    public const string AdminAccess = "QualityChecks.AdminAccess";

    public static IReadOnlyDictionary<string, string> ConfigurationKeys { get; } =
        new Dictionary<string, string>
        {
            [BasicAccess] = "BasicAccessRole",
            [BankMember] = "BankTeamMemberRole",
            [BankAdmin] = "BankAdminRole",
            [BankLeader] = "BankTeamLeaderRole",
            [OutboundMember] = "OutboundTeamMemberRole",
            [OutboundManager] = "OutboundTeamManagerRole",
            [OutboundAdmin] = "OutboundAdminRole"
        };

    private readonly IReadOnlyDictionary<string, string> roles;

    public QualityPermissions(IReadOnlyDictionary<string, string> roles) =>
        this.roles = new Dictionary<string, string>(roles, StringComparer.Ordinal);

    public bool HasPermission(ClaimsPrincipal user, string permission) =>
        user?.Identity?.IsAuthenticated == true &&
        roles.TryGetValue(permission, out var role) &&
        user.Claims.Any(claim => claim.Type == "roles" && string.Equals(claim.Value, role, StringComparison.Ordinal));
}
