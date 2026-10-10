export const YlAside = {
  name: 'YlAside',
  props: { width: { type: [String, Number], default: 200 }, scroll: { type: Boolean, default: true }, collapsed: Boolean },
  setup(props, { slots }) {
    return () => h('aside', { hidden: props.collapsed, class: ['yl-layout-aside', { 'yl-layout-scroll': props.scroll }], style: { width: typeof props.width === 'number' ? props.width + 'px' : props.width } }, slots.default?.())
  },
}
