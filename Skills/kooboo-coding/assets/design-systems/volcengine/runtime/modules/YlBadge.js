export const YlBadge = {
  name: 'YlBadge',
  props: {
    count: { type: Number, default: 0 },
    max: { type: Number, default: 99 },
    dot: Boolean,
    showZero: Boolean,
    label: String
    ,size: { type: String, default: 'default', validator: oneOf(['small', 'default', 'large']) },
    muted: Boolean,
    status: { type: String, validator: oneOf(['success', 'warning', 'error', 'skip', 'awaiting']) }
  },
  setup(props, { slots }) {
    return () =>
      h('span', { class: ['badge-wrapper', { 'yl-badge-standalone': !slots.default }] }, [
        slots.default?.(),
        props.status || props.dot || props.count > 0 || props.showZero
          ? h(
              'span',
              {
                class: ['badge', props.muted ? 'badge-ash' : 'badge-active', `badge-${props.size}`,
                  props.status ? `badge-icon badge-icon-${props.status}` : props.dot ? 'badge-dot' : props.count > props.max ? 'badge-number-plus' : 'badge-number'],
                'aria-label': props.label || (props.status ? ({ success: '成功', warning: '警告', error: '错误', skip: '跳过', awaiting: '等待' })[props.status] : props.dot ? '有新消息' : `${props.count} 条`)
              },
              props.status ? h(YlIcon, { name: ({ success: 'check', warning: 'triangle-alert', error: 'x', skip: 'minus', awaiting: 'clock' })[props.status], size: props.size === 'small' ? 10 : props.size === 'large' ? 16 : 12 }) : props.dot ? '' : props.count > props.max ? `${props.max}+` : String(props.count)
            )
          : null
      ])
  }
}
