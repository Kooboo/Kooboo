# ElSlider

Drag the slider within a fixed range.

Kind: component. Import `ElSlider` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/slider.html#slider

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number / number[] | 0 | binding value |
| min | number | 0 | minimum value |
| max | number | 100 | maximum value |
| disabled | boolean | false | whether Slider is disabled |
| step | number / 'mark' | 1 | step size, can be a number or `'mark'`  to restrict values to marks. When set to `'mark'`, the `marks` attribute must be set |
| show-input | boolean | false | whether to display an input box, works when `range` is false and `step` is not `'mark'` |
| show-input-controls | boolean | true | whether to display control buttons when `show-input` is true |
| size | '' \| 'large' \| 'default' \| 'small' | default | size of the slider wrapper, will not work in vertical mode |
| input-size | '' \| 'large' \| 'default' \| 'small' | default | size of the input box, when set `size`, the default is the value of `size` |
| show-stops | boolean | false | whether to display breakpoints |
| show-tooltip | boolean | true | whether to display tooltip value |
| format-tooltip | (value: number) => number / string | not declared | format to display tooltip value |
| range | boolean | false | whether to select a range |
| vertical | boolean | false | vertical mode |
| height | string | not declared | slider height, required in vertical mode |
| aria-label | string | not declared | native `aria-label` attribute |
| range-start-label | string | not declared | when `range` is true, screen reader label for the start of the range |
| range-end-label | string | not declared | when `range` is true, screen reader label for the end of the range |
| format-value-text | (value: number) => string | not declared | format to display the `aria-valuenow` attribute for screen readers |
| tooltip-class | string | not declared | custom class name for the tooltip |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | top | position of Tooltip |
| marks | SliderMarks | not declared | marks, type of key must be `number` and must in closed interval `[min, max]`, each mark can custom style |
| validate-event | boolean | true | whether to trigger form validation |
| persistent | boolean | true | when slider tooltip inactive and `persistent` is `false` , tooltip will be destroyed. `persistent` always be `false` when `show-tooltip ` is `false` |
| label | string | not declared | native `aria-label` attribute |

## Events

- `change`: triggers when the value changes (if the mouse is being dragged, this event only fires when the mouse is released)
- `input`: triggers when the data changes (It'll be emitted in real time during sliding)
