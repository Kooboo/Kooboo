export const YlLayout = {
  name: 'YlLayout',
  props: {
    direction: { type: String, default: 'vertical', validator: oneOf(['horizontal','vertical']) },
    heightMode: { type: String, default: 'flow', validator: oneOf(['flow','fill','viewport']) },
  },
  setup(props, { slots }) {
    return () => h('div', { class: ['yl-layout', 'yl-layout-' + props.direction, 'yl-layout-' + props.heightMode] }, slots.default?.())
  },
}
