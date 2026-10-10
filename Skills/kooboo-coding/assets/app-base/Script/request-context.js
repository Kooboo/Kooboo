/* kooboo-app-base:v2 */
import { useRoute } from 'vue-router'

// Keep only bounded, printable ASCII identifiers. Never include URLs with query data.
export function createRequestContext(values = {}) {
  const context = {}
  for (const key of ['screen', 'route', 'source', 'service']) {
    const value = values[key]
    if (typeof value === 'string' && value.length > 0 && value.length <= 256 && /^[\x21-\x7e]+$/.test(value)) {
      context[key] = value
    }
  }
  return Object.freeze(context)
}

// Call in setup. Screens retain their identity after navigation; persistent layouts
// opt into the current route. Capture before the first await and reuse for retries.
export function useRequestContext(source, { currentRoute = false } = {}) {
  const route = useRoute()
  const readRoute = () => ({ screen: route.meta?.screenId || route.name, route: route.name })
  const ownerRoute = readRoute()
  return {
    capture() {
      return createRequestContext({
        ...(currentRoute ? readRoute() : ownerRoute),
        source: new URL(source, window.location.href).pathname
      })
    }
  }
}

// Bind once in setup; ordinary service calls then capture their source automatically.
export function useServices(moduleUrl, services, options) {
  const context = useRequestContext(moduleUrl, options)
  return bindServices(services, () => context.capture())
}

// Also usable outside Vue with a fixed context (bootstrap/background jobs).
export function bindServices(services, context) {
  return Object.freeze(Object.fromEntries(Object.entries(services).map(([name, service]) => [
    name, service.withContext(context)
  ])))
}
