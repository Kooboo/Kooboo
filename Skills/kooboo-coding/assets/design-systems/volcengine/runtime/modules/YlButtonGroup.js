export const YlButtonGroup = {
  name: 'YlButtonGroup',
  props: { label: { type: String, required: true } },
  setup(props, { slots }) { return () => h('div', { class: 'btn-group yl-button-group', role: 'group', 'aria-label': props.label }, slots.default?.()) }
}
