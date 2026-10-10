export const YlCheckboxGroup = {
  name: 'YlCheckboxGroup',
  props: { modelValue: { type: Array, default: () => [] }, options: { type: Array, default: () => [] },
    disabled: Boolean, invalid: Boolean, direction: { type: String, default: 'horizontal', validator: oneOf(['horizontal', 'vertical']) },
    label: { type: String, required: true } },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const update = (item, checked) => {
      if (props.disabled || item.disabled) return
      emit('update:modelValue', checked ? [...new Set([...props.modelValue, item.value])] : props.modelValue.filter(value => value !== item.value))
    }
    return () => h('div', { class: ['yl-checkbox-group', `yl-checkbox-group-${props.direction}`], role: 'group', 'aria-label': props.label, 'aria-invalid': props.invalid || undefined },
      props.options.map(item => h(YlCheckbox, { key: item.value, modelValue: props.modelValue.includes(item.value),
        disabled: props.disabled || item.disabled, invalid: props.invalid, 'onUpdate:modelValue': checked => update(item, checked) }, () => item.label)))
  }
}
