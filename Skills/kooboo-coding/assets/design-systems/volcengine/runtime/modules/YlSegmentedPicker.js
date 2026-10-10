export const YlSegmentedPicker = {
  name: 'YlSegmentedPicker',
  props: { modelValue: [String, Number], options: { type: Array, default: () => [] },
    size: { type: String, default: 'md', validator: oneOf(sizes) }, disabled: Boolean,
    label: { type: String, required: true } },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const controls = new Map()
    const choose = item => { if (!props.disabled && !item.disabled) emit('update:modelValue', item.value) }
    const keydown = (event, item) => {
      const enabled = props.options.filter(option => !option.disabled)
      if (props.disabled || item.disabled || !enabled.length) return
      const index = enabled.indexOf(item)
      let next
      if (event.key === 'Home') next = enabled[0]
      else if (event.key === 'End') next = enabled.at(-1)
      else if (['ArrowRight', 'ArrowDown', 'ArrowLeft', 'ArrowUp'].includes(event.key)) {
        next = enabled[(index + (['ArrowRight', 'ArrowDown'].includes(event.key) ? 1 : -1) + enabled.length) % enabled.length]
      }
      if (next) { event.preventDefault(); choose(next); controls.get(next.value)?.focus() }
    }
    return () => {
      const entry = props.options.find(item => item.value === props.modelValue && !item.disabled) || props.options.find(item => !item.disabled)
      return h('div', { class: ['segmented-picker', `segmented-picker-${props.size === 'mini' ? 'xs' : props.size}`, 'yl-segmented'], role: 'radiogroup', 'aria-label': props.label },
        props.options.map(item => h('button', { key: item.value, ref: el => { el ? controls.set(item.value, el) : controls.delete(item.value) },
          type: 'button', role: 'radio', 'aria-checked': item.value === props.modelValue,
          disabled: props.disabled || item.disabled, tabindex: !props.disabled && item === entry ? 0 : -1,
          class: ['segmented-picker-item', { active: item.value === props.modelValue, disabled: props.disabled || item.disabled }],
          onClick: () => choose(item), onKeydown: event => keydown(event, item) }, item.label)))
    }
  }
}
