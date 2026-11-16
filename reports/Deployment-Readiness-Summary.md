# Deployment Readiness Summary — rpa-quality-checks (RPA.QualityPortal)

**Date:** 2026-11-16
**Scope:** Cloud-readiness review, in-memory cache/session audit, Key Vault & Data Protection wiring, containerization, and Infrastructure-as-Code generation for RPA.QualityPortal (Azure Container Apps target).

---

## 1. Cloud readiness status

Source: [Cloud-Readiness-Assessment-ACA.md](./Cloud-Readiness-Assessment-ACA.md) (AppCAT, last full scan 2026-11-13).

| Gate | Status |
|---|---|
| Containerizable runtime (`net10.0`, ASP.NET Core minimal hosting) | ✅ Pass |
| Auth compatible with ACA (Entra ID, no Windows/Negotiate) | ✅ Pass |
| SQL connectivity from a Linux container | ✅ Fixed (SQL auth + `Encrypt=True`, `Microsoft.Data.SqlClient`) |
| Outbound mail without a UNC share | ✅ Fixed (`Mail:Host` required, authenticated SMTP) |
| Safe to run with >1 replica / rolling revisions | ⚠️ **Still open** — see Section 2 |
| Dockerfile / container build | ✅ **Done this session** — see Section 3 |
| Infrastructure as Code | ✅ **Done this session** — see Section 4 |

**Verdict:** Conditional Go for a **single-replica** launch. Multi-replica scale-out needs the session-state item below addressed first.

## 2. In-memory cache / session-state findings

`IMemoryCache` is used for two distinct purposes in this app — only one is a genuine blocker:

| Mechanism | File | Purpose | Multi-replica risk |
|---|---|---|---|
| Role-claims cache | [`IdtRoleClaimsTransformation.cs`](../RPA.QualityPortal/RPA.QualityPortal/Infrastructure/IdtRoleClaimsTransformation.cs) | 5-min absolute expiry, per-user role lookup | Low — transient staleness only, same pattern shared with `rpa-mts-inspections`/`rpa-inspections-workbench` |
| Multi-step check-wizard session | [`SessionStore.cs`](../RPA.QualityPortal/RPA.QualityPortal/Infrastructure/SessionStore.cs) | Cookie-keyed, 480-min sliding expiry, holds in-progress Bank Account / Outbound Correspondence check objects (`CreateGeneral`, `CreateQuestions`, `ChallengeGeneral`, etc. across 5 controllers) | **High** — a user's in-progress check is silently lost if their next request lands on a different replica or a replica restarts |

**Not yet remediated.** Recommended fix (not implemented this session, by design — deferred per earlier user direction): replace `AddMemoryCache()` backing in `SessionStore.cs` with `AddStackExchangeRedisCache()` / `IDistributedCache`, or explicitly pin `minReplicas=maxReplicas=1` until that's done. The generated `app.bicep` (Section 4) documents this constraint directly in its parameter table.

Cross-portfolio comparison also found `people-api` (hand-rolled `Dictionary` singletons — a security-relevant stale-API-key risk, not just performance), `rpa-claim-statements`/`baqu` (machine-scoped `Mutex`/no distributed lock — duplicate-processing risk), and `bank-holidays` (`RefreshGate` semaphore — per-replica only) carrying analogous risks, out of scope for this app's remediation.

## 3. Key Vault, Blob Storage, and containerization

- **Key Vault secrets (initial pass, since superseded — see Section 4):** added `Azure.Extensions.AspNetCore.Configuration.Secrets` + `Azure.Identity`, conditional `AddAzureKeyVault()` in `Program.cs`. **Later replaced** once the portfolio's actual standard (ACA-native `secretRef` from Key Vault, not an app-level pull) was identified during IaC generation — see Section 4.
- **Data Protection (Blob Storage + Key Vault key):** closed a real gap — the app's own README already *required* a shared, persistent Data Protection key ring for multi-instance hosting, but no code implemented it. Added Blob-persisted, Key-Vault-wrapped Data Protection, later upgraded to the portfolio-standard `SharedDataProtection.cs` pattern (versionless key identifier, explicit user-assigned managed identity, no credential-chain fallback).
- **Dockerfile:** [`Dockerfile`](../Dockerfile) + [`.dockerignore`](../.dockerignore), multi-stage `.NET 10` build matching the `rpa-mts-inspections`/`rpa-cph-non-subsidy` convention (`mcr.microsoft.com/dotnet/sdk:10.0.403-noble` → `mcr.microsoft.com/dotnet/aspnet:10.0.14-noble`, non-root `$APP_UID`, port `8080`, `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` for `en-GB` culture).
- **Validated by actually running the image**, not just building it:
  - Zero config → fails fast with a clear `InvalidOperationException` (by design).
  - Fake-but-well-formed config → Kestrel starts, listens on `8080`, and a request correctly reaches the OpenID Connect challenge handler and attempts live metadata discovery against `login.microsoftonline.com` (fails only because the test tenant isn't real) — confirms the full pipeline works end-to-end in-container.

## 4. Infrastructure as Code

Reference standard: `rpa-cph-non-subsidy`/`rpa-mts-inspections`'s `infra/` (confirmed byte-identical foundation template between the two — a genuine shared portfolio standard). **`rpa-claim-statements` has no `infra/` folder at all**, so it could not be the literal reference despite being named as one.

Generated under [`infra/`](../../infra): `main.bicep`, `app.bicep`, `modules/private-endpoint.bicep`, `Grant-DatabaseAccess.sql`, `Initialize-DataProtectionKey.ps1`, `Test-Templates.ps1`, `Test-KeyBootstrap.ps1`, `main.parameters.example.json`, `app.parameters.example.json`, `README.md`.

### One deliberate, documented deviation: SQL mixed-mode authentication
Every other app in the portfolio uses Entra-only SQL auth with a passwordless managed-identity connection string. `rpa-quality-checks` **cannot** — `QualityContext`/`PeopleContext` are EF6 Code-First contexts hard-wired to the classic `System.Data.SqlClient` provider, and Microsoft never shipped a stable EF6 provider for `Microsoft.Data.SqlClient` (tracked as a future EF Core migration, not a blocker). Consequences, all implemented:
- `main.bicep` sets `azureADOnlyAuthentication: false` (mixed mode) — the Entra admin is retained for DBA access only, never as the runtime identity.
- `app.bicep` supplies both connection strings as full Key-Vault-backed `secretRef` values, not managed-identity connection strings.
- `Grant-DatabaseAccess.sql` creates a **SQL-authentication contained user**, not the portfolio's usual `CREATE USER ... FROM EXTERNAL PROVIDER`.
- `PeopleContext` is modeled as an **externally owned, shared database** — this template does not provision it or manage its access grants.

### App-code gaps closed to make the IaC actually functional (not just naming parity)
Generating infra that matched the standard required the app to speak the same config contract:
- [`SharedDataProtection.cs`](../RPA.QualityPortal/RPA.QualityPortal/Services/SharedDataProtection.cs) — replaced the earlier ad-hoc Key Vault/DataProtection code with the portfolio-standard pattern (`DataProtection:Enabled/BlobUri/KeyIdentifier/ApplicationName/ManagedIdentityClientId`).
- [`ApplicationInsightsRegistration.cs`](../RPA.QualityPortal/RPA.QualityPortal/Services/ApplicationInsightsRegistration.cs) — App Insights telemetry was completely absent before.
- [`SqlReadinessHealthCheck.cs`](../RPA.QualityPortal/RPA.QualityPortal/Services/SqlReadinessHealthCheck.cs) — `/health/live` and `/health/ready` didn't exist; `app.bicep`'s liveness/readiness probes would have pointed at nothing without this. Type-activated once each for `QualityContext` and `PeopleContext`.
- [`TrustedForwarding.cs`](../RPA.QualityPortal/RPA.QualityPortal/Services/TrustedForwarding.cs) — reverse-proxy (`X-Forwarded-*`) trust configuration was also absent; `app.bicep` already assumed it via `ReverseProxy__KnownIPNetworks__0`.
- Removed the now-unused `KeyVault:Uri`/`AddAzureKeyVault()` pull-based mechanism and its NuGet package, since ACA resolves all of this app's secrets natively via `secretRef` — consistent with the rest of the portfolio.

### Verification performed
| Check | Result |
|---|---|
| `dotnet build` | 0 errors |
| `dotnet test` | **415/415 passed**, unchanged |
| Docker rebuild + run (×2) | `/health/live` → 200 always; `/health/ready` → 503 with real, per-connection-string SQL failure logs |
| `az bicep build` / `Test-Templates.ps1` | ❌ **Not run** — this environment's `az` CLI has a pre-existing, unrelated corrupted extension cache (`containerapp`) that breaks every `az` invocation; attempted a fix (deleting the cache), it re-corrupts on the next call. **The Bicep templates are therefore unverified by a compiler.** |

## 5. Outstanding items before production deployment

1. **Run `Test-Templates.ps1` with a working Bicep CLI** before any `what-if`/deployment — not yet done in this session (see above).
2. **Session-state distributed cache** (Section 2) — required before `minReplicas`/`maxReplicas` > 1.
3. **Pending decisions** per `reports/Decisions-Required.md`: D-05 (DB migration tool), D-12 (cost ceiling), D-13 (RPO/RTO), D-15 (acceptable downtime) — referenced in `reports/Report-Status.md`, not re-litigated here. *(Note: `Decisions-Required.md`/`Capability-Matrix.yaml`/`Discovery-Dossier.md` were previously flagged in `reports/Report-Status.md` as logged-but-missing-on-disk; this summary does not regenerate them.)*
4. **Provision a real SMTP relay** (SendGrid, Exchange Online, or Azure Communication Services Email) — the on-prem IIS SMTP pickup-directory mechanism this app's production environment previously relied on does not exist on Azure Container Apps.
5. **DBA must run `Grant-DatabaseAccess.sql`** against `QualityContext`, and the `PeopleContext` database owner must separately supply its own connection-string secret — neither happens automatically.
6. Full details, required parameters, and step-by-step deployment checkpoints are in [`infra/README.md`](../../infra/README.md).

---
*This summary consolidates work performed in one chat session covering: cloud-readiness recheck, cross-portfolio cache/session audit, Key Vault and Data Protection wiring, Dockerfile creation and container validation, and Infrastructure-as-Code generation. See `reports/Report-Status.md` Action Log for the detailed, timestamped entry-by-entry record.*