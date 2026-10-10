export const YlPopover = {
  name: 'YlPopover',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    width: { type: Number, default: 320 },
    align: { type: String, default: 'end', validator: oneOf(['start', 'end']) },
    persistent: Boolean,
    id: String
  },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    // Keep the panel out of layout before the first measurement, including initial-open mounts.
    const root = ref(null),
      panel = ref(null),
      position = ref({ position: 'fixed' })
    const generatedId = `yl-popover-${useId()}`
    let disposed = false
    const close = () => { if (!props.persistent) emit('update:modelValue', false) }
    function place() {
      const rect = root.value?.getBoundingClientRect?.()
      if (!rect || typeof window === 'undefined') return
      const width = Math.min(props.width, window.innerWidth - 16)
      const below = Math.max(0, window.innerHeight - rect.bottom - 16)
      const above = Math.max(0, rect.top - 16)
      const height = panel.value?.getBoundingClientRect?.().height || 0
      const up = height > below && above > below
      const available = Math.min(window.innerHeight - 16, up ? above : below)
      position.value = {
        position: 'fixed',
        width: `${width}px`,
        left: `${Math.max(8, Math.min(props.align === 'start' ? rect.left : rect.right - width, window.innerWidth - width - 8))}px`,
        top: `${Math.max(8, up ? rect.top - Math.min(height, available) - 8 : rect.bottom + 8)}px`,
        maxHeight: `${Math.max(0, available)}px`
      }
    }
    function outside(event) {
      if (!root.value?.contains(event.target)) close()
    }
    function listen(enabled) {
      if (typeof window === 'undefined') return
      const method = enabled ? 'addEventListener' : 'removeEventListener'
      document[method]('pointerdown', outside, true)
      window[method]('resize', place)
      window[method]('scroll', place, true)
    }
    watch(
      () => props.modelValue,
      async (open) => {
        listen(open)
        await nextTick()
        if (!disposed && open && props.modelValue) {
          place()
          await nextTick()
          if (!disposed && props.modelValue) { place(); panel.value?.focus({ preventScroll: true }) }
        }
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      listen(false)
    })
    return () =>
      h(
        'div',
        {
          ref: root,
          class: 'yl-popover',
          onFocusout: (event) => {
            if (props.modelValue && !root.value?.contains(event.relatedTarget)) close()
          },
          onKeydown: (event) => {
            if (props.modelValue && !props.persistent && event.key === 'Escape') {
              event.preventDefault()
              event.stopPropagation()
              close()
              root.value?.querySelector?.('button')?.focus()
            }
          }
        },
        [
          slots.trigger?.({
            open: props.modelValue,
            toggle: () => { if (!props.persistent) emit('update:modelValue', !props.modelValue) },
            triggerAttrs: {
              'aria-haspopup': 'dialog',
              'aria-expanded': props.modelValue,
              'aria-controls': props.id || generatedId
            }
          }),
          h(
            'section',
            {
              id: props.id || generatedId,
              ref: panel,
              class: 'dropdown yl-popover-panel',
              style: [position.value, { display: props.modelValue ? undefined : 'none' }],
              role: 'dialog',
              'aria-label': props.title,
              tabindex: -1
            },
            props.modelValue ? slots.default?.({ close }) : null
          )
        ]
      )
  }
}
