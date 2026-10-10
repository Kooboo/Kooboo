export const YlButton = {
  name: 'YlButton',
  props: {
    variant: {
      type: String,
      default: 'secondary',
      validator: oneOf(['primary', 'secondary', 'outline', 'text', 'danger', 'success', 'warning', 'dashed'])
    },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    loading: Boolean,
    disabled: Boolean,
    icon: String,
    suffixIcon: String,
    type: { type: String, default: 'button', validator: oneOf(['button', 'submit', 'reset']) }
  },
  emits: ['click'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        'button',
        {
          type: props.type,
          class: [
            'btn',
            `btn-${props.variant}`,
            `btn-${props.size}`,
            { disabled: props.disabled || props.loading }
          ],
          disabled: props.disabled || props.loading,
          'aria-busy': props.loading || undefined,
          onClick: (event) => {
            if (!props.disabled && !props.loading) emit('click', event)
          }
        },
        [
          props.loading
            ? h('span', { class: 'yl-spinner', 'aria-hidden': 'true' })
            : props.icon
            ? h(YlIcon, { name: props.icon })
            : null,
          slots.default?.(),
          props.suffixIcon ? h(YlIcon, { name: props.suffixIcon }) : null
        ]
      )
  }
}
