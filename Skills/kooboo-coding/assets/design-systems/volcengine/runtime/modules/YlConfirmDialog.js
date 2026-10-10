export const YlConfirmDialog = {
  name: 'YlConfirmDialog',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    description: { type: String, required: true },
    busy: Boolean,
    danger: Boolean,
    tone: { type: String, default: 'warning', validator: oneOf(['info', 'success', 'warning', 'error']) },
    showCancel: { type: Boolean, default: true },
    confirmLabel: { type: String, default: '确认' },
    cancelLabel: { type: String, default: '取消' }
  },
  emits: ['update:modelValue', 'confirm'],
  setup(props, { emit }) {
    const close = () => {
      if (!props.busy) emit('update:modelValue', false)
    }
    return () =>
      h(
        YlDialog,
        {
          modelValue: props.modelValue,
          title: props.title,
          busy: props.busy,
          size: 'sm',
          'onUpdate:modelValue': close
        },
        {
          default: () =>
            h('div', { class: 'yl-confirm-content' }, [
              h(YlIcon, { name: { info: 'info', success: 'circle-check', warning: 'triangle-alert', error: 'circle-x' }[props.danger ? 'error' : props.tone], class: `yl-confirm-${props.danger ? 'error' : props.tone}`, size: 20 }),
              h('p', props.description)
            ]),
          footer: () => [
            props.showCancel ? h(
              YlButton,
              { autofocus: true, disabled: props.busy, onClick: close },
              () => props.cancelLabel
            ) : null,
            h(
              YlButton,
              {
                variant: props.danger ? 'danger' : 'primary',
                loading: props.busy,
                onClick: () => {
                  if (!props.busy) emit('confirm')
                }
              },
              () => props.confirmLabel
            )
          ]
        }
      )
  }
}
