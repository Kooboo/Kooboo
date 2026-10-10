export const YlIcon = {
  name: 'YlIcon',
  props: { name: { type: String, required: true }, size: { type: Number, default: 16 } },
  setup(props) {
    const remote = ref(null)
    let disposed = false
    onBeforeUnmount(() => {
      disposed = true
    })
    watch(
      () => props.name,
      async (name) => {
        remote.value = null
        if (bundledIcons[name]) return
        try {
          const lucide = await loadLucide()
          if (!disposed && name === props.name) {
            remote.value =
              lucide.icons[
                name.replace(/(^|-)(\w)/g, (_, prefix, letter) => letter.toUpperCase())
              ] || null
            if (!remote.value) console.warn(`Unknown Lucide icon: ${name}`)
          }
        } catch (error) {
          if (!disposed) console.warn('Lucide icon unavailable', error)
        }
      },
      { immediate: true }
    )
    // Vue owns the SVG tree. Do not use data-lucide DOM replacement inside components.
    const renderNode = ([tag, attributes, children]) =>
      h(tag, attributes, children?.map(renderNode))
    return () => {
      const node = bundledIcons[props.name] || remote.value
      return h(
        'svg',
        {
          ...(node?.[1] || { viewBox: '0 0 24 24' }),
          class: 'yl-icon',
          width: props.size,
          height: props.size,
          'aria-hidden': 'true',
          focusable: 'false'
        },
        node?.[2]?.map(renderNode)
      )
    }
  }
}
