# Quality Portal authentication

The .NET 10 portal registers authentication, claims, permissions and application
services centrally in `RPA.QualityPortal\RPA.QualityPortal\Services\ApplicationServices.cs`,
called from `Program.cs`. Microsoft Entra OpenID Connect authorization-code sign-in
and secure cookies replace IIS/Negotiate and the IDT Security database.

## Required deployment configuration

Supply these externally for each environment (environment variables use `__` in
place of `:`). Empty checked-in values deliberately prevent startup. Never commit
client credentials.

| Key | Requirement |
|---|---|
| `AzureAd:Instance` | Approved HTTPS Entra authority; defaults to `https://login.microsoftonline.com/` |
| `AzureAd:TenantId` | Single tenant GUID, not `common` or `organizations` |
| `AzureAd:ClientId` | Application registration GUID |
| `AzureAd:ClientSecret` | Confidential-client credential from the deployment secret store |
| `AzureAd:CallbackPath` | Defaults to `/signin-oidc`; register the exact public HTTPS redirect URI |
| `Identity:AuditNameClaimType` | Explicit, single-valued staff-number claim emitted in the ID token |
| `Authorization:BasicAccessRole` | Approved app-role **value** for Basic Access |
| `Authorization:BankTeamMemberRole` | Approved app-role value for BA Team Member |
| `Authorization:BankAdminRole` | Approved app-role value for BA Admin |
| `Authorization:BankTeamLeaderRole` | Approved app-role value for BA Team Leader (existing row-detail link visibility) |
| `Authorization:OutboundTeamMemberRole` | Approved app-role value for OC Team Member |
| `Authorization:OutboundTeamManagerRole` | Approved app-role value for OC Team Manager |
| `Authorization:OutboundAdminRole` | Approved app-role value for OC Admin |
| `ConnectionStrings:QualityContext` | Existing application database; schema/audit contracts unchanged |
| `ConnectionStrings:PeopleContext` | Existing staff-number to display-name lookup database |

All seven role values must be nonempty, distinct Entra token values (not legacy
display names with spaces). There are **no default or guessed role mappings**.
The identity administrator must approve the mappings, define the app roles, and
assign users/groups externally. There is **no automatic migration of IDT users,
roles or assignments**, no Graph role-management permission, and no runtime IDT
SQL dependency.

## Permission matrix

All permissions require an authenticated user. Each permission independently
matches its configured value in the raw `roles` claim; role comparisons are
case-sensitive. Admin and manager roles do not imply membership or Basic Access.

| Surface | Required permission |
|---|---|
| Home controller | Basic Access |
| BankAccount and BankAccountReCheck controllers | BA Team Member |
| OutboundCorrespondence, OutboundCorrespondenceReCheck and OutboundCorrespondenceMI controllers | OC Team Member **OR** OC Team Manager |
| Admin controller (including Access) | OC Admin **OR** BA Admin |
| Other controllers/components without an explicit policy | Authenticated user (fallback policy) |
| Account/AccessDenied; Account/AuthenticationFailed | Anonymous; return 403 and 401 respectively |

The existing view and service expressions retain their original OR/AND behavior:
Home cards are OC Admin OR OC Team Manager OR OC Team Member, and BA Admin OR
BA Team Member. Existing OC manager/admin links and BA admin links retain those
checks; the BA row-detail link retains BA Admin OR BA Team Leader. Controller
policies still apply after navigation, so visibility is not an implicit permission
grant. Filters still apply the OC Team Member behavior independently, even when
that user has additional manager/admin roles. Existing action HTTP-method and
antiforgery restrictions remain unchanged.

The Access page is now an informational, external-administration page, not an
Entra management client. Its old POST is retired (410 after existing admin and
antiforgery checks) and never changes permissions.

## Identity and operational contract

The validated token must have exactly one `tid` matching the configured tenant
and exactly one nonempty configured audit claim. Quality Portal keeps its own
normalization: lowercase, remove `earth\` / `demeter\`, replace `m0` with `m`.
UPNs, emails, display names and other domain prefixes are rejected. The normalized
staff number still queries `People.StaffNumber` and returns `People.Name`, or
`Unknown` when not found, just as before. No audit column or user lookup is changed
to an Entra object ID or display name. Identity owners must certify the emitted
claim and People database mapping before cutover.

Cookies are `__Host-QualityPortal.Auth`, HTTP-only, HTTPS-only, SameSite=Lax,
path `/`, 30-minute sliding expiry. Token claim mapping is disabled; the configured
audit claim is `Identity.Name`, and `roles` is the role claim. Authentication
failures log only an event code and redirect to the generic anonymous failure
endpoint. Missing/invalid configuration fails startup; missing roles deny access.

Enable anonymous access in IIS and disable Windows Authentication for this app.
Provide trusted HTTPS termination/forwarded-header configuration where applicable,
and shared persistent ASP.NET Core Data Protection keys for multi-instance hosting.
Keep Quality/People SQL access and email configuration available to the hosting
identity. Role assignment changes take effect on a fresh authentication ticket;
existing cookies may remain valid until expiry. Plan operational session
revocation and cutover accordingly.

Excluded legacy accreditation/work-in-progress controllers remain excluded by the
project file; enabling them requires a separate permission review.

## Focused validation

From `RPA.QualityPortal`:

```powershell
dotnet build .\RPA.QualityPortal\RPA.QualityPortal.csproj
dotnet build .\RPA.QualityPortal.Tests\RPA.QualityPortal.Tests.csproj
dotnet test .\RPA.QualityPortal.Tests\RPA.QualityPortal.Tests.csproj --no-build --filter "FullyQualifiedName~Authentication|FullyQualifiedName~AdminControllerTest"
```

Tests use fictional role values and an in-process server with real protected
authentication cookies and offline OIDC metadata. They do not contact Entra or
production databases. Real tenant sign-in, emitted staff claim, enterprise-app
assignments, public HTTPS callbacks, People lookup and domain audit persistence
remain deployment acceptance checks.
