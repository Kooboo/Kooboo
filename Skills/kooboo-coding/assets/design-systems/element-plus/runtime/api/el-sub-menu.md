# ElSubMenu

ElSubMenu: native Element Plus component.

Kind: component. Import `ElSubMenu` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/menu.html#submenu

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| index | string | not declared | unique identification |
| popper-class | string | not declared | custom class name for the popup menu |
| popper-style | string / object | not declared | custom style for the popup menu |
| show-timeout | number | not declared | timeout before showing a sub-menu(inherit `show-timeout` of the menu by default.) |
| hide-timeout | number | not declared | timeout before hiding a sub-menu(inherit `hide-timeout` of the menu by default.) |
| disabled | boolean | false | whether the sub-menu is disabled |
| teleported | boolean | undefined | whether popup menu is teleported to the body, the default is true for the level one SubMenu, false for other SubMenus |
| popper-offset | number | not declared | offset of the popper (overrides the `popper` of menu) |
| expand-close-icon | string / Component | not declared | Icon when menu are expanded and submenu are closed, `expand-close-icon` and `expand-open-icon` need to be passed together to take effect |
| expand-open-icon | string / Component | not declared | Icon when menu are expanded and submenu are opened, `expand-open-icon` and `expand-close-icon` need to be passed together to take effect |
| collapse-close-icon | string / Component | not declared | Icon when menu are collapsed and submenu are closed, `collapse-close-icon` and `collapse-open-icon` need to be passed together to take effect |
| collapse-open-icon | string / Component | not declared | Icon when menu are collapsed and submenu are opened, `collapse-open-icon` and `collapse-close-icon` need to be passed together to take effect |

## Slots

- `default`: customize default content
- `title`: customize title content
