export const YlShell = {
  name: 'YlShell',
  props: {
    appName: { type: String, required: true },
    navigation: { type: Array, default: () => [] },
    activePath: { type: String, default: '' },
    navigationLoading: Boolean,
    navigationLabel: { type: String, default: '主导航' },
    menuLabel: { type: String, default: '打开导航' },
    closeLabel: { type: String, default: '关闭导航' },
    collapsed: { type: Boolean, default: undefined },
    collapseLabel: { type: String, default: '收起侧栏' },
    expandLabel: { type: String, default: '展开侧栏' },
  },
  emits: ['navigate', 'update:collapsed'],
  setup(props, { slots, emit, expose }) {
    const mobileOpen = ref(false)
    const content = ref(null)
    const localCollapsed = ref(false)
    const collapsed = computed(() => props.collapsed === undefined ? localCollapsed.value : props.collapsed)
    function setSidebarCollapsed(value) {
      value = Boolean(value)
      if (props.collapsed === undefined) localCollapsed.value = value
      emit('update:collapsed', value)
    }
    function toggleSidebar() { setSidebarCollapsed(!collapsed.value) }
    expose({ collapsed, toggleSidebar, setSidebarCollapsed })
    const navigationId = `yl-navigation-${useId()}`
    const sidebarScope = () => ({
      collapsed: collapsed.value, toggleSidebar, setSidebarCollapsed,
      sidebarTriggerAttrs: {
        'aria-label': collapsed.value ? props.expandLabel : props.collapseLabel,
        'aria-controls': navigationId + '-desktop', 'aria-expanded': !collapsed.value,
      },
    })
    const { navigation } = createYlNavigation(props, navigationId, path => { mobileOpen.value = false; emit('navigate', path) })
    let desktopQuery
    const closeOnDesktop = (event) => {
      if (event.matches) mobileOpen.value = false
    }
    onMounted(() => {
      if (typeof window === 'undefined') return
      desktopQuery = window.matchMedia('(min-width: 761px)')
      desktopQuery.addEventListener('change', closeOnDesktop)
    })
    onBeforeUnmount(() =>
      desktopQuery?.removeEventListener('change', closeOnDesktop)
    )
    watch(
      () => props.activePath,
      () => {
        mobileOpen.value = false
        content.value?.resetScroll()
      }
    )
    return () =>
      h(YlLayout, { class: 'yl-shell', heightMode: 'viewport', 'data-design-system': 'volcengine' }, { default: () => [
        h(YlHeader, { class: 'yl-shell-topnav' }, { default: () => [
          h(
            'span',
            { class: 'yl-shell-identity', 'data-app-region': 'identity' },
            [
              h('span', { class: 'yl-shell-logo', 'aria-hidden': 'true' }, Array.from(props.appName.trim())[0] || 'A'),
              h('span', { class: 'yl-shell-name', title: props.appName }, props.appName),
            ]
          ),
          h(YlButton, {
            class: 'yl-mobile-toggle',
            variant: 'text',
            icon: 'menu',
            'aria-label': props.menuLabel,
            title: props.menuLabel,
            'aria-controls': navigationId,
            'aria-haspopup': 'dialog',
            'aria-expanded': mobileOpen.value,
            onClick: () => {
              mobileOpen.value = true
            },
          }),
          slots.actions
            ? h('div', { class: 'yl-shell-actions' }, slots.actions(sidebarScope()))
            : null,
        ] }),
        h(YlLayout, { class: 'yl-shell-workspace', direction: 'horizontal', heightMode: 'fill' }, { default: () => [
          h(YlAside, { id: navigationId + '-desktop', collapsed: collapsed.value, class: 'yl-desktop-navigation' }, { default: () => props.navigationLoading ? h(YlSkeleton, { class: 'yl-shell-navigation-skeleton', title: false, rows: 6 }) : navigation('desktop') }),
          h(
            YlMain,
            { ref: content, scroll: true, class: 'yl-shell-content', 'data-app-region': 'content' },
            { default: () => slots.default?.(sidebarScope()) }
          ),
        ] }),
        h(
          YlDialog,
          {
            id: navigationId,
            modelValue: mobileOpen.value,
            'onUpdate:modelValue': (value) => {
              mobileOpen.value = value
            },
            title: props.navigationLabel,
            closeLabel: props.closeLabel,
            size: 'sm',
          },
          { default: () => props.navigationLoading ? h(YlSkeleton, { class: 'yl-shell-navigation-skeleton', title: false, rows: 6 }) : navigation('mobile') }
        ),
      ] })
  },
}
