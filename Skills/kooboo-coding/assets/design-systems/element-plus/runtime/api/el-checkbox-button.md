# ElCheckboxButton

ElCheckboxButton: native Element Plus component.

Kind: component. Import `ElCheckboxButton` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/checkbox.html#checkboxbutton

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| value | string / number / boolean / object | not declared | value of the checkbox when used inside a `checkbox-group` |
| label | string / number / boolean / object | not declared | label of the checkbox when used inside a `checkbox-group`. If there's no value, `label` will act as `value` |
| true-value | string / number | not declared | value of the checkbox if it's checked |
| false-value | string / number | not declared | value of the checkbox if it's not checked |
| disabled | boolean | false | whether the checkbox is disabled |
| name | string | not declared | native 'name' attribute |
| checked | boolean | false | if the checkbox is checked |
| true-label | string / number | not declared | value of the checkbox if it's checked |
| false-label | string / number | not declared | value of the checkbox if it's not checked |

## Slots

- `default`: customize default content
