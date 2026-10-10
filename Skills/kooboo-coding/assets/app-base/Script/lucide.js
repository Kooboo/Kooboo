/* kooboo-app-base:v2 */
const LUCIDE_CDN_URL = 'https://unpkg.com/lucide@0.468.0/dist/umd/lucide.min.js'
const PENDING_ICON_SELECTOR = '[data-lucide]:not(svg)'

let loadingPromise
let iconObserver
let renderFrame

export async function loadLucide() {
  if (window.lucide) return Promise.resolve(window.lucide)

  if (!loadingPromise) {
    loadingPromise = new Promise((resolve, reject) => {
      const script = document.createElement('script')
      script.src = LUCIDE_CDN_URL
      script.async = true
      script.onload = () => window.lucide
        ? resolve(window.lucide)
        : reject(new Error('Lucide loaded without exposing its browser API.'))
      script.onerror = () => reject(new Error('Lucide icon library could not be loaded.'))
      document.head.appendChild(script)
    })
  }

  try {
    return await loadingPromise
  } catch (error) {
    loadingPromise = undefined
    throw error
  }
}

export async function renderLucideIcons(options) {
  if (!document.querySelector(PENDING_ICON_SELECTOR)) return

  const lucide = await loadLucide()
  lucide.createIcons(options)
}

function containsPendingIcon(node) {
  if (!(node instanceof Element)) return false
  return node.matches(PENDING_ICON_SELECTOR) || Boolean(node.querySelector(PENDING_ICON_SELECTOR))
}

function scheduleIconRender() {
  if (renderFrame !== undefined) return

  renderFrame = requestAnimationFrame(async () => {
    renderFrame = undefined
    try {
      await renderLucideIcons()
    } catch (error) {
      console.warn('Lucide icons could not be rendered', error)
    }
  })
}

export async function observeLucideIcons(root = document.body) {
  iconObserver?.disconnect()
  iconObserver = new MutationObserver(mutations => {
    const hasNewIcons = mutations.some(mutation =>
      mutation.type === 'attributes'
        ? containsPendingIcon(mutation.target)
        : [...mutation.addedNodes].some(containsPendingIcon)
    )

    if (hasNewIcons) scheduleIconRender()
  })
  iconObserver.observe(root, {
    childList: true,
    subtree: true,
    attributes: true,
    attributeFilter: ['data-lucide']
  })

  await renderLucideIcons()
}
