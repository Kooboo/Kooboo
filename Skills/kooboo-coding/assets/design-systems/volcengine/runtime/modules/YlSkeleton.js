export const YlSkeleton = {
  name: 'YlSkeleton',
  props: {
    loading: { type: Boolean, default: true },
    rows: { type: Number, default: 4 },
    title: { type: Boolean, default: true },
    animated: { type: Boolean, default: true },
    label: { type: String, default: '' }
  },
  setup(props, { slots }) {
    return () => !props.loading ? slots.default?.() : h('div', {
      class: ['yl-skeleton', { 'yl-skeleton-animated': props.animated }], role: 'status', 'aria-label': props.label || undefined, 'aria-busy': 'true'
    }, [
      props.label ? h('span', { class: 'yl-sr-only' }, props.label) : null,
      h('div', { class: 'yl-skeleton-shapes', 'aria-hidden': 'true' }, [
        props.title ? h('div', { class: 'yl-skeleton-line yl-skeleton-title' }) : null,
        ...Array.from({ length: Math.min(20, Math.max(1, Math.floor(props.rows) || 1)) }, (_, index) => h('div', { key: index, class: 'yl-skeleton-line' }))
      ])
    ])
  }
}
