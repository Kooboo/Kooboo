export const YlSwitch = {
  name: 'YlSwitch',
  props: {
    modelValue: Boolean,
    disabled: Boolean,
    loading: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['md', 'sm']) },
    label: { type: String, required: true }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    return () =>
      h(
        'button',
        {
          type: 'button',
          role: 'switch',
          'aria-label': props.label,
          'aria-checked': props.modelValue,
          'aria-busy': props.loading || undefined,
          disabled: props.disabled || props.loading,
          class: [
            'switch',
            `switch-${props.size}`,
            { checked: props.modelValue, disabled: props.disabled, loading: props.loading }
          ],
          onClick: () => {
            if (!props.disabled && !props.loading) emit('update:modelValue', !props.modelValue)
          }
        },
        h('span', { class: 'switch-knob', 'aria-hidden': 'true' })
      )
  }
}
