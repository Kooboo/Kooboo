export const YlAnchor = {
  name: 'YlAnchor',
  props: { items: { type: Array, default: () => [] }, modelValue: String, container: [Object, Function],
    offset: { type: Number, default: 16 }, label: { type: String, default: '本页导航' } },
  emits: ['update:modelValue', 'navigate'],
  setup(props, { emit }) {
    const current = ref('')
    const entries = computed(() => {
      const visit = (items, level = 1, seen = new Set()) => items.flatMap(item => {
        if (!item || seen.has(item)) return []
        return [{ ...item, level }, ...visit(item.children || [], level + 1, new Set([...seen, item]))]
      })
      return visit(props.items)
    })
    const container = () => (typeof props.container === 'function' ? props.container() : props.container) || (typeof window === 'undefined' ? null : window)
    const set = id => { if (current.value !== id) { current.value = id; emit('update:modelValue', id) } }
    const target = id => typeof document === 'undefined' ? null : document.getElementById?.(id)
    const scan = () => {
      const host = container(), top = host?.getBoundingClientRect?.().top || 0
      let active = entries.value.find(item => target(item.id))?.id || ''
      for (const item of entries.value) if (target(item.id)?.getBoundingClientRect().top <= top + props.offset + 1) active = item.id
      if (active) set(active)
    }
    let host
    const bind = () => { host?.removeEventListener?.('scroll', scan); host = container(); host?.addEventListener?.('scroll', scan, { passive: true }); scan() }
    onMounted(bind)
    watch(container, bind, { flush: 'post' })
    watch(entries, () => nextTick(scan))
    onBeforeUnmount(() => host?.removeEventListener?.('scroll', scan))
    const navigate = (event, item) => {
      if (event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || event.button > 0) return
      event.preventDefault()
      const element = target(item.id), host = container()
      if (!element || !host) return
      const top = element.getBoundingClientRect().top - (host.getBoundingClientRect?.().top || 0) + (host.scrollTop ?? host.scrollY ?? 0) - props.offset
      host.scrollTo?.({ top, behavior: 'auto' })
      set(item.id); emit('navigate', item.id)
    }
    return () => h('nav', { class: 'anchor yl-anchor', 'aria-label': props.label }, entries.value.map(item => h('a', {
      key: item.id, href: '#' + encodeURIComponent(item.id), class: ['anchor-item', `anchor-item--${['1st', '2nd', '3rd'][Math.min(3, item.level) - 1]}`, { active: (props.modelValue ?? current.value) === item.id }],
      'aria-current': (props.modelValue ?? current.value) === item.id ? 'location' : undefined,
      onClick: event => navigate(event, item)
    }, [h('span', { class: 'anchor-item__bar', 'aria-hidden': 'true' }), h('span', { class: 'anchor-item__text', title: item.label }, item.label)])))
  }
}
