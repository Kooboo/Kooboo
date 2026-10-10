# ElDialog

Informs users while preserving the current page state.

Kind: component. Import `ElDialog` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/dialog.html#dialog

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | boolean | false | visibility of Dialog |
| title | string | '' | title of Dialog. Can also be passed with a named slot (see the following table) |
| width | string / number | '' | width of Dialog, default is 50% |
| fullscreen | boolean | false | whether the Dialog takes up full screen |
| top | string | '' | value for `margin-top` of Dialog CSS, default is 15vh |
| modal | boolean | true | whether a mask is displayed |
| modal-penetrable | boolean | false | whether the mask is penetrable. The modal attribute must be `false`. |
| modal-class | string | not declared | custom class names for mask |
| header-class | string | not declared | custom class names for header wrapper |
| body-class | string | not declared | custom class names for body wrapper |
| footer-class | string | not declared | custom class names for footer wrapper |
| append-to-body | boolean | false | whether to append Dialog itself to body. A nested Dialog should have this attribute set to `true` |
| append-to | CSSSelector / HTMLElement | body | which element the Dialog appends to. Will override `append-to-body` |
| lock-scroll | boolean | true | whether scroll of body is disabled while Dialog is displayed |
| open-delay | number | 0 | the Time(milliseconds) before open |
| close-delay | number | 0 | the Time(milliseconds) before close |
| close-on-click-modal | boolean | true | whether the Dialog can be closed by clicking the mask |
| close-on-press-escape | boolean | true | whether the Dialog can be closed by pressing ESC |
| show-close | boolean | true | whether to show a close button |
| before-close | (done: DoneFn) => void | not declared | callback before Dialog closes, and it will prevent Dialog from closing, use done to close the dialog |
| draggable | boolean | false | enable dragging feature for Dialog |
| overflow | boolean | false | draggable Dialog can overflow the viewport |
| center | boolean | false | whether to align the header and footer in center |
| align-center | boolean | false | whether to align the dialog both horizontally and vertically |
| destroy-on-close | boolean | false | destroy elements in Dialog when closed |
| close-icon | string / Component | not declared | custom close icon, default is Close |
| z-index | number | not declared | same as z-index in native CSS, z-order of dialog |
| header-aria-level | string | 2 | header's `aria-level` attribute |
| transition | string / TransitionProps | dialog-fade | custom transition configuration for dialog animation. Can be a string (transition name) or an object with Vue transition props |
| custom-class | string | '' | custom class names for Dialog |

## Events

- `open`: triggers when the Dialog opens
- `opened`: triggers when the Dialog opening animation ends
- `close`: triggers when the Dialog closes
- `closed`: triggers when the Dialog closing animation ends
- `open-auto-focus`: triggers after Dialog opens and content focused
- `close-auto-focus`: triggers after Dialog closed and content focused

## Slots

- `default`: default content of Dialog
- `header`: content of the Dialog header; Replacing this removes the title, but does not remove the close button.
- `footer`: content of the Dialog footer
- `title`: works the same as the header slot. Use that instead.
