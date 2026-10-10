export const YlDatePicker = {
  name: 'YlDatePicker',
  inheritAttrs: false,
  props: {
    modelValue: { type: String, default: '' },
    mode: { type: String, default: 'date', validator: oneOf(['date', 'month', 'datetime-local']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    min: String, max: String,
    disabled: Boolean, readonly: Boolean, invalid: Boolean, clearable: Boolean,
    clearLabel: { type: String, default: '清除日期' }
  },
  emits: ['update:modelValue', 'change'],
  setup(props, { attrs, emit }) {
    const update = value => {
      if (props.disabled || props.readonly) return
      emit('update:modelValue', value)
      emit('change', value)
    }
    return () => {
      const { class: cls, style, ...inputAttrs } = attrs
      const canClear = props.clearable && props.modelValue && !props.disabled && !props.readonly
      return h('div', { class: ['yl-date-picker', { 'yl-date-picker-clearable': canClear }, cls], style,
        onClick: event => {
          if (canClear && event.target?.tagName === 'INPUT') {
            try { event.target.showPicker?.() } catch { /* Native editing remains available when picker access is unavailable. */ }
          }
        } }, [
        h(YlInput, { ...inputAttrs, type: props.mode, modelValue: props.modelValue,
          size: props.size, min: props.min, max: props.max, disabled: props.disabled,
          readonly: props.readonly, invalid: props.invalid, 'onUpdate:modelValue': update }),
        canClear
          ? h('button', { type: 'button', class: 'yl-date-clear', 'aria-label': props.clearLabel,
              onClick: () => update('') }, h(YlIcon, { name: 'x', size: 14 })) : null
      ])
    }
  }
}
