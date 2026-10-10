export const YlPageHeader = {
  name: 'YlPageHeader',
  props: { title: { type: String, required: true }, description: String, breadcrumbs: { type: Array, default: () => [] },
    tabs: { type: Array, default: () => [] }, modelValue: String, status: String, statusText: String },
  emits: ['navigate', 'update:modelValue'],
  setup(props, { slots, emit }) {
    return () => h('header', { class: 'yl-page-header' }, [
      props.breadcrumbs.length ? h(YlBreadcrumb, { items: props.breadcrumbs, onNavigate: path => emit('navigate', path) }) : null,
      h('div', { class: 'yl-page-header-row' }, [h('div', { class: 'yl-page-header-heading' }, [
        h('div', { class: 'yl-page-header-title' }, [h('h1', {}, props.title), props.statusText ? h(YlStatusTag, { type: props.status || 'stop' }, () => props.statusText) : null]),
        props.description ? h('p', { class: 'yl-page-header-description' }, props.description) : null
      ]), slots.actions ? h('div', { class: 'yl-page-header-actions' }, slots.actions()) : null]),
      props.tabs.length ? h(YlTabs, { variant: 'card', items: props.tabs, modelValue: props.modelValue || '', 'onUpdate:modelValue': value => emit('update:modelValue', value) }, slots) : null
    ])
  }
}
