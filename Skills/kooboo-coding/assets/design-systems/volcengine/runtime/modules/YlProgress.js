export const YlProgress = {
  name: 'YlProgress',
  props: { value: { type: Number, default: 0 }, label: { type: String, required: true },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    status: { type: String, default: 'normal', validator: oneOf(['normal', 'success', 'warning', 'error']) } },
  setup(props) {
    return () => {
      const value = Number.isFinite(props.value) ? Math.min(100, Math.max(0, props.value)) : 0
      return h(
        'div',
        {
          class: ['progress progress-line yl-progress', `progress-${props.size === 'md' ? 'default' : props.size}`, `progress-${props.status}`],
          role: 'progressbar',
          'aria-label': props.label,
          'aria-valuemin': 0,
          'aria-valuemax': 100,
          'aria-valuenow': value
        },
        [
          h(
            'div',
            { class: 'progress-track' },
            h('div', { class: 'progress-fill', style: { width: `${value}%` } })
          ),
          props.size !== 'mini' ? h('span', { class: 'progress-text' }, `${value}%`) : null
        ]
      )
    }
  }
}
