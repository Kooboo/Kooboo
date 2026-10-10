import assert from "node:assert/strict";
import { readFileSync, readdirSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import test from "node:test";

const scriptsDir = dirname(fileURLToPath(import.meta.url));
const packageDir = resolve(scriptsDir, "..");
const rulesDir = resolve(packageDir, "references/Preflight/rules");
const scannerSource = readFileSync(resolve(scriptsDir, "preflight-scan.js"), "utf8");

function setRuleProfile(text, id, profile) {
  return text.replace(new RegExp(`(  - id: ${id}\\n)([\\s\\S]*?)(?=  - id: |$)`), (_, head, body) => head + body.replace(/    profiles:.*\n/, `    profiles: [${profile}]\n`));
}

function runScanner({
  artifacts,
  lists = {},
  profile = "site",
  appPlanId,
  appPlanRead,
  scopeKey = "scope-1",
  deliveryStage,
  ruleTransform = (text) => text,
  onCall = () => {},
}) {
  const parameters = {
    profile,
    taskType: "site-page",
    layers: ["frontend", "kooboo-template", "integration"],
    mode: "capability",
    artifacts,
    appPlanId,
    deliveryStage,
    scopeKey,
  };
  const k = {
    request: {
      get(name) {
        return parameters[name];
      },
    },
    ai: {
      callFunction(name, args) {
        onCall(name, args);
        if (name === "read_skill_resource") {
          return ruleTransform(readFileSync(resolve(packageDir, args.resourceName), "utf8"));
        }
        if (name === "list_site_object") {
          const value = lists[args.objectType];
          if (value instanceof Error) throw value;
          return { list: value || [] };
        }
        if (name === "read_app_plan") {
          return appPlanRead;
        }
        throw new Error(`Unexpected function call: ${name}`);
      },
    },
  };
  return new Function("k", scannerSource)(k);
}

function runPage(content, lists) {
  return runScanner({
    artifacts: [{ name: "orders", objectType: "Page", content }],
    lists,
  });
}

test("keeps Preflight rule IDs unique across rule resources", () => {
  const owners = new Map();
  for (const fileName of readdirSync(rulesDir).filter((name) =>
    name.endsWith(".yaml")
  )) {
    const content = readFileSync(resolve(rulesDir, fileName), "utf8");
    for (const match of content.matchAll(/^\s*-\s+id:\s*([A-Z0-9-]+)/gm)) {
      const ruleId = match[1];
      assert.equal(
        owners.get(ruleId),
        undefined,
        `${ruleId} is declared in both ${owners.get(ruleId)} and ${fileName}`
      );
      owners.set(ruleId, fileName);
    }
  }
});

const appScanContext = {
  profile: "app", appPlanId: "plan-current",
  appPlanRead: { exists: true, plan: { id: "plan-current" } }
};

test("reads the latest Plan and rejects an obsolete binding", () => {
  const result = runScanner({
    ...appScanContext,
    appPlanRead: { exists: true, plan: { id: "plan-new" } },
    artifacts: [{ objectType: "Page", name: "app", content: "<main></main>" }],
    onCall(name, args) {
      if (name === "read_app_plan") assert.deepEqual(args, {});
    },
  });
  assert.equal(result.code, "app_plan_context_mismatch");
});

test("App without validated identifiers stops before loading any rules", () => {
  const result = runScanner({
    profile: "app",
    artifacts: [{ objectType: "Page", name: "app", content: "<main></main>" }],
    onCall() { assert.fail("Invalid App context must stop before any function call"); },
  });
  assert.equal(result.code, "invalid_arguments");
});

test("profile exclusions prevent cross checks and inventory reads", () => {
  const excluded = ["PF-KOBOO-TEMPLATE-007", "PF-KOBOO-TEMPLATE-013", "PF-KOBOO-TEMPLATE-014", "PF-KOBOO-RESOURCE-001", "PF-KOBOO-RESOURCE-002", "PF-COMMERCE-003"];
  const calls = [];
  const result = runScanner({
    ...appScanContext,
    deliveryStage: "commerce-runtime",
    artifacts: [{ objectType: "Page", name: "app", layoutName: "missing", commerceSurface: "products", content: '<view id="missing"></view><link rel="stylesheet" href="/missing.css">' }],
    ruleTransform(text) {
      for (const id of excluded) text = setRuleProfile(text, id, "site");
      return text;
    },
    onCall(name, args) { calls.push({ name, args }); },
  });
  assert.ok(!calls.some((call) => call.name === "read_object_text"));
  // Page size lookup is independent of cross-rule inventories.
  assert.ok(!calls.some((call) => call.name === "list_site_object" && call.args.objectType !== "Page"));
  assert.equal(result.structuralReceipt.complete, true);
  assert.ok(!result.findings.some((item) => excluded.includes(item.id)));
});

test("App-only cross rules are excluded from Site and run in App", () => {
  const options = {
    artifacts: [{ objectType: "View", name: "body", content: '<view id="missing"></view>' }],
    ruleTransform: (text) => setRuleProfile(text, "PF-KOBOO-RESOURCE-001", "app"),
  };
  assert.ok(!runScanner(options).findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"));
  assert.ok(runScanner({ ...options, ...appScanContext }).findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"));
});

test("cache context separates profiles, current Plans and session evidence", () => {
  const api = new Function("k", scannerSource.replace(/return main\(\);\s*$/, "return { buildCacheContextKey, canReuseArtifact, RULE_VERSION };"))({
    ai: { callFunction: () => ({ list: [{ name: "main", size: 10 }] }) },
  });
  const contexts = [
    { profile: "site" },
    { profile: "app", appPlanId: "p1", scopeKey: "e1" },
    { profile: "app", appPlanId: "p2", scopeKey: "e1" },
    { profile: "app", appPlanId: "p1", scopeKey: "e2" },
  ];
  const artifact = { objectType: "Api", name: "main" };
  const keys = contexts.map((context) => api.buildCacheContextKey(context, artifact));
  for (let i = 0; i < keys.length; i++) {
    const entry = { ruleVersion: api.RULE_VERSION, contextKey: keys[i], size: 10, capabilities: { common: true, kscript: true } };
    for (let j = 0; j < keys.length; j++) {
      assert.equal(api.canReuseArtifact(true, "focused", entry, {}, "Api:main", artifact, keys[j]), i === j);
    }
  }
});

test("excluding one cross rule preserves other applicable checks", () => {
  const options = {
    deliveryStage: "commerce-runtime",
    artifacts: [{ objectType: "Page", name: "products", commerceSurface: "products", content: '<view id="missing"></view><k-data></k-data>' }],
  };
  assert.ok(runScanner(options).findings.some((item) => item.id === "PF-COMMERCE-003"));
  const result = runScanner({ ...options,
    ruleTransform: (text) => setRuleProfile(text, "PF-COMMERCE-003", "app"),
  });
  assert.ok(!result.findings.some((item) => item.id === "PF-COMMERCE-003"));
  assert.ok(result.findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"));
});

test("placeholder image policy is advisory", () => {
  const siteResult = runScanner({
    artifacts: [
      {
        name: "home",
        objectType: "Page",
        content: '<img src="https://picsum.photos/200/300">',
      },
    ],
  });
  assert.ok(siteResult.advisories.some((item) => item.id === "PF-FRONTEND-001"));
});

test("binds current server Plan and validates import maps", () => {
  const result = runScanner({ ...appScanContext,
    artifacts: [{ name: "app", objectType: "Page", content: '<script type="importmap">{invalid}</script><main>App</main>' }] });
  assert.equal(result.structuralReceipt.profile, "app");
  assert.deepEqual(result.structuralReceipt.appContext, { appPlanId: "plan-current", scopeKey: "scope-1" });
  assert.ok(result.findings.some(item => item.id === "PF-APP-001"));
});
test("missing or failed server Plan reads stop before rule loading", () => {
  for (const appPlanRead of [{exists: false, plan: null}, {exists: true}, {exists: true, plan: {}}, {error: {message: "read failed"}}]) {
    let loaded = false;
    const result = runScanner({...appScanContext, appPlanRead,
      artifacts: [{name:"app",objectType:"Page",content:"<main></main>"}],
      onCall(name) { if(name === "read_skill_resource") loaded = true; }});
    assert.equal(result.code, "app_plan_read_unavailable"); assert.equal(loaded, false);
  }
});
test("reports an unassembled empty Page block skeleton", () => {
  const result = runPage('<div data-block="orders-main"></div>');
  assert.ok(result.advisories.some((item) => item.id === "PF-KOBOO-TEMPLATE-022"));
});

test("reports a referenced View missing from an available inventory", () => {
  const result = runPage('<view id="orders_content"></view>', { View: [] });
  assert.equal(result.result, "FAIL");
  assert.ok(result.findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"));
  assert.equal(result.skippedChecks, undefined);
  assert.equal(result.structuralReceipt.complete, true);
});

test("fails the completion gate closed when View inventory is unavailable", () => {
  const result = runPage('<view id="orders_content"></view>', {
    View: new Error("list_site_object unavailable"),
  });
  assert.equal(result.result, "PASS");
  assert.equal(result.findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"), false);
  assert.deepEqual(result.skippedChecks.map((item) => item.id), ["PF-KOBOO-RESOURCE-001"]);
  assert.equal(result.structuralReceipt.passed, true);
  assert.equal(result.structuralReceipt.complete, false);
  assert.equal(result.completionGate.claimAllowed, false);
  assert.equal(result.completionGate.maxCompletionLevel, "modified");
  assert.ok(result.completionGate.missingEvidence.some((item) => item.includes("list_site_object(View)")));
});

test("accepts a referenced View present in the inventory", () => {
  const result = runPage('<view id="orders_content"></view>', {
    View: [{ id: "view-id", name: "orders_content", url: "/orders-content" }],
  });
  assert.equal(result.findings.some((item) => item.id === "PF-KOBOO-RESOURCE-001"), false);
  assert.equal(result.skippedChecks, undefined);
  assert.equal(result.structuralReceipt.complete, true);
});

test("marks local resource route verification incomplete when an inventory is unavailable", () => {
  const result = runPage('<script src="/app.js"></script>', {
    Script: new Error("list_site_object unavailable"),
    CmsFile: [],
  });
  assert.equal(result.findings.some((item) => item.id === "PF-KOBOO-RESOURCE-002"), false);
  assert.deepEqual(result.skippedChecks.map((item) => item.id), ["PF-KOBOO-RESOURCE-002"]);
  assert.equal(result.structuralReceipt.complete, false);
  assert.equal(result.completionGate.claimAllowed, false);
  assert.equal(result.completionGate.maxCompletionLevel, "modified");
});

function universalPlaceholderFinding(content) {
  const result = runScanner({
    artifacts: [{ name: "content", objectType: "View", content }],
  });
  return result.findings.find((item) => item.id === "PF-UNIVERSAL-001");
}

test("allows natural prose containing placeholder counts", () => {
  assert.equal(universalPlaceholderFinding("<p>No placeholder counts are shown.</p>"), undefined);
});

test("reports standalone placeholder content", () => {
  assert.ok(universalPlaceholderFinding("<p>placeholder text</p>"));
});

test("allows the HTML placeholder attribute", () => {
  assert.equal(universalPlaceholderFinding('<input placeholder="Email">'), undefined);
});

function moduleAppArtifacts() {
  return [
    { objectType: "Page", name: "index", url: "/", content: '<script type="importmap">{"imports":{"vue":"/vue.js","vue-router":"/vue-router.js"}}</script><script type="module" src="/main.js"></script>' },
    { objectType: "Script", name: "main.js", url: "/main.js", content: "import App from './app.js'; import './router.js';" },
    { objectType: "Script", name: "app.js", url: "/app.js", content: "import { ref } from 'vue'; export { default } from './component.js';" },
    { objectType: "Script", name: "router.js", url: "/router.js", content: "import { createRouter } from 'vue-router'; buildRouteReturnUrl(to.fullPath); buildLoginUrl(returnUrl);" },
    { objectType: "Script", name: "component.js", url: "/component.js", content: "import { ref } from 'vue'; import './store.js';" },
    { objectType: "Script", name: "store.js", url: "/store.js", content: "import { reactive } from 'vue'; import './app.js';" },
    { objectType: "Script", name: "vue.js", url: "/vue.js", content: "export const ref = () => {}; export const reactive = x => x;" },
    { objectType: "Script", name: "vue-router.js", url: "/vue-router.js", content: "export const createRouter = () => {};" },
  ];
}

test("inventory App follows transitive imports, re-exports and cycles to the Page import map", () => {
  const result = runScanner({ ...appScanContext, artifacts: moduleAppArtifacts() });
  assert.equal(result.structuralReceipt.complete, true, JSON.stringify(result.skippedChecks));
  assert.equal(result.structuralStatus, "passed");
  assert.ok(!result.findings.some(item => item.id === "PF-KOBOO-RESOURCE-002"));
  assert.equal(result.completionGate.claimAllowed, false, "runtime evidence remains required");
});

test("missing module chain stays incomplete and gives a recovery action instead of blind retries", () => {
  const artifacts = moduleAppArtifacts().filter(item => item.name !== "main.js");
  // main.js exists on the site but its source cannot be read, so the chain cannot be traced.
  const lists = { Script: moduleAppArtifacts().filter(item => item.objectType === "Script").map(item => ({ name: item.name, url: item.url })) };
  const result = runScanner({ ...appScanContext, artifacts, lists });
  assert.equal(result.structuralReceipt.complete, false);
  assert.equal(result.structuralStatus, "incomplete");
  assert.equal(result.completionGate.claimAllowed, false);
  assert.ok(result.skippedChecks.some(item => item.reason.includes("app.js -> vue")));
  assert.ok(result.completionGate.missingEvidence.some(item => item.includes("app.js -> vue")));
  assert.deepEqual(result.nextActions.beforeClaimCompletion, ["resolve_skipped_checks_before_rerunning_preflight"]);
  assert.match(result.nextActions.retryGuidance, /stop unchanged retries/);
});

test("an unrelated Page cannot supply an import map to another Page's module graph", () => {
  const artifacts = moduleAppArtifacts();
  artifacts.push({ objectType: "Page", name: "other", url: "/other", content: '<script type="module" src="/main.js"></script>' });
  const result = runScanner({ ...appScanContext, artifacts });
  assert.equal(result.structuralReceipt.complete, false);
  assert.ok(result.skippedChecks.some(item => item.reason.includes("app.js -> vue")));
});

test("inline module entry and scoped prefix maps resolve against the owning Page base", () => {
  const artifacts = [
    { objectType: "Page", name: "index", url: "/app/index", content: '<base href="/assets/"><script type="importmap">{"imports":{"lib/":"./fallback/"},"scopes":{"./feature/":{"lib/":"./scoped/"}}}</script><script type="module">import "./feature/main.js";</script>' },
    { objectType: "Script", name: "main", url: "/assets/feature/main.js", content: "import { ref } from 'lib/vue.js';" },
    { objectType: "Script", name: "vue", url: "/assets/scoped/vue.js", content: "export const ref = () => {};" },
  ];
  const result = runScanner({ ...appScanContext, artifacts });
  assert.equal(result.structuralReceipt.complete, true, JSON.stringify(result.skippedChecks));
  assert.ok(!result.findings.some(item => item.id === "PF-KOBOO-RESOURCE-002"));
});

test("computed imports remain incomplete even with a complete static module graph", () => {
  const artifacts = moduleAppArtifacts();
  artifacts.find(item => item.name === "app.js").content += " import(modulePath);";
  const result = runScanner({ ...appScanContext, artifacts });
  assert.equal(result.structuralReceipt.complete, false);
  assert.ok(result.skippedChecks.some(item => item.reason.includes("computed module import")));
});

test("JavaScript URL helpers are not CSS URLs, while real CSS references are still checked", () => {
  const scanner = new Function("k", scannerSource.replace(/return main\(\);\s*$/, "return { extractResourceReferenceCandidates };"))({});
  const helpers = 'buildRouteReturnUrl(to.fullPath); buildLoginUrl(returnUrl); url(value); const help = "url(example)"; const style = "url(variable)";';
  assert.deepEqual(scanner.extractResourceReferenceCandidates(helpers, "Script"), []);
  for (const [objectType, content] of [
    ["Style", '.hero { background: url("/missing.png") }'],
    ["Page", '<style>.hero { background: url(/missing.png) }</style>'],
    ["Script", 'export default { template: `<div style="background: url(/missing.png)"></div>` }'],
  ]) {
    const result = runScanner({ artifacts: [{ name: "test", objectType, url: "/test", content }] });
    assert.ok(result.findings.some(item => item.id === "PF-KOBOO-RESOURCE-002"), objectType);
  }
});

test("planned App read selector is empty and receipts carry only the App context", () => {
  let selector;
  const artifacts = [{objectType:"Script",name:"stock.js",content:"export const stock = [];"}];
  const result = runScanner({...appScanContext, artifacts,
    onCall(name,args) { if(name === "read_app_plan") selector=args; }});
  assert.deepEqual(selector, {});
  assert.deepEqual(result.structuralReceipt.appContext,{appPlanId:"plan-current",scopeKey:"scope-1"});
  assert.equal(result.structuralReceipt.siteMilestoneContext, undefined);
});
