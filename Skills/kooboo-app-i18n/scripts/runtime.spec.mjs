import assert from 'node:assert/strict'
import { readFileSync, writeFileSync, mkdtempSync, rmSync } from 'node:fs'
import { tmpdir } from 'node:os'
import { join } from 'node:path'
import { pathToFileURL } from 'node:url'
import test, { after } from 'node:test'

// Exercise the shipped Vue and vue-i18n, without replacing their translation logic.
const temp = mkdtempSync(join(tmpdir(), 'kooboo-i18n-'))
after(() => rmSync(temp, { recursive: true, force: true }))
const asset = name => readFileSync(new URL('../assets/Script/' + name, import.meta.url), 'utf8')
writeFileSync(join(temp, 'vue.mjs'), readFileSync(new URL('../../kooboo-coding/assets/app-base/Script/vue.js', import.meta.url)))
writeFileSync(join(temp, 'vue-i18n.mjs'), asset('vue-i18n.js').replaceAll('from"vue"', 'from"./vue.mjs"'))
writeFileSync(join(temp, 'service.mjs'), asset('service.i18n.js').replace("from 'vue-i18n'", "from './vue-i18n.mjs'").replace("from 'vue'", "from './vue.mjs'"))
writeFileSync(join(temp, 'router.mjs'), readFileSync(new URL('../../kooboo-coding/assets/app-base/Script/vue-router.js', import.meta.url), 'utf8').replace(/from\s*(['"])vue\1/g, 'from "./vue.mjs"'))
const { createAppI18n, bindTranslatedTitle } = await import(pathToFileURL(join(temp, 'service.mjs')))
const { ref, computed, createRenderer, h, nextTick } = await import(pathToFileURL(join(temp, 'vue.mjs')))
const { useI18n } = await import(pathToFileURL(join(temp, 'vue-i18n.mjs')))
const { createRouter, createMemoryHistory } = await import(pathToFileURL(join(temp, 'router.mjs')))

const translations = {
  'zh-CN': { 'app.test.save': '保存', 'app.test.onlyDefault': '默认', 'app.test.hello': '你好 {name}', 'app.test.name': '应用' },
  en: { 'app.test.save': 'Save', 'app.test.hello': 'Hello {name}', 'app.test.name': 'App', 'app.test.enOnly': 'English only', 'app.test.email': "user{'@'}example.com", 'app.test.pipe': "left{'|'}right", 'app.test.brace': "{'{'}name{'}'}", 'app.test.count': 'no items | one item | {count} items' },
  'en-US': { 'app.test.save': 'Save (US)' },
  fr: { 'app.test.save': 'Enregistrer' }
}
function payload(locale = 'zh-CN', overrides = {}) {
  return { siteId: 'site-a', locale, currentLocale: 'zh-CN', defaultLocale: 'zh-CN', supportedLocales: Object.keys(translations),
    messages: { [locale]: { ...translations[locale] }, 'zh-CN': { ...translations['zh-CN'] } }, ...overrides }
}
function harness() {
  const calls = []
  const values = new Map()
  const page = { title: '', documentElement: { lang: '' } }
  const options = {
    siteId: 'site-a',
    url: 'https://app.test/?preview=1#/orders', document: page,
    storage: { getItem: key => values.get(key), setItem: (key, value) => values.set(key, value) },
    fetcher: async url => {
      calls.push(url)
      const query = new URL(url).searchParams
      const canonical = value => Object.keys(translations).find(code => code.toLowerCase() === value?.toLowerCase())
      const locale = query.get('locale') || canonical(query.get('preferredLocale')) || canonical(query.get('savedLocale'))
      return { ok: true, json: async () => payload(locale || undefined) }
    }
  }
  return { options, calls, values, page }
}

test('flat Label keys, interpolation, literal syntax, pluralization and default-only fallback', async () => {
  const h = harness()
  const runtime = await createAppI18n(h.options)
  assert.equal(runtime.i18n.global.t('app.test.hello', { name: 'Ada' }), '你好 Ada')
  await runtime.setLocale('EN')
  const t = runtime.i18n.global.t
  assert.equal(t('app.test.save'), 'Save')
  assert.equal(t('app.test.onlyDefault'), '默认')
  assert.equal(t('app.test.hello', { name: 'Ada' }), 'Hello Ada')
  assert.equal(t('app.test.email'), 'user@example.com')
  assert.equal(t('app.test.pipe'), 'left|right')
  assert.equal(t('app.test.brace'), '{name}')
  assert.equal(t('app.test.count', { count: 2 }, 2), '2 items')
  await runtime.setLocale('en-US')
  // en has already been loaded, but must not become an implicit region fallback.
  assert.equal(t('app.test.enOnly'), 'app.test.enOnly')
  assert.equal(t('app.test.onlyDefault'), '默认')
  assert.equal(t('toString'), 'toString')
})

test('explicit language beats site-scoped preference; invalid choices use server language', async () => {
  const h = harness()
  h.values.set('kooboo:app-i18n:site-a', 'fr')
  h.options.url = 'https://app.test/?lang=EN#/orders?lang=fr'
  const first = await createAppI18n(h.options)
  assert.equal(first.locale.value, 'en')
  assert.equal(h.page.documentElement.lang, 'en')
  assert.equal(h.calls.length, 1)
  h.options.url = 'https://app.test/?lang=unknown#/orders'
  const stored = await createAppI18n(h.options)
  assert.equal(stored.locale.value, 'fr')
  assert.equal(h.calls.length, 2)
  h.values.clear()
  const fallback = await createAppI18n(h.options)
  assert.equal(fallback.locale.value, 'zh-CN')
  assert.equal(h.calls.length, 3)
})

test('saved English preference starts in one request and switching to the received default uses the cache', async () => {
  const h = harness()
  h.values.set('kooboo:app-i18n:site-a', 'en')
  h.values.set('kooboo:app-i18n:other-site', 'fr')
  const runtime = await createAppI18n(h.options)
  assert.equal(runtime.locale.value, 'en')
  assert.equal(h.calls.length, 1)
  assert.equal(new URL(h.calls[0]).searchParams.get('savedLocale'), 'en')
  assert.equal(runtime.i18n.global.t('app.test.onlyDefault'), '默认')
  await runtime.setLocale('zh-CN')
  assert.equal(runtime.i18n.global.t('app.test.save'), '保存')
  await runtime.setLocale('en')
  assert.equal(runtime.i18n.global.t('app.test.save'), 'Save')
  assert.equal(h.calls.length, 1)
})

test('a removed saved language falls back in the first response, without a failed probe', async () => {
  const h = harness()
  h.values.set('kooboo:app-i18n:site-a', 'removed-language')
  const runtime = await createAppI18n(h.options)
  assert.equal(runtime.locale.value, 'zh-CN')
  assert.equal(h.calls.length, 1)
})

test('unbound legacy service remains usable and rejects a bound service copied to another site', async () => {
  const h = harness()
  h.values.set('kooboo:app-i18n:site-a', 'en')
  const legacy = await createAppI18n({ ...h.options, siteId: null })
  assert.equal(legacy.locale.value, 'en')
  assert.equal(h.calls.length, 2)
  await assert.rejects(createAppI18n({ ...h.options, siteId: 'other-site' }), /settings changed/)
})

test('menus and titles react to switching without navigation', async () => {
  const h = harness()
  const runtime = await createAppI18n(h.options)
  const router = { currentRoute: ref({ meta: { titleKey: 'app.test.save' } }) }
  const navigation = computed(() => [{ label: runtime.i18n.global.t('app.test.save') }])
  const stop = bindTranslatedTitle(router, runtime.i18n, { appTitleKey: 'app.test.name', document: h.page })
  assert.equal(h.page.title, '保存 · 应用')
  await runtime.setLocale('en')
  assert.equal(h.page.title, 'Save · App')
  assert.equal(navigation.value[0].label, 'Save')
  router.currentRoute.value = { meta: { titleKey: 'missing', title: 'Legacy' } }
  assert.equal(h.page.title, 'Legacy · App')
  stop()
})

test('failed switches preserve locale, component locale, document language and preference', async () => {
  const h = harness()
  let componentLocale
  h.options.prepareLocale = async locale => { if (locale === 'en') throw new Error('component locale failed'); return locale }
  h.options.commitLocale = (_, prepared) => { componentLocale = prepared }
  const runtime = await createAppI18n(h.options)
  await assert.rejects(runtime.setLocale('en'), /component locale failed/)
  assert.equal(runtime.locale.value, 'zh-CN')
  assert.equal(componentLocale, 'zh-CN')
  assert.equal(h.page.documentElement.lang, 'zh-CN')
  assert.equal(h.values.size, 0)
  await assert.rejects(runtime.setLocale('xx'), /Unsupported/)
})

test('concurrent loads coalesce and only the latest switch commits', async () => {
  const h = harness()
  const releases = new Map()
  let requests = 0
  h.options.fetcher = async url => {
    const locale = new URL(url).searchParams.get('locale')
    if (!locale) return { ok: true, json: async () => payload() }
    requests++
    await new Promise(resolve => releases.set(locale, resolve))
    return { ok: true, json: async () => payload(locale) }
  }
  const runtime = await createAppI18n(h.options)
  const one = runtime.setLocale('en')
  const two = runtime.setLocale('en')
  const three = runtime.setLocale('fr')
  releases.get('fr')()
  assert.equal(await three, true)
  releases.get('en')()
  assert.deepEqual(await Promise.all([one, two]), [false, false])
  assert.equal(runtime.locale.value, 'fr')
  assert.equal(requests, 2)
  await runtime.setLocale('en')
  assert.equal(requests, 2)
})

test('fetch failure can retry; unavailable storage is optional; reload fetches fresh messages', async () => {
  const h = harness()
  h.options.storage = { getItem() { throw Error('blocked') }, setItem() { throw Error('blocked') } }
  const ordinary = h.options.fetcher
  let fail = true
  h.options.fetcher = url => new URL(url).searchParams.get('locale') === 'en' && fail
    ? Promise.resolve({ ok: false, status: 503 }) : ordinary(url)
  const runtime = await createAppI18n(h.options)
  await assert.rejects(runtime.setLocale('en'), /503/)
  fail = false
  await runtime.setLocale('en')
  assert.equal(runtime.locale.value, 'en')
  const calls = h.calls.length
  await createAppI18n(h.options)
  assert.equal(h.calls.length, calls + 1)
})

test('reject changed site/configuration or malformed language responses before committing', async () => {
  const h = harness()
  const original = h.options.fetcher
  h.options.fetcher = async url => new URL(url).searchParams.has('locale')
    ? { ok: true, json: async () => payload('en', { siteId: 'other-site' }) } : original(url)
  const runtime = await createAppI18n(h.options)
  await assert.rejects(runtime.setLocale('en'), /settings changed/)
  assert.equal(runtime.locale.value, 'zh-CN')
  await assert.rejects(createAppI18n({ ...h.options, fetcher: async () => ({ ok: true, json: async () => ({}) }) }), /Invalid/)
  await assert.rejects(createAppI18n({ ...h.options, endpoint: 'https://other.test/lang' }), /same-origin/)
})

test('translated title wins after the existing router title hook and cleans up', async () => {
  const h = harness()
  const runtime = await createAppI18n(h.options)
  const router = createRouter({ history: createMemoryHistory(), routes: [
    { path: '/', component: {}, meta: { title: 'Legacy', titleKey: 'app.test.save' } },
    { path: '/next', component: {}, meta: { title: 'Next', titleKey: 'app.test.hello' } }
  ] })
  router.afterEach(to => { h.page.title = to.meta.title })
  const stop = bindTranslatedTitle(router, runtime.i18n, { appTitleKey: 'app.test.name', document: h.page })
  await router.push('/')
  assert.equal(h.page.title, '保存 · 应用')
  await runtime.setLocale('en')
  assert.equal(h.page.title, 'Save · App')
  await router.push('/next')
  assert.equal(h.page.title, 'Hello  · App')
  stop()
  await router.push('/')
  assert.equal(h.page.title, 'Legacy')
})

test('explicit URL language updates on switch without losing hash or history state', async () => {
  const h = harness()
  h.options.url = 'https://app.test/?preview=1&lang=en#/orders?filter=open'
  const calls = []
  const state = { router: 'preserved' }
  h.options.history = { state, replaceState: (...args) => calls.push(args) }
  const runtime = await createAppI18n(h.options)
  await runtime.setLocale('fr')
  assert.equal(calls.length, 1)
  assert.equal(calls[0][0], state)
  const changed = new URL(calls[0][2])
  assert.equal(changed.searchParams.get('lang'), 'fr')
  assert.equal(changed.searchParams.get('preview'), '1')
  assert.equal(changed.hash, '#/orders?filter=open')
})

test('the plugin and useI18n share the shipped Vue and update an actually mounted component', async () => {
  const fixture = harness()
  const runtime = await createAppI18n(fixture.options)
  const renderer = createRenderer({
    createElement: type => ({ type, children: [] }),
    createText: text => ({ text }), createComment: text => ({ text }),
    setText: (node, text) => { node.text = text },
    setElementText: (node, text) => { node.text = text },
    insert: (node, parent) => { parent.children.push(node) },
    remove: () => {}, parentNode: () => null, nextSibling: () => null,
    patchProp: (node, key, _old, value) => { node[key] = value }
  })
  const root = { children: [] }
  const app = renderer.createApp({ setup() {
    const { t } = useI18n({ useScope: 'global' })
    return () => h('button', { 'aria-label': t('app.test.save') }, t('app.test.save'))
  } })
  app.use(runtime.i18n)
  app.mount(root)
  assert.equal(root.children[0].text, '保存')
  await runtime.setLocale('en')
  await nextTick()
  assert.equal(root.children[0].text, 'Save')
  assert.equal(root.children[0]['aria-label'], 'Save')
  app.unmount()
})
