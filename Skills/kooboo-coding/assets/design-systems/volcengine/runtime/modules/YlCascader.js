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
