/* kooboo-app-i18n:v1; application-owned; route: /api/app-i18n/{action} */
import { getAppMessages } from './Services.i18n'

k.api.get('/', () => {
  k.response.setHeader('Cache-Control', 'no-store')
  const query = k.request.queryString
  const result = getAppMessages(query.locale, query.preferredLocale, query.savedLocale)
  if (!result) {
    k.response.statusCode(400)
    return { code: 'UNSUPPORTED_LOCALE', message: 'Unsupported application language' }
  }
  return result
})
k.api.get(() => k.api.notFound())
