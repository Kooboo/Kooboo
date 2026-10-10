export const YlTimePicker = {
  name: 'YlTimePicker', inheritAttrs: false,
  props: { modelValue: { type: [String, Array], default: '' }, range: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['lg', 'md', 'sm']) },
    disabled: Boolean, readonly: Boolean, invalid: Boolean, min: String, max: String,
    step: { type: Number, default: 1 }, placeholder: { type: String, default: '请选择时间' },
    startLabel: { type: String, default: '开始时间' }, endLabel: { type: String, default: '结束时间' } },
  emits: ['update:modelValue'],
  setup(props, { attrs, emit }) {
    const update = (index, event) => {
      if (props.disabled || props.readonly) return
      if (!props.range) emit('update:modelValue', event.target.value)
      else { const value = [props.modelValue?.[0] || '', props.modelValue?.[1] || '']; value[index] = event.target.value; emit('update:modelValue', value) }
    }
    return () => {
      const { class: cls, style, ...inputAttrs } = attrs
      const values = props.range ? [props.modelValue?.[0] || '', props.modelValue?.[1] || ''] : [typeof props.modelValue === 'string' ? props.modelValue : '']
      return h('div', { class: ['timepicker', `timepicker-${props.size}`, 'yl-timepicker', cls,
        { 'timepicker-range': props.range, disabled: props.disabled, error: props.invalid }], style },
        values.flatMap((value, index) => [index ? h('span', { class: 'timepicker-separator', 'aria-hidden': 'true' }, '~') : null,
          h('span', { class: ['yl-timepicker-field', { 'yl-timepicker-empty': !value }] }, [
            h('input', { ...inputAttrs, id: index && inputAttrs.id ? `${inputAttrs.id}-end` : inputAttrs.id,
              class: 'timepicker-input', type: 'time', value, min: props.min, max: props.max, step: props.step,
              disabled: props.disabled, readonly: props.readonly, 'aria-invalid': props.invalid || undefined,
              'aria-label': props.range ? `${inputAttrs['aria-label'] || ''} ${index ? props.endLabel : props.startLabel}`.trim() : inputAttrs['aria-label'] || props.placeholder,
              onInput: event => update(index, event) }),
            !value ? h('span', { class: 'timepicker-placeholder', 'aria-hidden': 'true' }, props.range ? index ? props.endLabel : props.startLabel : props.placeholder) : null
          ])]))
    }
  }
}
