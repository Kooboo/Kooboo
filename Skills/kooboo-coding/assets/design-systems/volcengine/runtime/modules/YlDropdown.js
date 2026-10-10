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
