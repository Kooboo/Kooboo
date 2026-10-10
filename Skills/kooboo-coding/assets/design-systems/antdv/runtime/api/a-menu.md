# AMenu

Ant Design Vue 4.2.6; native export `Menu`. Import `AMenu` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `forceSubMenuRender`: boolean; default false. render submenu into DOM before it shows
- `inlineCollapsed`: boolean; default -. specifies the collapsed status when menu is inline mode
- `inlineIndent`: number; default 24. indent px of inline menu item on each level
- `items`: [ItemType[]](#itemtype); default -. Menu item content
- `mode`: `vertical` | `horizontal` | `inline`; default `vertical`. type of the menu; `vertical`, `horizontal`, and `inline` modes are supported
- `multiple`: boolean; default false. Allow selection of multiple items
- `openKeys(v-model)`: (string | number)[]; default unspecified. array with the keys of currently opened sub menus
- `overflowedIndicator`: slot; default `<EllipsisOutlined />`. Customized the ellipsis icon when menu is collapsed horizontally
- `selectable`: boolean; default true. allow selecting menu items
- `selectedKeys(v-model)`: (string | number)[]; default unspecified. array with the keys of currently selected menu items
- `style`: object; default unspecified. style of the root node
- `subMenuCloseDelay`: number; default 0.1. delay time to hide submenu when mouse leave, unit: second
- `subMenuOpenDelay`: number; default 0. delay time to show submenu when mouse enter, unit: second
- `theme`: `light` | `dark`; default `light`. color theme of the menu
- `triggerSubMenuAction`: `click` | `hover`; default `hover`. method of trigger submenu
- `click`: function({ item, key, keyPath }); default unspecified. callback executed when a menu item is clicked
- `deselect`: function({ item, key, selectedKeys }); default unspecified. callback executed when a menu item is deselected, only supported for multiple mode
- `openChange`: function(openKeys: (string | number)[]); default unspecified. called when open/close sub menu
- `select`: function({ item, key, selectedKeys }); default unspecified. callback executed when a menu item is selected

## Events

No events listed in the pinned upstream metadata.

## Slots

- `overflowedIndicator`: Customized the ellipsis icon when menu is collapsed horizontally

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AMenu v-model:selectedKeys="selectedKeys" mode="inline" :items="menuItems" style="max-width:260px"/><p>{{ selectedKeys }}</p>
```

Source: pinned upstream es/components.js, es/menu, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
