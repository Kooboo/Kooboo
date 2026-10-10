export const YlTooltip = {
  name: 'YlTooltip',
  props: {
    text: { type: String, required: true },
    placement: {
      type: String,
      default: 'top',
      validator: oneOf(['top', 'bottom', 'left', 'right']),
    },
  },
  setup(props, { slots }) {
    const open = ref(false),
      root = ref(null),
      bubble = ref(null)
    const position = ref({ position: 'fixed', visibility: 'hidden' })
    const id = `yl-tip-${useId()}`
    let hovered = false,
      focused = false,
      disposed = false,
      closeTimer,
      observer
    function place() {
      const anchor = root.value?.getBoundingClientRect?.()
      const tip = bubble.value?.getBoundingClientRect?.()
      if (!anchor || !tip || typeof window === 'undefined') return
      const margin = 8,
        gap = 6
      const space = { top: anchor.top - margin - gap, bottom: window.innerHeight - anchor.bottom - margin - gap,
        left: anchor.left - margin - gap, right: window.innerWidth - anchor.right - margin - gap }
      const opposite = { top: 'bottom', bottom: 'top', left: 'right', right: 'left' }
      let side = props.placement
      const required = side === 'left' || side === 'right' ? tip.width : tip.height
      if (space[side] < required && space[opposite[side]] > space[side]) side = opposite[side]
      const left = side === 'left' ? anchor.left - tip.width - gap : side === 'right' ? anchor.right + gap : anchor.left + (anchor.width - tip.width) / 2
      const top = side === 'top' ? anchor.top - tip.height - gap : side === 'bottom' ? anchor.bottom + gap : anchor.top + (anchor.height - tip.height) / 2
      position.value = { position: 'fixed', visibility: 'visible',
        left: `${Math.max(margin, Math.min(left, window.innerWidth - tip.width - margin))}px`,
        top: `${Math.max(margin, Math.min(top, window.innerHeight - tip.height - margin))}px` }

    }
    function close() {
      clearTimeout(closeTimer)
      open.value = false
    }
    function escape(event) {
      if (open.value && event.key === 'Escape') {
        close()
        event.stopPropagation()
      }
    }
    function listen(enabled) {
      if (typeof window === 'undefined') return
      const method = enabled ? 'addEventListener' : 'removeEventListener'
      window[method]('resize', place)
      window[method]('scroll', place, true)
      document[method]('keydown', escape, true)
    }
    function reveal() {
      clearTimeout(closeTimer)
      if (props.text) open.value = true
    }
    function leave() {
      clearTimeout(closeTimer)
      // Allow crossing the small visual gap to read/hover a long tooltip.
      closeTimer = setTimeout(() => {
        if (!hovered && !focused) close()
      }, 100)
    }
    watch(open, async (value) => {
      listen(value)
      observer?.disconnect()
      if (!value) {
        bubble.value?.hidePopover?.()
        position.value = { position: 'fixed', visibility: 'hidden' }
        return
      }
      await nextTick()
      if (disposed || !open.value) return
      // Native top layer avoids clipping and transformed containing blocks, including dialogs.
      bubble.value?.showPopover?.()
      place()
      if (typeof ResizeObserver !== 'undefined') {
        observer = new ResizeObserver(place)
        observer.observe(root.value)
        observer.observe(bubble.value)
      }
    })
    watch(
      () => [props.text, props.placement],
      async () => {
        if (!props.text) close()
        else if (open.value) {
          await nextTick()
          if (!disposed && open.value) place()
        }
      }
    )
    onBeforeUnmount(() => {
      disposed = true
      clearTimeout(closeTimer)
      observer?.disconnect()
      listen(false)
      bubble.value?.hidePopover?.()
    })
    return () =>
      h(
        'span',
        {
          ref: root,
          class: 'yl-tooltip',
          onMouseenter: () => {
            hovered = true
            reveal()
          },
          onMouseleave: () => {
            hovered = false
            leave()
          },
          onFocusin: () => {
            focused = true
            reveal()
          },
          onFocusout: (event) => {
            if (root.value?.contains(event.relatedTarget)) return
            focused = false
            if (!hovered) close()
          },
          onKeydown: escape,
        },
        [
          slots.default?.({ 'aria-describedby': props.text ? id : undefined }),
          h(
            'span',
            {
              id,
              ref: bubble,
              role: 'tooltip',
              popover: 'manual',
              class: 'tooltip-bubble yl-tooltip-bubble',
              hidden: !open.value,
              style: position.value,
              onMouseenter: () => {
                hovered = true
                reveal()
              },
              onMouseleave: () => {
                hovered = false
                leave()
              },
            },
            props.text
          ),
        ]
      )
  },
}
