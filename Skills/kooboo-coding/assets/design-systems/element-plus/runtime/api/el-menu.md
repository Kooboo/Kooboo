# ElMenu

Menu that provides navigation for your website.

Kind: component. Import `ElMenu` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/menu.html#menu

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| mode | 'horizontal' \| 'vertical' | vertical | menu display mode |
| collapse | boolean | false | whether the menu is collapsed (available only in vertical mode) |
| ellipsis | boolean | true | whether the menu is ellipsis (available only in horizontal mode) |
| ellipsis-icon | string / Component | not declared | custom ellipsis icon (available only in horizontal mode and ellipsis is true) |
| popper-offset | number | 6 | offset of the popper (effective for all submenus) |
| default-active | string | '' | index of active menu on page load |
| default-openeds | string[] | [] | array that contains indexes of currently active sub-menus |
| unique-opened | boolean | false | whether only one sub-menu can be active |
| menu-trigger | 'hover' \| 'click' | hover | how sub-menus are triggered, only works when `mode` is 'horizontal' |
| router | boolean | false | whether `vue-router` mode is activated. If true, index will be used as 'path' to activate the route action. Use with `default-active` to set the active item on load. |
| collapse-transition | boolean | true | whether to enable the collapse transition |
| popper-effect | 'dark' \| 'light' / string | dark | Tooltip theme, built-in theme: `dark` / `light` when menu is collapsed |
| close-on-click-outside | boolean | false | optional, whether menu is collapsed when clicking outside |
| popper-class | string | not declared | custom class name for all popup menus and titles' tooltips |
| popper-style | string / object | not declared | custom style for all popup menus and titles' tooltips |
| show-timeout | number | 300 | control timeout for all menus before showing |
| hide-timeout | number | 300 | control timeout for all menus before hiding |
| background-color | string | #ffffff | background color of Menu (hex format) (use `--el-menu-bg-color` in a style class instead) |
| text-color | string | #303133 | text color of Menu (hex format) ( use `--el-menu-text-color` in a style class instead) |
| active-text-color | string | #409eff | text color of currently active menu item (hex format) ( use `--el-menu-active-color` in a style class instead) |
| persistent | boolean | true | when menu inactive and `persistent` is `false` , dropdown menu will be destroyed |

## Events

- `select`: callback function when menu is activated
- `open`: callback function when sub-menu expands
- `close`: callback function when sub-menu collapses

## Slots

- `default`: customize default content
