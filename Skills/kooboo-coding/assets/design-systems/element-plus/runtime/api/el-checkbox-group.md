# ElCheckboxGroup

ElCheckboxGroup: native Element Plus component.

Kind: component. Import `ElCheckboxGroup` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/checkbox.html#checkboxgroup

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string[] / number[] | [] | binding value |
| size | 'large' \| 'default' \| 'small' | not declared | size of checkbox |
| disabled | boolean | false | whether the nesting checkboxes are disabled |
| min | number | not declared | minimum number of checkbox checked |
| max | number | not declared | maximum number of checkbox checked |
| aria-label | string | not declared | native `aria-label` attribute |
| text-color | string | #ffffff | font color when button is active |
| fill | string | #409eff | border and background color when button is active |
| tag | string | div | element tag of the checkbox group |
| validate-event | boolean | true | whether to trigger form validation |
| label | string | not declared | native `aria-label` attribute |
| options | Array<{[key: string]: any}> | not declared | data of the options, the key of `value` and `label` and `disabled` can be customize by `props` |
| props | { value?: string, label?: string, disabled?: string} | {value: 'value', label: 'label', disabled: 'disabled'} | configuration options |
| type | 'checkbox' \| 'button' | 'checkbox' | component type to render options (e.g. `'button'`) |

## Events

- `change`: triggers when the binding value changes

## Slots

- `default`: customize default content
