# ElAlert

Displays important alert messages.

Kind: component. Import `ElAlert` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/alert.html#alert

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| title | string | not declared | alert title. |
| type | 'primary'  \| 'success' \| 'warning' \| 'info' \| 'error' | info | alert type. |
| description | string | not declared | descriptive text. |
| closable | boolean | true | whether alert can be dismissed. |
| center | boolean | false | whether content is placed in the center. |
| close-text | string | not declared | customized close button text. |
| show-icon | boolean | false | whether a type icon is displayed. |
| effect | 'light' \| 'dark' | light | theme style. |

## Events

- `close`: trigger when alert is closed.

## Slots

- `default`: content of the alert description.
- `title`: content of the alert title.
- `icon`: content of the alert icon.
