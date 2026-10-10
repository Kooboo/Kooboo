function createYlNavigation(props, navigationId, onNavigate) {
    const expanded = ref(new Set())
    const controls = new Map()
    const tree = computed(() => {
      const visit = (items, parent = null, ancestors = new Set()) =>
        items.flatMap((item, index) => {
          // Invalid/cyclic entries should not make the entire App unrenderable.
          if (!item || typeof item !== 'object' || ancestors.has(item))
            return []
          const key = `${parent?.key || ''}/${encodeURIComponent(
            item.key || item.path || item.label || String(index)
          )}`
          const node = {
            ...item,
            key,
            parent,
            disabled: Boolean(parent?.disabled || item.disabled),
          }
          node.children = visit(
            Array.isArray(item.children) ? item.children : [],
            node,
            new Set([...ancestors, item])
          )
          return [node]
        })
      return visit(props.navigation)
    })
    const groups = computed(() => {
      const result = new Map()
      for (const item of tree.value) {
        const group = item.group || ''
        if (!result.has(group)) result.set(group, [])
        result.get(group).push(item)
      }
      return [...result]
    })
    watch(
      [tree, () => props.activePath],
      () => {
        const valid = new Set(),
          activeAncestors = []
        const visit = (items) =>
          items.forEach((item) => {
            if (item.children.length) valid.add(item.key)
            if (item.path === props.activePath) {
              let parent = item.parent
              while (parent) {
                activeAncestors.push(parent.key)
                parent = parent.parent
              }
            }
            visit(item.children)
          })
        visit(tree.value)
        expanded.value = new Set(
          [...expanded.value]
            .filter((key) => valid.has(key))
            .concat(activeAncestors)
        )
      },
      { immediate: true }
    )
    const setExpanded = (item, value) => {
      if (item.disabled) return
      const next = new Set(expanded.value)
      value ? next.add(item.key) : next.delete(item.key)
      expanded.value = next
    }
    const navigate = (event, item) => {
      if (item.disabled) {
        event.preventDefault()
        return
      }
      if (
        event.button !== 0 ||
        event.metaKey ||
        event.ctrlKey ||
        event.shiftKey ||
        event.altKey
      )
        return
      event.preventDefault()
      onNavigate(item.path)
    }
    function keydown(event, item, scope) {
      if (
        item.disabled ||
        ![
          'ArrowDown',
          'ArrowUp',
          'ArrowRight',
          'ArrowLeft',
          'Home',
          'End',
        ].includes(event.key)
      )
        return
      event.preventDefault()
      const focus = (target) => controls.get(`${scope}:${target?.key}`)?.focus()
      if (event.key === 'ArrowRight') {
        if (item.children.length && !expanded.value.has(item.key))
          setExpanded(item, true)
        else focus(item.children.find((child) => !child.disabled))
      } else if (event.key === 'ArrowLeft') {
        if (item.children.length && expanded.value.has(item.key))
          setExpanded(item, false)
        else focus(item.parent)
      } else {
        const visible = []
        const visit = (items) =>
          items.forEach((node) => {
            if (node.disabled) return
            visible.push(node)
            if (expanded.value.has(node.key)) visit(node.children)
          })
        groups.value.forEach(([, items]) => visit(items))
        const index = visible.indexOf(item)
        const next =
          event.key === 'Home'
            ? 0
            : event.key === 'End'
            ? visible.length - 1
            : Math.max(
                0,
                Math.min(
                  visible.length - 1,
                  index + (event.key === 'ArrowDown' ? 1 : -1)
                )
              )
        focus(visible[next])
      }
    }
    function renderItems(items, scope, depth = 0) {
      return items.map((item) => {
        const branch = item.children.length > 0
        const isOpen = expanded.value.has(item.key)
        const submenuId = `${navigationId}-${scope}-${encodeURIComponent(
          item.key
        )}`
        const chevron = () =>
          h(YlIcon, {
            name: 'chevron-down',
            size: 16,
            class: ['menu-chevron', { 'yl-nav-chevron-open': isOpen }],
          })
        const toggle = () => setExpanded(item, !isOpen)
        const content = [
          h(
            'span',
            { class: 'menu-icon' },
            h(YlIcon, {
              name: item.icon || 'layout-grid',
              size: 20,
            })
          ),
          h('span', { class: 'menu-text' }, item.label),
        ]
        if (branch && !item.path) content.push(chevron())
        return h('li', { key: item.key, class: 'yl-nav-item' }, [
          h(
            'div',
            { class: 'yl-nav-row', style: { '--yl-nav-depth': depth } },
            [
              h(
                item.path ? 'a' : 'button',
                {
                  ref: (el) => {
                    const key = `${scope}:${item.key}`
                    if (el) controls.set(key, el)
                    else controls.delete(key)
                  },
                  href: item.path && !item.disabled ? item.path : undefined,
                  type: item.path ? undefined : 'button',
                  role: item.path && item.disabled ? 'link' : undefined,
                  disabled: !item.path ? item.disabled || !branch : undefined,
                  'aria-disabled': item.disabled || undefined,
                  tabindex: item.disabled ? -1 : undefined,
                  class: [
                    'menu-item',
                    'yl-nav-link',
                    {
                      active: props.activePath === item.path,
                      disabled: item.disabled,
                    },
                  ],
                  'aria-current':
                    item.path && props.activePath === item.path
                      ? 'page'
                      : undefined,
                  'aria-expanded': branch && !item.path ? isOpen : undefined,
                  'aria-controls': branch && !item.path ? submenuId : undefined,
                  onClick: item.path
                    ? (event) => navigate(event, item)
                    : toggle,
                  onKeydown: (event) => keydown(event, item, scope),
                },
                content
              ),
              branch && item.path
                ? h(
                    'button',
                    {
                      type: 'button',
                      class: 'yl-nav-expand',
                      disabled: item.disabled,
                      'aria-label': `${isOpen ? '收起' : '展开'}${item.label}`,
                      'aria-expanded': isOpen,
                      'aria-controls': submenuId,
                      onClick: toggle,
                      onKeydown: (event) => keydown(event, item, scope),
                    },
                    chevron()
                  )
                : null,
            ]
          ),
          branch
            ? h(
                'ul',
                { id: submenuId, class: 'yl-nav-children', hidden: !isOpen },
                renderItems(item.children, scope, depth + 1)
              )
            : null,
        ])
      })
    }
    function navigation(scope) {
      return h(
        'nav',
        { class: 'sidenav yl-shell-nav', 'aria-label': props.navigationLabel },
        groups.value.map(([group, items]) =>
          h('div', { class: 'menu-group-block', key: group }, [
            group ? h('div', { class: 'group-title' }, group) : null,
            h('ul', { class: 'yl-nav-list' }, renderItems(items, scope)),
          ])
        )
      )
    }
    return { navigation, renderItems, tree, groups, expanded, navigate }
}

export const YlSidenav = {
  name: 'YlSidenav',
  props: { navigation: { type: Array, default: () => [] }, activePath: { type: String, default: '' },
    navigationLabel: { type: String, default: '主导航' }, title: String, collapsed: Boolean },
  emits: ['navigate'],
  setup(props, { emit, slots }) {
    const opened = ref(null), id = `yl-sidenav-${useId()}`
    const nav = createYlNavigation(props, id, path => { opened.value = null; emit('navigate', path) })
    watch(() => props.collapsed, () => { opened.value = null })
    const active = node => node.path === props.activePath || node.children.some(active)
    return () => h('div', { class: ['yl-sidenav', { 'yl-sidenav-collapsed': props.collapsed }] }, [
      !props.collapsed && props.title ? h('div', { class: 'yl-sidenav-title' }, props.title) : null,
      props.collapsed ? h('nav', { class: 'yl-sidenav-rail', 'aria-label': props.navigationLabel }, nav.tree.value.map(item =>
        item.children.length ? h(YlPopover, { key: item.key, modelValue: opened.value === item.key, title: item.label, width: 260,
          'onUpdate:modelValue': value => { if (!item.disabled) opened.value = value ? item.key : null } }, {
          trigger: ({ triggerAttrs, toggle }) => h(YlButton, { ...triggerAttrs, variant: 'text', icon: item.icon || 'layout-grid',
            disabled: item.disabled, title: item.label, 'aria-label': item.label, class: { 'yl-sidenav-active': active(item) }, onClick: toggle }),
          default: () => h('ul', { class: 'yl-nav-list' }, nav.renderItems([item], 'rail'))
        }) : h('a', { key: item.key, href: item.disabled ? undefined : item.path, title: item.label, 'aria-label': item.label,
          'aria-disabled': item.disabled || undefined, tabindex: item.disabled ? -1 : 0,
          'aria-current': item.path === props.activePath ? 'page' : undefined,
          class: ['yl-sidenav-rail-link', { 'yl-sidenav-active': active(item), disabled: item.disabled }],
          onClick: event => nav.navigate(event, item) }, h(YlIcon, { name: item.icon || 'layout-grid', size: 20 }))
      )) : nav.navigation('standalone'),
      slots.footer?.()
    ])
  }
}
