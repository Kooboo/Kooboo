import assert from 'node:assert/strict';
import { readFileSync, readdirSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import test from 'node:test';
const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const source = readFileSync(resolve(root, 'scripts/preflight-scan.js'), 'utf8');
function run(artifacts, options = {}) {
  const calls = [];
  const parameters = { profile: 'site', layers: [], taskType: 'all', mode: 'capability', debug: true, artifacts, ...options.parameters };
  if (parameters.profile === 'app') Object.assign(parameters, { appPlanId: 'plan', scopeKey: 'scope-1' });
  const k = { request: { get: name => parameters[name] }, ai: { callFunction(name, args) {
    calls.push({ name, args });
    if (name === 'read_skill_resource') {
      if (options.ruleError) throw new Error('rules unavailable');
      return options.ruleContent ?? readFileSync(resolve(root, args.resourceName), 'utf8');
    }
    if (name === 'read_app_plan') return { exists: true, plan: { id: 'plan' } };
    if (name === 'get_site_info') { if (options.settingsError) throw new Error('unavailable'); return { siteSettings: { isMultiLingual: options.multilingual ?? false } }; }
    if (name === 'list_site_object') { const list = options.lists?.[args.objectType]; if (list instanceof Error) throw list; return { list: list || [] }; }
    if (name === 'read_object_text') { const value = options.objects?.[`${args.objectType}:${args.nameOrId}`]; if (value === undefined) throw new Error('missing object'); return value; }
    throw new Error(`unexpected ${name}`);
  } } };
  return { result: new Function('k', source)(k), calls };
}
const page = content => ({ objectType: 'Page', name: 'home', content });
const js = content => ({ objectType: 'Script', name: 'main', url: '/main.js', content });
const api = content => ({ objectType: 'Api', name: 'handler', url: '/api/{action}', content });
const has = (result, id) => (result.findings || []).some(x => x.id === id);
const advises = (result, id) => (result.advisories || []).some(x => x.id === id);
const scan = (artifact, options) => run([artifact], options).result;

test('lightweight App scans use the shared site profile without reading a Plan or carrying a checklist receipt', () => {
  const { result, calls } = run([js('export const items = [];')]);
  assert.equal(result.structuralReceipt.profile, 'site');
  assert.equal(result.structuralReceipt.siteMilestoneContext, undefined);
  assert.ok(!calls.some(call => call.name === 'read_app_plan'));
  assert.ok(result.structuralReceipt.evidenceRequirements.some(item => item.id === 'site.ui-runtime'));
});

test('all rules have explicit profile and capability, and every custom check is callable', () => {
  const internals = new Function(source.replace(/return main\(\);\s*$/, 'return {parseRulesYaml,runCustomCheck,RULE_RESOURCES};'))();
  const ids = new Set();
  for (const file of readdirSync(resolve(root, 'references/Preflight/rules'))) {
    const rules = internals.parseRulesYaml(readFileSync(resolve(root, 'references/Preflight/rules', file), 'utf8'));
    assert.equal(readFileSync(resolve(root, '../zh/kooboo-coding/references/Preflight/rules', file), 'utf8'), readFileSync(resolve(root, 'references/Preflight/rules', file), 'utf8'), `${file}: executable rule mirrors must agree`);
    assert.ok(rules.length, `${file} must not be empty`);
    for (const rule of rules) {
      assert.ok(!ids.has(rule.id), rule.id); ids.add(rule.id);
      assert.ok(rule.profiles?.length && rule.capability, rule.id);
      if (rule.customCheck) assert.ok(source.includes(`customCheck === "${rule.customCheck}"`), rule.id);
    }
  }
  assert.ok(!ids.has('PF-DESIGN-002'));
  assert.ok(!ids.has('PF-KOBOO-TEMPLATE-004'));
  assert.ok(!ids.has('PF-COMMERCE-010'));
  assert.deepEqual(Object.values(internals.RULE_RESOURCES).map(x => x.split('/').at(-1)).sort(), readdirSync(resolve(root, 'references/Preflight/rules')).sort());
});

test('k-data rules load from content even with no model layers, only in Site', () => {
  const content = '<query source="content" />';
  const site = scan(page(content));
  const app = scan(page(content), { parameters: { profile: 'app' } });
  assert.ok(has(site, 'PF-KOBOO-TEMPLATE-017'));
  assert.ok(site.debug.loadedRuleResources.some(x => x.endsWith('/k-data.yaml')));
  assert.ok(!app.debug.loadedRuleResources.some(x => x.endsWith('/k-data.yaml')));
});

test('an ordinary App login/orders/account does not activate Commerce', () => {
  for (const name of ['login', 'orders', 'account', 'payment']) {
    const { result, calls } = run([{ ...js('export const items = [];'), name }], { parameters: { profile: 'app' } });
    assert.equal(result.status, undefined);
    assert.ok(!calls.some(x => /commerce|payment\.yaml/.test(x.args.resourceName || '')));
    assert.ok(!result.structuralReceipt.evidenceRequirements.some(x => x.id.startsWith('commerce.')));
  }
});

test('persisted Commerce code is detected after reading, regardless of object name', () => {
  const { result } = run([{ objectType: 'Api', name: 'anything' }], { objects: { 'Api:anything': 'k.commerce.order.pay(orderId);' } });
  assert.equal(result.code, 'invalid_arguments'); // explicit Commerce stage required
  const ready = scan(api('k.commerce.order.pay(orderId);'), { parameters: { deliveryStage: 'static-first' } });
  assert.ok(has(ready, 'PF-COMMERCE-007'));
  assert.equal(ready.result, 'FAIL');
});

test('App template assembly is shared and hydrates the real Layout binding', () => {
  const { result } = run([{ objectType: 'Page', name: 'home' }], {
    parameters: { profile: 'app' },
    lists: { Page: [{ name: 'home', url: '/home', layoutName: 'shell' }] },
    objects: { 'Page:home': '<main>missing layout wrapper</main>', 'Layout:shell': '<main k-placeholder="body"></main>' },
  });
  assert.ok(has(result, 'PF-KOBOO-TEMPLATE-003'));
});

test('unavailable persisted Page binding and named API route fail completeness', () => {
  const { result } = run([{ objectType: 'Page', name: 'home' }], { objects: { 'Page:home': '<main>Hi</main>' }, lists: { Page: new Error('offline') } });
  assert.equal(result.structuralReceipt.complete, false);
  assert.ok(result.skippedChecks.some(x => x.id === 'PF-KOBOO-TEMPLATE-014'));
  const route = scan({ objectType: 'Api', name: 'no-route', content: 'k.api.get("list", () => []);' });
  assert.equal(route.structuralReceipt.complete, false);
  assert.ok(route.skippedChecks.some(x => x.id === 'PF-KSCRIPT-007'));
});

test('empty Layout names fail, repeated Views are composition advice', () => {
  assert.ok(has(scan({ objectType: 'Layout', name: 'shell', content: '<main k-placeholder=""></main>' }), 'PF-KOBOO-TEMPLATE-005'));
  const { result } = run([
    { ...page('<layout id="shell"><placeholder id="body"><view id="card"></view></placeholder></layout>'), layoutName: 'shell' },
    { objectType: 'Layout', name: 'shell', content: '<view id="card"></view><main k-placeholder="body"></main>' },
    { objectType: 'View', name: 'card', content: '<p>Reusable</p>' },
  ]);
  assert.ok(advises(result, 'PF-KOBOO-TEMPLATE-007'));
  assert.ok(!has(result, 'PF-KOBOO-TEMPLATE-007'));
});

test('business statuses, client mounts and nested SVG CDATA are valid', () => {
  assert.ok(!has(scan(js('const status = "TODO"; const items = [];')), 'PF-UNIVERSAL-001'));
  assert.ok(has(scan(js('// TODO implement the endpoint')), 'PF-UNIVERSAL-001'));
  assert.ok(!has(scan(page('<svg><style><![CDATA[.a{fill:red}]]></style></svg>')), 'PF-UNIVERSAL-004'));
  assert.ok(has(scan(page('<![CDATA[<main>Hi</main>]]>')), 'PF-UNIVERSAL-004'));
  assert.ok(!advises(scan(page('<div id="app" data-block="main"></div><script>createApp({}).mount("#app")</script>')), 'PF-KOBOO-TEMPLATE-022'));
});

test('emoji and chosen visual designs are advice, not structural blockers', () => {
  assert.ok(!advises(scan(page('<p>𠀀</p>')), 'PF-UNIVERSAL-002'));
  assert.ok(advises(scan(page('<p>😀</p>')), 'PF-UNIVERSAL-002'));
  const layout = scan(page('<main class="md:h-screen grid grid-cols-3 gap-4 left-[50%]"></main>'));
  assert.ok(!has(layout, 'PF-DESIGN-002'));
  assert.ok(!advises(layout, 'PF-FRONTEND-002'));
  assert.ok(!advises(layout, 'PF-FRONTEND-003'));
});

test('App functional icons reject Unicode and Emoji substitutes but allow Lucide', () => {
  const options = { parameters: { profile: 'app' } };
  const invalid = scan(js('export const nav = `<span class="nav-icon">▣</span><span>🔍</span>`;'), options);
  assert.ok(has(invalid, 'PF-FRONTEND-008'));
  assert.equal(invalid.findings.find(item => item.id === 'PF-FRONTEND-008').occurrences[0].matches.length, 2);
  assert.ok(!has(scan(js('export const nav = `<i data-lucide="search" aria-hidden="true"></i>`;'), options), 'PF-FRONTEND-008'));
  assert.ok(!has(scan(js('export const nav = `<span class="nav-icon">▣</span>`;')), 'PF-FRONTEND-008'));
});

test('link attributes and per-control accessible labels are checked', () => {
  assert.ok(has(scan(page('<a href = "#">Go</a>')), 'PF-UNIVERSAL-003'));
  assert.ok(!has(scan(page('<form><input aria-label="Search"></form>')), 'PF-FORM-001'));
  assert.ok(!has(scan(page('<label for="a">Name</label><input id="a"><span id="b">Email</span><input aria-labelledby="b">')), 'PF-FORM-001'));
  assert.ok(has(scan(page('<label for="a">Name</label><input id="a"><input id="unlabelled">')), 'PF-FORM-001'));
  assert.ok(!advises(scan(page('<form><input aria-label="Search"><p>提交失败，请重试</p></form>')), 'PF-FORM-002'));
});

test('client framework templates and header server scripts are allowed', () => {
  const result = scan(page('<div v-if="ready"></div><script>const template = "<div>{{ name }}</div>";</script>'));
  assert.ok(!has(result, 'PF-KOBOO-TEMPLATE-001'));
  assert.ok(!has(result, 'PF-KOBOO-TEMPLATE-020'));
  assert.ok(!has(scan(page('<head><script env="server">const value = 1;</script></head>')), 'PF-KOBOO-TEMPLATE-004'));
  assert.ok(has(scan(page('<script>k.request.queryString.get("q")</script>')), 'PF-KOBOO-TEMPLATE-002'));
  assert.ok(!has(scan(page('<script env = "server">k.request.queryString.get("q")</script>')), 'PF-KOBOO-TEMPLATE-002'));
});

test('platform multilingual configuration overrides model flags', () => {
  assert.ok(!has(scan({ ...page('<a href="/en/cart">Cart</a>'), multilingual: true }), 'PF-KSCRIPT-004'));
  assert.ok(has(scan({ ...page('<a href="/en/cart">Cart</a>'), multilingual: false }, { multilingual: true }), 'PF-KSCRIPT-004'));
  assert.equal(scan(page('<a href="/en/cart">Cart</a>'), { settingsError: true }).structuralReceipt.complete, false);
});

test('API response, route parameter names, fixtures and checked writes are not banned', () => {
  assert.ok(!has(scan(api('return "OK";')), 'PF-KSCRIPT-002'));
  assert.ok(!advises(scan(api('k.api.get((params) => params);')), 'PF-KSCRIPT-006'));
  const mock = scan(api('const items = [{id: 1}]; return items;'), { parameters: { profile: 'app' } });
  assert.ok(!has(mock, 'PF-DATA-001'));
  assert.ok(advises(mock, 'PF-DATA-001'));
  assert.ok(!advises(scan(api('const body = k.request.body; if (!body.name) throw new Error("name"); store.add(body);')), 'PF-BACKEND-001'));
});

test('safe HTML and placeholder credentials do not trigger; candidates are redacted', () => {
  for (const content of ['element.innerHTML = "";', 'element.innerHTML = DOMPurify.sanitize(input);']) assert.ok(!advises(scan(js(content)), 'PF-SECURITY-002'));
  assert.ok(advises(scan(js('element.innerHTML = input;')), 'PF-SECURITY-002'));
  assert.ok(!advises(scan(api('const secret = "YOUR_SECRET_HERE";')), 'PF-SECURITY-001'));
  const result = scan(api('const secret = "synthetic-test-value-123";'));
  assert.ok(advises(result, 'PF-SECURITY-001'));
  assert.ok(!JSON.stringify(result).includes('synthetic-test-value-123'));
});

test('verified external images and attribution are allowed; random assets get advice', () => {
  assert.ok(!advises(scan(page('<a href="https://unsplash.com/@author">Credit</a><img src="https://images.unsplash.com/photo.jpg">')), 'PF-FRONTEND-001'));
  assert.ok(advises(scan(page('<img src="https://picsum.photos/200/300">')), 'PF-FRONTEND-001'));
});

test('valid import map entry is supported in App and Site', () => {
  const content = '<script type="importmap">{"imports":{"vue":"https://cdn.example/vue.js"}}</script><script type="module">import {createApp} from "vue";</script>';
  for (const profile of ['site', 'app']) {
    const result = scan({ ...page(content), url: '/index' }, { parameters: { profile } });
    assert.ok(!has(result, 'PF-APP-001'));
    assert.equal(result.structuralReceipt.complete, true);
  }
  for (const body of ['{invalid}', '{"imports":{"vue":12}}', '{"imports":{"pkg/":"/pkg.js"}}']) assert.ok(has(scan(page(`<script type="importmap">${body}</script>`)), 'PF-APP-001'));
});

test('relative resource URLs resolve against actual routes and base href', () => {
  const options = { lists: { Style: [{ name: 'theme', url: '/assets/theme.css' }] } };
  const good = scan({ ...page('<base href="/assets/"><link rel="stylesheet" href="./theme.css">'), url: '/nested/page' }, options);
  assert.ok(!has(good, 'PF-KOBOO-RESOURCE-002')); assert.equal(good.structuralReceipt.complete, true);
  const bad = scan({ ...page('<link rel="stylesheet" href="../missing.css">'), url: '/nested/page' }, options);
  assert.ok(has(bad, 'PF-KOBOO-RESOURCE-002'));
  assert.equal(scan({ objectType: 'View', name: 'fragment', content: '<img src="./image.png">' }).structuralReceipt.complete, false);
});

test('static/dynamic module imports and Page import maps verify local modules', () => {
  const artifact = { ...js('import { x } from "./dep.js"; import("../lazy.js");'), url: '/assets/main.js' };
  const good = scan(artifact, { lists: { Script: [{ name: 'dep', url: '/assets/dep.js' }, { name: 'lazy', url: '/lazy.js' }] } });
  assert.ok(!has(good, 'PF-KOBOO-RESOURCE-002'));
  assert.ok(has(scan(artifact), 'PF-KOBOO-RESOURCE-002'));
  const { result } = run([
    { ...page('<script type="importmap">{"imports":{"pkg/":"/lib/"}}</script><script type="module" src="/main.js"></script>'), url: '/index' },
    js('import { x } from "pkg/util.js";'),
  ], { lists: { Script: [{ name: 'util', url: '/lib/util.js' }] } });
  assert.ok(!has(result, 'PF-KOBOO-RESOURCE-002')); assert.equal(result.structuralReceipt.complete, true);
  assert.equal(scan(js('import { x } from "unmapped";')).structuralReceipt.complete, false);
});

test('unlisted platform entry Scripts still link changed modules to the Page import map', () => {
  const { result } = run([
    { ...page('<script type="importmap">{"imports":{"vue-router":"/vue-router.js"}}</script><script type="module" src="/main.js"></script>'), url: '/' },
    { objectType: 'Script', name: 'router.js', url: '/router.js', content: 'import { createRouter } from "vue-router";' },
  ], {
    lists: { Script: [{ name: 'main.js', url: '/main.js' }, { name: 'router.js', url: '/router.js' }, { name: 'vue-router.js', url: '/vue-router.js' }] },
    objects: { 'Script:main.js': 'import "/router.js";' },
  });
  assert.ok(!has(result, 'PF-KOBOO-RESOURCE-002'));
  assert.equal(result.structuralReceipt.complete, true);
});

test('a missing static module route blocks while other missing resources stay HIGH', () => {
  const severity = result => result.findings.find(x => x.id === 'PF-KOBOO-RESOURCE-002')?.severity;
  const moduleResult = scan(js('import { list } from "/service/warehouse.js";'));
  assert.equal(severity(moduleResult), 'BLOCKER');
  assert.equal(moduleResult.result, 'FAIL');
  const imageResult = scan({ ...page('<img src="/missing.png" alt="">'), url: '/index' });
  assert.equal(severity(imageResult), 'HIGH');
  assert.notEqual(imageResult.result, 'FAIL');
});

test('sanitizing one expression does not make a concatenated HTML sink safe', () => {
  assert.ok(advises(scan(js('element.innerHTML = DOMPurify.sanitize(safe) + unsafe;')), 'PF-SECURITY-002'));
  assert.ok(!advises(scan(js('element.innerHTML = DOMPurify.sanitize(safe + unsafe);')), 'PF-SECURITY-002'));
});

test('private key material is a redacted structural blocker', () => {
  const result = scan(api('const key = "-----BEGIN PRIVATE KEY-----synthetic-fixture-----END PRIVATE KEY-----";'));
  assert.ok(has(result, 'PF-SECURITY-001'));
  assert.equal(result.result, 'FAIL');
  assert.ok(!JSON.stringify(result).includes('synthetic-fixture'));
});

test('module parsing ignores comments and quoted imports but keeps imports beside HTML strings', () => {
  const harmless = scan(js('// import "./missing.js";\nconst text = "import(\'./also-missing.js\')";'));
  assert.ok(!has(harmless, 'PF-KOBOO-RESOURCE-002'));
  assert.equal(harmless.structuralReceipt.complete, true);
  const actual = scan(js('const template = "<div>hello</div>"; import "./missing.js";'));
  assert.ok(has(actual, 'PF-KOBOO-RESOURCE-002'));
  assert.equal(scan(js('import(moduleName);')).structuralReceipt.complete, false);
});

test('unavailable or malformed rule resources cannot produce a passing receipt', () => {
  for (const options of [{ ruleError: true }, { ruleContent: 'not a rule' }, { ruleContent: '- id: PF-INVALID\n  title: Missing classification' }]) {
    const result = scan(page('<main>Hello</main>'), options);
    assert.equal(result.code, 'rule_load_failed');
    assert.equal(result.structuralReceipt, undefined);
  }
});
