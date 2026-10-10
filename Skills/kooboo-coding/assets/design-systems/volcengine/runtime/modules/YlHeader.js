export const YlHeader = {
  name: 'YlHeader',
  props: { height: { type: [String, Number], default: null } },
  setup(props, { slots }) {
    return () => h('header', { class: 'yl-layout-header', style: { height: typeof props.height === 'number' ? props.height + 'px' : props.height } }, slots.default?.())
  },
}
