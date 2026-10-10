export const YlDateRangePicker = {
  name: 'YlDateRangePicker',
  props: {
    modelValue: { type: Array, default: () => ['', ''] },
    mode: { type: String, default: 'date', validator: oneOf(['date', 'month', 'datetime-local']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    min: String, max: String, disabled: Boolean, readonly: Boolean, invalid: Boolean, clearable: Boolean,
    startLabel: { type: String, default: '开始日期' },
    endLabel: { type: String, default: '结束日期' },
    orderError: { type: String, default: '结束日期不能早于开始日期' }
  },
  emits: ['update:modelValue', 'change'],
  setup(props, { emit }) {
    const errorId = useId()
    const reversed = computed(() => !!(props.modelValue[0] && props.modelValue[1] && props.modelValue[0] > props.modelValue[1]))
    const update = (index, value) => {
      if (props.disabled || props.readonly) return
      const range = [props.modelValue[0] || '', props.modelValue[1] || '']
      range[index] = value
      emit('update:modelValue', range)
      emit('change', range)
    }
    return () => h('div', { class: 'yl-date-range', role: 'group' }, [
      h('div', { class: 'yl-date-range-fields' }, [0, 1].map(index => h('label', { class: 'yl-date-range-field' }, [
        h('span', {}, index === 0 ? props.startLabel : props.endLabel),
        h(YlDatePicker, { modelValue: props.modelValue[index] || '', mode: props.mode, size: props.size,
          min: props.min, max: props.max, disabled: props.disabled, readonly: props.readonly,
          invalid: props.invalid || reversed.value, clearable: props.clearable,
          'aria-describedby': reversed.value ? errorId : undefined,
          'onUpdate:modelValue': value => update(index, value) })
      ]))),
      reversed.value ? h('p', { id: errorId, class: 'yl-date-range-error', role: 'status' }, props.orderError) : null
    ])
  }
}
