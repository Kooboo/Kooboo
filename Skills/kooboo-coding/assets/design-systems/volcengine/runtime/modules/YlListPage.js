export const YlListPage = {
  name: 'YlListPage',
  props: { title: { type: String, required: true }, description: { type: String, default: '' } },
  setup(props, { slots }) {
    return () =>
      h('section', { class: 'yl-list-page' }, [
        h('header', { class: 'page-header-comp yl-list-header' }, [
          h('div', [
            h('h1', { class: 'page-header-title' }, props.title),
            props.description ? h('p', { class: 'yl-list-description' }, props.description) : null
          ]),
          slots.actions ? h('div', { class: 'toolbar-actions' }, slots.actions()) : null
        ]),
        slots.filters
          ? h(
              'div',
              { class: 'table-toolbar' },
              h('div', { class: 'toolbar-filters yl-filters' }, slots.filters())
            )
          : null,
        slots.default?.(),
        slots.pagination ? h('footer', { class: 'yl-list-pagination' }, slots.pagination()) : null
      ])
  }
}
