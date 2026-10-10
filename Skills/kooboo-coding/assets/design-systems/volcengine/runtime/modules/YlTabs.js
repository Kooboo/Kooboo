export const YlTabs = {
  name: 'YlTabs',
  props: {
    modelValue: { type: String, required: true },
    items: { type: Array, default: () => [] },
    label: { type: String, default: '选项卡' },
    variant: { type: String, default: 'line', validator: oneOf(['line', 'card', 'capsule', 'button', 'segmented', 'vertical']) },
    size: { type: String, default: 'md', validator: oneOf(['lg', 'md', 'sm']) },
    scrollable: Boolean, addable: Boolean, bordered: Boolean, addLabel: { type: String, default: '新增选项卡' }
  },
  emits: ['update:modelValue', 'add'],
  setup(props, { slots, emit }) {
    const id = `yl-tabs-${useId()}`
    const buttons = new Map()
    const list = ref(null)
    const choose = (item) => {
      if (item && !item.disabled) emit('update:modelValue', item.key)
    }
    function keydown(event, index) {
      const forward = props.variant === 'vertical' ? 'ArrowDown' : 'ArrowRight'
      const backward = props.variant === 'vertical' ? 'ArrowUp' : 'ArrowLeft'
      if (![backward, forward, 'Home', 'End'].includes(event.key)) return
      event.preventDefault()
      const enabled = props.items.filter((item) => !item.disabled)
      if (!enabled.length) return
      const current = enabled.indexOf(props.items[index])
      const next =
        event.key === 'Home'
          ? enabled[0]
          : event.key === 'End'
          ? enabled.at(-1)
          : enabled[
              (current + (event.key === forward ? 1 : -1) + enabled.length) % enabled.length
            ]
      choose(next)
      buttons.get(next.key)?.focus()
      buttons.get(next.key)?.scrollIntoView?.({ block: 'nearest', inline: 'nearest' })
    }
    return () =>
      h('div', { class: ['yl-tabs', `yl-tabs-${props.variant}`, `yl-tabs-${props.size}`] }, [
        h('div', { class: 'yl-tabs-bar' }, [
        props.scrollable ? h(YlButton, { variant: 'text', icon: 'chevron-left', 'aria-label': '向前滚动选项卡', onClick: () => list.value?.scrollBy?.({ left: -200 }) }) : null,
        h(
          'div',
          { ref: list, class: [props.variant === 'segmented' ? 'othertabs' : `tabs-${props.variant === 'vertical' ? 'line' : props.variant}`, 'yl-tabs-list'], role: 'tablist', 'aria-label': props.label, 'aria-orientation': props.variant === 'vertical' ? 'vertical' : 'horizontal' },
          props.items.map((item, index) =>
            h(
              'button',
              {
                type: 'button',
                key: item.key,
                ref: (el) => {
                  if (el) buttons.set(item.key, el)
                  else buttons.delete(item.key)
                },
                id: `${id}-tab-${index}`,
                class: ['tab-item', props.variant === 'segmented' ? 'othertabs-item' : '', `tab-size-${props.size}`, { active: props.modelValue === item.key }],
                role: 'tab',
                disabled: item.disabled,
                'aria-selected': props.modelValue === item.key,
                'aria-controls': `${id}-panel-${index}`,
                tabindex: props.modelValue === item.key ? 0 : -1,
                onClick: () => choose(item),
                onKeydown: (event) => keydown(event, index)
              },
              [item.icon ? h(YlIcon, { name: item.icon, class: 'tab-icon' }) : null, h('span', { class: 'yl-tab-label' }, item.label), item.count != null ? h('span', { class: 'tab-count' }, `(${item.count})`) : null, item.badge != null ? h('span', { class: 'tab-badge' }, item.badge) : null]
            )
          )
        ),
        props.scrollable ? h(YlButton, { variant: 'text', icon: 'chevron-right', 'aria-label': '向后滚动选项卡', onClick: () => list.value?.scrollBy?.({ left: 200 }) }) : null,
        props.addable ? h(YlButton, { variant: 'text', icon: 'plus', 'aria-label': props.addLabel, onClick: () => emit('add') }) : null,
        slots.extra ? h('div', { class: 'tabs-extra' }, slots.extra()) : null,
        ]),
        ...props.items.map((item, index) =>
          h(
            'section',
            {
              id: `${id}-panel-${index}`,
              role: 'tabpanel',
              class: ['tab-panel', { 'tab-panel-bordered': props.bordered, 'tab-panel-vertical': props.variant === 'vertical' }],
              'aria-labelledby': `${id}-tab-${index}`,
              hidden: props.modelValue !== item.key,
              tabindex: 0
            },
            props.modelValue === item.key ? slots[item.key]?.() : null
          )
        )
      ])
  }
}
