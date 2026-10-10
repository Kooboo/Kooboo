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
