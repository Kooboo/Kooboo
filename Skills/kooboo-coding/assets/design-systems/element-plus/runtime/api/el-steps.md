# ElSteps

Guide the user to complete tasks in accordance with the process. Its steps can be set according to the actual application scenario and the number of the steps can't be less than 2.

Kind: component. Import `ElSteps` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/steps.html#steps

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| space | number / string | '' | the spacing of each step, will be responsive if omitted. Supports percentage. |
| direction | 'vertical' \| 'horizontal' | horizontal | display direction |
| active | number | 0 | current activation step |
| process-status | 'wait' \| 'process' \| 'finish' \| 'error' \| 'success' | process | status of current step |
| finish-status | 'wait' \| 'process' \| 'finish' \| 'error' \| 'success' | finish | status of end step |
| align-center | boolean | not declared | center title and description |
| simple | boolean | not declared | whether to apply simple theme |

## Events

- `change`: triggers when the active step changes

## Slots

- `default`: customize default content
