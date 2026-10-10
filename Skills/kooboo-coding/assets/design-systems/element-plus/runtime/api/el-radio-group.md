# ElRadioGroup

ElRadioGroup: native Element Plus component.

Kind: component. Import `ElRadioGroup` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/radio.html#radiogroup

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / boolean | not declared | binding value |
| size | string | default | the size of radio buttons or bordered radios |
| disabled | boolean | false | whether the nesting radios are disabled |
| validate-event | boolean | true | whether to trigger form validation |
| text-color | string | #ffffff | font color when button is active |
| fill | string | #409eff | border and background color when button is active |
| aria-label | string | not declared | same as `aria-label` in RadioGroup |
| name | string | not declared | native `name` attribute |
| id | string | not declared | native `id` attribute |
| label | string | not declared | same as `aria-label` in RadioGroup |
| options | Array<{[key: string]: any}> | not declared | data of the options, the key of `value` and `label` and `disabled` can be customize by `props` |
| props | { value?: string, label?: string, disabled?: string} | {value: 'value', label: 'label', disabled: 'disabled'} | configuration options |
| type | 'radio' \| 'button' | 'radio' | component type to render options (e.g. `'button'`) |

## Events

- `change`: triggers when the bound value changes

## Slots

- `default`: customize default content
