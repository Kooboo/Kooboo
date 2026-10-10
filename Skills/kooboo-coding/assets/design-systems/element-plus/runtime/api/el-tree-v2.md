# ElTreeV2

ElTreeV2: native Element Plus component.

Kind: component. Import `ElTreeV2` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tree-v2.html#treev2

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| data | Array<{[key: string]: any}> | not declared | tree data |
| empty-text | string | not declared | text displayed when data is void |
| [props] | object | not declared | configuration options, see the following table |
| highlight-current | boolean | false | whether current node is highlighted |
| expand-on-click-node | boolean | true | whether to expand or collapse node when clicking on the node, if false, then expand or collapse node only when clicking on the arrow icon. |
| check-on-click-node | boolean | false | whether to check or uncheck node when clicking on the node, if false, the node can only be checked or unchecked by clicking on the checkbox. |
| check-on-click-leaf | boolean | true | whether to check or uncheck node when clicking on leaf node (last children). |
| default-expanded-keys | Array<string \| number> | not declared | array of keys of initially expanded nodes |
| show-checkbox | boolean | false | whether node is selectable |
| check-strictly | boolean | false | whether checked state of a node not affects its father and child nodes when `show-checkbox` is `true` |
| default-checked-keys | Array<string \| number> | not declared | array of keys of initially checked nodes |
| current-node-key | string / number | not declared | key of initially selected node |
| filter-method | (query: string, data: TreeNodeData, node: TreeNode) => boolean | not declared | this function will be executed on each node when use filter method. if return `false`, tree node will be hidden. |
| indent | number | 16 | horizontal indentation of nodes in adjacent levels in pixels |
| icon | string / Component | not declared | custom tree node icon component |
| item-size | number | 26 | custom tree node height |
| scrollbar-always-on | boolean | false | always show scrollbar |
| height | number | 200 | height of the tree |

## Events

- `node-click`: triggers when a node is clicked
- `node-drop`: triggers when drag something and drop on a node
- `node-contextmenu`: triggers when a node is clicked by right button
- `check-change`: triggers when the selected state of the node changes
- `check`: triggers after clicking the checkbox of a node
- `current-change`: triggers when current node changes
- `node-expand`: triggers when current node open
- `node-collapse`: triggers when current node close

## Slots

- `default`: custom content for tree nodes
- `empty`: custom content when data is empty
