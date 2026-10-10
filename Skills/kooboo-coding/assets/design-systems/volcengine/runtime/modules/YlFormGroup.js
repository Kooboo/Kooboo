export const YlFormGroup = {
  name: 'YlFormGroup',
  props: { title: { type: String, required: true }, disabled: Boolean, columns: { type: Number, default: 1, validator: oneOf([1, 2]) } },
  setup(props, { slots }) { return () => h('fieldset', { class: 'form-section yl-form-group', disabled: props.disabled }, [
    h('legend', { class: 'form-section-title' }, props.title),
    h('div', { class: ['yl-form-grid', { 'yl-form-grid-two': props.columns === 2 }] }, slots.default?.())
  ]) }
}
