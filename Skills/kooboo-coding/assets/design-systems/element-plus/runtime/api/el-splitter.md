# ElSplitter

ElSplitter: native Element Plus component.

Kind: component. Import `ElSplitter` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/splitter.html#splitter

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| layout | 'horizontal' \| 'vertical' | horizontal | Layout direction of the splitter |
| lazy | boolean | false | Whether to enable lazy mode |

## Events

- `resize-start`: Triggered when starting to resize a panel, `index` is the drag bar index
- `resize`: Triggered while resizing a panel, `index` is the drag bar index
- `resize-end`: Triggered when panel resizing ends, `index` is the drag bar index
- `collapse`: Triggered when a panel is collapsed, `index` is the drag bar index
