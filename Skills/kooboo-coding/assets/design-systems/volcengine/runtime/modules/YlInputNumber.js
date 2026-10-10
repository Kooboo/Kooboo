export const YlInputNumber = {
  name: 'YlInputNumber',
  props: {
    modelValue: { type: Number, default: null },
    min: Number,
    max: Number,
    step: { type: Number, default: 1 },
    disabled: Boolean,
    readonly: Boolean,
    invalid: Boolean
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    return () =>
      h('input', {
        type: 'number',
        class: ['ui-input', 'input-md', { 'input-error': props.invalid }],
        value: props.modelValue ?? '',
        min: props.min,
        max: props.max,
        step: props.step,
        disabled: props.disabled,
        readonly: props.readonly,
        'aria-invalid': props.invalid || undefined,
        onInput: (event) => {
          if (props.disabled || props.readonly) return
          const value = event.target.value
          if (value === '') {
            if (!event.target.validity?.badInput) emit('update:modelValue', null)
            return
          }
          const number = Number(value)
          if (Number.isFinite(number)) emit('update:modelValue', number)
        }
      })
  }
}
