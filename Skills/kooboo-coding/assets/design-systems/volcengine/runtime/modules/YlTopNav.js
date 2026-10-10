export const YlTopNav = {
  name: 'YlTopNav',
  props: { title: { type: String, required: true }, theme: { type: String, default: 'light', validator: oneOf(['light', 'dark']) },
    links: { type: Array, default: () => [] }, products: { type: Array, default: () => [] }, product: [String, Number],
    secondary: { type: Array, default: () => [] }, secondaryTitle: String, activePath: String,
    searchable: Boolean, searchValue: { type: String, default: '' }, searchLabel: { type: String, default: '搜索' } },
  emits: ['navigate', 'update:product', 'update:searchValue', 'search'],
  setup(props, { slots, emit }) {
    const link = (item, cls) => h('a', { key: item.path, class: [cls, { active: item.path === props.activePath, disabled: item.disabled }],
      href: item.disabled ? undefined : item.path, tabindex: item.disabled ? -1 : undefined,
      'aria-disabled': item.disabled || undefined, 'aria-current': item.path === props.activePath ? 'page' : undefined,
      onClick: event => {
        if (item.disabled) { event.preventDefault(); return }
        if (event.button === 0 && !event.metaKey && !event.ctrlKey && !event.shiftKey && !event.altKey) { event.preventDefault(); emit('navigate', item.path) }
      } }, item.label)
    return () => h('header', { class: ['yl-topnav', `yl-topnav-${props.theme}`] }, [
      h('div', { class: 'top-nav yl-topnav-main' }, [
        h('div', { class: 'nav-logo yl-topnav-brand' }, slots.brand?.() || props.title),
        slots.navigationControl?.(),
        props.products.length ? h('nav', { class: 'product-tabs', 'aria-label': '产品' }, props.products.map(item => h('button', {
          key: item.key, type: 'button', class: ['product-tab', { active: item.key === props.product }], disabled: item.disabled,
          'aria-pressed': item.key === props.product, onClick: () => { if (!item.disabled) emit('update:product', item.key) }
        }, item.label))) : null,
        h('div', { class: 'nav-right yl-topnav-actions' }, [
          props.searchable ? h(YlInput, { modelValue: props.searchValue, prefixIcon: 'search', 'aria-label': props.searchLabel, placeholder: props.searchLabel,
            'onUpdate:modelValue': value => emit('update:searchValue', value),
            onKeydown: event => { if (event.key === 'Enter') { event.preventDefault(); emit('search', props.searchValue) } } }) : null,
          props.links.length ? h('nav', { class: 'yl-topnav-links', 'aria-label': '快捷导航' }, props.links.map(item => link(item, 'yl-topnav-link'))) : null,
          slots.actions?.()
        ])
      ]),
      props.secondary.length || props.secondaryTitle ? h('div', { class: 'secondary-nav yl-topnav-secondary' }, [
        props.secondaryTitle ? h('span', { class: 'secondary-nav-product' }, props.secondaryTitle) : null,
        h('nav', { class: 'secondary-nav-tabs', 'aria-label': '产品导航' }, props.secondary.map(item => link(item, 'sec-tab'))),
        slots.secondaryActions ? h('div', { class: 'secondary-nav-right' }, slots.secondaryActions()) : null
      ]) : null
    ])
  }
}
