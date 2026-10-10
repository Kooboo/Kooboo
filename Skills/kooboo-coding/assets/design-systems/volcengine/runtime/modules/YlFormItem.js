export const YlFormItem = {
  name: 'YlFormItem',
  props: {
    label: { type: String, required: true },
    for: { type: String, required: true },
    error: String,
    hint: String,
    success: String,
    required: Boolean
  },
  setup(props, { slots }) {
    return () =>
      h('div', { class: 'form-field' }, [
        h('label', { for: props.for, class: 'form-label' }, [
          props.label,
          props.required ? h('span', { 'aria-hidden': 'true', class: 'yl-required' }, ' *') : null
        ]),
        slots.default?.({
          id: props.for,
          'aria-describedby': props.error || props.success || props.hint ? `${props.for}-help` : undefined,
          'aria-invalid': !!props.error,
          required: props.required
        }),
        props.error || props.success || props.hint
          ? h(
              'p',
              {
                id: `${props.for}-help`,
                class: props.error ? 'form-error' : props.success ? 'form-success-text' : 'form-help',
                role: props.error ? 'alert' : undefined
              },
              props.error || props.success || props.hint
            )
          : null
      ])
  }
}
