export const YlCheckbox = {
  name: 'YlCheckbox',
  inheritAttrs: false,
  props: { modelValue: Boolean, disabled: Boolean, indeterminate: Boolean, invalid: Boolean },
  emits: ['update:modelValue'],
  setup(props, { slots, attrs, emit }) {
    return () =>
      h(
        'label',
        {
          class: [
            'checkbox',
            'yl-checkbox',
            {
              checked: props.modelValue,
              disabled: props.disabled,
              indeterminate: props.indeterminate,
              invalid: props.invalid
            }
          ]
        },
        [
          h('input', {
            ...attrs,
            type: 'checkbox',
            class: 'yl-checkbox-native',
            checked: props.modelValue,
            indeterminate: props.indeterminate,
            disabled: props.disabled,
            'aria-invalid': props.invalid || undefined,
            onChange: (event) => {
              if (!props.disabled) emit('update:modelValue', event.target.checked)
            }
          }),
          h(
            'span',
            { class: 'checkbox-box', 'aria-hidden': 'true' },
            props.indeterminate
              ? h('span', { class: 'checkbox-dash' })
              : props.modelValue
              ? h(YlIcon, { name: 'check', size: 10 })
              : null
          ),
          slots.default?.()
        ]
      )
  }
}
