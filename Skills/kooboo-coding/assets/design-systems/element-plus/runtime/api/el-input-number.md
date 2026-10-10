# ElInputNumber

Input numerical values with a customizable range.

Kind: component. Import `ElInputNumber` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/input-number.html#input-number

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number / null | not declared | binding value |
| min | number | Number.MIN_SAFE_INTEGER | the minimum allowed value |
| max | number | Number.MAX_SAFE_INTEGER | the maximum allowed value |
| step | number | 1 | incremental step |
| step-strictly | boolean | false | whether input value can only be multiple of step |
| precision | number | not declared | precision of input value |
| size | 'large' \| 'default' \| 'small' | default | size of the component |
| readonly | boolean | false | same as `readonly` in native input |
| disabled | boolean | false | whether the component is disabled |
| controls | boolean | true | whether to enable the control buttons |
| controls-position | '' \| 'right' | not declared | position of the control buttons |
| name | string | not declared | same as `name` in native input |
| aria-label | string | not declared | same as `aria-label` in native input |
| placeholder | string | not declared | same as `placeholder` in native input |
| id | string | not declared | same as `id` in native input |
| value-on-clear | number / null / 'min' \| 'max' | not declared | value should be set when input box is cleared |
| validate-event | boolean | true | whether to trigger form validation |
| label | string | not declared | same as `aria-label` in native input |
| inputmode | string | not declared | same as `inputmode` in native input |
| align | 'left' \| 'center' \| 'right' | 'center' | alignment for the inner input text |
| disabled-scientific | boolean | false | disables input of scientific notation (e.g. 'e') |
| tabindex | string / number | 0 | same as `tabindex` in native input |
| formatter | (value: string) => string | not declared | specifies the format of the value presented in the input |
| parser | (value: string) => string | not declared | specifies the value extracted from the formatted input |

## Events

- `change`: triggers when the value changes
- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses

## Slots

- `decrease-icon`: custom input box button decrease icon
- `increase-icon`: custom input box button increase icon
- `prefix`: content as Input prefix
- `suffix`: content as Input suffix
