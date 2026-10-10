# ElColorPicker

ColorPicker is a color selector supporting multiple color formats.

Kind: component. Import `ElColorPicker` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/color-picker.html#colorpicker

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string | not declared | binding value |
| disabled | boolean | false | whether to disable the ColorPicker |
| clearable | boolean | true | whether to show clear button |
| size | 'large' \| 'default' \| 'small' | not declared | size of ColorPicker |
| show-alpha | boolean | false | whether to display the alpha slider |
| color-format | 'rgb' \| 'prgb' \| 'hex' \| 'hex3' \| 'hex4' \| 'hex6' \| 'hex8' \| 'name' \| 'hsl' \| 'hsv' | ^[enum]`'hex' (when show-alpha is false) \| 'rgb' (when show-alpha is true)` | color format of v-model |
| popper-class | string / object | '' | custom class name for ColorPicker's dropdown |
| popper-style | string / object | not declared | custom style for ColorPicker's dropdown |
| predefine | string[] | not declared | predefined color options |
| validate-event | boolean | true | whether to trigger form validation |
| tabindex | string / number | 0 | ColorPicker tabindex |
| aria-label | string | not declared | ColorPicker aria-label |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| id | string | not declared | ColorPicker id |
| teleported | boolean | true | whether color-picker popper is teleported to the body |
| label | string | not declared | ColorPicker aria-label |
| persistent | boolean | true | when color-picker inactive and persistent is false, the color panel will be destroyed |
| append-to | CSSSelector / HTMLElement | not declared | which element the color-picker panel appends to |

## Events

- `change`: triggers when input value changes
- `active-change`: triggers when the current active color changes
- `focus`: triggers when Component focuses
- `blur`: triggers when Component blurs
- `clear`: triggers when the clear button is clicked
