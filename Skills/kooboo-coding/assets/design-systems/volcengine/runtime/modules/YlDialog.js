export const YlDialog = {
  name: 'YlDialog',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    size: { type: String, default: 'md', validator: oneOf(['sm', 'md', 'lg']) },
    busy: Boolean,
    closeLabel: { type: String, default: '关闭' }
  },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    const dialog = ref(null)
    const titleId = `yl-dialog-${useId()}`
    let opener = null
    let disposed = false
    function requestClose() {
      if (!props.busy) emit('update:modelValue', false)
    }
    watch(
      () => props.modelValue,
      async (open) => {
        await nextTick()
        if (disposed || !dialog.value || open !== props.modelValue) return
        if (open && !dialog.value.open) {
          opener = document.activeElement
          dialog.value.showModal()
        } else if (!open && dialog.value.open) {
          dialog.value.close()
          if (opener?.isConnected) opener.focus()
        }
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      if (dialog.value?.open) {
        dialog.value.close()
        if (opener?.isConnected) opener.focus()
      }
    })
    return () =>
      h(
        'dialog',
        {
          ref: dialog,
          class: ['modal', 'modal-standard', `modal-${props.size}`, 'yl-dialog'],
          'aria-labelledby': titleId,
          'aria-busy': props.busy || undefined,
          onCancel: (event) => {
            event.preventDefault()
            requestClose()
          },
          onClose: () => {
            if (!disposed && props.modelValue && !dialog.value?.open) requestClose()
          }
        },
        [
          h('div', { class: 'modal-header' }, [
            h('h2', { id: titleId, class: 'modal-title' }, props.title),
            h(
              'button',
              {
                type: 'button',
                class: 'modal-close',
                'aria-label': props.closeLabel,
                title: props.closeLabel,
                disabled: props.busy,
                onClick: requestClose
              },
              h(YlIcon, { name: 'x', size: 20 })
            )
          ]),
          h('div', { class: 'modal-body' }, slots.default?.()),
          slots.footer
            ? h('div', { class: 'modal-footer' }, slots.footer({ close: requestClose }))
            : null
        ]
      )
  }
}
