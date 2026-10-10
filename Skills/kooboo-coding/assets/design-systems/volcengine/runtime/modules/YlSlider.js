export const YlSlider = {
  name: 'YlSlider',
  props: { modelValue: { type: [Number, Array], default: 0 }, range: Boolean,
    min: { type: Number, default: 0 }, max: { type: Number, default: 100 }, step: { type: Number, default: 1 },
    size: { type: String, default: 'md', validator: oneOf(['md', 'sm']) }, disabled: Boolean,
    label: { type: String, required: true }, startLabel: { type: String, default: '最小值' }, endLabel: { type: String, default: '最大值' } },
  emits: ['update:modelValue', 'change'],
  setup(props, { emit }) {
    const bounds = computed(() => {
      const min = Number.isFinite(props.min) ? props.min : 0
      return { min, max: Math.max(min, Number.isFinite(props.max) ? props.max : 100) }
    })
    const clamp = value => Math.max(bounds.value.min, Math.min(bounds.value.max, Number.isFinite(value) ? value : bounds.value.min))
    const values = computed(() => props.range ? [clamp(props.modelValue?.[0]), clamp(props.modelValue?.[1])].sort((a, b) => a - b) : [clamp(props.modelValue)])
    const percent = value => bounds.value.max === bounds.value.min ? 0 : (value - bounds.value.min) / (bounds.value.max - bounds.value.min) * 100
    const update = (index, event, commit = false) => {
      if (props.disabled || bounds.value.max <= bounds.value.min) return
      const next = [...values.value], value = clamp(Number(event.target.value))
      next[index] = props.range ? index === 0 ? Math.min(value, next[1]) : Math.max(value, next[0]) : value
      emit(commit ? 'change' : 'update:modelValue', props.range ? next : next[0])
    }
    return () => h('div', { class: ['slider', `slider-${props.size}`, 'yl-slider', { disabled: props.disabled, 'slider-range': props.range }], role: 'group', 'aria-label': props.label }, [
      h('div', { class: 'slider-track', 'aria-hidden': 'true' }, h('div', { class: 'slider-track-fill', style: {
        left: `${props.range ? percent(values.value[0]) : 0}%`, width: `${percent(values.value.at(-1)) - (props.range ? percent(values.value[0]) : 0)}%` } })),
      ...values.value.map((value, index) => h('input', { key: index, type: 'range', class: 'yl-slider-native',
        value, min: bounds.value.min, max: bounds.value.max, step: props.step > 0 ? props.step : 1,
        disabled: props.disabled || bounds.value.max <= bounds.value.min,
        'aria-label': props.range ? `${props.label} ${index ? props.endLabel : props.startLabel}` : props.label,
        'aria-valuemin': props.range && index === 1 ? values.value[0] : bounds.value.min,
        'aria-valuemax': props.range && index === 0 ? values.value[1] : bounds.value.max,
        onInput: event => update(index, event), onChange: event => update(index, event, true) }))
    ])
  }
}
