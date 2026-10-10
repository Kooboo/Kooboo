export const YlMain = {
  name: 'YlMain',
  props: { scroll: Boolean, as: { type: String, default: 'main', validator: oneOf(['main','section','div']) } },
  setup(props, { slots, expose }) {
    const element = ref(null)
    expose({ resetScroll() { if (element.value) { element.value.scrollTop = 0; element.value.scrollLeft = 0 } } })
    return () => h(props.as, { ref: element, class: ['yl-layout-main', { 'yl-layout-scroll': props.scroll }], tabindex: props.scroll ? 0 : undefined }, slots.default?.())
  },
}
