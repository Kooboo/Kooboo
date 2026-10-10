# ElLink

Text hyperlink

Kind: component. Import `ElLink` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/link.html#link

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| type | 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' \| 'default' | default | type |
| underline | 'always' \| 'hover' \| 'never' / boolean | hover | when underlines should appear |
| disabled | boolean | false | whether the component is disabled |
| href | string | not declared | same as native hyperlink's `href` |
| target | '_blank' \| '_parent' \| '_self' \| '_top' | \_self | same as native hyperlink's `target` |
| icon | string / Component | not declared | icon component |

## Slots

- `default`: customize default content
- `icon`: customize icon component
