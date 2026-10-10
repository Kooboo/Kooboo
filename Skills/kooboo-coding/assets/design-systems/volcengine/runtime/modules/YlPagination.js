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
