# ElTag

Used for marking and selection.

Kind: component. Import `ElTag` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tag.html#tag

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| type | 'primary' \| 'success' \| 'info' \| 'warning' \| 'danger' | primary | type of Tag |
| closable | boolean | false | whether Tag can be removed |
| disable-transitions | boolean | false | whether to disable animations |
| hit | boolean | false | whether Tag has a highlighted border |
| color | string | not declared | background color of the Tag |
| size | 'large' \| 'default' \| 'small' | not declared | size of Tag |
| effect | 'dark' \| 'light' \| 'plain' | light | theme of Tag |
| round | boolean | false | whether Tag is rounded |

## Events

- `click`: triggers when Tag is clicked
- `close`: triggers when Tag is removed

## Slots

- `default`: customize default content
