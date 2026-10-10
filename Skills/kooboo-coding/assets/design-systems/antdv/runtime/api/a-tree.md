# ATree

Ant Design Vue 4.2.6; native export `Tree`. Import `ATree` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `allowDrop`: ({ dropNode, dropPosition }) => boolean; default -. Whether to allow dropping on the node
- `autoExpandParent`: boolean; default false. Whether to automatically expand a parent treeNode
- `blockNode`: boolean; default false. Whether treeNode fill remaining horizontal space
- `checkable`: boolean; default false. Adds a `Checkbox` before the treeNodes
- `checkedKeys(v-model)`: string[] | number[] | {checked: string\[] | number\[], halfChecked: string\[] | number\[]}; default \[]. (Controlled) Specifies the keys of the checked treeNodes (PS: When this specifies the key of a treeNode which is also a parent treeNode, all the children treeNodes of will be checked; and vice versa, when it specifies the key of a treeNode which is a child treeNode, its parent treeNode will also be checked. When `checkable` and `checkStrictly` is true, its object has `checked` and `halfChecked` property. Regardless of whether the child or parent treeNode is checked, they won't impact each other.
- `checkStrictly`: boolean; default false. Check treeNode precisely; parent treeNode and children treeNodes are not associated
- `defaultExpandAll`: boolean; default false. Whether to expand all treeNodes by default
- `disabled`: bool; default false. whether disabled the tree
- `draggable`: boolean; default false. Specifies whether this Tree is draggable (IE > 8)
- `expandedKeys(v-model)`: string[] | number[]; default \[]. (Controlled) Specifies the keys of the expanded treeNodes
- `fieldNames`: object; default { children:'children', title:'title', key:'key' }. Replace the title,key and children fields in treeNode with the corresponding fields in treeData
- `filterTreeNode`: function(node); default -. Defines a function to filter (highlight) treeNodes. When the function returns `true`, the corresponding treeNode will be highlighted
- `height`: number; default -. Config virtual scroll height. Will not support horizontal scroll when enable this
- `loadData`: function(node); default -. Load data asynchronously
- `loadedKeys`: string[] | number[]; default \[]. (Controlled) Set loaded tree nodes. Need work with `loadData`
- `multiple`: boolean; default false. Allows selecting multiple treeNodes
- `selectable`: boolean; default true. whether can be selected
- `selectedKeys(v-model)`: string[] | number[]; default -. (Controlled) Specifies the keys of the selected treeNodes
- `showIcon`: boolean; default false. Shows the icon before a TreeNode's title. There is no default style; you must set a custom style for it if set to `true`
- `showLine`: boolean | {showLeafIcon: boolean}(3.0+); default false. Shows a connecting line
- `switcherIcon`: v-slot:switcherIcon="{active, checked, expanded, loading, selected, halfChecked, title, key, children, dataRef, data, defaultIcon, switcherCls}"; default -. customize collapse/expand icon of tree node
- `title`: slot; default unspecified. custom title
- `treeData`: [TreeNode[]](#treenode); default -. treeNode of tree, please use `treeNodes` before v1.1.4
- `virtual`: boolean; default true. Disable virtual scroll when set to false
- `scrollTo({ key: string | number; align?: 'top' | 'bottom' | 'auto'; offset?: number })`: see upstream; default unspecified. Scroll to key item in virtual scroll

## Events

No events listed in the pinned upstream metadata.

## Slots

- `switcherIcon`: customize collapse/expand icon of tree node
- `title`: custom title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATree v-model:selectedKeys="selectedKeys" v-model:checkedKeys="treeChecked" :tree-data="treeData" checkable default-expand-all/><p>{{ selectedKeys }} {{ treeChecked }}</p>
```

Source: pinned upstream es/components.js, es/tree, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
