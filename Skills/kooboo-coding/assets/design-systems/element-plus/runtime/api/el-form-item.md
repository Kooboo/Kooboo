# ElFormItem

ElFormItem: native Element Plus component.

Kind: component. Import `ElFormItem` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/form.html#formitem

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| prop | string / string&#91;&#93; | not declared | A key of `model`. It could be a path of the property (e.g `a.b.0` or `['a', 'b', '0']`). In the use of `validate` and `resetFields` method, the attribute is required. |
| label | string | not declared | Label text. |
| label-position | 'left' \| 'right' \| 'top' | '' | Position of item label. If set to `'left'` or `'right'`, `label-width` prop is also required. Default extend `label-position` of `form`. |
| label-width | string / number | not declared | Width of label, e.g. `'50px'`. `'auto'` is supported. |
| required | boolean | not declared | Whether the field is required or not, will be determined by validation rules if omitted. |
| rules | Arrayable<FormItemRule> | not declared | Validation rules of form, see the [following table](#formitemrule), more advanced usage at [async-validator](https://github.com/yiminghe/async-validator). |
| error | string | not declared | Field error message, set its value and the field will validate error and show this message immediately. |
| show-message | boolean | true | Whether to show the error message. |
| inline-message | boolean | false | Inline style validate message. |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | Control the size of components in this form-item. |
| for | string | not declared | Same as for in native label. |
| validate-status | '' \| 'error' \| 'validating' \| 'success' | not declared | Validation state of formItem. |

## Slots

- `default`: Content of Form Item.
- `label`: Custom content to display on label.
- `error`: Custom content to display validation message.
