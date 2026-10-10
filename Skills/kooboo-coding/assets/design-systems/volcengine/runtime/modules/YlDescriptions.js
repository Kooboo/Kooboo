export const YlDescriptions = {
  name: 'YlDescriptions',
  props: { title: String, items: { type: Array, default: () => [] },
    layout: { type: String, default: 'stacked', validator: oneOf(['stacked', 'label-left', 'label-right', 'inline', 'inline-colon']) } },
  setup(props, { slots }) {
    return () =>
      h('section', { class: ['descriptions', `descriptions-${props.layout}`] }, [
        props.title ? h('h2', { class: 'descriptions-title' }, props.title) : null,
        h(
          'dl',
          { class: [props.layout.startsWith('label-') ? 'descriptions-list' : 'descriptions-grid', 'yl-descriptions'] },
          props.items.map((item) =>
            h('div', { key: item.key, class: ['desc-item', props.layout === 'stacked' ? 'desc-item-stacked' : props.layout.startsWith('label-') ? 'desc-item-horizontal' : 'desc-item-inline'] }, [
              h('dt', { class: 'desc-label' }, item.label + (props.layout === 'inline-colon' ? '：' : '')),
              h(
                'dd',
                { class: 'desc-value' },
                slots[`item-${item.key}`]?.({ item }) ?? String(item.value ?? '—')
              )
            ])
          )
        )
      ])
  }
}
