export const YlStatusTag = {
  name: 'YlStatusTag',
  props: {
    type: {
      type: String,
      default: 'stop',
      validator: oneOf(['success', 'warning', 'processing', 'stop', 'error', 'loading', 'waiting'])
    },
    filled: Boolean,
    variant: { type: String, validator: oneOf(['text', 'fill', 'outline']) }
  },
  setup(props, { slots }) {
    return () =>
      h(
        'span',
        {
          class: [
            'status-tag',
            `status-tag-${props.type}`,
            props.variant ? `status-tag-${props.variant}` : props.filled ? 'status-tag-fill' : 'status-tag-text'
          ]
        },
        [h('span', { class: ['yl-status-dot', { 'yl-status-loading': props.type === 'loading' }], 'aria-hidden': 'true' }), slots.default?.()]
      )
  }
}
