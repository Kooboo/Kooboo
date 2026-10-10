# ElStep

ElStep: native Element Plus component.

Kind: component. Import `ElStep` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/steps.html#step

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| title | string | '' | step title |
| description | string | '' | step description |
| icon | string / Component | not declared | step custom icon. Icons can be passed via named slot as well |
| status | '' \| 'wait' \| 'process' \| 'finish' \| 'error' \| 'success' | '' | current status. It will be automatically set by Steps if not configured. |

## Slots

- `icon`: custom icon
- `title`: step title
- `description`: step description
