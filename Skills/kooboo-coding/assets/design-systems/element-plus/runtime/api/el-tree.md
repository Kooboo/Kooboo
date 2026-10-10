# ElTree

Display a set of data with hierarchies.

Kind: component. Import `ElTree` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tree.html#tree

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| data | Array<{[key: string]: any}> | not declared | tree data |
| empty-text | string | not declared | text displayed when data is void |
| node-key | string | not declared | unique identity key name for nodes, its value should be unique across the whole tree |
| [props] | object | not declared | configuration options, see the following table |
| render-after-expand | boolean | true | whether to render child nodes only after a parent node is expanded for the first time |
| load | (node, resolve, reject) => void | not declared | method for loading subtree data, only works when `lazy` is true |
| render-content | (h, { node, data, store }) => void | not declared | render function for tree node |
| highlight-current | boolean | false | whether current node is highlighted |
| default-expand-all | boolean | false | whether to expand all nodes by default |
| expand-on-click-node | boolean | true | whether to expand or collapse node when clicking on the node, if false, then expand or collapse node only when clicking on the arrow icon. |
| check-on-click-node | boolean | false | whether to check or uncheck node when clicking on the node, if false, the node can only be checked or unchecked by clicking on the checkbox. |
| check-on-click-leaf | boolean | true | whether to check or uncheck node when clicking on leaf node (last children). |
| auto-expand-parent | boolean | true | whether to expand father node when a child node is expanded |
| default-expanded-keys | Array<string \| number> | not declared | array of keys of initially expanded nodes |
| show-checkbox | boolean | false | whether node is selectable |
| check-strictly | boolean | false | whether checked state of a node not affects its father and child nodes when `show-checkbox` is `true` |
| default-checked-keys | Array<string \| number> | not declared | array of keys of initially checked nodes |
| current-node-key | string / number | not declared | key of initially selected node |
| filter-node-method | (value, data, node) => boolean | not declared | this function will be executed on each node when use filter method. if return `false`, tree node will be hidden. |
| accordion | boolean | false | whether only one node among the same level can be expanded at one time |
| indent | number | 18 | horizontal indentation of nodes in adjacent levels in pixels |
| icon | string / Component | not declared | custom tree node icon component |
| lazy | boolean | false | whether to lazy load leaf node, used with `load` attribute |
| draggable | boolean | false | whether enable tree nodes drag and drop |
| allow-drag | (node) => boolean | not declared | this function will be executed before dragging a node. If `false` is returned, the node can not be dragged |
| allow-drop | (draggingNode, dropNode, type) => boolean | not declared | this function will be executed before the dragging node is dropped. If `false` is returned, the dragging node can not be dropped at the target node. `type` has three possible values: 'prev' (inserting the dragging node before the target node), 'inner' (inserting the dragging node to the target node) and 'next' (inserting the dragging node after the target node) |

## Events

- `node-click`: triggers when a node is clicked
- `node-contextmenu`: triggers when a node is clicked by right button
- `check-change`: triggers when the selected state of the node changes
- `check`: triggers after clicking the checkbox of a node
- `current-change`: triggers when current node changes
- `node-expand`: triggers when current node open
- `node-collapse`: triggers when current node close
- `node-drag-start`: triggers when dragging starts
- `node-drag-enter`: triggers when the dragging node enters another node
- `node-drag-leave`: triggers when the dragging node leaves a node
- `node-drag-over`: triggers when dragging over a node (like mouseover event)
- `node-drag-end`: triggers when dragging ends
- `node-drop`: triggers after the dragging node is dropped

## Slots

- `default`: custom content for tree nodes
- `empty`: custom content when data is empty
