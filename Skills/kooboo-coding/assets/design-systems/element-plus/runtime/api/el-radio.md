# ElRadio

Single selection among multiple options.

Kind: component. Import `ElRadio` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/radio.html#radio

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / boolean | not declared | binding value |
| value | string / number / boolean | not declared | the value of Radio |
| label | string / number / boolean | not declared | the label of Radio. If there's no `value`, `label` will act as `value` |
| disabled | boolean | false | whether Radio is disabled |
| border | boolean | false | whether to add a border around Radio |
| size | 'large' \| 'default' \| 'small' | not declared | size of the Radio |
| name | string | not declared | native `name` attribute |

## Events

- `change`: triggers when the bound value changes

## Slots

- `default`: customize default content
