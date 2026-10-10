export const YlTextarea = {
  name: 'YlTextarea',
  props: {
    modelValue: { type: String, default: '' },
    disabled: Boolean,
    readonly: Boolean,
    invalid: Boolean,
    rows: { type: Number, default: 3 }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    let composing = false
    const update = (event) => {
      if (!composing && !props.disabled && !props.readonly)
        emit('update:modelValue', event.target.value)
    }
    return () =>
      h('textarea', {
        class: ['ui-input', 'input-md', 'yl-textarea', { 'input-error': props.invalid }],
        value: props.modelValue,
        disabled: props.disabled,
        readonly: props.readonly,
        rows: props.rows,
        'aria-invalid': props.invalid || undefined,
        onInput: update,
        onCompositionstart: () => {
          composing = true
        },
        onCompositionend: (event) => {
          composing = false
          update(event)
        }
      })
  }
}
