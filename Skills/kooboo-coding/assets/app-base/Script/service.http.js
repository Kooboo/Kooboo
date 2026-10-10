/* kooboo-app-base:v2 */
import { createRequestContext } from '/request-context.js'

const sourceHeaders = {
  screen: 'X-Kooboo-Screen',
  route: 'X-Kooboo-Route',
  source: 'X-Kooboo-Source',
  service: 'X-Kooboo-Service'
}

// Each method receives its own request closure. Its context survives awaits and
// concurrent calls without relying on a global current caller or stack inspection.
export function defineService(moduleUrl, methods) {
  const source = new URL(moduleUrl, window.location.href).pathname
  const unboundMethods = new Set()
  function withContext(context = {}) {
    return Object.freeze(Object.fromEntries(Object.entries(methods).map(([name, handler]) => [
      name,
      (...args) => {
        const snapshot = createRequestContext({
          ...(typeof context === 'function' ? context() : context),
          service: `${source}#${name}`
        })
        if (!snapshot.source && !unboundMethods.has(name)) {
          unboundMethods.add(name)
          console.warn(`[Kooboo request source] ${source}#${name} has no caller. Bind it with useServices in setup, or bindServices outside Vue.`)
        }
        const request = (url, options = {}) => requestJson(url, { ...options, context: snapshot })
        return handler(request, ...args)
      }
    ])))
  }
  return Object.freeze({ ...withContext(), withContext })
}

export async function requestJson(url, options = {}) {
  const { context: sourceContext, ...fetchOptions } = options
  const context = createRequestContext(sourceContext)
  const headers = new Headers(fetchOptions.headers)
  if (!headers.has('Accept')) headers.set('Accept', 'application/json')

  const target = new URL(url, window.location.href)
  // API routes are application-owned and need not use an /api/app/ prefix.
  const isSameOrigin = target.origin === window.location.origin
  for (const [key, name] of Object.entries(sourceHeaders)) {
    // Reserved source headers are managed here, including for external requests.
    headers.delete(name)
    if (isSameOrigin && context[key]) headers.set(name, context[key])
  }

  let body = fetchOptions.body
  if (body !== undefined && body !== null && typeof body !== 'string') {
    if (!headers.has('Content-Type')) headers.set('Content-Type', 'application/json')
    body = JSON.stringify(body)
  }

  const request = Object.freeze({
    method: (fetchOptions.method || 'GET').toUpperCase(),
    path: target.pathname,
    context
  })

  let response
  try {
    response = await fetch(url, {
      credentials: 'same-origin',
      ...fetchOptions,
      headers,
      body
    })
  } catch (error) {
    // Preserve the original error (including AbortError) and add code location hints.
    if (error instanceof Error) error.request = request
    throw error
  }

  const payload = response.status === 204
    ? null
    : await response.json().catch(() => null)

  if (!response.ok) {
    const message = payload?.message || `Request failed with status ${response.status}`
    const error = new Error(message)
    error.status = response.status
    error.payload = payload
    error.request = request
    throw error
  }

  return payload
}
