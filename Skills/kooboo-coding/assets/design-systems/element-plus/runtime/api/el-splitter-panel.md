# ElSplitterPanel

ElSplitterPanel: native Element Plus component.

Kind: component. Import `ElSplitterPanel` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/splitter.html#splitterpanel

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| size | string / number | not declared | Size of the panel (in pixels or percentage) |
| min | string / number | not declared | Minimum size of the panel (in pixels or percentage) |
| max | string / number | not declared | Maximum size of the panel (in pixels or percentage) |
| resizable | boolean | true | Whether the panel can be resized |
| collapsible | boolean | false | Whether the panel can be collapsed |

## Events

- `update:size`: Triggered when panel size changes

## Slots

- `default`: Default content of the panel
- `start-collapsible`: Custom content for the start collapsible button
- `end-collapsible`: Custom content for the end collapsible button
