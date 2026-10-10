/* kooboo-app-base:v2; object-type: API */
// Route: /api/app/user/{action}. Named operations go before base callbacks.
import { getCurrentAppUser, updateCurrentAppUser, getCurrentIdentity } from './Services.user'
import { successResponse, errorResponse } from './Utils.response'

k.api.post('logout', () => {
  /* mock:start */
  // Preview logout must not clear a real Kooboo session in the same browser.
  if (getCurrentIdentity()?.isMock) return successResponse()

  /* mock:end */
  k.cookie.remove('_kooboo_api_user')
  k.cookie.remove('kooboo_redirect_url')
  k.account.logout()

  return successResponse()
})

k.api.get('/', () => {
  const user = getCurrentAppUser()
  if (!user) {
    return errorResponse(401, 'UNAUTHENTICATED', '请先登录')
  }

  return successResponse(user)
})

k.api.put('/', (body: unknown) => {
  const user = getCurrentAppUser()
  if (!user) {
    return errorResponse(401, 'UNAUTHENTICATED', '请先登录')
  }

  // Explicit writable fields: account IDs, roles and account profile are not client-writable.
  if (!body || typeof body !== 'object' || Array.isArray(body)) {
    return errorResponse(400, 'INVALID_ARGUMENT', '请求体须为对象')
  }

  if (Object.keys(body).some(key => key !== 'displayName')) {
    return errorResponse(400, 'INVALID_ARGUMENT', '仅允许修改 displayName')
  }

  if (!('displayName' in body) || typeof body.displayName !== 'string') {
    return errorResponse(400, 'INVALID_ARGUMENT', 'displayName 须为字符串')
  }

  const displayName = body.displayName.trim()
  if (!displayName || displayName.length > 80) {
    return errorResponse(400, 'INVALID_ARGUMENT', '显示名称须为 1–80 个字符')
  }

  const result = updateCurrentAppUser(user, { displayName })

  return successResponse(result)
})

k.api.get(() => k.api.notFound())
