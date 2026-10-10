# ElColorPickerPanel

`ColorPickerPanel` is the core component of `ColorPicker`.

Kind: component. Import `ElColorPickerPanel` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/color-picker-panel.html#colorpickerpanel

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string | not declared | binding value |
| border | boolean | true | whether the color picker panel is bordered |
| disabled | boolean | false | whether to disable the color picker |
| show-alpha | boolean | false | whether to display the alpha slider |
| color-format | 'rgb' \| 'prgb' \| 'hex' \| 'hex3' \| 'hex4' \| 'hex6' \| 'hex8' \| 'name' \| 'hsl' \| 'hsv' | ^[enum]`'hex' (when show-alpha is false) \| 'rgb' (when show-alpha is true)` | color format of v-model |
| predefine | string[] | not declared | predefined color options |
| validate-event | boolean | true | whether to trigger form validation |
| hue-slider-class | string / string[] / Record<string, boolean> | not declared | class names will be passed to hue-slider |
| hue-slider-style | string / StyleValue | not declared | styles will be passed to hue-slider |

## Slots

- `footer`: content to append after the Input
