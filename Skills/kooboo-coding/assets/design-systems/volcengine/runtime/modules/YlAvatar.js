export const YlAvatar = {
  name: 'YlAvatar',
  props: {
    name: { type: String, required: true },
    src: String,
    color: { type: String, default: 'primary', validator: oneOf(['red', 'beige', 'green', 'teal', 'purple', 'primary', 'grey']) },
    iconOnly: Boolean, capsule: Boolean, editable: Boolean, disabled: Boolean, bordered: Boolean,
    kind: { type: String, default: 'person', validator: oneOf(['person', 'department']) },
    subtitle: String,
    size: {
      type: String,
      default: 'default',
      validator: oneOf(['mini', 'default', 'medium', 'large'])
    }
  },
  emits: ['edit'],
  setup(props, { emit }) {
    const failed = ref(false)
    watch(
      () => props.src,
      () => {
        failed.value = false
      }
    )
    return () =>
      h(props.editable ? 'button' : 'span', { class: ['avatar', `avatar-${props.size}`, 'yl-avatar', { 'avatar-capsule': props.capsule, 'avatar-twoline': props.subtitle, 'yl-avatar-editable': props.editable, 'yl-avatar-department': props.kind === 'department' }],
        type: props.editable ? 'button' : undefined, disabled: props.editable ? props.disabled : undefined,
        'aria-label': props.editable ? `编辑 ${props.name}` : props.iconOnly ? props.name : undefined,
        onClick: () => { if (props.editable && !props.disabled) emit('edit') } }, [
        h(
          'span',
          { class: [`avatar-icon avatar-icon-${props.size} avatar-icon-${props.color}`, { 'avatar-icon-bordered': props.bordered }] },
          props.src && !failed.value
            ? h('img', {
                src: props.src,
                alt: props.name,
                onError: () => {
                  failed.value = true
                }
              })
            : h('span', { 'aria-label': props.name }, Array.from(props.name)[0] || '?')
        ),
        !props.iconOnly ? h('span', { class: 'avatar-info' }, [h('span', { class: 'avatar-name' }, props.name), props.subtitle ? h('span', { class: 'avatar-subtitle' }, props.subtitle) : null]) : null,
        props.editable ? h(YlIcon, { name: 'pencil', size: 14 }) : null
      ])
  }
}
