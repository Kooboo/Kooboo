# ElDropdown

Toggleable menu for displaying lists of links and actions.

Kind: component. Import `ElDropdown` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/dropdown.html#dropdown

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| type | '' \| 'default' \| 'primary' \| 'success' \| 'warning' \| 'info' \| 'danger' \| 'text' | '' | menu button type, refer to `Button` Component, only works when `split-button` is true |
| size | '' \| 'large' \| 'default' \| 'small' | '' | menu size, also works on the split button |
| button-props | object | not declared | props for the button component, refer to [Button Attributes](./button.html#button-attributes) |
| max-height | string / number | '' | the max height of menu |
| split-button | boolean | false | whether a button group is displayed |
| disabled | boolean | false | whether to disable |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' | bottom | placement of pop menu |
| effect | 'dark' \| 'light' / string | light | Tooltip theme, built-in theme: `dark` / `light` |
| trigger | 'click' \| 'hover' \| 'contextmenu' / Array<'click' \| 'hover' \| 'contextmenu'> | hover | how to trigger |
| trigger-keys | string[] | ['Enter', 'Space', 'ArrowDown', 'NumpadEnter'] | specify which keys on the keyboard can trigger when pressed |
| virtual-triggering | boolean | not declared | indicates whether virtual triggering is enabled |
| virtual-ref | HTMLElement | not declared | indicates the reference element to which the dropdown is attached |
| hide-on-click | boolean | true | whether to hide menu after clicking menu-item |
| show-arrow | boolean | true | whether the tooltip content has an arrow |
| show-timeout | number | 150 | delay time before show a dropdown (only works when trigger is `hover`) |
| hide-timeout | number | 150 | delay time before hide a dropdown (only works when trigger is `hover`) |
| role | 'dialog' \| 'grid' \| 'group' \| 'listbox' \| 'menu' \| 'navigation' \| 'tooltip' \| 'tree' | menu | the ARIA role attribute for the dropdown menu. Depending on the use case, you may want to change this to 'navigation' |
| tabindex | number / string | 0 | [tabindex](https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex) of Dropdown |
| popper-class | string / object | '' | custom class name for Dropdown's dropdown |
| popper-style | string / object | not declared | custom style for Dropdown's dropdown |
| popper-options | object | {modifiers: [{name: 'computeStyles',options: {gpuAcceleration: false}}]} | [popper.js](https://popper.js.org/docs/v2/) parameters |
| teleported | boolean | true | whether the dropdown popup is teleported to the body |
| append-to | CSSSelector / HTMLElement | not declared | which element the dropdown CONTENT appends to |
| persistent | boolean | true | when dropdown inactive and `persistent` is `false` , dropdown menu will be destroyed |

## Events

- `click`: if `split-button` is `true`, triggers when left button is clicked
- `command`: triggers when a dropdown item is clicked, the parameters is the command dispatched from the dropdown item
- `visible-change`: triggers when the dropdown appears/disappears, the param is true when it appears, and false otherwise

## Slots

- `default`: content of Dropdown. Notice: Must be a valid html dom element (ex. `<span>, <button> etc.`) or `el-component`, to attach the trigger listener
- `dropdown`: content of the Dropdown Menu, usually a `<el-dropdown-menu>` element
