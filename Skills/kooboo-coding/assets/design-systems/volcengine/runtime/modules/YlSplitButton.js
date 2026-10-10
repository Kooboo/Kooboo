export const YlSplitButton = {
  name: 'YlSplitButton',
  props: { label: { type: String, required: true }, items: { type: Array, default: () => [] },
    variant: { type: String, default: 'primary', validator: oneOf(['primary', 'secondary']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) }, disabled: Boolean, loading: Boolean,
    moreLabel: { type: String, default: '更多操作' } },
  emits: ['click', 'select'],
  setup(props, { emit }) {
    return () => h(YlButtonGroup, { label: props.label, class: 'yl-split-button' }, () => [
      h(YlButton, { variant: props.variant, size: props.size, disabled: props.disabled, loading: props.loading, onClick: event => emit('click', event) }, () => props.label),
      h(YlDropdown, { items: props.items, label: props.moreLabel, disabled: props.disabled || props.loading, onSelect: item => emit('select', item) }, {
        trigger: ({ triggerAttrs, toggle, disabled }) => h(YlButton, { ...triggerAttrs, variant: props.variant, size: props.size, disabled,
          icon: 'chevron-down', 'aria-label': props.moreLabel, onClick: toggle })
      })
    ])
  }
}
