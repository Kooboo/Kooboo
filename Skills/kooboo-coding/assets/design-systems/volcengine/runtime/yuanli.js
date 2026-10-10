// Yuanli Vue runtime. Native ESM, shared with the App Base Vue instance.
import { computed, h, nextTick, onBeforeUnmount, onMounted, ref, useId, watch } from 'vue'
import bundledIcons from '/yuanli-icons.js'
import { loadLucide } from '/lucide.js'

export const version = '0.10.1'
const sizes = ['lg', 'md', 'sm', 'mini']
const oneOf = values => value => values.includes(value)

export const YlIcon = {
  name: 'YlIcon',
  props: { name: { type: String, required: true }, size: { type: Number, default: 16 } },
  setup(props) {
    const remote = ref(null)
    let disposed = false
    onBeforeUnmount(() => {
      disposed = true
    })
    watch(
      () => props.name,
      async (name) => {
        remote.value = null
        if (bundledIcons[name]) return
        try {
          const lucide = await loadLucide()
          if (!disposed && name === props.name) {
            remote.value =
              lucide.icons[
                name.replace(/(^|-)(\w)/g, (_, prefix, letter) => letter.toUpperCase())
              ] || null
            if (!remote.value) console.warn(`Unknown Lucide icon: ${name}`)
          }
        } catch (error) {
          if (!disposed) console.warn('Lucide icon unavailable', error)
        }
      },
      { immediate: true }
    )
    // Vue owns the SVG tree. Do not use data-lucide DOM replacement inside components.
    const renderNode = ([tag, attributes, children]) =>
      h(tag, attributes, children?.map(renderNode))
    return () => {
      const node = bundledIcons[props.name] || remote.value
      return h(
        'svg',
        {
          ...(node?.[1] || { viewBox: '0 0 24 24' }),
          class: 'yl-icon',
          width: props.size,
          height: props.size,
          'aria-hidden': 'true',
          focusable: 'false'
        },
        node?.[2]?.map(renderNode)
      )
    }
  }
}

export const YlButton = {
  name: 'YlButton',
  props: {
    variant: {
      type: String,
      default: 'secondary',
      validator: oneOf(['primary', 'secondary', 'outline', 'text', 'danger', 'success', 'warning', 'dashed'])
    },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    loading: Boolean,
    disabled: Boolean,
    icon: String,
    suffixIcon: String,
    type: { type: String, default: 'button', validator: oneOf(['button', 'submit', 'reset']) }
  },
  emits: ['click'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        'button',
        {
          type: props.type,
          class: [
            'btn',
            `btn-${props.variant}`,
            `btn-${props.size}`,
            { disabled: props.disabled || props.loading }
          ],
          disabled: props.disabled || props.loading,
          'aria-busy': props.loading || undefined,
          onClick: (event) => {
            if (!props.disabled && !props.loading) emit('click', event)
          }
        },
        [
          props.loading
            ? h('span', { class: 'yl-spinner', 'aria-hidden': 'true' })
            : props.icon
            ? h(YlIcon, { name: props.icon })
            : null,
          slots.default?.(),
          props.suffixIcon ? h(YlIcon, { name: props.suffixIcon }) : null
        ]
      )
  }
}

export const YlInput = {
  name: 'YlInput',
  inheritAttrs: false,
  props: {
    modelValue: { type: String, default: '' },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    prefixIcon: String, suffixIcon: String, insetLabel: String,
    invalid: Boolean,
    disabled: Boolean,
    readonly: Boolean
  },
  emits: ['update:modelValue'],
  setup(props, { attrs, slots, emit }) {
    let composing = false
    const update = (event) => {
      if (!composing && !props.disabled && !props.readonly)
        emit('update:modelValue', event.target.value)
    }
    return () => {
      const decorated = props.prefixIcon || props.suffixIcon || props.insetLabel || slots.prefix || slots.suffix
      const { class: cls, style, ...inputAttrs } = attrs
      const input = h('input', {
        ...inputAttrs, type: inputAttrs.type || 'text', value: props.modelValue,
        class: decorated ? 'yl-input-control' : ['ui-input', `input-${props.size}`, cls,
          { 'input-error': props.invalid, disabled: props.disabled, readonly: props.readonly }],
        style: decorated ? undefined : style,
        'aria-invalid': props.invalid || inputAttrs['aria-invalid'] || undefined,
        disabled: props.disabled, readonly: props.readonly,
        onInput: update,
        onCompositionstart: () => { composing = true },
        onCompositionend: event => { composing = false; update(event) }
      })
      return decorated ? h('span', { class: ['ui-input', `input-${props.size}`, 'yl-input-affix', cls,
        { 'input-error': props.invalid, disabled: props.disabled, readonly: props.readonly }], style }, [
        props.insetLabel ? h('span', { class: 'yl-input-inset-label' }, props.insetLabel) : null,
        slots.prefix?.() || (props.prefixIcon ? h(YlIcon, { name: props.prefixIcon }) : null),
        input,
        slots.suffix?.() || (props.suffixIcon ? h(YlIcon, { name: props.suffixIcon }) : null)
      ]) : input
    }
  }
}

export const YlCheckbox = {
  name: 'YlCheckbox',
  inheritAttrs: false,
  props: { modelValue: Boolean, disabled: Boolean, indeterminate: Boolean, invalid: Boolean },
  emits: ['update:modelValue'],
  setup(props, { slots, attrs, emit }) {
    return () =>
      h(
        'label',
        {
          class: [
            'checkbox',
            'yl-checkbox',
            {
              checked: props.modelValue,
              disabled: props.disabled,
              indeterminate: props.indeterminate,
              invalid: props.invalid
            }
          ]
        },
        [
          h('input', {
            ...attrs,
            type: 'checkbox',
            class: 'yl-checkbox-native',
            checked: props.modelValue,
            indeterminate: props.indeterminate,
            disabled: props.disabled,
            'aria-invalid': props.invalid || undefined,
            onChange: (event) => {
              if (!props.disabled) emit('update:modelValue', event.target.checked)
            }
          }),
          h(
            'span',
            { class: 'checkbox-box', 'aria-hidden': 'true' },
            props.indeterminate
              ? h('span', { class: 'checkbox-dash' })
              : props.modelValue
              ? h(YlIcon, { name: 'check', size: 10 })
              : null
          ),
          slots.default?.()
        ]
      )
  }
}

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

export const YlSelect = {
  name: 'YlSelect',
  inheritAttrs: false,
  props: {
    modelValue: { type: [String, Number, Array], default: null },
    options: { type: Array, default: () => [] },
    placeholder: { type: String, default: '请选择' },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    disabled: Boolean,
    readonly: Boolean,
    invalid: Boolean,
    clearable: Boolean,
    multiple: Boolean,
    searchable: Boolean,
    insetLabel: String,
    maxTagCount: { type: Number, default: 2 },
    searchLabel: { type: String, default: '搜索选项' },
    clearLabel: { type: String, default: '清除选择' },
    emptyText: { type: String, default: '暂无选项' },
  },
  emits: ['update:modelValue'],
  setup(props, { attrs, emit }) {
    const root = ref(null),
      trigger = ref(null),
      popup = ref(null),
      search = ref(null),
      query = ref('')
    const open = ref(false),
      active = ref(-1),
      position = ref({})
    const listId = `yl-select-${useId()}`
    const values = computed(() =>
      props.multiple
        ? Array.isArray(props.modelValue)
          ? props.modelValue
          : []
        : [props.modelValue]
    )
    const isSelected = (item) => values.value.includes(item.value)
    const selected = computed(() => props.options.filter(isSelected))
    const visible = (item) =>
      !query.value ||
      item.label.toLocaleLowerCase().includes(query.value.toLocaleLowerCase())
    const visibleCount = computed(() => props.options.filter(visible).length)
    watch(query, () => {
      active.value = props.options.findIndex(
        (item) => !item.disabled && visible(item)
      )
    })
    const blocked = computed(() => props.disabled || props.readonly)
    const canClear = computed(
      () => props.clearable && selected.value.length > 0 && !blocked.value
    )
    let disposed = false,
      prefix = '',
      prefixTimer
    function place() {
      const rect = trigger.value?.getBoundingClientRect?.()
      if (!rect || typeof window === 'undefined') return
      const below = window.innerHeight - rect.bottom - 8
      const above = rect.top - 8
      const up = below < 180 && above > below
      position.value = {
        position: 'fixed',
        left: `${Math.max(
          4,
          Math.min(rect.left, window.innerWidth - rect.width - 4)
        )}px`,
        width: `${rect.width}px`,
        top: up ? 'auto' : `${rect.bottom + 4}px`,
        bottom: up ? `${window.innerHeight - rect.top + 4}px` : 'auto',
        maxHeight: `${Math.max(48, Math.min(240, up ? above : below))}px`,
      }
    }
    function close() {
      open.value = false
      prefix = ''
      query.value = ''
    }
    async function reveal() {
      if (blocked.value) return
      active.value = props.options.findIndex(
        (item) => !item.disabled && visible(item) && isSelected(item)
      )
      if (active.value < 0)
        active.value = props.options.findIndex(
          (item) => !item.disabled && visible(item)
        )
      open.value = true
      await nextTick()
      if (!disposed && open.value) {
        place()
        await nextTick()
        if (!disposed && open.value && props.searchable)
          search.value?.focus({ preventScroll: true })
      }
    }
    function choose(index) {
      const item = props.options[index]
      if (blocked.value || !item || item.disabled) return
      if (props.multiple) {
        emit(
          'update:modelValue',
          isSelected(item)
            ? values.value.filter((value) => value !== item.value)
            : [...values.value, item.value]
        )
      } else {
        emit('update:modelValue', item.value)
        close()
        trigger.value?.focus()
      }
    }
    function move(direction) {
      for (let step = 1; step <= props.options.length; step++) {
        const index =
          (active.value + direction * step + props.options.length) %
          props.options.length
        if (!props.options[index].disabled && visible(props.options[index])) {
          active.value = index
          return
        }
      }
    }
    function keydown(event) {
      if (blocked.value) return
      if (event.key === 'Tab') {
        close()
        return
      }
      if (event.key === 'Escape') {
        if (open.value) {
          event.preventDefault()
          event.stopPropagation()
          close()
        }
        return
      }
      if (
        ['ArrowDown', 'ArrowUp', 'Home', 'End', 'Enter', ' '].includes(
          event.key
        )
      ) {
        event.preventDefault()
        if (!open.value) {
          reveal()
          return
        }
        if (event.key === 'Enter' || event.key === ' ') choose(active.value)
        else if (event.key === 'Home' || event.key === 'End') {
          active.value = event.key === 'Home' ? -1 : 0
          move(event.key === 'Home' ? 1 : -1)
        } else move(event.key === 'ArrowDown' ? 1 : -1)
      } else if (
        event.key.length === 1 &&
        !event.ctrlKey &&
        !event.metaKey &&
        !event.altKey
      ) {
        if (!open.value) reveal()
        prefix += event.key.toLocaleLowerCase()
        clearTimeout(prefixTimer)
        prefixTimer = setTimeout(() => {
          prefix = ''
        }, 700)
        const index = props.options.findIndex(
          (item) =>
            !item.disabled && item.label.toLocaleLowerCase().startsWith(prefix)
        )
        if (index >= 0) active.value = index
      }
    }
    function outside(event) {
      if (!root.value?.contains(event.target)) close()
    }
    watch(active, async () => {
      await nextTick()
      if (!disposed)
        popup.value
          ?.querySelector?.('[data-active="true"]')
          ?.scrollIntoView({ block: 'nearest' })
    })
    watch(blocked, (value) => {
      if (value) close()
    })
    watch(
      () => props.options,
      () => {
        if (open.value) reveal()
      },
      { deep: true }
    )
    watch(open, (value) => {
      if (typeof window === 'undefined') return
      const method = value ? 'addEventListener' : 'removeEventListener'
      window[method]('resize', place)
      window[method]('scroll', place, true)
      document[method]('pointerdown', outside, true)
    })
    onBeforeUnmount(() => {
      disposed = true
      clearTimeout(prefixTimer)
      if (typeof window !== 'undefined') {
        window.removeEventListener('resize', place)
        window.removeEventListener('scroll', place, true)
        document.removeEventListener('pointerdown', outside, true)
      }
    })
    return () =>
      h(
        'div',
        {
          ref: root,
          class: [
            'select',
            'yl-select',
            `select-${
              { lg: 'large', md: 'medium', sm: 'small', mini: 'mini' }[
                props.size
              ]
            }`,
            attrs.class,
            {
              focus: open.value,
              disabled: props.disabled,
              readonly: props.readonly,
              error: props.invalid,
            },
          ],
          style: attrs.style,
          onFocusout: (event) => {
            if (!root.value?.contains(event.relatedTarget)) close()
          },
        },
        [
          h(
            props.multiple ? 'div' : 'button',
            {
              ...Object.fromEntries(
                Object.entries(attrs).filter(
                  ([key]) => key !== 'class' && key !== 'style'
                )
              ),
              ref: trigger,
              type: props.multiple ? undefined : 'button',
              tabindex: props.multiple ? props.disabled ? -1 : 0 : undefined,
              'aria-disabled': props.disabled || undefined,
              class: 'select-trigger',
              role: 'combobox',
              disabled: props.disabled,
              'aria-readonly': props.readonly || undefined,
              'aria-invalid': props.invalid || undefined,
              'aria-expanded': open.value,
              'aria-haspopup': 'listbox',
              'aria-controls': listId,
              'aria-activedescendant':
                open.value && active.value >= 0
                  ? `${listId}-${active.value}`
                  : undefined,
              onClick: () => {
                if (open.value) close()
                else reveal()
              },
              onKeydown: keydown,
            },
            [
              props.insetLabel ? h('span', { class: 'yl-select-inset-label' }, props.insetLabel) : null,
              h(
                'span',
                {
                  class: selected.value.length
                    ? 'select-value'
                    : 'select-value select-placeholder',
                },
                props.multiple && selected.value.length ? [
                  ...selected.value.slice(0, Math.max(0, props.maxTagCount)).map(item => h('span', { class: 'yl-select-tag', key: item.value }, [
                    h('span', {}, item.label),
                    !blocked.value && !item.disabled ? h('button', { type: 'button', class: 'yl-select-tag-remove', 'aria-label': `移除 ${item.label}`,
                      onKeydown: event => event.stopPropagation(),
                      onClick: event => { event.stopPropagation(); if (!blocked.value) { emit('update:modelValue', values.value.filter(value => value !== item.value)); trigger.value?.focus() } }
                    }, h(YlIcon, { name: 'x', size: 10 })) : null
                  ])),
                  selected.value.length > props.maxTagCount ? h('span', { class: 'yl-select-tag', title: selected.value.slice(Math.max(0, props.maxTagCount)).map(item => item.label).join('、') }, `+${selected.value.length - Math.max(0, props.maxTagCount)}`) : null
                ] : selected.value.map((item) => item.label).join('、') || props.placeholder
              ),
              !canClear.value
                ? h(
                    'span',
                    { class: 'select-suffix' },
                    h(YlIcon, {
                      name: 'chevron-down',
                      class: ['select-arrow', { rotated: open.value }],
                    })
                  )
                : null,
            ]
          ),
          canClear.value
            ? h(
                'button',
                {
                  type: 'button',
                  class: 'yl-select-clear',
                  'aria-label': props.clearLabel,
                  title: props.clearLabel,
                  onClick: () => {
                    if (!blocked.value) {
                      emit('update:modelValue', props.multiple ? [] : null)
                      close()
                      trigger.value?.focus()
                    }
                  },
                },
                h(YlIcon, { name: 'x', size: 12 })
              )
            : null,
          h(
            'div',
            { ref: popup, class: 'select-dropdown', style: position.value },
            [
              props.searchable
                ? h('input', {
                    ref: search,
                    class: 'ui-input input-md yl-select-search',
                    value: query.value,
                    'aria-label': props.searchLabel,
                    role: 'combobox',
                    'aria-expanded': open.value,
                    'aria-controls': listId,
                    'aria-autocomplete': 'list',
                    'aria-activedescendant':
                      active.value >= 0
                        ? `${listId}-${active.value}`
                        : undefined,
                    onInput: (event) => {
                      query.value = event.target.value
                    },
                    onKeydown: (event) => {
                      if (
                        !event.isComposing &&
                        [
                          'ArrowDown',
                          'ArrowUp',
                          'Enter',
                          'Escape',
                          'Tab',
                        ].includes(event.key)
                      )
                        keydown(event)
                    },
                  })
                : null,
              h(
                'div',
                {
                  id: listId,
                  role: 'listbox',
                  'aria-multiselectable': props.multiple || undefined,
                  'aria-label': attrs['aria-label'] || props.placeholder,
                  'aria-labelledby': attrs['aria-labelledby'],
                  onMousedown: (event) => event.preventDefault(),
                },
                visibleCount.value
                  ? props.options.flatMap((item, index) => !visible(item) ? [] : [
                      item.group && props.options.slice(0, index).filter(visible).at(-1)?.group !== item.group
                        ? h('div', { class: 'yl-select-group-label', role: 'presentation' }, item.group) : null,
                      h('div', {
                        id: `${listId}-${index}`, key: `${typeof item.value}:${item.value}`,
                        role: 'option', 'aria-selected': isSelected(item), 'aria-disabled': item.disabled || undefined,
                        'aria-label': item.group ? `${item.group} ${item.label}` : undefined,
                        'data-active': active.value === index,
                        class: ['select-option', { selected: isSelected(item), disabled: item.disabled, 'yl-select-active': active.value === index }],
                        onClick: () => choose(index), onMousemove: () => { if (!item.disabled) active.value = index }
                      }, [h('span', item.label), isSelected(item) ? h(YlIcon, { name: 'check', class: 'select-option-check' }) : null])
                    ])
                  : h('div', { class: 'select-empty' }, props.emptyText)
              ),
            ]
          ),
        ]
      )
  },
}

export const YlPagination = {
  name: 'YlPagination',
  props: {
    modelValue: {
      type: Number,
      default: 1,
      validator: (value) => Number.isInteger(value) && value >= 1
    },
    total: {
      type: Number,
      required: true,
      validator: (value) => Number.isFinite(value) && value >= 0
    },
    pageSize: {
      type: Number,
      default: 20,
      validator: (value) => Number.isInteger(value) && value > 0
    },
    disabled: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['md', 'sm']) },
    showTotal: Boolean, showSizeChanger: Boolean, showJumper: Boolean, simple: Boolean,
    pageSizes: { type: Array, default: () => [10, 20, 50, 100] },
    label: { type: String, default: '分页' },
    previousLabel: { type: String, default: '上一页' },
    nextLabel: { type: String, default: '下一页' }
  },
  emits: ['update:modelValue', 'update:pageSize'],
  setup(props, { emit }) {
    const jump = ref('')
    const pages = computed(() => Math.max(1, Math.ceil(props.total / Math.max(1, props.pageSize))))
    const current = computed(() => Math.min(pages.value, Math.max(1, props.modelValue)))
    function go(page) {
      if (!props.disabled && Number.isInteger(page) && page >= 1 && page <= pages.value && page !== props.modelValue)
        emit('update:modelValue', page)
    }
    watch(
      [pages, () => props.modelValue],
      () => {
        if (current.value !== props.modelValue) emit('update:modelValue', current.value)
      },
      { immediate: true }
    )
    return () => {
      const visible = [
        ...new Set([1, current.value - 1, current.value, current.value + 1, pages.value])
      ]
        .filter((page) => page >= 1 && page <= pages.value)
        .sort((a, b) => a - b)
      const items = []
      visible.forEach((page, index) => {
        if (index && page - visible[index - 1] > 1)
          items.push(h('span', { class: 'page-item page-item-more', 'aria-hidden': 'true' }, '…'))
        items.push(
          h(
            'button',
            {
              type: 'button',
              class: ['page-item', { active: current.value === page }],
              'aria-current': current.value === page ? 'page' : undefined,
              disabled: props.disabled,
              onClick: () => go(page)
            },
            String(page)
          )
        )
      })
      return h(
        'nav',
        { class: ['pagination yl-pagination', props.size === 'sm' ? 'pagination-small' : 'pagination-default'], 'aria-label': props.label },
        [
          props.showTotal ? h('span', { class: 'pagination-total' }, `共 ${props.total} 项`) : null,
          h(
            'button',
            {
              type: 'button',
              class: 'page-item yl-page-direction',
              'aria-label': props.previousLabel,
              title: props.previousLabel,
              disabled: props.disabled || current.value === 1,
              onClick: () => go(current.value - 1)
            },
            h(YlIcon, { name: 'chevron-left' })
          ),
          ...(props.simple ? [h('span', { class: 'pagination-simple' }, `${current.value} / ${pages.value}`)] : items),
          h(
            'button',
            {
              type: 'button',
              class: 'page-item yl-page-direction',
              'aria-label': props.nextLabel,
              title: props.nextLabel,
              disabled: props.disabled || current.value === pages.value,
              onClick: () => go(current.value + 1)
            },
            h(YlIcon, { name: 'chevron-right' })
          ),
          props.showSizeChanger ? h(YlSelect, { size: props.size, modelValue: props.pageSize, disabled: props.disabled,
            'aria-label': '每页条数', options: [...new Set([props.pageSize, ...props.pageSizes])].filter(value => Number.isInteger(value) && value > 0).map(value => ({ value, label: `${value} 条/页` })),
            'onUpdate:modelValue': value => { if (!props.disabled && value !== props.pageSize) { emit('update:pageSize', value); emit('update:modelValue', 1) } } }) : null,
          props.showJumper ? h('label', { class: 'yl-pagination-jumper' }, ['跳至 ', h('input', { type: 'number', class: 'ui-input',
            value: jump.value, min: 1, max: pages.value, step: 1, disabled: props.disabled, 'aria-label': '跳转页码',
            onInput: event => { jump.value = event.target.value },
            onKeydown: event => { if (event.key === 'Enter') { event.preventDefault(); go(Number(jump.value)); jump.value = '' } } }), ' 页']) : null
        ]
      )
    }
  }
}

export const YlDialog = {
  name: 'YlDialog',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    size: { type: String, default: 'md', validator: oneOf(['sm', 'md', 'lg']) },
    busy: Boolean,
    closeLabel: { type: String, default: '关闭' }
  },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    const dialog = ref(null)
    const titleId = `yl-dialog-${useId()}`
    let opener = null
    let disposed = false
    function requestClose() {
      if (!props.busy) emit('update:modelValue', false)
    }
    watch(
      () => props.modelValue,
      async (open) => {
        await nextTick()
        if (disposed || !dialog.value || open !== props.modelValue) return
        if (open && !dialog.value.open) {
          opener = document.activeElement
          dialog.value.showModal()
        } else if (!open && dialog.value.open) {
          dialog.value.close()
          if (opener?.isConnected) opener.focus()
        }
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      if (dialog.value?.open) {
        dialog.value.close()
        if (opener?.isConnected) opener.focus()
      }
    })
    return () =>
      h(
        'dialog',
        {
          ref: dialog,
          class: ['modal', 'modal-standard', `modal-${props.size}`, 'yl-dialog'],
          'aria-labelledby': titleId,
          'aria-busy': props.busy || undefined,
          onCancel: (event) => {
            event.preventDefault()
            requestClose()
          },
          onClose: () => {
            if (!disposed && props.modelValue && !dialog.value?.open) requestClose()
          }
        },
        [
          h('div', { class: 'modal-header' }, [
            h('h2', { id: titleId, class: 'modal-title' }, props.title),
            h(
              'button',
              {
                type: 'button',
                class: 'modal-close',
                'aria-label': props.closeLabel,
                title: props.closeLabel,
                disabled: props.busy,
                onClick: requestClose
              },
              h(YlIcon, { name: 'x', size: 20 })
            )
          ]),
          h('div', { class: 'modal-body' }, slots.default?.()),
          slots.footer
            ? h('div', { class: 'modal-footer' }, slots.footer({ close: requestClose }))
            : null
        ]
      )
  }
}

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

export const YlLayout = {
  name: 'YlLayout',
  props: {
    direction: { type: String, default: 'vertical', validator: oneOf(['horizontal','vertical']) },
    heightMode: { type: String, default: 'flow', validator: oneOf(['flow','fill','viewport']) },
  },
  setup(props, { slots }) {
    return () => h('div', { class: ['yl-layout', 'yl-layout-' + props.direction, 'yl-layout-' + props.heightMode] }, slots.default?.())
  },
}

export const YlHeader = {
  name: 'YlHeader',
  props: { height: { type: [String, Number], default: null } },
  setup(props, { slots }) {
    return () => h('header', { class: 'yl-layout-header', style: { height: typeof props.height === 'number' ? props.height + 'px' : props.height } }, slots.default?.())
  },
}

export const YlAside = {
  name: 'YlAside',
  props: { width: { type: [String, Number], default: 200 }, scroll: { type: Boolean, default: true }, collapsed: Boolean },
  setup(props, { slots }) {
    return () => h('aside', { hidden: props.collapsed, class: ['yl-layout-aside', { 'yl-layout-scroll': props.scroll }], style: { width: typeof props.width === 'number' ? props.width + 'px' : props.width } }, slots.default?.())
  },
}

export const YlMain = {
  name: 'YlMain',
  props: { scroll: Boolean, as: { type: String, default: 'main', validator: oneOf(['main','section','div']) } },
  setup(props, { slots, expose }) {
    const element = ref(null)
    expose({ resetScroll() { if (element.value) { element.value.scrollTop = 0; element.value.scrollLeft = 0 } } })
    return () => h(props.as, { ref: element, class: ['yl-layout-main', { 'yl-layout-scroll': props.scroll }], tabindex: props.scroll ? 0 : undefined }, slots.default?.())
  },
}

export const YlPopover = {
  name: 'YlPopover',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    width: { type: Number, default: 320 },
    align: { type: String, default: 'end', validator: oneOf(['start', 'end']) },
    persistent: Boolean,
    id: String
  },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    // Keep the panel out of layout before the first measurement, including initial-open mounts.
    const root = ref(null),
      panel = ref(null),
      position = ref({ position: 'fixed' })
    const generatedId = `yl-popover-${useId()}`
    let disposed = false
    const close = () => { if (!props.persistent) emit('update:modelValue', false) }
    function place() {
      const rect = root.value?.getBoundingClientRect?.()
      if (!rect || typeof window === 'undefined') return
      const width = Math.min(props.width, window.innerWidth - 16)
      const below = Math.max(0, window.innerHeight - rect.bottom - 16)
      const above = Math.max(0, rect.top - 16)
      const height = panel.value?.getBoundingClientRect?.().height || 0
      const up = height > below && above > below
      const available = Math.min(window.innerHeight - 16, up ? above : below)
      position.value = {
        position: 'fixed',
        width: `${width}px`,
        left: `${Math.max(8, Math.min(props.align === 'start' ? rect.left : rect.right - width, window.innerWidth - width - 8))}px`,
        top: `${Math.max(8, up ? rect.top - Math.min(height, available) - 8 : rect.bottom + 8)}px`,
        maxHeight: `${Math.max(0, available)}px`
      }
    }
    function outside(event) {
      if (!root.value?.contains(event.target)) close()
    }
    function listen(enabled) {
      if (typeof window === 'undefined') return
      const method = enabled ? 'addEventListener' : 'removeEventListener'
      document[method]('pointerdown', outside, true)
      window[method]('resize', place)
      window[method]('scroll', place, true)
    }
    watch(
      () => props.modelValue,
      async (open) => {
        listen(open)
        await nextTick()
        if (!disposed && open && props.modelValue) {
          place()
          await nextTick()
          if (!disposed && props.modelValue) { place(); panel.value?.focus({ preventScroll: true }) }
        }
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      listen(false)
    })
    return () =>
      h(
        'div',
        {
          ref: root,
          class: 'yl-popover',
          onFocusout: (event) => {
            if (props.modelValue && !root.value?.contains(event.relatedTarget)) close()
          },
          onKeydown: (event) => {
            if (props.modelValue && !props.persistent && event.key === 'Escape') {
              event.preventDefault()
              event.stopPropagation()
              close()
              root.value?.querySelector?.('button')?.focus()
            }
          }
        },
        [
          slots.trigger?.({
            open: props.modelValue,
            toggle: () => { if (!props.persistent) emit('update:modelValue', !props.modelValue) },
            triggerAttrs: {
              'aria-haspopup': 'dialog',
              'aria-expanded': props.modelValue,
              'aria-controls': props.id || generatedId
            }
          }),
          h(
            'section',
            {
              id: props.id || generatedId,
              ref: panel,
              class: 'dropdown yl-popover-panel',
              style: [position.value, { display: props.modelValue ? undefined : 'none' }],
              role: 'dialog',
              'aria-label': props.title,
              tabindex: -1
            },
            props.modelValue ? slots.default?.({ close }) : null
          )
        ]
      )
  }
}

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

export const YlSkeleton = {
  name: 'YlSkeleton',
  props: {
    loading: { type: Boolean, default: true },
    rows: { type: Number, default: 4 },
    title: { type: Boolean, default: true },
    animated: { type: Boolean, default: true },
    label: { type: String, default: '正在加载内容' }
  },
  setup(props, { slots }) {
    return () => !props.loading ? slots.default?.() : h('div', {
      class: ['yl-skeleton', { 'yl-skeleton-animated': props.animated }], role: 'status', 'aria-label': props.label, 'aria-busy': 'true'
    }, [
      h('span', { class: 'yl-sr-only' }, props.label),
      h('div', { class: 'yl-skeleton-shapes', 'aria-hidden': 'true' }, [
        props.title ? h('div', { class: 'yl-skeleton-line yl-skeleton-title' }) : null,
        ...Array.from({ length: Math.min(20, Math.max(1, Math.floor(props.rows) || 1)) }, (_, index) => h('div', { key: index, class: 'yl-skeleton-line' }))
      ])
    ])
  }
}

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
          h(YlAside, { id: navigationId + '-desktop', collapsed: collapsed.value, class: 'yl-desktop-navigation' }, { default: () => props.navigationLoading ? h(YlSkeleton, { class: 'yl-shell-navigation-skeleton', title: false, rows: 6, label: '正在准备导航' }) : navigation('desktop') }),
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
          { default: () => props.navigationLoading ? h(YlSkeleton, { class: 'yl-shell-navigation-skeleton', title: false, rows: 6, label: '正在准备导航' }) : navigation('mobile') }
        ),
      ] })
  },
}

export const YlConfirmDialog = {
  name: 'YlConfirmDialog',
  props: {
    modelValue: Boolean,
    title: { type: String, required: true },
    description: { type: String, required: true },
    busy: Boolean,
    danger: Boolean,
    tone: { type: String, default: 'warning', validator: oneOf(['info', 'success', 'warning', 'error']) },
    showCancel: { type: Boolean, default: true },
    confirmLabel: { type: String, default: '确认' },
    cancelLabel: { type: String, default: '取消' }
  },
  emits: ['update:modelValue', 'confirm'],
  setup(props, { emit }) {
    const close = () => {
      if (!props.busy) emit('update:modelValue', false)
    }
    return () =>
      h(
        YlDialog,
        {
          modelValue: props.modelValue,
          title: props.title,
          busy: props.busy,
          size: 'sm',
          'onUpdate:modelValue': close
        },
        {
          default: () =>
            h('div', { class: 'yl-confirm-content' }, [
              h(YlIcon, { name: { info: 'info', success: 'circle-check', warning: 'triangle-alert', error: 'circle-x' }[props.danger ? 'error' : props.tone], class: `yl-confirm-${props.danger ? 'error' : props.tone}`, size: 20 }),
              h('p', props.description)
            ]),
          footer: () => [
            props.showCancel ? h(
              YlButton,
              { autofocus: true, disabled: props.busy, onClick: close },
              () => props.cancelLabel
            ) : null,
            h(
              YlButton,
              {
                variant: props.danger ? 'danger' : 'primary',
                loading: props.busy,
                onClick: () => {
                  if (!props.busy) emit('confirm')
                }
              },
              () => props.confirmLabel
            )
          ]
        }
      )
  }
}

export const YlMessage = {
  name: 'YlMessage',
  props: {
    modelValue: { type: Object, default: null },
    duration: { type: Number, default: 3000 },
    closeLabel: { type: String, default: '关闭提示' }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const element = ref(null)
    let timer,
      disposed = false
    const close = () => {
      clearTimeout(timer)
      emit('update:modelValue', null)
    }
    function startTimer() {
      clearTimeout(timer)
      if (props.modelValue && props.modelValue.type !== 'loading' && props.duration > 0) timer = setTimeout(close, props.duration)
    }
    watch(
      [() => props.modelValue, () => props.duration],
      async () => {
        startTimer()
        await nextTick()
        if (disposed || !element.value) return
        // Native manual popover puts feedback above modal dialogs without moving keyboard focus.
        if (props.modelValue) element.value.showPopover?.()
        else element.value.hidePopover?.()
      },
      { immediate: true }
    )
    onBeforeUnmount(() => {
      disposed = true
      clearTimeout(timer)
      element.value?.hidePopover?.()
    })
    return () => {
      const type = ['success', 'error', 'warning', 'info', 'loading'].includes(props.modelValue?.type)
        ? props.modelValue.type
        : 'info'
      return h(
        'div',
        {
          ref: element,
          popover: 'manual',
          class: ['message', `message-${type}`, 'yl-message'],
          style: { display: props.modelValue ? undefined : 'none' },
          role: type === 'error' ? 'alert' : 'status',
          'aria-atomic': 'true',
          onMouseenter: () => clearTimeout(timer),
          onMouseleave: startTimer,
          onFocusin: () => clearTimeout(timer),
          onFocusout: startTimer
        },
        props.modelValue
          ? [
              h(YlIcon, {
                name: {
                  success: 'circle-check',
                  error: 'circle-x',
                  warning: 'triangle-alert',
                  info: 'info', loading: 'loader-circle'
                }[type],
                class: ['message-icon', { 'yl-message-loading': type === 'loading' }],
                size: 20
              }),
              h('span', { class: 'message-content' }, props.modelValue.text),
              h(
                'button',
                {
                  type: 'button',
                  class: 'message-close',
                  'aria-label': props.closeLabel,
                  onClick: close
                },
                h(YlIcon, { name: 'x' })
              )
            ]
          : []
      )
    }
  }
}

// A page composition, not another App Shell. Caller owns querying, sorting and mutations.

export const YlSwitch = {
  name: 'YlSwitch',
  props: {
    modelValue: Boolean,
    disabled: Boolean,
    loading: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['md', 'sm']) },
    label: { type: String, required: true }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    return () =>
      h(
        'button',
        {
          type: 'button',
          role: 'switch',
          'aria-label': props.label,
          'aria-checked': props.modelValue,
          'aria-busy': props.loading || undefined,
          disabled: props.disabled || props.loading,
          class: [
            'switch',
            `switch-${props.size}`,
            { checked: props.modelValue, disabled: props.disabled, loading: props.loading }
          ],
          onClick: () => {
            if (!props.disabled && !props.loading) emit('update:modelValue', !props.modelValue)
          }
        },
        h('span', { class: 'switch-knob', 'aria-hidden': 'true' })
      )
  }
}

export const YlForm = {
  name: 'YlForm',
  props: { busy: Boolean, title: String, columns: { type: Number, default: 1, validator: oneOf([1, 2]) } },
  emits: ['submit'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        'form',
        {
          class: 'form',
          'aria-busy': props.busy || undefined,
          onSubmit: (event) => {
            event.preventDefault()
            if (!props.busy) emit('submit', event)
          }
        },
        [props.title ? h('h2', { class: 'form-title' }, props.title) : null,
          h('div', { class: ['yl-form-grid', { 'yl-form-grid-two': props.columns === 2 }] }, slots.default?.()),
          slots.actions ? h('div', { class: 'form-actions' }, slots.actions()) : null]
      )
  }
}

export const YlFormItem = {
  name: 'YlFormItem',
  props: {
    label: { type: String, required: true },
    for: { type: String, required: true },
    error: String,
    hint: String,
    success: String,
    required: Boolean
  },
  setup(props, { slots }) {
    return () =>
      h('div', { class: 'form-field' }, [
        h('label', { for: props.for, class: 'form-label' }, [
          props.label,
          props.required ? h('span', { 'aria-hidden': 'true', class: 'yl-required' }, ' *') : null
        ]),
        slots.default?.({
          id: props.for,
          'aria-describedby': props.error || props.success || props.hint ? `${props.for}-help` : undefined,
          'aria-invalid': !!props.error,
          required: props.required
        }),
        props.error || props.success || props.hint
          ? h(
              'p',
              {
                id: `${props.for}-help`,
                class: props.error ? 'form-error' : props.success ? 'form-success-text' : 'form-help',
                role: props.error ? 'alert' : undefined
              },
              props.error || props.success || props.hint
            )
          : null
      ])
  }
}

export const YlStatusTag = {
  name: 'YlStatusTag',
  props: {
    type: {
      type: String,
      default: 'stop',
      validator: oneOf(['success', 'warning', 'processing', 'stop', 'error', 'loading', 'waiting'])
    },
    filled: Boolean,
    variant: { type: String, validator: oneOf(['text', 'fill', 'outline']) }
  },
  setup(props, { slots }) {
    return () =>
      h(
        'span',
        {
          class: [
            'status-tag',
            `status-tag-${props.type}`,
            props.variant ? `status-tag-${props.variant}` : props.filled ? 'status-tag-fill' : 'status-tag-text'
          ]
        },
        [h('span', { class: ['yl-status-dot', { 'yl-status-loading': props.type === 'loading' }], 'aria-hidden': 'true' }), slots.default?.()]
      )
  }
}

export const YlAlert = {
  name: 'YlAlert',
  props: {
    title: { type: String, required: true },
    description: String,
    type: {
      type: String,
      default: 'info',
      validator: oneOf(['info', 'success', 'warning', 'error'])
    },
    closable: Boolean,
    banner: Boolean,
    closeLabel: { type: String, default: '关闭提示' }
  },
  emits: ['close'],
  setup(props, { emit, slots }) {
    return () =>
      h(
        'div',
        {
          class: ['alert', `alert-${props.type}`, { 'alert-with-desc': !!props.description, 'alert-banner': props.banner }],
          role: props.type === 'error' ? 'alert' : 'status'
        },
        [
          h(YlIcon, {
            name: {
              info: 'info',
              success: 'circle-check',
              warning: 'triangle-alert',
              error: 'circle-x'
            }[props.type],
            class: 'alert-icon',
            size: 20
          }),
          h('div', { class: 'alert-content' }, [
            h('span', { class: 'alert-title' }, props.title),
            props.description ? h('p', { class: 'alert-desc' }, props.description) : null,
            slots.default?.()
          ]),
          props.closable
            ? h(
                'button',
                {
                  type: 'button',
                  class: 'alert-close',
                  'aria-label': props.closeLabel,
                  onClick: () => emit('close')
                },
                h(YlIcon, { name: 'x' })
              )
            : null
        ]
      )
  }
}

export const YlBadge = {
  name: 'YlBadge',
  props: {
    count: { type: Number, default: 0 },
    max: { type: Number, default: 99 },
    dot: Boolean,
    showZero: Boolean,
    label: String
    ,size: { type: String, default: 'default', validator: oneOf(['small', 'default', 'large']) },
    muted: Boolean,
    status: { type: String, validator: oneOf(['success', 'warning', 'error', 'skip', 'awaiting']) }
  },
  setup(props, { slots }) {
    return () =>
      h('span', { class: ['badge-wrapper', { 'yl-badge-standalone': !slots.default }] }, [
        slots.default?.(),
        props.status || props.dot || props.count > 0 || props.showZero
          ? h(
              'span',
              {
                class: ['badge', props.muted ? 'badge-ash' : 'badge-active', `badge-${props.size}`,
                  props.status ? `badge-icon badge-icon-${props.status}` : props.dot ? 'badge-dot' : props.count > props.max ? 'badge-number-plus' : 'badge-number'],
                'aria-label': props.label || (props.status ? ({ success: '成功', warning: '警告', error: '错误', skip: '跳过', awaiting: '等待' })[props.status] : props.dot ? '有新消息' : `${props.count} 条`)
              },
              props.status ? h(YlIcon, { name: ({ success: 'check', warning: 'triangle-alert', error: 'x', skip: 'minus', awaiting: 'clock' })[props.status], size: props.size === 'small' ? 10 : props.size === 'large' ? 16 : 12 }) : props.dot ? '' : props.count > props.max ? `${props.max}+` : String(props.count)
            )
          : null
      ])
  }
}

export const YlAvatar = {
  name: 'YlAvatar',
  props: {
    name: { type: String, required: true },
    src: String,
    color: { type: String, default: 'primary', validator: oneOf(['red', 'beige', 'green', 'teal', 'purple', 'primary', 'grey']) },
    iconOnly: Boolean, capsule: Boolean, editable: Boolean, disabled: Boolean, bordered: Boolean,
    kind: { type: String, default: 'person', validator: oneOf(['person', 'department']) },
    subtitle: String,
    size: {
      type: String,
      default: 'default',
      validator: oneOf(['mini', 'default', 'medium', 'large'])
    }
  },
  emits: ['edit'],
  setup(props, { emit }) {
    const failed = ref(false)
    watch(
      () => props.src,
      () => {
        failed.value = false
      }
    )
    return () =>
      h(props.editable ? 'button' : 'span', { class: ['avatar', `avatar-${props.size}`, 'yl-avatar', { 'avatar-capsule': props.capsule, 'avatar-twoline': props.subtitle, 'yl-avatar-editable': props.editable, 'yl-avatar-department': props.kind === 'department' }],
        type: props.editable ? 'button' : undefined, disabled: props.editable ? props.disabled : undefined,
        'aria-label': props.editable ? `编辑 ${props.name}` : props.iconOnly ? props.name : undefined,
        onClick: () => { if (props.editable && !props.disabled) emit('edit') } }, [
        h(
          'span',
          { class: [`avatar-icon avatar-icon-${props.size} avatar-icon-${props.color}`, { 'avatar-icon-bordered': props.bordered }] },
          props.src && !failed.value
            ? h('img', {
                src: props.src,
                alt: props.name,
                onError: () => {
                  failed.value = true
                }
              })
            : h('span', { 'aria-label': props.name }, Array.from(props.name)[0] || '?')
        ),
        !props.iconOnly ? h('span', { class: 'avatar-info' }, [h('span', { class: 'avatar-name' }, props.name), props.subtitle ? h('span', { class: 'avatar-subtitle' }, props.subtitle) : null]) : null,
        props.editable ? h(YlIcon, { name: 'pencil', size: 14 }) : null
      ])
  }
}

export const YlProgress = {
  name: 'YlProgress',
  props: { value: { type: Number, default: 0 }, label: { type: String, required: true },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    status: { type: String, default: 'normal', validator: oneOf(['normal', 'success', 'warning', 'error']) } },
  setup(props) {
    return () => {
      const value = Number.isFinite(props.value) ? Math.min(100, Math.max(0, props.value)) : 0
      return h(
        'div',
        {
          class: ['progress progress-line yl-progress', `progress-${props.size === 'md' ? 'default' : props.size}`, `progress-${props.status}`],
          role: 'progressbar',
          'aria-label': props.label,
          'aria-valuemin': 0,
          'aria-valuemax': 100,
          'aria-valuenow': value
        },
        [
          h(
            'div',
            { class: 'progress-track' },
            h('div', { class: 'progress-fill', style: { width: `${value}%` } })
          ),
          props.size !== 'mini' ? h('span', { class: 'progress-text' }, `${value}%`) : null
        ]
      )
    }
  }
}

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

export const YlDrawer = {
  name: 'YlDrawer',
  props: { modelValue: Boolean, title: { type: String, required: true }, busy: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['mini', 'sm', 'md', 'lg']) } },
  emits: ['update:modelValue'],
  setup(props, { slots, emit }) {
    return () =>
      h(
        YlDialog,
        {
          modelValue: props.modelValue,
          title: props.title,
          busy: props.busy,
          class: ['yl-drawer', `yl-drawer-${props.size}`],
          style: { width: `${{ mini: 480, sm: 600, md: 720, lg: 1080 }[props.size]}px` },
          size: 'lg',
          'onUpdate:modelValue': (value) => emit('update:modelValue', value)
        },
        slots
      )
  }
}

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

export const YlTooltip = {
  name: 'YlTooltip',
  props: {
    text: { type: String, required: true },
    placement: {
      type: String,
      default: 'top',
      validator: oneOf(['top', 'bottom', 'left', 'right']),
    },
  },
  setup(props, { slots }) {
    const open = ref(false),
      root = ref(null),
      bubble = ref(null)
    const position = ref({ position: 'fixed', visibility: 'hidden' })
    const id = `yl-tip-${useId()}`
    let hovered = false,
      focused = false,
      disposed = false,
      closeTimer,
      observer
    function place() {
      const anchor = root.value?.getBoundingClientRect?.()
      const tip = bubble.value?.getBoundingClientRect?.()
      if (!anchor || !tip || typeof window === 'undefined') return
      const margin = 8,
        gap = 6
      const space = { top: anchor.top - margin - gap, bottom: window.innerHeight - anchor.bottom - margin - gap,
        left: anchor.left - margin - gap, right: window.innerWidth - anchor.right - margin - gap }
      const opposite = { top: 'bottom', bottom: 'top', left: 'right', right: 'left' }
      let side = props.placement
      const required = side === 'left' || side === 'right' ? tip.width : tip.height
      if (space[side] < required && space[opposite[side]] > space[side]) side = opposite[side]
      const left = side === 'left' ? anchor.left - tip.width - gap : side === 'right' ? anchor.right + gap : anchor.left + (anchor.width - tip.width) / 2
      const top = side === 'top' ? anchor.top - tip.height - gap : side === 'bottom' ? anchor.bottom + gap : anchor.top + (anchor.height - tip.height) / 2
      position.value = { position: 'fixed', visibility: 'visible',
        left: `${Math.max(margin, Math.min(left, window.innerWidth - tip.width - margin))}px`,
        top: `${Math.max(margin, Math.min(top, window.innerHeight - tip.height - margin))}px` }

    }
    function close() {
      clearTimeout(closeTimer)
      open.value = false
    }
    function escape(event) {
      if (open.value && event.key === 'Escape') {
        close()
        event.stopPropagation()
      }
    }
    function listen(enabled) {
      if (typeof window === 'undefined') return
      const method = enabled ? 'addEventListener' : 'removeEventListener'
      window[method]('resize', place)
      window[method]('scroll', place, true)
      document[method]('keydown', escape, true)
    }
    function reveal() {
      clearTimeout(closeTimer)
      if (props.text) open.value = true
    }
    function leave() {
      clearTimeout(closeTimer)
      // Allow crossing the small visual gap to read/hover a long tooltip.
      closeTimer = setTimeout(() => {
        if (!hovered && !focused) close()
      }, 100)
    }
    watch(open, async (value) => {
      listen(value)
      observer?.disconnect()
      if (!value) {
        bubble.value?.hidePopover?.()
        position.value = { position: 'fixed', visibility: 'hidden' }
        return
      }
      await nextTick()
      if (disposed || !open.value) return
      // Native top layer avoids clipping and transformed containing blocks, including dialogs.
      bubble.value?.showPopover?.()
      place()
      if (typeof ResizeObserver !== 'undefined') {
        observer = new ResizeObserver(place)
        observer.observe(root.value)
        observer.observe(bubble.value)
      }
    })
    watch(
      () => [props.text, props.placement],
      async () => {
        if (!props.text) close()
        else if (open.value) {
          await nextTick()
          if (!disposed && open.value) place()
        }
      }
    )
    onBeforeUnmount(() => {
      disposed = true
      clearTimeout(closeTimer)
      observer?.disconnect()
      listen(false)
      bubble.value?.hidePopover?.()
    })
    return () =>
      h(
        'span',
        {
          ref: root,
          class: 'yl-tooltip',
          onMouseenter: () => {
            hovered = true
            reveal()
          },
          onMouseleave: () => {
            hovered = false
            leave()
          },
          onFocusin: () => {
            focused = true
            reveal()
          },
          onFocusout: (event) => {
            if (root.value?.contains(event.relatedTarget)) return
            focused = false
            if (!hovered) close()
          },
          onKeydown: escape,
        },
        [
          slots.default?.({ 'aria-describedby': props.text ? id : undefined }),
          h(
            'span',
            {
              id,
              ref: bubble,
              role: 'tooltip',
              popover: 'manual',
              class: 'tooltip-bubble yl-tooltip-bubble',
              hidden: !open.value,
              style: position.value,
              onMouseenter: () => {
                hovered = true
                reveal()
              },
              onMouseleave: () => {
                hovered = false
                leave()
              },
            },
            props.text
          ),
        ]
      )
  },
}

export const YlTextarea = {
  name: 'YlTextarea',
  props: {
    modelValue: { type: String, default: '' },
    disabled: Boolean,
    readonly: Boolean,
    invalid: Boolean,
    rows: { type: Number, default: 3 }
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    let composing = false
    const update = (event) => {
      if (!composing && !props.disabled && !props.readonly)
        emit('update:modelValue', event.target.value)
    }
    return () =>
      h('textarea', {
        class: ['ui-input', 'input-md', 'yl-textarea', { 'input-error': props.invalid }],
        value: props.modelValue,
        disabled: props.disabled,
        readonly: props.readonly,
        rows: props.rows,
        'aria-invalid': props.invalid || undefined,
        onInput: update,
        onCompositionstart: () => {
          composing = true
        },
        onCompositionend: (event) => {
          composing = false
          update(event)
        }
      })
  }
}

export const YlInputNumber = {
  name: 'YlInputNumber',
  props: {
    modelValue: { type: Number, default: null },
    min: Number,
    max: Number,
    step: { type: Number, default: 1 },
    disabled: Boolean,
    readonly: Boolean,
    invalid: Boolean
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    return () =>
      h('input', {
        type: 'number',
        class: ['ui-input', 'input-md', { 'input-error': props.invalid }],
        value: props.modelValue ?? '',
        min: props.min,
        max: props.max,
        step: props.step,
        disabled: props.disabled,
        readonly: props.readonly,
        'aria-invalid': props.invalid || undefined,
        onInput: (event) => {
          if (props.disabled || props.readonly) return
          const value = event.target.value
          if (value === '') {
            if (!event.target.validity?.badInput) emit('update:modelValue', null)
            return
          }
          const number = Number(value)
          if (Number.isFinite(number)) emit('update:modelValue', number)
        }
      })
  }
}

export const YlRadioGroup = {
  name: 'YlRadioGroup',
  props: {
    modelValue: { type: [String, Number], default: null },
    options: { type: Array, default: () => [] },
    disabled: Boolean,
    label: { type: String, required: true },
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const name = `yl-radio-${useId()}`
    return () =>
      h('fieldset', { class: 'yl-radio-group', disabled: props.disabled }, [
        h('legend', props.label),
        h(
          'div',
          { class: 'radio-group' },
          props.options.map((item) =>
            h(
              'label',
              {
                class: ['radio', { disabled: props.disabled || item.disabled }],
                key: item.value,
              },
              [
                h('input', {
                  type: 'radio',
                  name,
                  value: item.value,
                  checked: props.modelValue === item.value,
                  disabled: props.disabled || item.disabled,
                  onChange: () => {
                    if (!props.disabled && !item.disabled)
                      emit('update:modelValue', item.value)
                  },
                }),
                item.label,
              ]
            )
          )
        ),
      ])
  },
}

export const YlFooter = {
  name: 'YlFooter',
  props: { height: { type: [String, Number], default: null } },
  setup(props, { slots }) {
    return () => h('footer', { class: 'yl-layout-footer', style: { height: typeof props.height === 'number' ? props.height + 'px' : props.height } }, slots.default?.())
  },
}

export const YlDatePicker = {
  name: 'YlDatePicker',
  inheritAttrs: false,
  props: {
    modelValue: { type: String, default: '' },
    mode: { type: String, default: 'date', validator: oneOf(['date', 'month', 'datetime-local']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    min: String, max: String,
    disabled: Boolean, readonly: Boolean, invalid: Boolean, clearable: Boolean,
    clearLabel: { type: String, default: '清除日期' }
  },
  emits: ['update:modelValue', 'change'],
  setup(props, { attrs, emit }) {
    const update = value => {
      if (props.disabled || props.readonly) return
      emit('update:modelValue', value)
      emit('change', value)
    }
    return () => {
      const { class: cls, style, ...inputAttrs } = attrs
      const canClear = props.clearable && props.modelValue && !props.disabled && !props.readonly
      return h('div', { class: ['yl-date-picker', { 'yl-date-picker-clearable': canClear }, cls], style,
        onClick: event => {
          if (canClear && event.target?.tagName === 'INPUT') {
            try { event.target.showPicker?.() } catch { /* Native editing remains available when picker access is unavailable. */ }
          }
        } }, [
        h(YlInput, { ...inputAttrs, type: props.mode, modelValue: props.modelValue,
          size: props.size, min: props.min, max: props.max, disabled: props.disabled,
          readonly: props.readonly, invalid: props.invalid, 'onUpdate:modelValue': update }),
        canClear
          ? h('button', { type: 'button', class: 'yl-date-clear', 'aria-label': props.clearLabel,
              onClick: () => update('') }, h(YlIcon, { name: 'x', size: 14 })) : null
      ])
    }
  }
}

export const YlDateRangePicker = {
  name: 'YlDateRangePicker',
  props: {
    modelValue: { type: Array, default: () => ['', ''] },
    mode: { type: String, default: 'date', validator: oneOf(['date', 'month', 'datetime-local']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) },
    min: String, max: String, disabled: Boolean, readonly: Boolean, invalid: Boolean, clearable: Boolean,
    startLabel: { type: String, default: '开始日期' },
    endLabel: { type: String, default: '结束日期' },
    orderError: { type: String, default: '结束日期不能早于开始日期' }
  },
  emits: ['update:modelValue', 'change'],
  setup(props, { emit }) {
    const errorId = useId()
    const reversed = computed(() => !!(props.modelValue[0] && props.modelValue[1] && props.modelValue[0] > props.modelValue[1]))
    const update = (index, value) => {
      if (props.disabled || props.readonly) return
      const range = [props.modelValue[0] || '', props.modelValue[1] || '']
      range[index] = value
      emit('update:modelValue', range)
      emit('change', range)
    }
    return () => h('div', { class: 'yl-date-range', role: 'group' }, [
      h('div', { class: 'yl-date-range-fields' }, [0, 1].map(index => h('label', { class: 'yl-date-range-field' }, [
        h('span', {}, index === 0 ? props.startLabel : props.endLabel),
        h(YlDatePicker, { modelValue: props.modelValue[index] || '', mode: props.mode, size: props.size,
          min: props.min, max: props.max, disabled: props.disabled, readonly: props.readonly,
          invalid: props.invalid || reversed.value, clearable: props.clearable,
          'aria-describedby': reversed.value ? errorId : undefined,
          'onUpdate:modelValue': value => update(index, value) })
      ]))),
      reversed.value ? h('p', { id: errorId, class: 'yl-date-range-error', role: 'status' }, props.orderError) : null
    ])
  }
}

export const YlUpload = {
  name: 'YlUpload',
  props: {
    modelValue: { type: Array, default: () => [] },
    request: Function,
    accept: { type: String, default: '' },
    multiple: Boolean, disabled: Boolean,
    autoUpload: { type: Boolean, default: true },
    maxSize: { type: Number, default: Infinity },
    limit: { type: Number, default: Infinity },
    listType: { type: String, default: 'text', validator: oneOf(['text', 'picture']) },
    label: { type: String, default: '选择文件' },
    hint: { type: String, default: '也可以将文件拖到这里' }
  },
  emits: ['update:modelValue', 'reject', 'success', 'error', 'remove'],
  setup(props, { emit, expose, slots }) {
    const input = ref(null)
    const files = ref([...props.modelValue])
    const message = ref('')
    const dragging = ref(false)
    const tasks = new Map()
    const previews = new Map()
    const prefix = useId()
    let serial = 0, disposed = false
    const release = id => {
      const task = tasks.get(id)
      tasks.delete(id)
      task?.abort()
      const url = previews.get(id)
      if (url) URL.revokeObjectURL(url)
      previews.delete(id)
    }
    const publish = next => { files.value = next; emit('update:modelValue', next) }
    const patch = (id, values) => publish(files.value.map(item => item.id === id ? { ...item, ...values } : item))
    watch(() => props.modelValue, next => {
      for (const item of files.value) {
        const replacement = next.find(candidate => candidate.id === item.id)
        if (!replacement || replacement.raw !== item.raw) release(item.id)
      }
      files.value = [...next]
    }, { flush: 'sync' })
    const cancel = id => {
      if (props.disabled || !tasks.has(id)) return
      const task = tasks.get(id)
      tasks.delete(id)
      task.abort()
      patch(id, { status: 'ready', progress: 0, error: '' })
    }
    const upload = async id => {
      const item = files.value.find(file => file.id === id)
      if (disposed || props.disabled || !props.request || !item?.raw || tasks.has(id) || item.status === 'success') return
      const task = new AbortController()
      tasks.set(id, task)
      const current = () => !disposed && tasks.get(id) === task && files.value.some(file => file.id === id && file.raw === item.raw)
      patch(id, { status: 'uploading', progress: 0, error: '' })
      try {
        const response = await props.request(item.raw, { signal: task.signal, onProgress: value => {
          if (current() && Number.isFinite(value)) patch(id, { progress: Math.round(Math.max(0, Math.min(99, value))) })
        } })
        if (!current()) return
        patch(id, { status: 'success', progress: 100, response })
        emit('success', { ...item, status: 'success', progress: 100, response })
      } catch (error) {
        if (!current()) return
        const reason = error?.message || '上传失败，请重试'
        patch(id, { status: 'error', error: reason })
        emit('error', { file: item, error })
      } finally {
        if (tasks.get(id) === task) tasks.delete(id)
      }
    }
    const remove = id => {
      if (props.disabled) return
      const item = files.value.find(file => file.id === id)
      if (!item) return
      release(id)
      publish(files.value.filter(file => file.id !== id))
      emit('remove', item)
    }
    const selectFiles = selected => {
      if (disposed || props.disabled) return
      const next = [...files.value], added = [], errors = []
      const rules = props.accept.toLowerCase().split(',').map(rule => rule.trim()).filter(Boolean)
      const selectedFiles = Array.from(selected || [])
      for (const [index, raw] of selectedFiles.entries()) {
        let reason = ''
        if (!props.multiple && index > 0) reason = '每次只能选择一个文件'
        else if (next.length >= props.limit) reason = '已达到文件数量上限'
        else if (raw.size > props.maxSize) reason = '文件大小超过限制'
        else if (rules.length && !rules.some(rule => rule.startsWith('.') ? raw.name.toLowerCase().endsWith(rule)
          : rule.endsWith('/*') ? raw.type.toLowerCase().startsWith(rule.slice(0, -1)) : raw.type.toLowerCase() === rule)) reason = '文件类型不符合要求'
        if (reason) { errors.push(raw.name + '：' + reason); emit('reject', { file: raw, reason }); continue }
        const item = { id: `${prefix}-file-${++serial}`, name: raw.name, size: raw.size, type: raw.type, raw, status: 'ready', progress: 0, error: '' }
        if (/^image\/(png|jpeg|gif|webp|avif|bmp)$/i.test(raw.type) && typeof URL !== 'undefined' && typeof URL.createObjectURL === 'function') {
          previews.set(item.id, URL.createObjectURL(raw))
        }
        next.push(item); added.push(item)
      }
      message.value = errors.join('；')
      publish(next)
      if (props.autoUpload && props.request) added.forEach(item => upload(item.id))
    }
    onBeforeUnmount(() => { disposed = true; for (const id of new Set([...tasks.keys(), ...previews.keys()])) release(id) })
    expose({ upload, cancel, remove, selectFiles })
    const action = (label, handler, name) => h(YlButton, { size: 'sm', variant: 'text', disabled: props.disabled, 'aria-label': `${label} ${name}`, onClick: handler }, () => label)
    const labels = { ready: '待上传', uploading: '上传中', success: '上传成功', error: '上传失败' }
    return () => h('div', { class: ['yl-upload', { 'yl-upload-disabled': props.disabled }] }, [
      h('div', { class: ['yl-upload-dropzone', { 'yl-upload-dragging': dragging.value && !props.disabled }],
        onDragover: event => { event.preventDefault(); if (!props.disabled) dragging.value = true },
        onDragleave: () => { dragging.value = false },
        onDrop: event => { event.preventDefault(); dragging.value = false; selectFiles(event.dataTransfer?.files) }
      }, [
        h('input', { ref: input, type: 'file', hidden: true, accept: props.accept, multiple: props.multiple, disabled: props.disabled,
          onChange: event => { selectFiles(event.target.files); event.target.value = '' } }),
        h(YlButton, { disabled: props.disabled, icon: 'plus', onClick: () => input.value?.click() }, () => props.label),
        h('div', { class: 'yl-upload-hint' }, slots.default?.() || props.hint)
      ]),
      h('p', { class: 'yl-upload-error', role: 'status' }, message.value),
      h('ul', { class: ['yl-upload-list', `yl-upload-list-${props.listType}`] }, files.value.map(item => h('li', { key: item.id, class: 'yl-upload-item' }, [
        props.listType === 'picture' && previews.has(item.id) ? h('img', { class: 'yl-upload-preview', src: previews.get(item.id), alt: item.name }) : null,
        h('div', { class: 'yl-upload-details' }, [
          h('div', { class: 'yl-upload-name' }, item.name),
          h('div', { class: 'yl-upload-status', 'aria-live': 'polite' }, `${Math.ceil(item.size / 1024)} KB · ${!props.request && item.status === 'ready' ? '已选择，尚未上传' : labels[item.status] || '已选择'}`),
          item.status === 'uploading' ? h(YlProgress, { value: item.progress, label: `${item.name} 上传进度` }) : null,
          item.status === 'error' ? h('div', { class: 'yl-upload-error', role: 'status' }, item.error) : null
        ]),
        h('div', { class: 'yl-upload-actions' }, [
          tasks.has(item.id) ? action('取消', () => cancel(item.id), item.name)
            : props.request && item.raw && item.status !== 'success' ? action(item.status === 'error' ? '重试' : '上传', () => upload(item.id), item.name) : null,
          action('移除', () => remove(item.id), item.name)
        ])
      ])))
    ])
  }
}

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

export const YlCascader = {
  name: 'YlCascader',
  props: { modelValue: { type: Array, default: () => [] }, options: { type: Array, default: () => [] },
    multiple: Boolean, searchable: Boolean, disabled: Boolean,
    label: { type: String, default: '级联选择' }, searchPlaceholder: { type: String, default: '搜索选项' },
    emptyText: { type: String, default: '暂无选项' } },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const active = ref([]), query = ref(''), controls = new Map()
    const tree = computed(() => {
      const visit = (items, path = [], disabled = false, ancestors = new Set()) => items.flatMap(item => {
        if (!item || ancestors.has(item)) return []
        const node = { ...item, path: [...path, item.value], disabled: disabled || !!item.disabled }
        node.children = visit(item.children || [], node.path, node.disabled, new Set([...ancestors, item]))
        return [node]
      })
      return visit(props.options)
    })
    const same = (a, b) => Array.isArray(a) && a.length === b.length && a.every((value, index) => value === b[index])
    const leaves = node => node.children.length ? node.children.flatMap(leaves) : [node]
    const selected = node => props.multiple ? props.modelValue.some(path => same(path, node.path)) : same(props.modelValue, node.path)
    const eligible = node => leaves(node).filter(leaf => !leaf.disabled)
    const checked = node => { const items = eligible(node); return items.length > 0 && items.every(selected) }
    const mixed = node => !checked(node) && eligible(node).some(selected)
    const choose = node => {
      if (props.disabled || node.disabled) return
      if (!props.multiple) { if (!node.children.length) emit('update:modelValue', [...node.path]); return }
      const paths = eligible(node).map(leaf => leaf.path), all = checked(node)
      const retained = props.modelValue.filter(path => !paths.some(candidate => same(path, candidate)))
      emit('update:modelValue', all ? retained : [...retained, ...paths])
    }
    const columns = computed(() => {
      if (query.value.trim()) {
        const term = query.value.trim().toLocaleLowerCase()
        const visit = (items, labels = []) => items.flatMap(node => node.children.length ? visit(node.children, [...labels, node.label])
          : [{ ...node, displayLabel: [...labels, node.label].join(' / ') }])
        return [visit(tree.value).filter(node => node.displayLabel.toLocaleLowerCase().includes(term))]
      }
      const result = [tree.value]
      let items = tree.value
      for (const value of active.value) {
        const node = items.find(item => item.value === value && !item.disabled)
        if (!node?.children.length) break
        result.push(node.children); items = node.children
      }
      return result
    })
    watch(() => props.modelValue, value => {
      const path = props.multiple ? value[0] : value
      if (Array.isArray(path) && path.length) active.value = path.slice(0, -1)
    }, { immediate: true })
    const controlKey = (column, node) => `${column}:${JSON.stringify(node.path)}`
    const expand = node => { if (!props.disabled && !node.disabled) { active.value = [...node.path]; if (!node.children.length) choose(node) } }
    const keydown = (event, node, column) => {
      if (props.disabled || node.disabled) return
      const items = columns.value[column].filter(item => !item.disabled), index = items.indexOf(node)
      let next
      if (event.key === 'Home') next = items[0]
      else if (event.key === 'End') next = items.at(-1)
      else if (event.key === 'ArrowDown' || event.key === 'ArrowUp') next = items[(index + (event.key === 'ArrowDown' ? 1 : -1) + items.length) % items.length]
      else if (event.key === 'ArrowRight' && node.children.length) {
        event.preventDefault(); active.value = [...node.path]
        nextTick(() => { const first = columns.value[column + 1]?.find(item => !item.disabled); if (first) controls.get(controlKey(column + 1, first))?.focus() })
      } else if (event.key === 'ArrowLeft' && column > 0) {
        event.preventDefault(); const parent = columns.value[column - 1].find(item => item.value === node.path[column - 1])
        active.value = node.path.slice(0, column - 1); if (parent) controls.get(controlKey(column - 1, parent))?.focus()
      }
      if (next) { event.preventDefault(); controls.get(controlKey(column, next))?.focus() }
    }
    return () => h('div', { class: 'cascader yl-cascader', role: 'group', 'aria-label': props.label }, [
      props.searchable ? h(YlInput, { modelValue: query.value, disabled: props.disabled, placeholder: props.searchPlaceholder,
        'aria-label': props.searchPlaceholder, 'onUpdate:modelValue': value => { query.value = value } }) : null,
      h('div', { class: 'yl-cascader-columns' }, columns.value.map((items, column) => h('ul', { class: 'cascader-panel yl-cascader-column', key: column }, items.length ? items.flatMap((node, index) => [
        node.dividerBefore ? h('li', { class: 'yl-cascader-divider', role: 'separator' }) : null,
        node.group && node.group !== items[index - 1]?.group ? h('li', { class: 'cascader-group-title' }, node.group) : null,
        h('li', { key: JSON.stringify(node.path), class: ['cascader-item yl-cascader-row', { active: active.value[column] === node.value, disabled: props.disabled || node.disabled }] }, [
          props.multiple ? h(YlCheckbox, { modelValue: checked(node), indeterminate: mixed(node), disabled: props.disabled || node.disabled || !eligible(node).length,
            'aria-label': `选择 ${node.displayLabel || node.label}`, 'onUpdate:modelValue': () => choose(node) }) : null,
          h('button', { type: 'button', class: 'yl-cascader-option', disabled: props.disabled || node.disabled,
            ref: el => { el ? controls.set(controlKey(column, node), el) : controls.delete(controlKey(column, node)) },
            'aria-expanded': node.children.length ? active.value[column] === node.value : undefined,
            'aria-pressed': !node.children.length ? selected(node) : undefined,
            onClick: () => expand(node), onKeydown: event => keydown(event, node, column) }, [
            node.icon ? h(YlIcon, { name: node.icon }) : null,
            h('span', { class: 'cascader-item-label' }, [h('span', {}, node.displayLabel || node.label), node.description ? h('small', { class: 'yl-cascader-description' }, node.description) : null]),
            node.children.length ? h(YlIcon, { name: 'chevron-right', size: 14 }) : null
          ])
        ])
      ]) : [h('li', { class: 'yl-cascader-empty', role: 'status' }, props.emptyText)])))
    ])
  }
}

export const YlPopconfirm = {
  name: 'YlPopconfirm',
  props: { modelValue: Boolean, title: { type: String, required: true }, description: String,
    tone: { type: String, default: 'warning', validator: oneOf(['warning', 'info', 'error']) },
    simple: Boolean, busy: Boolean, confirmText: { type: String, default: '确认' }, cancelText: { type: String, default: '取消' } },
  emits: ['update:modelValue', 'confirm', 'cancel'],
  setup(props, { slots, emit }) {
    const update = value => { if (!props.busy) emit('update:modelValue', value) }
    const cancel = () => { if (!props.busy) { emit('cancel'); update(false) } }
    return () => h(YlPopover, { modelValue: props.modelValue, title: props.title, persistent: props.busy, align: 'start',
      'onUpdate:modelValue': value => value ? update(true) : cancel() }, {
      trigger: scope => slots.trigger?.(scope),
      default: () => h('div', { class: ['popconfirm', 'yl-popconfirm', { 'popconfirm-simple': props.simple }], 'aria-busy': props.busy || undefined }, [
        h('div', { class: 'popconfirm-header' }, [!props.simple ? h(YlIcon, { name: props.tone === 'info' ? 'info' : props.tone === 'error' ? 'circle-x' : 'triangle-alert', size: 20, class: ['popconfirm-icon', `popconfirm-icon-${props.tone}`] }) : null,
          h('div', { class: 'popconfirm-content' }, [h('div', { class: 'popconfirm-title' }, props.title), props.description ? h('p', { class: 'popconfirm-description' }, props.description) : null, slots.default?.()])]),
        h('div', { class: 'popconfirm-footer' }, [
          h(YlButton, { size: 'sm', disabled: props.busy, onClick: cancel }, () => props.cancelText),
          h(YlButton, { size: 'sm', variant: props.tone === 'error' ? 'danger' : 'primary', loading: props.busy,
            onClick: () => { if (!props.busy) emit('confirm') } }, () => props.confirmText)
        ])
      ])
    })
  }
}

export const YlSegmentedPicker = {
  name: 'YlSegmentedPicker',
  props: { modelValue: [String, Number], options: { type: Array, default: () => [] },
    size: { type: String, default: 'md', validator: oneOf(sizes) }, disabled: Boolean,
    label: { type: String, required: true } },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const controls = new Map()
    const choose = item => { if (!props.disabled && !item.disabled) emit('update:modelValue', item.value) }
    const keydown = (event, item) => {
      const enabled = props.options.filter(option => !option.disabled)
      if (props.disabled || item.disabled || !enabled.length) return
      const index = enabled.indexOf(item)
      let next
      if (event.key === 'Home') next = enabled[0]
      else if (event.key === 'End') next = enabled.at(-1)
      else if (['ArrowRight', 'ArrowDown', 'ArrowLeft', 'ArrowUp'].includes(event.key)) {
        next = enabled[(index + (['ArrowRight', 'ArrowDown'].includes(event.key) ? 1 : -1) + enabled.length) % enabled.length]
      }
      if (next) { event.preventDefault(); choose(next); controls.get(next.value)?.focus() }
    }
    return () => {
      const entry = props.options.find(item => item.value === props.modelValue && !item.disabled) || props.options.find(item => !item.disabled)
      return h('div', { class: ['segmented-picker', `segmented-picker-${props.size === 'mini' ? 'xs' : props.size}`, 'yl-segmented'], role: 'radiogroup', 'aria-label': props.label },
        props.options.map(item => h('button', { key: item.value, ref: el => { el ? controls.set(item.value, el) : controls.delete(item.value) },
          type: 'button', role: 'radio', 'aria-checked': item.value === props.modelValue,
          disabled: props.disabled || item.disabled, tabindex: !props.disabled && item === entry ? 0 : -1,
          class: ['segmented-picker-item', { active: item.value === props.modelValue, disabled: props.disabled || item.disabled }],
          onClick: () => choose(item), onKeydown: event => keydown(event, item) }, item.label)))
    }
  }
}

export const YlSlider = {
  name: 'YlSlider',
  props: { modelValue: { type: [Number, Array], default: 0 }, range: Boolean,
    min: { type: Number, default: 0 }, max: { type: Number, default: 100 }, step: { type: Number, default: 1 },
    size: { type: String, default: 'md', validator: oneOf(['md', 'sm']) }, disabled: Boolean,
    label: { type: String, required: true }, startLabel: { type: String, default: '最小值' }, endLabel: { type: String, default: '最大值' } },
  emits: ['update:modelValue', 'change'],
  setup(props, { emit }) {
    const bounds = computed(() => {
      const min = Number.isFinite(props.min) ? props.min : 0
      return { min, max: Math.max(min, Number.isFinite(props.max) ? props.max : 100) }
    })
    const clamp = value => Math.max(bounds.value.min, Math.min(bounds.value.max, Number.isFinite(value) ? value : bounds.value.min))
    const values = computed(() => props.range ? [clamp(props.modelValue?.[0]), clamp(props.modelValue?.[1])].sort((a, b) => a - b) : [clamp(props.modelValue)])
    const percent = value => bounds.value.max === bounds.value.min ? 0 : (value - bounds.value.min) / (bounds.value.max - bounds.value.min) * 100
    const update = (index, event, commit = false) => {
      if (props.disabled || bounds.value.max <= bounds.value.min) return
      const next = [...values.value], value = clamp(Number(event.target.value))
      next[index] = props.range ? index === 0 ? Math.min(value, next[1]) : Math.max(value, next[0]) : value
      emit(commit ? 'change' : 'update:modelValue', props.range ? next : next[0])
    }
    return () => h('div', { class: ['slider', `slider-${props.size}`, 'yl-slider', { disabled: props.disabled, 'slider-range': props.range }], role: 'group', 'aria-label': props.label }, [
      h('div', { class: 'slider-track', 'aria-hidden': 'true' }, h('div', { class: 'slider-track-fill', style: {
        left: `${props.range ? percent(values.value[0]) : 0}%`, width: `${percent(values.value.at(-1)) - (props.range ? percent(values.value[0]) : 0)}%` } })),
      ...values.value.map((value, index) => h('input', { key: index, type: 'range', class: 'yl-slider-native',
        value, min: bounds.value.min, max: bounds.value.max, step: props.step > 0 ? props.step : 1,
        disabled: props.disabled || bounds.value.max <= bounds.value.min,
        'aria-label': props.range ? `${props.label} ${index ? props.endLabel : props.startLabel}` : props.label,
        'aria-valuemin': props.range && index === 1 ? values.value[0] : bounds.value.min,
        'aria-valuemax': props.range && index === 0 ? values.value[1] : bounds.value.max,
        onInput: event => update(index, event), onChange: event => update(index, event, true) }))
    ])
  }
}

export const YlSteps = {
  name: 'YlSteps',
  props: { items: { type: Array, default: () => [] }, current: { type: Number, default: 0 },
    direction: { type: String, default: 'horizontal', validator: oneOf(['horizontal', 'vertical']) },
    label: { type: String, default: '步骤' } },
  setup(props, { slots }) {
    return () => h('ol', { class: ['yl-steps', `yl-steps-${props.direction}`], 'aria-label': props.label }, props.items.map((item, index) => {
      const status = item.disabled ? 'disabled' : item.status || (index < props.current ? 'completed' : index === props.current ? 'active' : 'pending')
      return h('li', { key: item.key ?? index, class: ['yl-step', `yl-step-${status}`], 'aria-current': status === 'active' ? 'step' : undefined }, [
        h('span', { class: 'yl-step-indicator', 'aria-hidden': 'true' }, status === 'completed' || status === 'error' ? h(YlIcon, { name: status === 'completed' ? 'check' : 'x', size: 14 }) : index + 1),
        h('div', { class: 'yl-step-content' }, [h('span', { class: 'yl-step-title' }, item.title),
          h('span', { class: 'yl-sr-only' }, ({ completed: '已完成', active: '当前步骤', pending: '未开始', disabled: '不可用', error: '错误' })[status]),
          item.description ? h('p', { class: 'yl-step-description' }, item.description) : null,
          slots[`item-${item.key ?? index}`]?.({ item, index, status })]),
        index < props.items.length - 1 ? h('span', { class: 'yl-step-line', 'aria-hidden': 'true' }) : null
      ])
    }))
  }
}

export const YlTimePicker = {
  name: 'YlTimePicker', inheritAttrs: false,
  props: { modelValue: { type: [String, Array], default: '' }, range: Boolean,
    size: { type: String, default: 'md', validator: oneOf(['lg', 'md', 'sm']) },
    disabled: Boolean, readonly: Boolean, invalid: Boolean, min: String, max: String,
    step: { type: Number, default: 1 }, placeholder: { type: String, default: '请选择时间' },
    startLabel: { type: String, default: '开始时间' }, endLabel: { type: String, default: '结束时间' } },
  emits: ['update:modelValue'],
  setup(props, { attrs, emit }) {
    const update = (index, event) => {
      if (props.disabled || props.readonly) return
      if (!props.range) emit('update:modelValue', event.target.value)
      else { const value = [props.modelValue?.[0] || '', props.modelValue?.[1] || '']; value[index] = event.target.value; emit('update:modelValue', value) }
    }
    return () => {
      const { class: cls, style, ...inputAttrs } = attrs
      const values = props.range ? [props.modelValue?.[0] || '', props.modelValue?.[1] || ''] : [typeof props.modelValue === 'string' ? props.modelValue : '']
      return h('div', { class: ['timepicker', `timepicker-${props.size}`, 'yl-timepicker', cls,
        { 'timepicker-range': props.range, disabled: props.disabled, error: props.invalid }], style },
        values.flatMap((value, index) => [index ? h('span', { class: 'timepicker-separator', 'aria-hidden': 'true' }, '~') : null,
          h('span', { class: ['yl-timepicker-field', { 'yl-timepicker-empty': !value }] }, [
            h('input', { ...inputAttrs, id: index && inputAttrs.id ? `${inputAttrs.id}-end` : inputAttrs.id,
              class: 'timepicker-input', type: 'time', value, min: props.min, max: props.max, step: props.step,
              disabled: props.disabled, readonly: props.readonly, 'aria-invalid': props.invalid || undefined,
              'aria-label': props.range ? `${inputAttrs['aria-label'] || ''} ${index ? props.endLabel : props.startLabel}`.trim() : inputAttrs['aria-label'] || props.placeholder,
              onInput: event => update(index, event) }),
            !value ? h('span', { class: 'timepicker-placeholder', 'aria-hidden': 'true' }, props.range ? index ? props.endLabel : props.startLabel : props.placeholder) : null
          ])]))
    }
  }
}

export const YlCheckboxGroup = {
  name: 'YlCheckboxGroup',
  props: { modelValue: { type: Array, default: () => [] }, options: { type: Array, default: () => [] },
    disabled: Boolean, invalid: Boolean, direction: { type: String, default: 'horizontal', validator: oneOf(['horizontal', 'vertical']) },
    label: { type: String, required: true } },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    const update = (item, checked) => {
      if (props.disabled || item.disabled) return
      emit('update:modelValue', checked ? [...new Set([...props.modelValue, item.value])] : props.modelValue.filter(value => value !== item.value))
    }
    return () => h('div', { class: ['yl-checkbox-group', `yl-checkbox-group-${props.direction}`], role: 'group', 'aria-label': props.label, 'aria-invalid': props.invalid || undefined },
      props.options.map(item => h(YlCheckbox, { key: item.value, modelValue: props.modelValue.includes(item.value),
        disabled: props.disabled || item.disabled, invalid: props.invalid, 'onUpdate:modelValue': checked => update(item, checked) }, () => item.label)))
  }
}

export const YlButtonGroup = {
  name: 'YlButtonGroup',
  props: { label: { type: String, required: true } },
  setup(props, { slots }) { return () => h('div', { class: 'btn-group yl-button-group', role: 'group', 'aria-label': props.label }, slots.default?.()) }
}

export const YlDropdown = {
  name: 'YlDropdown',
  props: { modelValue: { type: [String, Number, Array], default: null }, open: { type: Boolean, default: undefined },
    items: { type: Array, default: () => [] }, multiple: Boolean, searchable: Boolean, disabled: Boolean,
    label: { type: String, default: '选择操作' }, width: { type: Number, default: 220 } },
  emits: ['update:modelValue', 'update:open', 'select'],
  setup(props, { emit, slots }) {
    const localOpen = ref(false)
    const isOpen = computed(() => props.open === undefined ? localOpen.value : props.open)
    const setOpen = value => { if (props.disabled && value) return; localOpen.value = value; emit('update:open', value) }
    const entries = computed(() => {
      const visit = (items, path = [], ancestors = new Set()) => items.flatMap(item => {
        if (!item || ancestors.has(item)) return []
        const next = [...path, item.value]
        return [{ item, path: next }, ...visit(item.children || [], next, new Set([...ancestors, item]))]
      })
      return visit(props.items)
    })
    const paths = computed(() => props.multiple ? entries.value.filter(entry => Array.isArray(props.modelValue) && props.modelValue.includes(entry.item.value)).map(entry => entry.path)
      : entries.value.find(entry => entry.item.value === props.modelValue)?.path || [])
    const select = value => {
      if (props.disabled) return
      if (props.multiple) emit('update:modelValue', value.map(path => path.at(-1)))
      else { const key = value.at(-1); emit('update:modelValue', key); emit('select', entries.value.find(entry => entry.item.value === key)?.item); setOpen(false) }
    }
    watch(() => props.disabled, value => { if (value) setOpen(false) })
    return () => h(YlPopover, { class: 'yl-dropdown', modelValue: isOpen.value, title: props.label, width: props.width, align: 'start',
      'onUpdate:modelValue': setOpen }, {
      trigger: scope => slots.trigger?.({ ...scope, disabled: props.disabled }) || h(YlButton, { disabled: props.disabled, ...scope.triggerAttrs, suffixIcon: 'chevron-down', onClick: scope.toggle }, () => props.label),
      default: () => h(YlCascader, { options: props.items, modelValue: paths.value, multiple: props.multiple, searchable: props.searchable,
        disabled: props.disabled, label: props.label, 'onUpdate:modelValue': select })
    })
  }
}

export const YlSplitButton = {
  name: 'YlSplitButton',
  props: { label: { type: String, required: true }, items: { type: Array, default: () => [] },
    variant: { type: String, default: 'primary', validator: oneOf(['primary', 'secondary']) },
    size: { type: String, default: 'md', validator: oneOf(sizes) }, disabled: Boolean, loading: Boolean,
    moreLabel: { type: String, default: '更多操作' } },
  emits: ['click', 'select'],
  setup(props, { emit }) {
    return () => h(YlButtonGroup, { label: props.label, class: 'yl-split-button' }, () => [
      h(YlButton, { variant: props.variant, size: props.size, disabled: props.disabled, loading: props.loading, onClick: event => emit('click', event) }, () => props.label),
      h(YlDropdown, { items: props.items, label: props.moreLabel, disabled: props.disabled || props.loading, onSelect: item => emit('select', item) }, {
        trigger: ({ triggerAttrs, toggle, disabled }) => h(YlButton, { ...triggerAttrs, variant: props.variant, size: props.size, disabled,
          icon: 'chevron-down', 'aria-label': props.moreLabel, onClick: toggle })
      })
    ])
  }
}

export const YlFormGroup = {
  name: 'YlFormGroup',
  props: { title: { type: String, required: true }, disabled: Boolean, columns: { type: Number, default: 1, validator: oneOf([1, 2]) } },
  setup(props, { slots }) { return () => h('fieldset', { class: 'form-section yl-form-group', disabled: props.disabled }, [
    h('legend', { class: 'form-section-title' }, props.title),
    h('div', { class: ['yl-form-grid', { 'yl-form-grid-two': props.columns === 2 }] }, slots.default?.())
  ]) }
}

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
