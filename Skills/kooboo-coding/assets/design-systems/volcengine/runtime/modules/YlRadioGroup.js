export const YlRadioGroup = {
  name: 'YlRadioGroup',
  props: {
    modelValue: { type: [String, Number], default: null },
    options: { type: Array, default: () => [] },
    disabled: Boolean,
    label: { type: String, required: true },
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const name = `yl-radio-${useId()}`
    return () =>
      h('fieldset', { class: 'yl-radio-group', disabled: props.disabled }, [
        h('legend', props.label),
        h(
          'div',
          { class: 'radio-group' },
          props.options.map((item) =>
            h(
              'label',
              {
                class: ['radio', { disabled: props.disabled || item.disabled }],
                key: item.value,
              },
              [
                h('input', {
                  type: 'radio',
                  name,
                  value: item.value,
                  checked: props.modelValue === item.value,
                  disabled: props.disabled || item.disabled,
                  onChange: () => {
                    if (!props.disabled && !item.disabled)
                      emit('update:modelValue', item.value)
                  },
                }),
                item.label,
              ]
            )
          )
        ),
      ])
  },
}
