# Capabilities and evidence boundaries

Preflight uses two independent boundaries: the adapter-authoritative Site/App profile and capabilities derived from hydrated objects. A profile is not a replacement for capability detection. App can use Kooboo Page/Layout/View composition even when business screens use JavaScript and APIs. k-data and Commerce+k-data are currently Site-only.

## Selection

The 64 rules are organized into 14 capability files. Both reference directories carry identical executable YAML; this page describes their evidence boundaries.

The scanner reads saved object content, Page LayoutName and routable object URLs from platform functions. It reads platform multilingual activation through `get_site_info`. Source tags, CSS, imports, API operations and explicit positive Commerce context determine applicable capabilities. Model-supplied `layers` cannot disable detected checks. Names such as `orders` or `login` alone cannot establish Commerce ownership.

| Capability | Trigger | Scope |
| --- | --- | --- |
| Common | Every scanned object | Site and App |
| Frontend / forms | Renderable objects; actual native form controls | Site and App |
| Template assembly | Layout object, Page binding, layout/placeholder/view tags or Kooboo bindings | Site and App |
| k-data | k-data operations/bindings, including malformed operations outside their container | Site |
| KScript | API/CodeBlock or server API usage | Site and App |
| Data contracts | Script, API/CodeBlock or embedded scripts | Site and App; advisory review |
| Security | Code or renderable HTML | Site and App; definite private keys block, inferred risks require review |
| Multilingual | Enabled platform multilingual configuration | Site and App |
| Assets / modules | Renderable resources, Script imports, import maps | Site and App |
| Commerce | Actual Commerce API/query usage or explicit Commerce context | Site and App |
| Commerce template | Commerce together with k-data | Site |
| Payment provider | Actual payment operations; Alipay rules require Alipay usage | Site and App |

Client framework syntax is valid in its own renderer. Page script placement and import-map validity follow the platform implementation; ordinary inline import maps and supported header server scripts are valid. Resource checks verify literal local module targets and mappings present in the scan. Computed imports, missing map/Page context and unavailable inventories require further evidence.

## Structural issues and advice

`findings` contains bounded structural assertions. `advisories` contains review questions about intent, presentation, inferred credentials, sanitizer coverage, response/data contracts or implementation conventions. Advice is not a finding of noncompliance and does not change structural PASS. It also does not certify the concern as resolved.

Honor explicit user/brand choices for emoji, colors and external images. Three-column layouts are valid. Repeated Views and empty client mounts can be intentional. HTML strings, endpoint response formats and module usage must follow the selected runtime and business contract.

App `mock-api` fixtures are permitted. Compare each data requirement against its accepted strategy; source pattern matching does not prove production persistence or business correctness. Security advice similarly cannot prove field validation or authorization across called services. Actual private-key material is a redacted structural BLOCKER. Never expose credential candidates in excerpts.

## Evidence

Structural PASS means no active BLOCKER was found in the checked scope. Required metadata/resource checks that cannot complete are reported in `skippedChecks`; `structuralReceipt.complete=false` keeps completion blocked. UI runtime and Commerce/payment evidence remain separate requirements. A successful payment-runtime stage retains Commerce runtime requirements; return URLs alone do not prove payment.

Inspect advice against the real user request and code path, obtain the task-specific runtime/business evidence, and report any remaining uncertainty. Do not create a generic model-controlled exemption list or disable a capability merely to make Preflight pass.
