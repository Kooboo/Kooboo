/* kooboo-app-i18n:v1; application-owned */
import { ENV } from './ENV'

// Deliberately read an exact existing culture. Label's ordinary GetValue fallback
// can return an unrelated language and must not manufacture translated entries.
export function getAppMessages(requestedLocale, preferredLocale, savedLocale) {
  const site = k.site.webSite
  const defaultLocale = String(site.defaultCulture)
  const { supportedLocales, prefixes } = ENV.i18n || {}
  const siteCultures = Object.keys(site.culture)
  if (!Array.isArray(supportedLocales) || !supportedLocales.length || supportedLocales.length > 30 ||
      supportedLocales.some(value => typeof value !== 'string' || !siteCultures.includes(value)) ||
      new Set(supportedLocales.map(value => value.toLowerCase())).size !== supportedLocales.length) {
    throw new Error('ENV.i18n.supportedLocales must contain unique configured site cultures')
  }
  if (!Array.isArray(prefixes) || !prefixes.length || prefixes.length > 20 ||
      prefixes.some(value => typeof value !== 'string' || !/^app\.[a-zA-Z0-9_.-]{1,55}\.$/.test(value))) {
    throw new Error('ENV.i18n.prefixes must contain explicit application namespaces')
  }
  const canonical = value => typeof value === 'string'
    ? supportedLocales.find(item => item.toLowerCase() === value.toLowerCase())
    : undefined
  if (!canonical(defaultLocale)) throw new Error('App i18n must include the site default culture')
  const fallback = canonical(defaultLocale)
  const currentLocale = canonical(k.request.culture) || fallback
  const locale = requestedLocale == null || requestedLocale === ''
    ? canonical(preferredLocale) || canonical(savedLocale) || currentLocale
    : canonical(requestedLocale)
  if (!locale) return null

  const messages = {}
  for (const culture of new Set([locale, fallback])) messages[culture] = {}
  for (const label of k.site.labels.all()) {
    const name = String(label.name)
    if (!prefixes.some(prefix => name.startsWith(prefix))) continue
    for (const culture of Object.keys(messages)) {
      const exact = Array.from(label.keys).find(key => String(key).toLowerCase() === culture.toLowerCase())
      if (exact === undefined) continue
      const value = label.getCultureValue(String(exact))
      if (typeof value === 'string' && value.length) messages[culture][name] = value
    }
  }
  return { siteId: String(site.id), locale, currentLocale, defaultLocale: fallback, supportedLocales, messages }
}
