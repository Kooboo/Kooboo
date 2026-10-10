export const YlTable = {
  name: 'YlTable',
  props: {
    rows: { type: Array, default: () => [] },
    columns: { type: Array, required: true },
    rowKey: { type: String, default: 'id' },
    caption: { type: String, required: true },
    loading: Boolean,
    striped: Boolean,
    sort: { type: Object, default: null },
    error: { type: String, default: '' },
    emptyText: { type: String, default: '暂无数据' },
    loadingText: { type: String, default: '正在加载' },
    selectable: Boolean,
    rowDisabled: { type: Function, default: () => false },
    selectedKeys: { type: Array, default: () => [] },
    selectAllLabel: { type: String, default: '选择当前页全部记录' },
    selectionLabel: { type: Function, default: (row) => `选择记录 ${row.id}` },
  },
  emits: ['update:selectedKeys', 'update:sort'],
  setup(props, { slots, emit }) {
    const selected = computed(() => new Set(props.selectedKeys))
    const blocked = computed(() => props.loading || Boolean(props.error))
    function select(keys, checked) {
      if (blocked.value) return
      const next = new Set(props.selectedKeys)
      keys.forEach((key) => (checked ? next.add(key) : next.delete(key)))
      emit('update:selectedKeys', [...next])
    }
    return () => {
      const keys = props.rows
        .filter((row) => !props.rowDisabled(row))
        .map((row) => row[props.rowKey])
      const count = keys.filter((key) => selected.value.has(key)).length
      const headings = props.columns.map((column) =>
        h('th', { key: column.key, scope: 'col', 'aria-sort': column.sortable ? props.sort?.key === column.key ? props.sort.order === 'asc' ? 'ascending' : 'descending' : 'none' : undefined },
          column.sortable ? h('button', { type: 'button', class: 'yl-table-sort', disabled: blocked.value,
            onClick: () => { if (!blocked.value) emit('update:sort', props.sort?.key !== column.key ? { key: column.key, order: 'asc' } : props.sort.order === 'asc' ? { key: column.key, order: 'desc' } : null) }
          }, [column.label, h(YlIcon, { name: props.sort?.key === column.key ? props.sort.order === 'asc' ? 'chevron-up' : 'chevron-down' : 'arrow-down-up', size: 14 })]) : column.label)
      )
      if (props.selectable)
        headings.unshift(
          h(
            'th',
            { class: 'th-checkbox', scope: 'col' },
            h(YlCheckbox, {
              'aria-label': props.selectAllLabel,
              modelValue: keys.length > 0 && count === keys.length,
              indeterminate: count > 0 && count < keys.length,
              disabled: blocked.value || !keys.length,
              'onUpdate:modelValue': (checked) => select(keys, checked),
            })
          )
        )
      const state = props.loading
        ? props.loadingText
        : props.error || (!props.rows.length ? props.emptyText : '')
      const body = state
        ? [
            h(
              'tr',
              h(
                'td',
                {
                  colspan: props.columns.length + Number(props.selectable),
                  class: 'yl-table-state',
                },
                h(
                  'div',
                  {
                    key: props.loading
                      ? 'loading'
                      : props.error
                      ? 'error'
                      : 'empty',
                    class: [
                      'yl-table-feedback',
                      { 'yl-table-feedback-loading': props.loading },
                    ],
                    role: !props.loading && props.error ? 'alert' : 'status',
                  },
                  [
                    props.loading
                      ? h('span', {
                          class: 'yl-table-loader',
                          'aria-hidden': 'true',
                        })
                      : h(YlIcon, {
                          name: props.error
                            ? 'triangle-alert'
                            : 'clipboard-list',
                          size: 28,
                          class: 'yl-table-state-icon',
                        }),
                    h('span', { class: 'yl-table-state-text' }, state),
                  ]
                )
              )
            ),
          ]
        : props.rows.map((row) => {
            const cells = props.columns.map((column) =>
              h(
                'td',
                { key: column.key },
                slots[`cell-${column.key}`]?.({
                  row,
                  value: row[column.key],
                }) ?? String(row[column.key] ?? '')
              )
            )
            if (props.selectable)
              cells.unshift(
                h(
                  'td',
                  h(YlCheckbox, {
                    'aria-label': props.selectionLabel(row),
                    modelValue: selected.value.has(row[props.rowKey]),
                    disabled: blocked.value || props.rowDisabled(row),
                    'onUpdate:modelValue': (checked) => {
                      if (!props.rowDisabled(row))
                        select([row[props.rowKey]], checked)
                    },
                  })
                )
              )
            return h('tr', { key: row[props.rowKey] }, cells)
          })
      return h(
        'div',
        {
          class: 'table-wrapper yl-table',
          'aria-busy': props.loading || undefined,
          tabindex: 0,
          role: 'region',
          'aria-label': props.caption,
        },
        [slots.toolbar ? h('div', { class: 'toolbar yl-table-toolbar' }, slots.toolbar({ selectedKeys: props.selectedKeys })) : null,
        h('table', { class: ['table', { 'table-striped': props.striped }] }, [
          h('caption', { class: 'yl-sr-only' }, props.caption),
          h('thead', h('tr', headings)),
          h('tbody', body),
        ]), slots.pagination ? h('div', { class: 'yl-table-pagination' }, slots.pagination()) : null]
      )
    }
  },
}
