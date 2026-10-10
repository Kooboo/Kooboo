export const YlPopconfirm = {
  name: 'YlPopconfirm',
  props: { modelValue: Boolean, title: { type: String, required: true }, description: String,
    tone: { type: String, default: 'warning', validator: oneOf(['warning', 'info', 'error']) },
    simple: Boolean, busy: Boolean, confirmText: { type: String, default: '确认' }, cancelText: { type: String, default: '取消' } },
  emits: ['update:modelValue', 'confirm', 'cancel'],
  setup(props, { slots, emit }) {
    const update = value => { if (!props.busy) emit('update:modelValue', value) }
    const cancel = () => { if (!props.busy) { emit('cancel'); update(false) } }
    return () => h(YlPopover, { modelValue: props.modelValue, title: props.title, persistent: props.busy, align: 'start',
      'onUpdate:modelValue': value => value ? update(true) : cancel() }, {
      trigger: scope => slots.trigger?.(scope),
      default: () => h('div', { class: ['popconfirm', 'yl-popconfirm', { 'popconfirm-simple': props.simple }], 'aria-busy': props.busy || undefined }, [
        h('div', { class: 'popconfirm-header' }, [!props.simple ? h(YlIcon, { name: props.tone === 'info' ? 'info' : props.tone === 'error' ? 'circle-x' : 'triangle-alert', size: 20, class: ['popconfirm-icon', `popconfirm-icon-${props.tone}`] }) : null,
          h('div', { class: 'popconfirm-content' }, [h('div', { class: 'popconfirm-title' }, props.title), props.description ? h('p', { class: 'popconfirm-description' }, props.description) : null, slots.default?.()])]),
        h('div', { class: 'popconfirm-footer' }, [
          h(YlButton, { size: 'sm', disabled: props.busy, onClick: cancel }, () => props.cancelText),
          h(YlButton, { size: 'sm', variant: props.tone === 'error' ? 'danger' : 'primary', loading: props.busy,
            onClick: () => { if (!props.busy) emit('confirm') } }, () => props.confirmText)
        ])
      ])
    })
  }
}
