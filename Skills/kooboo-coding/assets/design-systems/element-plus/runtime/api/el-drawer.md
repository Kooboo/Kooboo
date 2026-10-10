# ElDrawer

Sometimes, `Dialog` does not always satisfy our requirements, let's say you have a massive form, or you need space to display something like `terms & conditions`, `Drawer` has almost identical API with `Dialog`, but it introduces different user experience.

Kind: component. Import `ElDrawer` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/drawer.html#drawer

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | boolean | false | Should Drawer be displayed |
| append-to-body | boolean | false | Controls should Drawer be inserted to DocumentBody Element, nested Drawer must assign this param to **true** |
| append-to | CSSSelector / HTMLElement | body | which element the Drawer appends to. Will override `append-to-body` |
| lock-scroll | boolean | true | whether scroll of body is disabled while Drawer is displayed |
| before-close | (done: (cancel?: boolean) => void) => void | not declared | If set, closing procedure will be halted |
| close-on-click-modal | boolean | true | whether the Drawer can be closed by clicking the mask |
| close-on-press-escape | boolean | true | Indicates whether Drawer can be closed by pressing ESC |
| open-delay | number | 0 | Time(milliseconds) before open |
| close-delay | number | 0 | Time(milliseconds) before close |
| destroy-on-close | boolean | false | Indicates whether children should be destroyed after Drawer closed |
| modal | boolean | true | Should show shadowing layer |
| modal-penetrable | boolean | false | whether the mask is penetrable. The modal attribute must be `false`. |
| direction | 'rtl' \| 'ltr' \| 'ttb' \| 'btt' | rtl | Drawer's opening direction |
| resizable | boolean | false | enable resizable feature for Drawer |
| show-close | boolean | true | Should show close button at the top right of Drawer |
| size | number / string | 30% | Drawer's size, if Drawer is horizontal mode, it effects the width property, otherwise it effects the height property, when size is `number` type, it describes the size by unit of pixels; when size is `string` type, it should be used with `x%` notation, other wise it will be interpreted to pixel unit |
| title | string | not declared | Drawer's title, can also be set by named slot, detailed descriptions can be found in the slot form |
| with-header | boolean | true | Flag that controls the header section's existence, default to true, when withHeader set to false, both `title attribute` and `title slot` won't work |
| modal-class | string | not declared | Extra class names for shadowing layer |
| header-class | string | not declared | custom class names for header wrapper |
| body-class | string | not declared | custom class names for body wrapper |
| footer-class | string | not declared | custom class names for footer wrapper |
| z-index | number | not declared | set z-index |
| header-aria-level | string | 2 | header's `aria-level` attribute |
| custom-class | string | not declared | Extra class names for Drawer |

## Events

- `open`: Triggered before Drawer opening animation begins
- `opened`: Triggered after Drawer opening animation ended
- `close`: Triggered before Drawer closing animation begins
- `closed`: Triggered after Drawer closing animation ended
- `open-auto-focus`: triggers after Drawer opens and content focused
- `close-auto-focus`: triggers after Drawer closed and content focused
- `resize-start`: Triggered when resizing starts (when `resizable` is enabled)
- `resize`: Triggered while resizing (when `resizable` is enabled)
- `resize-end`: Triggered when resizing ends (when `resizable` is enabled)

## Slots

- `default`: Drawer's Content
- `header`: Drawer header section; Replacing this removes the title, but does not remove the close button.
- `footer`: Drawer footer Section
- `title`: Works the same as the header slot. Use that instead.
