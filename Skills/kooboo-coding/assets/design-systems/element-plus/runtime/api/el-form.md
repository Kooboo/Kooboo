# ElForm

Form consists of `input`, `radio`, `select`, `checkbox` and so on. With form, you can collect, verify and submit data.

Kind: component. Import `ElForm` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/form.html#form

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model | Record<string, any> | not declared | Data of form component. |
| rules | FormRules | not declared | Validation rules of form. |
| inline | boolean | false | Whether the form is inline. |
| label-position | 'left' \| 'right' \| 'top' | right | Position of label. If set to `'left'` or `'right'`, `label-width` prop is also required. |
| label-width | string / number | '' | Width of label, e.g. `'50px'`. All its direct child form items will inherit this value. `auto` is supported. |
| label-suffix | string | '' | Suffix of the label. |
| hide-required-asterisk | boolean | false | Whether to hide required fields should have a red asterisk (star) beside their labels. |
| require-asterisk-position | 'left' \| 'right' | left | Position of asterisk. |
| show-message | boolean | true | Whether to show the error message. |
| inline-message | boolean | false | Whether to display the error message inline with the form item. |
| status-icon | boolean | false | Whether to display an icon indicating the validation result. |
| validate-on-rule-change | boolean | true | Whether to trigger validation when the `rules` prop is changed. |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | Control the size of components in this form. |
| disabled | boolean | false | Whether to disable all components in this form. Before , if set to `true`, it will override the `disabled` prop of the inner component. After , the configuration of the internal components takes precedence. |
| scroll-to-error | boolean | false | When validation fails, scroll to the first error form entry. |
| scroll-into-view-options | ScrollIntoViewOptions / boolean | true | When validation fails, it scrolls to the first error item based on the scrollIntoView option. [scrollIntoView](https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView). |

## Events

- `validate`: triggers after a form item is validated

## Slots

- `default`: customize default content

## Usage and state ownership

Pass a reactive model and rules; each ElFormItem prop points to the corresponding model field. Use formRef.validate() before mutation; validation rejection is not a save success. Busy state and server errors belong to the business. Use resetFields()/clearValidate() intentionally; resetFields restores initial field values. Put action buttons outside field grids.

```js
try { await formRef.value.validate(); } catch { return; }
await saveRecord(model);
```
