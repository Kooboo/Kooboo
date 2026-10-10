# ElResult

Used to give feedback on the result of user's operation or access exception.

Kind: component. Import `ElResult` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/result.html#result

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| title | string | '' | title of result |
| sub-title | string | '' | sub title of result |
| icon | 'primary'  \| 'success' \| 'warning' \| 'info' \| 'error' | info | icon type of result |

## Slots

- `icon`: content as result icon
- `title`: content as result title
- `sub-title`: content as result sub title
- `extra`: content as result extra area
