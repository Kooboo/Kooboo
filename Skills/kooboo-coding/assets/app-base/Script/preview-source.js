/* kooboo-app-base:v2; role: preview-source */

// Vue 3 adapter. Keep all VNode/instance internals here, never in business components.
// The parent preview bridge transports only the bounded plain-data result.
export function installPreviewSource(app, router) {
  if (window.parent === window || new URLSearchParams(window.location.search).get('kooboobridge') !== 'true') return

  let root
  const text = value => typeof value === 'string' && value ? value.slice(0, 500) : undefined

  function describe(type, resolveAsync = false) {
    if (!type) return undefined
    if (resolveAsync) type = type.__asyncResolved || type
    const name = text(type.name || type.__name)
    let moduleUrl
    try {
      const value = type.koobooSource?.moduleUrl
      if (typeof value === 'string') {
        const url = new URL(value)
        if (url.origin === window.location.origin && /^https?:$/.test(url.protocol) && !url.username && !url.password) {
          url.search = ''
          url.hash = ''
          if (url.href.length <= 2048) moduleUrl = url.href
        }
      }
    } catch { /* Missing/invalid metadata is not a file identity. */ }
    return name || moduleUrl ? { ...(name ? { name } : {}), ...(moduleUrl ? { moduleUrl } : {}) } : undefined
  }

  function chain(instance) {
    const result = []
    for (let current = instance, depth = 0; current && depth < 32; current = current.parent, depth++) {
      const source = describe(current.type)
      if (source) result.unshift(source)
    }
    return result
  }

  function routeContext() {
    const current = router?.currentRoute?.value
    if (!current) return undefined
    const record = current.matched?.[current.matched.length - 1]
    const component = describe(record?.components?.default, true)
    return {
      path: text(current.fullPath) || '/',
      ...(text(current.meta?.title) ? { title: text(current.meta.title) } : {}),
      ...(text(current.meta?.screenId) ? { screenId: text(current.meta.screenId) } : {}),
      ...(component ? { component } : {})
    }
  }

  function resolveElement(element) {
    if (!root || root.isUnmounted || !element?.isConnected) return undefined
    // Include a bounded ancestor fallback for DOM inserted by v-html/directives.
    const ancestors = new Map()
    for (let node = element, distance = 0; node && distance < 64; node = node.parentElement, distance++) {
      ancestors.set(node, distance)
    }
    const pending = [{ vnode: root.subTree, renderer: root }]
    const visited = new Set()
    let match
    let count = 0
    while (pending.length) {
      if (++count > 50000) return undefined
      const { vnode, renderer } = pending.pop()
      if (!vnode || typeof vnode !== 'object' || visited.has(vnode)) continue
      visited.add(vnode)
      if (vnode.component) {
        const instance = vnode.component
        if (!instance.isUnmounted && !instance.isDeactivated) {
          pending.push({ vnode: instance.subTree, renderer: instance })
        }
        continue
      }
      if (vnode.suspense) {
        pending.push({ vnode: vnode.suspense.activeBranch, renderer })
        continue
      }
      // Match host elements only; a component/Fragment/Teleport can share an el.
      if (vnode.shapeFlag & 1) {
        const distance = ancestors.get(vnode.el)
        if (distance !== undefined && (!match || distance < match.distance)) {
          match = { renderer, declaredBy: vnode.ctx, distance }
          if (distance === 0) break
        }
      }
      // The compiler may collapse static HTML into a VNode spanning sibling DOM
      // roots. Those nodes have no child VNodes, but still belong to this renderer.
      if (vnode.staticCount > 0) {
        for (let node = vnode.el; node; node = node.nextSibling) {
          if (++count > 50000) return undefined
          const distance = ancestors.get(node)
          if (distance !== undefined && (!match || distance < match.distance)) {
            match = { renderer, declaredBy: vnode.ctx, distance }
          }
          if (node === vnode.anchor) break
        }
      }
      if (Array.isArray(vnode.children)) {
        for (const child of vnode.children) pending.push({ vnode: child, renderer })
      }
    }
    const route = routeContext()
    if (!match) return route ? { framework: 'vue', route, componentChain: [] } : undefined
    const renderedBy = describe(match.renderer?.type)
    const declaredBy = describe(match.declaredBy?.type)
    return {
      framework: 'vue',
      ...(route ? { route } : {}),
      match: match.distance === 0 ? 'element' : 'ancestor',
      componentChain: chain(match.renderer),
      ...(renderedBy ? { renderedBy } : {}),
      ...(declaredBy ? { declaredBy } : {})
    }
  }

  const bridge = {
    version: 1,
    resolveElement(element) {
      try { return resolveElement(element) } catch { return undefined }
    }
  }
  app.mixin({
    beforeMount() {
      root = this.$.root
    },
    unmounted() {
      if (this.$ === root && window.__koobooPreviewSource === bridge) {
        delete window.__koobooPreviewSource
        root = undefined
      }
    }
  })
  window.__koobooPreviewSource = bridge
}
