export const YlAlert = {
  name: 'YlAlert',
  props: {
    title: { type: String, required: true },
    description: String,
    type: {
      type: String,
      default: 'info',
      validator: oneOf(['info', 'success', 'warning', 'error'])
    },
    closable: Boolean,
    banner: Boolean,
    closeLabel: { type: String, default: '关闭提示' }
  },
  emits: ['close'],
  setup(props, { emit, slots }) {
    return () =>
      h(
        'div',
        {
          class: ['alert', `alert-${props.type}`, { 'alert-with-desc': !!props.description, 'alert-banner': props.banner }],
          role: props.type === 'error' ? 'alert' : 'status'
        },
        [
          h(YlIcon, {
            name: {
              info: 'info',
              success: 'circle-check',
              warning: 'triangle-alert',
              error: 'circle-x'
            }[props.type],
            class: 'alert-icon',
            size: 20
          }),
          h('div', { class: 'alert-content' }, [
            h('span', { class: 'alert-title' }, props.title),
            props.description ? h('p', { class: 'alert-desc' }, props.description) : null,
            slots.default?.()
          ]),
          props.closable
            ? h(
                'button',
                {
                  type: 'button',
                  class: 'alert-close',
                  'aria-label': props.closeLabel,
                  onClick: () => emit('close')
                },
                h(YlIcon, { name: 'x' })
              )
            : null
        ]
      )
  }
}
