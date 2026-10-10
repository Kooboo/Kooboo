export const YlMessage = {
  name: 'YlMessage',
  props: {
    modelValue: { type: Object, default: null },
    duration: { type: Number, default: 3000 },
    closeLabel: { type: String, default: '关闭提示' }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const element = ref(null)
    let timer,
      disposed = false
    const close = () => {
      clearTimeout(timer)
      emit('update:modelValue', null)
    }
    function startTimer() {
      clearTimeout(timer)
      if (props.modelValue && props.modelValue.type !== 'loading' && props.duration > 0) timer = setTimeout(close, props.duration)
    }
    watch(
      [() => props.modelValue, () => props.duration],
      async () => {
        startTimer()
        await nextTick()
        if (disposed || !element.value) return
        // Native manual popover puts feedback above modal dialogs without moving keyboard focus.
        if (props.modelValue) element.value.showPopover?.()
        else element.value.hidePopover?.()
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      clearTimeout(timer)
      element.value?.hidePopover?.()
    })
    return () => {
      const type = ['success', 'error', 'warning', 'info', 'loading'].includes(props.modelValue?.type)
        ? props.modelValue.type
        : 'info'
      return h(
        'div',
        {
          ref: element,
          popover: 'manual',
          class: ['message', `message-${type}`, 'yl-message'],
          style: { display: props.modelValue ? undefined : 'none' },
          role: type === 'error' ? 'alert' : 'status',
          'aria-atomic': 'true',
          onMouseenter: () => clearTimeout(timer),
          onMouseleave: startTimer,
          onFocusin: () => clearTimeout(timer),
          onFocusout: startTimer
        },
        props.modelValue
          ? [
              h(YlIcon, {
                name: {
                  success: 'circle-check',
                  error: 'circle-x',
                  warning: 'triangle-alert',
                  info: 'info', loading: 'loader-circle'
                }[type],
                class: ['message-icon', { 'yl-message-loading': type === 'loading' }],
                size: 20
              }),
              h('span', { class: 'message-content' }, props.modelValue.text),
              h(
                'button',
                {
                  type: 'button',
                  class: 'message-close',
                  'aria-label': props.closeLabel,
                  onClick: close
                },
                h(YlIcon, { name: 'x' })
              )
            ]
          : []
      )
    }
  }
}

// A page composition, not another App Shell. Caller owns querying, sorting and mutations.
