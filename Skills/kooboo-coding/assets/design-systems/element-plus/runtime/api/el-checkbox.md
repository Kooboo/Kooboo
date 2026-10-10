# ElCheckbox

A group of options for multiple choices.

Kind: component. Import `ElCheckbox` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/checkbox.html#checkbox

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / boolean | not declared | binding value |
| value | string / number / boolean / object | not declared | value of the Checkbox when used inside a `checkbox-group` |
| label | string / number / boolean / object | not declared | label of the Checkbox when used inside a `checkbox-group`. If there's no value, `label` will act as `value` |
| true-value | string / number | not declared | value of the Checkbox if it's checked |
| false-value | string / number | not declared | value of the Checkbox if it's not checked |
| disabled | boolean | false | whether the Checkbox is disabled |
| border | boolean | false | whether to add a border around Checkbox |
| size | 'large' \| 'default' \| 'small' | not declared | size of the Checkbox |
| name | string | not declared | native 'name' attribute |
| checked | boolean | false | if the Checkbox is checked |
| indeterminate | boolean | false | Set indeterminate state, only responsible for style control |
| validate-event | boolean | true | whether to trigger form validation |
| tabindex | string / number | not declared | input tabindex |
| id | string | not declared | input id |
| aria-controls | string | not declared | same as [aria-controls](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Attributes/aria-controls), takes effect when `indeterminate` is `true` |
| aria-label | string | not declared | native `aria-label` attribute |
| true-label | string / number | not declared | value of the Checkbox if it's checked |
| false-label | string / number | not declared | value of the Checkbox if it's not checked |
| controls | string | not declared | same as [aria-controls](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Attributes/aria-controls), takes effect when `indeterminate` is `true` |

## Events

- `change`: triggers when the binding value changes

## Slots

- `default`: customize default content
