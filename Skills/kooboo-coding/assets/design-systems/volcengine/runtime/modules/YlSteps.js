export const YlSteps = {
  name: 'YlSteps',
  props: { items: { type: Array, default: () => [] }, current: { type: Number, default: 0 },
    direction: { type: String, default: 'horizontal', validator: oneOf(['horizontal', 'vertical']) },
    label: { type: String, default: '步骤' } },
  setup(props, { slots }) {
    return () => h('ol', { class: ['yl-steps', `yl-steps-${props.direction}`], 'aria-label': props.label }, props.items.map((item, index) => {
      const status = item.disabled ? 'disabled' : item.status || (index < props.current ? 'completed' : index === props.current ? 'active' : 'pending')
      return h('li', { key: item.key ?? index, class: ['yl-step', `yl-step-${status}`], 'aria-current': status === 'active' ? 'step' : undefined }, [
        h('span', { class: 'yl-step-indicator', 'aria-hidden': 'true' }, status === 'completed' || status === 'error' ? h(YlIcon, { name: status === 'completed' ? 'check' : 'x', size: 14 }) : index + 1),
        h('div', { class: 'yl-step-content' }, [h('span', { class: 'yl-step-title' }, item.title),
          h('span', { class: 'yl-sr-only' }, ({ completed: '已完成', active: '当前步骤', pending: '未开始', disabled: '不可用', error: '错误' })[status]),
          item.description ? h('p', { class: 'yl-step-description' }, item.description) : null,
          slots[`item-${item.key ?? index}`]?.({ item, index, status })]),
        index < props.items.length - 1 ? h('span', { class: 'yl-step-line', 'aria-hidden': 'true' }) : null
      ])
    }))
  }
}
