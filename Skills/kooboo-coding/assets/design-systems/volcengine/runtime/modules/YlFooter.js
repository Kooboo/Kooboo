export const YlFooter = {
  name: 'YlFooter',
  props: { height: { type: [String, Number], default: null } },
  setup(props, { slots }) {
    return () => h('footer', { class: 'yl-layout-footer', style: { height: typeof props.height === 'number' ? props.height + 'px' : props.height } }, slots.default?.())
  },
}
