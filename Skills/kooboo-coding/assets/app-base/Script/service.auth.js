/* kooboo-app-base:v2 */
import { defineService } from '/service.http.js'

export const authService = defineService(import.meta.url, {
  async getCurrentUser(request) {
    try {
      const response = await request('/api/app/user')
      if (!response || response.success !== true || !response.data) throw new Error(response?.message || 'Invalid user response')
      return response.data
    } catch (error) {
      if (error.status === 401 && error.payload?.code === 'UNAUTHENTICATED') return null
      throw error
    }
  },

  async updateCurrentUser(request, displayName) {
    const response = await request('/api/app/user', { method: 'PUT', body: { displayName } })
    if (!response || response.success !== true) throw new Error(response?.message || 'Unable to update user')
    return response.data
  },

  async logout(request, returnUrl = window.location.pathname) {
    // Vue may pass a click event when the template binds @click="logout" directly.
    const destination = typeof returnUrl === 'string' && returnUrl.startsWith('/') && !returnUrl.startsWith('//')
      ? returnUrl
      : window.location.pathname
    const loginUrl = buildLoginUrl(destination)

    try {
      await request('/api/app/user/logout', { method: 'POST' })
    } finally {
      window.location.assign(loginUrl)
    }
  }
})

// Preserve the original named imports for callers that do not yet bind a source.
export const { getCurrentUser, updateCurrentUser, logout } = authService

export function buildLoginUrl(
  returnUrl = `${window.location.pathname}${window.location.search}${window.location.hash}`
) {
  const params = new URLSearchParams({ returnurl: returnUrl })
  return `/_Admin/login?${params.toString()}`
}

export function buildRouteReturnUrl(fullPath) {
  const routeHash = fullPath.startsWith('/') ? `#${fullPath}` : window.location.hash
  return `${window.location.pathname}${window.location.search}${routeHash}`
}
