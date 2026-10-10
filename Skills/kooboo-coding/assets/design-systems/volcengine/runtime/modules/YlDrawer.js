export const YlDrawer = {
  name: 'YlDrawer',
  props: { modelValue: Boolean, title: { type: String, required: true }, busy: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['mini', 'sm', 'md', 'lg']) } },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        YlDialog,
        {
          modelValue: props.modelValue,
          title: props.title,
          busy: props.busy,
          class: ['yl-drawer', `yl-drawer-${props.size}`],
          style: { width: `${{ mini: 480, sm: 600, md: 720, lg: 1080 }[props.size]}px` },
          size: 'lg',
          'onUpdate:modelValue': (value) => emit('update:modelValue', value)
        },
        slots
      )
  }
}
