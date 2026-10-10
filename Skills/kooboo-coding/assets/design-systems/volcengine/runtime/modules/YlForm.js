export const YlForm = {
  name: 'YlForm',
  props: { busy: Boolean, title: String, columns: { type: Number, default: 1, validator: oneOf([1, 2]) } },
  emits: ['submit'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        'form',
        {
          class: 'form',
          'aria-busy': props.busy || undefined,
          onSubmit: (event) => {
            event.preventDefault()
            if (!props.busy) emit('submit', event)
          }
        },
        [props.title ? h('h2', { class: 'form-title' }, props.title) : null,
          h('div', { class: ['yl-form-grid', { 'yl-form-grid-two': props.columns === 2 }] }, slots.default?.()),
          slots.actions ? h('div', { class: 'form-actions' }, slots.actions()) : null]
      )
  }
}
