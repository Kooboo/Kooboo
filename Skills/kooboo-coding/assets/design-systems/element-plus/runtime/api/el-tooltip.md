# ElTooltip

Display prompt information for mouse hover.

Kind: component. Import `ElTooltip` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/tooltip.html#tooltip

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| append-to | CSSSelector / HTMLElement | not declared | Which element the tooltip CONTENT appends to |
| effect | 'dark' \| 'light' | dark | Tooltip theme, built-in theme: `dark` / `light` |
| content | string | '' | Display content, can be overridden by `slot#content` |
| raw-content | boolean | false | Whether `content` is treated as HTML string |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end' \| 'right' \| 'right-start' \| 'right-end' | bottom | Position of Tooltip |
| fallback-placements | Placement[] | not declared | List of possible positions for Tooltip [popper.js](https://popper.js.org/docs/v2/modifiers/flip/#fallbackplacements) |
| visible | boolean | not declared | Visibility of Tooltip |
| disabled | boolean | not declared | Whether Tooltip is disabled |
| offset | number | 12 | Offset of the Tooltip |
| transition | string | not declared | Animation name |
| popper-options | objectrefer to  doc | {} | [popper.js](https://popper.js.org/docs/v2/) parameters |
| arrow-offset | number | 5 | Controls the offset (padding) of the tooltip’s arrow relative to the popper. |
| show-after | number | 0 | Delay of appearance, in millisecond, not valid in controlled mode |
| show-arrow | boolean | true | Whether the tooltip content has an arrow |
| hide-after | number | 200 | Delay of disappear, in millisecond, not valid in controlled mode |
| auto-close | number | 0 | Timeout in milliseconds to hide tooltip, not valid in controlled mode |
| popper-class | string | not declared | Custom class name for Tooltip's popper |
| popper-style | string / object | not declared | Custom style for Tooltip's popper |
| enterable | boolean | true | Whether the mouse can enter the tooltip |
| teleported | boolean | true | Whether tooltip content is teleported, if `true` it will be teleported to where `append-to` sets |
| trigger | 'hover' \| 'click' \| 'focus' \| 'contextmenu' / Array<'click' \| 'focus' \| 'hover' \| 'contextmenu'> | hover | How should the tooltip be triggered (to show), not valid in controlled mode |
| virtual-triggering | boolean | not declared | Indicates whether virtual triggering is enabled |
| virtual-ref | HTMLElement | not declared | Indicates the reference element to which the tooltip is attached |
| trigger-keys | Array | ['Enter','Space'] | When you click the mouse to focus on the trigger element, you can define a set of keyboard codes to control the display of tooltip through the keyboard, not valid in controlled mode |
| persistent | boolean | not declared | When tooltip inactive and `persistent` is `false` , tooltip will be destroyed |
| aria-label | string | not declared | Same as `aria-label` |
| focus-on-target | boolean | false | When triggering tooltips through hover, whether to focus the trigger element, which improves accessibility |

## Events

- `before-show`: Triggers before tooltip is shown. Passes trigger reason as argument.
- `show`: Triggers when tooltip is shown. Passes trigger reason as argument.
- `before-hide`: Triggers before tooltip is hidden. Passes trigger reason as argument.
- `hide`: Triggers when tooltip is hidden. Passes trigger reason as argument.

## Slots

- `default`: Tooltip triggering & reference element, only a single root element is accepted
- `content`: Customize content
