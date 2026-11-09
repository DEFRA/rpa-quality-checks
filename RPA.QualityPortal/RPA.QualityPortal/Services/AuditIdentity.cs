using System.Security.Claims;
using System.Text.RegularExpressions;

namespace RPA.QualityPortal.Services;

public static class AuditIdentity
{
    public static string GetName(ClaimsPrincipal principal, string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            throw new InvalidOperationException("An explicit staff-number audit claim type is required.");
        var claims = principal?.FindAll(claimType).ToArray() ?? [];
        if (principal?.Identity?.IsAuthenticated != true || claims.Length != 1 || string.IsNullOrWhiteSpace(claims[0].Value))
            throw new InvalidOperationException("The authenticated user must have exactly one nonempty staff-number audit claim.");

        // Keep Quality Portal's existing normalization and People.StaffNumber lookup contract.
        var name = claims[0].Value.ToLowerInvariant()
            .Replace("earth\\", "").Replace("m0", "m").Replace("demeter\\", "");
        if (!Regex.IsMatch(name, @"\A[a-z0-9]+\z"))
            throw new InvalidOperationException("The audit claim must contain a staff number, not a UPN, email or display name.");
        return name;
    }

    public static void Validate(ClaimsPrincipal principal, Guid tenantId, string auditClaim)
    {
        var tenants = principal?.FindAll("tid").ToArray() ?? [];
        if (tenants.Length != 1 || !Guid.TryParse(tenants[0].Value, out var actualTenant) || actualTenant != tenantId)
            throw new InvalidOperationException("The identity does not belong to the configured tenant.");
        GetName(principal, auditClaim);
    }
}
