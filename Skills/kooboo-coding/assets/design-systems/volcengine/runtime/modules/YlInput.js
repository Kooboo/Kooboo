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
