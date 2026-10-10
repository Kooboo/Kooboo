export const YlBreadcrumb = {
  name: 'YlBreadcrumb',
  props: { items: { type: Array, default: () => [] }, label: { type: String, default: '面包屑' }, size: { type: String, default: 'md', validator: oneOf(['md', 'lg']) }, maxItems: { type: Number, default: 0 } },
  emits: ['navigate'],
  setup(props, { emit }) {
    const expanded = ref(false)
    const visible = computed(() => !expanded.value && props.maxItems >= 3 && props.items.length > props.maxItems ? [props.items[0], { ellipsis: true }, ...props.items.slice(-(props.maxItems - 2))] : props.items)
    return () =>
      h(
        'nav',
        { class: ['breadcrumb', `breadcrumb-${props.size}`], 'aria-label': props.label },
        visible.value.flatMap((item, index) => [
          index ? h('span', { class: 'breadcrumb-separator', 'aria-hidden': 'true' }, '/') : null,
          item.ellipsis ? h('button', { type: 'button', class: 'breadcrumb-item yl-breadcrumb-expand', 'aria-label': '展开完整路径', onClick: () => { expanded.value = true } }, '…') : index === visible.value.length - 1 || !item.path || item.disabled
            ? h(
                'span',
                {
                  class: ['breadcrumb-item', { current: index === visible.value.length - 1, disabled: item.disabled }],
                  'aria-disabled': item.disabled || undefined,
                  'aria-current': index === visible.value.length - 1 ? 'page' : undefined
                },
                item.label
              )
            : h(
                'a',
                {
                  class: 'breadcrumb-item',
                  href: item.path,
                  onClick: (event) => {
                    if (
                      event.button === 0 &&
                      !event.ctrlKey &&
                      !event.metaKey &&
                      !event.shiftKey &&
                      !event.altKey
                    ) {
                      event.preventDefault()
                      emit('navigate', item.path)
                    }
                  }
                },
                item.label
              )
        ])
      )
  }
}
