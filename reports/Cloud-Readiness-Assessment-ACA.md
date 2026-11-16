# Cloud Readiness Assessment — RPA.QualityPortal (rpa-quality-checks)

**Assessment type:** Cloud Readiness only
**Analysis depth:** Full analysis (source code + binary/package dependencies)
**Migration target:** Azure Container Apps (ACA)
**Assessment date:** 2026-11-13
**Tool:** Microsoft Azure Migrate App & Code Assessment for .NET (AppCAT) CLI `dotnet-appcat` v1.0.878
**Raw report:** [`report.json`](</.github/modernize/assessment/reports/report-20261113123926/report.json>)

> This is a point-in-time, code-only cloud readiness scan. It does not replace a build, test run, or live Azure deployment validation. It supplements — and is narrower in scope than — the full per-app assessment already on file at [reports/rpa-quality-checks.md](./rpa-quality-checks.md) and the earlier portfolio-level [reports/ACA-Migration-Assessment.md](../../reports/ACA-Migration-Assessment.md).

## 1. Why this assessment, and what's different from earlier reports

The portfolio-level [ACA-Migration-Assessment.md](../../reports/ACA-Migration-Assessment.md) (2026-09-30) described this app as it existed **before** modernization: ASP.NET MVC 5 on .NET Framework 4.7.2, IDT SQL Membership/RoleManager auth, in-proc session. That is no longer the current state.

Per [Capability-Matrix.yaml](./Capability-Matrix.yaml) and [Decisions-Required.md](./Decisions-Required.md), Phase 1 planning is largely complete (14/18 decisions confirmed, including **D-06 Hosting platform = Azure Container Apps** and **D-09 Authentication = Microsoft Entra ID**), and Phase 2 code migration work has already landed in the repository: the project now targets **`net10.0`**, uses **ASP.NET Core minimal hosting** (`Program.cs`), and authenticates via **Microsoft.Identity.Web 4.16.1** / Entra ID instead of Windows/IDT auth (see [reports/rpa-quality-checks.md](./rpa-quality-checks.md)).

This assessment answers a narrower question than a full re-assessment: **given the code as it stands today, what specifically still blocks or complicates running it on Azure Container Apps?** It is scoped to Cloud Readiness findings only (no code-quality, security-CVE, or dependency-currency findings included) and run at full depth (both source code and compiled package/binary dependencies scanned).

## 2. Methodology

```powershell
dotnet tool update dotnet-appcat
appcat analyze "RPA.QualityPortal\RPA.QualityPortal.sln" `
  --source Solution `
  --target ACA `
  --code --binaries `
  --privacyMode Restricted `
  --non-interactive `
  --serializer JSON `
  --report ".github\modernize\appcat\result\report"
```

- **Source:** Visual Studio solution `RPA.QualityPortal.sln` (2 projects: `RPA.QualityPortal.csproj`, `RPA.QualityPortal.Tests.csproj`)
- **Target:** `ACA` → resolved by the tool as **"Azure Container Apps"** (`settings.targetId = ACA`, `settings.targetDisplayName = Azure Container Apps`)
- **Components analyzed:** `code: true`, `binaries: true` — full analysis, not a quick/code-only pass
- **Privacy mode:** Restricted (no source snippets or identifiers exported into the report)

## 3. Executive summary

| Metric | Value |
|---|---|
| Projects analyzed | 2 (`RPA.QualityPortal`, `RPA.QualityPortal.Tests`) |
| Distinct cloud-readiness rules triggered | **11** |
| Total incidents (rule hits across files) | **70** |
| Total estimated remediation effort | **192 story points** |
| Mandatory-severity incidents | 3 |
| Optional-severity incidents | 17 |
| Potential-severity incidents | 50 |
| Information-severity incidents | 0 |

**Severity distribution**

| Severity | Incidents | Share |
|---|---|---|
| Mandatory | 3 | 4% |
| Optional | 17 | 24% |
| Potential | 50 | 71% |
| Information | 0 | 0% |

**Category distribution**

| Category | Incidents | Dominant rule |
|---|---|---|
| SMTP | 33 | `SMTP.0001` — SMTP connections detected |
| Database | 14 | `Database.0003` — `System.Data.SqlClient` dependency |
| Local (filesystem/network paths) | 10 | `Local.0002` (1) + `Local.0006` (9) |
| Connection | 5 | `Connection.0001` — connection string detected |
| Security | 3 | `Security.0003` (2) + `Security.0103` (1) |
| Scale | 1 | `Scale.0001` — static content detected |
| Cache | 1 | `Cache.0001` — data caching detected |
| HTTP | 1 | `HTTP.0001` — external HTTP access detected |
| Identity | 2 | `Identity.0002` — Windows authentication detected |

**Bottom line — ACA readiness verdict: Conditional Go.** No finding blocks containerizing and deploying the app as-is, but **3 Mandatory items and 2 high-incident-count Potential themes (SMTP, SQL Integrated Security) must be resolved before the app can safely run with more than one Container Apps replica or reach its on-prem SQL Server from a Linux container.** See Section 6 for the go/no-go breakdown.

## 4. Findings by rule (full detail)

### 🔴 Mandatory (must fix before ACA deployment)

#### `Local.0002` — Hardcoded local or network paths detected
- **Severity:** Mandatory · **Effort:** 1 story point · **Incidents:** 1
- **Affected file:** [`RPA.QualityPortal/appsettings.json`](</RPA.QualityPortal/RPA.QualityPortal/appsettings.json>)
- **Evidence:** `Mail:PickupDirectory` is set to a UNC share — `\\D3VMPRWWAS001\G$\Default Site\mailroot\Pickup`
- **Why it matters for ACA:** Azure Container Apps containers have an ephemeral, non-shared filesystem and no line-of-sight to this on-prem UNC share. Mail delivery that falls back to the pickup directory (when `Mail:Host` is blank) will silently fail in ACA.
- **Recommendation:** Require `Mail:Host` to always be set in Azure environments (fail fast if blank, don't fall back to pickup-directory mode there), and point it at an authenticated SMTP relay or a managed email API (Azure Communication Services Email, or SendGrid) — see `SMTP.0001` below, same remediation.

#### `Identity.0002` — Windows authentication detected
- **Severity:** Mandatory · **Effort:** 3 story points · **Incidents:** 2
- **Affected file:** [`RPA.QualityPortal.Tests/Authentication/AuthenticationTests.cs`](</RPA.QualityPortal/RPA.QualityPortal.Tests/Authentication/AuthenticationTests.cs>)
- **⚠ Assessed as a likely false positive.** AppCAT's pattern matcher flags the *string* `"Negotiate"` wherever it appears. The actual code at this location is a test **asserting that the Negotiate (Windows) authentication scheme is `null`/absent** — i.e., the test is confirming Windows auth has been correctly *removed*, not that the app still uses it. This is consistent with [Decision D-09](./Decisions-Required.md) (Authentication → Microsoft Entra ID, decided) and the Entra OIDC wiring already present in `ApplicationServices.cs`.
- **Recommendation:** No code change required. Keep this test — it's a regression guard against Windows auth being reintroduced. Flagging here only so the Mandatory-severity count in the raw AppCAT report doesn't get misread as a live blocker.

### 🟠 Optional

#### `Database.0003` — `System.Data.SqlClient` dependency detected
- **Severity:** Optional · **Effort:** 3 story points · **Incidents:** 14
- **Affected files:** [`Infrastructure/IdtRoleClaimsTransformation.cs`](</RPA.QualityPortal/RPA.QualityPortal/Infrastructure/IdtRoleClaimsTransformation.cs>), [`Services/AccessService.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/AccessService.cs>)
- **Why it matters:** `System.Data.SqlClient` is in maintenance mode; `Microsoft.Data.SqlClient` is the actively developed client and is required to use **Azure AD/Entra ID (managed identity) authentication** to Azure SQL — see `Connection.0001` below, which is the bigger blocker these two rules share.
- **Recommendation:** Replace `using System.Data.SqlClient;` with `Microsoft.Data.SqlClient` in both files as part of the SQL-connectivity remediation (Section 5).

#### `Security.0003` — Hardcoded sensitive data detected
- **Severity:** Optional · **Effort:** 3 story points · **Incidents:** 2
- **Affected files:** [`Services/ApplicationServices.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/ApplicationServices.cs>), [`RPA.QualityPortal.Tests/Authentication/AuthenticationTests.cs`](</RPA.QualityPortal/RPA.QualityPortal.Tests/Authentication/AuthenticationTests.cs>)
- **⚠ Assessed as a likely false positive.** The hit in `ApplicationServices.cs` is the string literal `"AzureAd:ClientSecret"` used as a **configuration key** passed to `Required(configuration, ...)` — the secret *value* is read from configuration (and, per [Decisions-Required.md](./Decisions-Required.md) D-09/operational conventions, should be Key Vault-backed in Azure), not hardcoded. AppCAT's pattern matcher trips on the word `ClientSecret` regardless of whether it's a key name or a literal value.
- **Recommendation:** No code change required here. Confirm (during Phase 3/4) that `AzureAd:ClientSecret` is supplied via an ACA secret backed by Key Vault + managed identity in every environment, never as a plaintext app setting.

#### `Security.0103` — Certificate management dependency detected
- **Severity:** Potential · **Effort:** 5 story points · **Incidents:** 1
- **Affected project:** `RPA.QualityPortal.Tests.csproj` (resolves transitively from the main app's `System.Security.Cryptography.Pkcs` v10.0.13 package reference via the project reference)
- **Why it matters:** Indicates the app may do custom certificate handling. If any part of the Entra ID token-validation or future client-certificate flow relies on the local Windows certificate store, that store doesn't exist the same way inside an ACA Linux container.
- **Recommendation:** Confirm this package is used only for standard PKCS cryptographic operations (not local certificate-store access); if certificates are needed at runtime, use **Azure Key Vault Certificates** rather than a local store.

#### `Scale.0001` — Static content detected
- **Severity:** Optional · **Effort:** 3 story points · **Incidents:** 1
- **Affected project:** `RPA.QualityPortal.csproj` (serves `wwwroot` via `app.UseStaticFiles()` in `Program.cs`)
- **Why it matters:** Serving static assets directly from the container works fine on ACA but doesn't scale as well as offloading to a CDN, and redeploys are required for any static-asset change.
- **Recommendation:** Low priority for initial ACA migration. Consider Azure Blob Storage + Azure CDN/Front Door for static assets in a later optimization pass — not a blocker.

### 🟡 Potential

#### `SMTP.0001` — SMTP connections detected (dominant finding — 33 of 70 incidents)
- **Severity:** Potential · **Effort:** 3 story points each · **Incidents:** 33 (26 in the main app, 7 in tests)
- **Affected files (main app):** `Controllers/AdminController.cs`, `Controllers/BankAccount/BankAccountController.cs`, `Controllers/BankAccount/BankAccountReCheckController.cs`, `Controllers/OutboundCorrespondence/OutboundCorrespondenceController.cs`, `Controllers/OutboundCorrespondence/OutboundCorrespondenceReCheckController.cs`, [`Services/EmailService.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/EmailService.cs>), `Services/IMessageService.cs`, `Services/ISendService.cs`
- **Why it matters:** `System.Net.Mail.SmtpClient` connections to `Mail:Host`/`Mail:Port` (configured, not hardcoded) still assume outbound SMTP reachability and, when `Mail:Host` is blank, fall back to the UNC pickup directory flagged under `Local.0002`.
- **Recommendation:** Same remediation as `Local.0002` — route through an authenticated SMTP relay (SendGrid, Exchange Online) or Azure Communication Services Email; store the relay credential in Key Vault.

#### `Connection.0001` — Connection string detected
- **Severity:** Potential · **Effort:** 3 story points · **Incidents:** 5
- **Affected files:** `RPA.QualityPortal/appsettings.json`, `appsettings.Production.json`, `appsettings.SIT.json`, `appsettings.UAT.json`
- **Why it matters:** Both `ConnectionStrings:QualityContext` and `ConnectionStrings:PeopleContext` use **`Integrated Security=true`** (Windows Authentication to SQL Server). Windows Integrated Security does not work from a Linux-based ACA container — there is no domain-joined Windows identity to present.
- **Recommendation:** This is the most significant *practical* blocker found (more so than its "Potential" severity label suggests) given the target is confirmed as ACA. Replace Integrated Security with **Azure SQL + Microsoft Entra ID (managed identity) authentication** via `Microsoft.Data.SqlClient`'s `Authentication=Active Directory Managed Identity` connection-string mode, paired with the `Database.0003` SqlClient-package swap above. `PeopleContext`'s connection string also has `encrypt=False` — this must flip to encrypted (Azure SQL enforces TLS by default) as part of the same change. This item is tracked structurally by [Decision D-04](./Decisions-Required.md) (Database engine → Azure SQL Database, decided) but the connection-string/auth-mode rewrite itself is not yet implemented in code.

#### `Cache.0001` — Data caching detected
- **Severity:** Potential · **Effort:** 3 story points · **Incidents:** 1
- **Affected file:** [`Services/ApplicationServices.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/ApplicationServices.cs>) (`services.AddMemoryCache()`)
- **Why it matters:** `IMemoryCache` is process-local. Per [reports/rpa-quality-checks.md](./rpa-quality-checks.md), multi-step workflow state ("checks, questions, answers in progress") is held in this cache keyed by a session cookie with a 480-minute sliding expiry. On ACA, multiple replicas (and any replica restart or revision rollout) do not share this cache — a user's in-progress multi-step check would be lost or routed to a replica that doesn't have their state.
- **Recommendation:** Before enabling more than 1 ACA replica (or allowing revision-based deploys mid-session), replace `AddMemoryCache()` with a distributed cache — **Azure Cache for Redis** via `AddStackExchangeRedisCache()` — or explicitly pin `minReplicas=maxReplicas=1` until this is addressed (not recommended long-term, since it removes ACA's scale-out/rolling-update benefits).

#### `HTTP.0001` — Access to external resources via HTTP detected
- **Severity:** Potential · **Effort:** 3 story points · **Incidents:** 1
- **Affected file:** `RPA.QualityPortal.Tests/Authentication/AuthenticationHttpTests.cs`
- **Assessment:** Test-only (in-memory `WebApplicationFactory`/`HttpClient` test harness), not a production code path. No action needed for ACA migration; verify at Phase 2 code-review time that no production controller issues outbound HTTP (vs. HTTPS) calls.

#### `Local.0006` — Hardcoded URLs detected
- **Severity:** Potential · **Effort:** 1 story point · **Incidents:** 9
- **Affected files:** `RPA.QualityPortal.Tests/Authentication/AuthenticationHttpTests.cs`, `RPA.QualityPortal.Tests/Authentication/AuthenticationTests.cs`
- **Assessment:** Test-only (test-harness base URLs / fixture endpoints), not production configuration. No action needed for ACA migration.

## 5. Cross-cutting themes for Azure Container Apps

Grouping the 11 rules above by the underlying ACA concern they represent (rather than by AppCAT category) gives a clearer remediation picture:

| Theme | Rules involved | Incidents | ACA impact |
|---|---|---|---|
| **SQL connectivity (Integrated Security + legacy client)** | `Connection.0001`, `Database.0003` | 19 | **Hard blocker for any Azure SQL target.** Windows Integrated Security cannot authenticate from a Linux container; must move to Entra ID/managed-identity SQL auth with `Microsoft.Data.SqlClient`. |
| **Outbound mail (SMTP + UNC pickup fallback)** | `SMTP.0001`, `Local.0002` | 34 | **Mandatory + highest incident volume.** UNC pickup directory is unreachable from ACA; must standardize on a relay/managed email service. |
| **Stateless/scale-out readiness (in-memory cache)** | `Cache.0001` | 1 | **Blocks safe multi-replica scaling**, not single-replica deployment. Needs Azure Cache for Redis before `minReplicas` > 1. |
| **Certificate/secret handling** | `Security.0103`, `Security.0003` (false positive) | 2 | Low risk; confirm Key Vault usage for any real secrets/certs at deployment time. |
| **Static content delivery** | `Scale.0001` | 1 | Optimization only, not a blocker. |
| **Test-only noise** | `Identity.0002` (false positive), `HTTP.0001`, `Local.0006` | 12 | No production impact; safe to exclude from the remediation backlog. |

Of the 70 raw incidents, **~17% (12 incidents: `HTTP.0001` + `Local.0006` + 2 of the 2 `Identity.0002` hits)** are test-only code paths with no production runtime impact, and **1 further incident** (`Security.0003`'s `ApplicationServices.cs` hit) is a configuration-key name match, not an actual hardcoded secret. After discounting these, the **effective production-code remediation surface is 9 rules / ~56 incidents / ~177 story points**, concentrated almost entirely in SQL connectivity and SMTP/mail delivery.

## 6. ACA readiness verdict

| Gate | Status | Blocking? |
|---|---|---|
| Containerizable runtime (no `System.Web`, Linux-compatible TFM) | ✅ Pass — already `net10.0`, ASP.NET Core minimal hosting | No |
| Authentication compatible with ACA (no Windows/Negotiate dependency) | ✅ Pass — Entra ID/Microsoft.Identity.Web already wired; `Identity.0002` hit is a test assertion confirming this, not a regression | No |
| SQL connectivity works from a Linux container | ❌ Fail today — Integrated Security connection strings | **Yes — must fix before first ACA deployment to Azure SQL** |
| Outbound mail works without a UNC file share | ❌ Fail today — UNC pickup-directory fallback | **Yes — must fix before first ACA deployment** |
| Safe to run with >1 replica / rolling revisions | ❌ Fail today — process-local `IMemoryCache` for workflow state | Only if `minReplicas` > 1 is required at launch; acceptable to defer if a single-replica launch is approved |
| No Dockerfile / container build exists yet | ⚠ Not yet created | Required for Phase 3 (Infrastructure Generation), not scored by this Cloud Readiness scan |

**Overall:** No code-architecture rewrite is required to reach ACA — the heavy lifting (framework upgrade, MVC→ASP.NET Core port, Windows/IDT auth→Entra ID) is already done. The remaining cloud-readiness gap is narrow and concrete: **swap SQL Integrated Security for Entra ID/managed-identity auth, replace the UNC-based mail pickup path with a managed SMTP relay/email API, and move multi-step workflow state to a distributed cache before scaling beyond one replica.**

## 7. Remediation status (updated 2026-11-13)

Per user direction, the following findings were implemented; `SMTP.0001` was folded into the `Local.0002` fix since both share the same root cause and remediation.

| Rule | Decision | Status | What changed |
|---|---|---|---|
| `Local.0002` | Proceed with recommended fix | ✅ **Fixed** | [`SendService.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/SendService.cs>) now requires `Mail:Host` and throws a clear configuration error instead of silently falling back to the UNC pickup directory; `PickupDirectory` removed from all `appsettings*.json` |
| `SMTP.0001` (bundled with `Local.0002`) | — | ✅ **Hardened, rule still fires (expected)** | `SendService.cs` now supports authenticated SMTP (`Mail:Username`/`Mail:Password`/`Mail:EnableSsl`, default port `587`). The rule itself can't be "resolved" — it flags any `SmtpClient` usage — but the underlying ACA-unreachable pickup-directory risk is gone |
| `Identity.0002` | Proceed with recommended fix | ✅ **Confirmed false positive, no code change** | Re-verified: the flagged line asserts the Negotiate scheme is absent. Kept as a regression guard |
| `Database.0003` | Proceed with recommended fix | ✅ **Fixed** | [`AccessService.cs`](</RPA.QualityPortal/RPA.QualityPortal/Services/AccessService.cs>) and [`IdtRoleClaimsTransformation.cs`](</RPA.QualityPortal/RPA.QualityPortal/Infrastructure/IdtRoleClaimsTransformation.cs>) now use `Microsoft.Data.SqlClient` (package added to the csproj); rule no longer fires |
| `Security.0003` | Proceed with recommended fix | ✅ **Confirmed false positive, no code change** | `"AzureAd:ClientSecret"` is a config-key name, not a literal secret; deferred to Phase 3 Key Vault wiring per README |
| `Security.0103` | Proceed with recommended fix | ✅ **Investigated — keep as-is, do NOT remove** | **Correction to the original report's suggestion:** `System.Security.Cryptography.Pkcs` is a deliberate top-level pin, not an unused dependency. `dotnet list package --include-transitive` shows `System.Data.SqlClient 4.8.6` (pulled in by EF6) transitively depends on an older, vulnerable `Pkcs` version; removing the pin would have **reintroduced a CVE** (`dotnet list package --vulnerable` confirms zero vulnerabilities with the pin in place). Left unchanged and documented |
| `Scale.0001` | Decide later (Blob Storage/CDN) | ⏸ **Deferred** | No change — revisit when ready to decide on Blob Storage + Azure CDN/Front Door |
| `Connection.0001` | Proceed with recommended fix | ✅ **Fixed, with one discovered constraint** | Removed `Integrated Security=true`/`encrypt=False`/hardcoded on-prem hostnames from `appsettings.json` and all 3 environment overlays; `QualityContext`/`PeopleContext` are now blank-and-fail-fast (same pattern as `AzureAd:ClientSecret`), to be supplied externally as **Azure SQL + SQL authentication + `Encrypt=True`**. **Constraint found:** true passwordless Managed Identity auth is *not* available for these two EF6 contexts — Microsoft never shipped a stable EF6 provider for `Microsoft.Data.SqlClient` (`EntityFramework.MicrosoftSqlServer` reached only an unlisted RC on NuGet); `QualityDbConfiguration.cs` remains on the classic `System.Data.SqlClient` provider. Documented in [README.md](</README.md>) as a follow-up for a future EF Core migration |
| `Cache.0001` | Decide later | ⏸ **Ignored for now** | No change |
| `HTTP.0001` | Decide later | ⏸ **Ignored for now** | No change (test-only code anyway) |
| `Local.0006` | Decide later | ⏸ **Ignored for now** | No change (test-only code anyway) |

### Verification

Build (`dotnet build`) succeeded for both projects, and the full test suite passed (**415/415**, `dotnet test`). Re-running the same AppCAT scan (`--target ACA --code --binaries`) after the fixes:

| Metric | Before | After |
|---|---|---|
| Incidents | 70 | **55** |
| Story points | 192 | **145** |
| Mandatory | 3 | **2** (remaining: `Identity.0002`, confirmed false positive) |
| Optional | 17 | **6** |
| Potential | 50 | 47 |

Two new, expected findings appeared as a side effect of the fixes (not regressions):
- **`Perf.0001`** (Optional, 2 incidents, `AccessService.cs`) — pre-existing synchronous ADO.NET calls, now visible to AppCAT's analyzer because they run through `Microsoft.Data.SqlClient`. Low priority; this code path serves the already-retired (410 Gone) IDT Access-change endpoint.
- **`Security.0004`** (Potential, 2 incidents, `SendService.cs`) — flags the new SMTP username/password credential support added for `SMTP.0001`/`Local.0002`. By design: the password is read from `IConfiguration` (Key Vault-backed in Azure), not hardcoded, matching this app's existing secret-handling convention.

Raw verification report: [`.github/modernize/assessment/reports/report-20261113144203/report.json`](</.github/modernize/assessment/reports/report-20261113144203/report.json>).

## 8. Suggested next steps

1. Resolve the remaining PENDING decisions that gate further work: **D-05** (DB migration tool — default guess Azure DMS, awaiting confirmation), **D-12** (cost ceiling), **D-13** (DR RPO/RTO), **D-15** (acceptable downtime).
2. When ready, decide on `Scale.0001` (Blob Storage + Azure CDN/Front Door for static content) and the deferred `Cache.0001`/`HTTP.0001`/`Local.0006` items.
3. Local developers must now supply their own `ConnectionStrings:QualityContext`/`PeopleContext` (e.g., via `dotnet user-secrets` or an untracked `appsettings.Development.json`) since the checked-in values are intentionally blank — this is a deliberate behavior change, not a gap.
4. Proceed to `/Phase3-GenerateInfra` with the SQL-authentication (not managed-identity) and SMTP-relay requirements as explicit infra inputs (Key Vault secrets + ACA secret references for `ConnectionStrings:*` and `Mail:Username`/`Mail:Password`).
5. Track the EF6→EF Core migration (needed to unlock passwordless Managed Identity SQL auth) as a separate, future-dated improvement — out of scope for this pass.

---
*Raw AppCAT output: [`.github/modernize/assessment/reports/report-20261113123926/report.json`](</.github/modernize/assessment/reports/report-20261113123926/report.json>) (pre-fix) and [`.github/modernize/assessment/reports/report-20261113144203/report.json`](</.github/modernize/assessment/reports/report-20261113144203/report.json>) (post-fix) · Generated via the `assessment` skill's .NET AppCAT workflow, scoped to `--target ACA` per this session's request for a Cloud-Readiness-only, full-analysis, Azure Container Apps assessment.*
