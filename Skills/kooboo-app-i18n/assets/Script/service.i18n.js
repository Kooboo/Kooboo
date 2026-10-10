/* kooboo-app-i18n:v1; application-owned */
import { createI18n } from 'vue-i18n'
import { watch } from 'vue'

// Filled by install-runtime.js so preferences can be read before the first fetch.
const installedSiteId = null
const own = (object, key) => Object.prototype.hasOwnProperty.call(object, key)
const labelMap = messages => Object.assign(Object.create(null), messages)

// A flat resolver preserves dots in Kooboo Label names and disables implicit
// nested-key/prototype lookup. Locale fallback is explicitly default-only.
function resolveLabel(messages, key) {
  return own(messages, key) && typeof messages[key] === 'string' ? messages[key] : null
}

export async function createAppI18n({
  siteId = installedSiteId,
  endpoint = '/api/app-i18n',
  fetcher = globalThis.fetch.bind(globalThis),
  storage,
  url = globalThis.location?.href || 'http://localhost/',
  document: page = globalThis.document,
  history = globalThis.history,
  prepareLocale = async () => {},
  commitLocale = () => {}
} = {}) {
  // Accessing localStorage itself can throw in restricted browser contexts.
  if (storage === undefined) {
    try { storage = globalThis.localStorage } catch { storage = null }
  }
  const read = key => { try { return storage?.getItem(key) } catch { return null } }
  const write = (key, value) => { try { storage?.setItem(key, value) } catch { /* optional preference */ } }
  const endpointUrl = new URL(endpoint, url)
  if (endpointUrl.origin !== new URL(url).origin) throw new Error('App i18n requires a same-origin endpoint')
  const pending = new Map()
  const cache = new Map()
  let metadata

  async function load(locale) {
    const key = locale || ''
    if (cache.has(key)) return cache.get(key)
    if (pending.has(key)) return pending.get(key)
    const request = (async () => {
      const target = new URL(endpointUrl)
      if (locale) target.searchParams.set('locale', locale)
      else {
        const preferred = new URL(url).searchParams.get('lang')
        const saved = siteId ? read(`kooboo:app-i18n:${siteId}`) : null
        if (preferred) target.searchParams.set('preferredLocale', preferred)
        if (saved) target.searchParams.set('savedLocale', saved)
      }
      const response = await fetcher(target.href, { credentials: 'same-origin', cache: 'no-store', headers: { Accept: 'application/json' } })
      if (!response.ok) throw new Error(`Unable to load application language (${response.status})`)
      const payload = await response.json()
      if (!payload || typeof payload.siteId !== 'string' || !payload.siteId ||
          !Array.isArray(payload.supportedLocales) || !payload.supportedLocales.length ||
          payload.supportedLocales.some(item => typeof item !== 'string' || !/^[A-Za-z]{2,8}(?:-[A-Za-z0-9]{1,8})*$/.test(item)) ||
          !payload.supportedLocales.includes(payload.locale) || !payload.supportedLocales.includes(payload.defaultLocale) ||
          !payload.supportedLocales.includes(payload.currentLocale) || (locale && payload.locale !== locale) ||
          !payload.messages || typeof payload.messages !== 'object') throw new Error('Invalid application language response')
      for (const language of new Set([payload.locale, payload.defaultLocale])) {
        const entries = payload.messages[language]
        if (!entries || Array.isArray(entries) || typeof entries !== 'object' ||
            Object.values(entries).some(value => typeof value !== 'string')) throw new Error('Invalid application messages')
      }
      if ((siteId && siteId !== payload.siteId) || (metadata && (metadata.siteId !== payload.siteId || metadata.defaultLocale !== payload.defaultLocale ||
          JSON.stringify(metadata.supportedLocales) !== JSON.stringify(payload.supportedLocales)))) {
        throw new Error('Application language settings changed; reload the page')
      }
      // The default pack has already arrived too; switching to it needs no fetch.
      for (const language of new Set([payload.locale, payload.defaultLocale])) {
        cache.set(language, { ...payload, locale: language, messages: {
          [language]: payload.messages[language],
          [payload.defaultLocale]: payload.messages[payload.defaultLocale]
        } })
      }
      return payload
    })()
    pending.set(key, request)
    try { return await request } finally { pending.delete(key) }
  }

  metadata = await load()
  const canonical = value => typeof value === 'string'
    ? metadata.supportedLocales.find(item => item.toLowerCase() === value.toLowerCase())
    : undefined
  const storageKey = `kooboo:app-i18n:${metadata.siteId}`
  const locale = canonical(new URL(url).searchParams.get('lang')) || canonical(read(storageKey)) || metadata.currentLocale
  const initial = locale === metadata.locale ? metadata : await load(locale)
  const prepared = await prepareLocale(locale)
  const i18n = createI18n({
    legacy: false,
    locale,
    fallbackLocale: Object.fromEntries([
      ...metadata.supportedLocales.map(language => [language, language === metadata.defaultLocale ? [] : [metadata.defaultLocale + '!']]),
      ['default', [metadata.defaultLocale + '!']]
    ]),
    messageResolver: resolveLabel,
    messages: Object.fromEntries([...new Set([locale, metadata.defaultLocale])].map(language => [language, labelMap(initial.messages[language])])),
    missingWarn: false,
    fallbackWarn: false
  })
  // Do not retain maps for unrequested languages supplied by a server extension.
  commitLocale(locale, prepared)
  if (page?.documentElement) page.documentElement.lang = locale
  let generation = 0

  async function setLocale(value) {
    const next = canonical(value)
    if (!next) throw new Error('Unsupported application language')
    const ticket = ++generation
    const [payload, prepared] = await Promise.all([load(next), prepareLocale(next)])
    if (ticket !== generation) return false
    // Consumer commit hooks must be synchronous and cannot fail after mutating UI.
    commitLocale(next, prepared)
    for (const language of new Set([next, metadata.defaultLocale])) {
      i18n.global.setLocaleMessage(language, labelMap(payload.messages[language]))
    }
    i18n.global.locale.value = next
    if (page?.documentElement) page.documentElement.lang = next
    write(storageKey, next)
    // An explicit URL selection has priority on reload. Keep it in sync without
    // touching Vue Router's hash or its history state.
    try {
      const current = new URL(globalThis.location?.href || url)
      if (current.searchParams.has('lang')) {
        current.searchParams.set('lang', next)
        history?.replaceState(history.state, '', current.href)
      }
    } catch { /* language preference still works without History access */ }
    return true
  }

  return { i18n, locale: i18n.global.locale, supportedLocales: [...metadata.supportedLocales], setLocale }
}

// Register once per App; retain the returned stop handle if the App is unmounted.
export function bindTranslatedTitle(router, i18n, { appTitleKey, appTitle = '', document: page = globalThis.document } = {}) {
  const update = () => {
    if (!page) return
    const meta = router.currentRoute.value.meta
    const translate = (key, fallback) => key && (i18n.global.te(key) || i18n.global.te(key, i18n.global.fallbackLocale.value.default[0].replace(/!$/, '')))
      ? i18n.global.t(key) : fallback
    const title = translate(meta.titleKey, meta.title || '')
    const name = translate(appTitleKey, appTitle)
    page.title = [title, name].filter(Boolean).join(' · ')
  }
  const stop = watch([() => router.currentRoute.value.meta, i18n.global.locale], update, { immediate: true, flush: 'sync' })
  // Register after createAppRouter's legacy title hook so it cannot overwrite us.
  const removeHook = router.afterEach?.(update)
  return () => { stop(); removeHook?.() }
}
