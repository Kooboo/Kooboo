# ATreeNode

Ant Design Vue 4.2.6; native export `TreeNode`. Import `ATreeNode` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `checkable`: boolean; default -. When Tree is checkable, set TreeNode display Checkbox or not
- `class`: string; default -. className
- `disableCheckbox`: boolean; default false. Disables the checkbox of the treeNode
- `disabled`: boolean; default false. Disables the treeNode
- `icon`: slot|slot-scope; default -. customize icon. When you pass component, whose render will receive full TreeNode props as component props
- `isLeaf`: boolean; default -. Determines if this is a leaf node(effective when `loadData` is specified)
- `key`: string | number; default internal calculated position of treeNode. Used with (default)ExpandedKeys / (default)CheckedKeys / (default)SelectedKeys. P.S.: It must be unique in all of treeNodes of the tree!
- `selectable`: boolean; default true. Set whether the treeNode can be selected
- `style`: string|object; default -. style
- `title`: string; default '---'. Title

## Events

No events listed in the pinned upstream metadata.

## Slots

- `icon`: customize icon. When you pass component, whose render will receive full TreeNode props as component props

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATree v-model:selectedKeys="selectedKeys" v-model:checkedKeys="treeChecked" :tree-data="treeData" checkable default-expand-all/><p>{{ selectedKeys }} {{ treeChecked }}</p>
```

Source: pinned upstream es/components.js, es/tree, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
