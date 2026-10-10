# ElProgress

Progress is used to show the progress of current operation, and inform the user the current status.

Kind: component. Import `ElProgress` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/progress.html#progress

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| percentage |  | 0 | percentage |
| type | 'line' \| 'circle' \| 'dashboard' | line | the type of progress bar |
| stroke-width | number | 6 | the width of progress bar |
| text-inside | boolean | false | whether to place the percentage inside progress bar, only works when `type` is 'line' |
| status | 'success' \| 'exception' \| 'warning' | not declared | the current status of progress bar |
| indeterminate | boolean | false | set indeterminate progress |
| duration | number | 3 | control the animation duration of indeterminate progress or striped flow progress |
| color | string / (percentage: number) => string / { color: string; percentage: number }[] | '' | background color of progress bar. Overrides `status` prop |
| width | number | 126 | the canvas width of circle progress bar |
| show-text | boolean | true | whether to show percentage |
| stroke-linecap | 'butt' \| 'round' \| 'square' | round | circle/dashboard type shape at the end path |
| format | (percentage: number) => string | not declared | custom text format |
| striped | boolean | false | stripe over the progress bar's color |
| striped-flow | boolean | false | get the stripes to flow |

## Slots

- `default`: Customized content
