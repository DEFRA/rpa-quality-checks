# Quality Portal authentication

The .NET 10 portal registers authentication, claims, permissions and application
services centrally in `RPA.QualityPortal\RPA.QualityPortal\Services\ApplicationServices.cs`,
called from `Program.cs`. Microsoft Entra OpenID Connect authorization-code sign-in
and secure cookies replace IIS/Negotiate and the IDT Security database.

## Required deployment configuration

Supply these externally for each environment (environment variables use `__` in
place of `:`). Empty checked-in values deliberately prevent startup. Never commit
client credentials.

### Key Vault-backed secrets

Setting `KeyVault:Uri` (the vault's `https://<vault-name>.vault.azure.net/` URI)
makes `Program.cs` resolve configuration from Azure Key Vault via
`DefaultAzureCredential` - in Azure this is the app's managed identity; locally
it falls back to `az login`/Visual Studio/VS Code credentials. Left unset (the
default), the app behaves exactly as before and reads `appsettings*.json`/
user-secrets/environment variables only - local development and the test suite
are unaffected.

Store the secret-valued keys below in Key Vault using `--` in place of `:` in
the secret name (Key Vault secret names cannot contain `:`), e.g. a secret named
`ConnectionStrings--QualityContext` is exposed to the app as configuration key
`ConnectionStrings:QualityContext`:

| Key Vault secret name | Configuration key |
|---|---|
| `AzureAd--ClientSecret` | `AzureAd:ClientSecret` |
| `ConnectionStrings--QualityContext` | `ConnectionStrings:QualityContext` |
| `ConnectionStrings--PeopleContext` | `ConnectionStrings:PeopleContext` |
| `Mail--Username` | `Mail:Username` |
| `Mail--Password` | `Mail:Password` |

The app's managed identity needs the **Key Vault Secrets User** RBAC role on
the vault (RBAC authorization model, not legacy access policies).

### Data Protection key ring (Blob Storage + Key Vault)

ASP.NET Core's Data Protection key ring encrypts the auth cookie and the
antiforgery token. Without a shared key ring, each Azure Container Apps replica
generates its own ephemeral keys, so a cookie issued by one replica fails to
decrypt on another - breaking sign-in (and losing the in-progress check-wizard
session held in `IMemoryCache`, see `SessionStore.cs`) whenever a request lands
on a different replica or a replica restarts.

| Key | Requirement |
|---|---|
| `DataProtection:BlobUri` | Full blob URL for the shared key ring, e.g. `https://<account>.blob.core.windows.net/dataprotection/keys.xml`. When set, keys are persisted to this blob instead of the local/ephemeral default, so all replicas share one key ring. Requires **Storage Blob Data Contributor** on the container for the app's managed identity |
| `DataProtection:KeyVaultKeyUri` | Optional. Full Key Vault key URI, e.g. `https://<vault-name>.vault.azure.net/keys/dataprotection-key/<version>`. When set (requires `DataProtection:BlobUri` also set), the persisted keys are wrapped with this Key Vault key instead of being stored at rest as plain XML. Requires **Key Vault Crypto User** on the vault for the app's managed identity |

Both are optional and independently gated - leaving them unset keeps the
previous (single-replica-only) default key ring behavior, which remains correct
for local development and a confirmed single-replica deployment.

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
| `ConnectionStrings:QualityContext` | Azure SQL connection string, SQL authentication (`User ID`/`Password` from the deployment secret store) with `Encrypt=True`; schema/audit contracts unchanged. **Windows Integrated Security is not supported** on Azure Container Apps' Linux containers and must not be used |
| `ConnectionStrings:PeopleContext` | Azure SQL connection string for the shared staff-number to display-name lookup database; same SQL-authentication + `Encrypt=True` requirement as `QualityContext` |
| `Mail:Host` | Authenticated SMTP relay/API host (e.g., SendGrid, Exchange Online). There is **no pickup-directory fallback** - UNC-based mail delivery is not supported on Azure Container Apps |
| `Mail:Port` | SMTP submission port; defaults to `587` |
| `Mail:EnableSsl` | Defaults to `true`; do not disable in production |
| `Mail:Username` / `Mail:Password` | Authenticated SMTP relay credential from the deployment secret store |

> **Data layer - EF Core 10:** `QualityContext`/`PeopleContext` now run on Entity Framework Core 10 with
> `Microsoft.Data.SqlClient`, so the EF6 provider limitation no longer applies and Microsoft Entra ID /
> managed-identity SQL authentication is possible at the connection-string level (the Entra-only infra
> change is still to be done). The schema is managed by EF Core migrations (`Migrations/`, applied on startup
> when the migrate flag is set). **Existing databases** must first run `Database/002_EfCoreBaseline.sql`, which
> records `InitialCreate` in `__EFMigrationsHistory` so EF Core does not try to recreate the schema. The
> `IDTSecurityConnection` usages in `AccessService.cs`/`IdtRoleClaimsTransformation.cs` are plain ADO.NET.

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
and shared persistent ASP.NET Core Data Protection keys for multi-instance hosting
(see `DataProtection:BlobUri`/`DataProtection:KeyVaultKeyUri` above - set both before
running more than one replica).
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

## Container

The multi-stage [Dockerfile](./Dockerfile) follows the Defra .NET container
standard ([defra-docker-dotnetcore](https://github.com/DEFRA/defra-docker-dotnetcore)):
a `development` stage (`defradigital/dotnetcore-development`) restores, tests
and publishes the app; a `production` stage (`defradigital/dotnetcore`) runs
it as the non-root `dotnet` user with write permissions removed.
`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` keeps ICU enabled for `en-GB`,
and it listens on HTTP 8080.

```powershell
docker build --target production -t quality-checks:local .
# Optional non-secret package source, replacing configured restore sources:
docker build --target production --build-arg NUGET_SOURCE="https://your-feed/v3/index.json" -t quality-checks:local .
```
