# ElTimelineItem

ElTimelineItem: native Element Plus component.

Kind: component. Import `ElTimelineItem` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/timeline.html#timeline-item

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| timestamp | string | '' | timestamp content |
| hide-timestamp | boolean | false | whether to show timestamp |
| center | boolean | false | whether vertically centered |
| placement | 'top' \| 'bottom' | bottom | position of timestamp |
| type | 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' | '' | node type |
| color | string | '' | background color of node |
| size | 'normal' \| 'large' | normal | node size |
| icon | string / Component | not declared | icon component |
| hollow | boolean | false | icon is hollow |

## Slots

- `default`: customize default content for timeline item
- `dot`: customize defined node for timeline item
