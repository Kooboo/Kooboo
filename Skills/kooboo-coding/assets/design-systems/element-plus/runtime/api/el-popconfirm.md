# ElPopconfirm

A simple confirmation dialog of an element click action.

Kind: component. Import `ElPopconfirm` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/popconfirm.html#popconfirm

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| title | string | not declared | Title |
| effect | 'dark' \| 'light' / string | light | Tooltip theme, built-in theme: `dark` / `light` |
| confirm-button-text | string | not declared | Confirm button text |
| cancel-button-text | string | not declared | Cancel button text |
| confirm-button-type | 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' \| 'text' | primary | Confirm button type |
| cancel-button-type | 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' \| 'text' | text | Cancel button type |
| icon | string / Component | QuestionFilled | Icon Component |
| icon-color | string | #f90 | Icon color |
| hide-icon | boolean | false | is hide Icon |
| hide-after | number | 200 | delay of disappear, in millisecond |
| teleported | boolean | true | whether popconfirm is teleported to the body |
| persistent | boolean | false | when popconfirm inactive and `persistent` is `false` , popconfirm will be destroyed |
| width | string / number | 150 | popconfirm width, min width 150px |
| [tooltip] |  | not declared | Inherits all attributes from Tooltip, except: `popper-class`, `popper-style`, `fallback-placements` |

## Events

- `confirm`: triggers when click confirm button
- `cancel`: triggers when click cancel button

## Slots

- `reference`: HTML element that triggers Popconfirm
- `actions`: content of the Popconfirm footer
