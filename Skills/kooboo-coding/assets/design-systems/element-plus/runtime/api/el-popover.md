# ElPopover

ElPopover: native Element Plus component.

Kind: component. Import `ElPopover` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/popover.html#popover

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| trigger | 'click' \| 'focus' \| 'hover' \| 'contextmenu' / Array<'click' \| 'focus' \| 'hover' \| 'contextmenu'> | hover | how the popover is triggered, not valid in controlled mode |
| trigger-keys | Array | ['Enter','Space'] | When you click the mouse to focus on the trigger element, you can define a set of keyboard codes to control the display of popover through the keyboard, not valid in controlled mode |
| title | string | not declared | popover title |
| effect | 'dark' \| 'light' / string | light | Tooltip theme, built-in theme: `dark` / `light` |
| content | string | '' | popover content, can be replaced with a default `slot` |
| width | string / number | 150 | popover width |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | bottom | popover placement |
| disabled | boolean | false | whether Popover is disabled |
| visible | boolean / null | null | whether popover is visible |
| offset | number | undefined | popover offset, `Popover` is built with `Tooltip`, offset of `Popover` is `undefined`, but offset of `Tooltip` is 12 |
| transition | string | not declared | popover transition animation, the default is el-fade-in-linear |
| show-arrow | boolean | true | whether a tooltip arrow is displayed or not. For more info, please refer to [ElPopper](https://github.com/element-plus/element-plus/tree/dev/packages/components/popper) |
| popper-options | object | {modifiers: [{name: 'computeStyles',options: {gpuAcceleration: false}}]} | parameters for [popper.js](https://popper.js.org/docs/v2/) |
| popper-class | string | not declared | custom class name for popover |
| popper-style | string / object | not declared | custom style for popover |
| show-after | number | 0 | delay of appearance, in millisecond, not valid in controlled mode |
| hide-after | number | 200 | delay of disappear, in millisecond, not valid in controlled mode |
| auto-close | number | 0 | timeout in milliseconds to hide tooltip, not valid in controlled mode |
| tabindex | number / string | 0 | [tabindex](https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex) of Popover |
| teleported | boolean | true | whether popover dropdown is teleported to the body |
| append-to | CSSSelector / HTMLElement | body | which element the popover CONTENT appends to |
| persistent | boolean | true | when popover inactive and `persistent` is `false` , popover will be destroyed |
| virtual-triggering | boolean | not declared | Indicates whether virtual triggering is enabled |
| virtual-ref | HTMLElement | not declared | Indicates the reference element to which the popover is attached |
| [tooltip] |  | not declared | Inherits all attributes from Tooltip |

## Events

- `show`: triggers when popover shows
- `before-enter`: triggers when the entering transition before
- `after-enter`: triggers when the entering transition ends
- `hide`: triggers when popover hides
- `before-leave`: triggers when the leaving transition before
- `after-leave`: triggers when the leaving transition ends

## Slots

- `default`: content of popover, version  and later can receive the hide parameter.
- `reference`: HTML element that triggers popover, only a single root element is accepted
