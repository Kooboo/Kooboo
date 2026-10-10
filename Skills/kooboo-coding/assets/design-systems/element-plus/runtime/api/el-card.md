# ElCard

Integrate information in a card container.

Kind: component. Import `ElCard` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/card.html#card

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| header | string | not declared | title of the card. Also accepts a DOM passed by `slot#header` |
| footer | string | not declared | footer of the card. Also accepts a DOM passed by `slot#footer` |
| body-style | CSSProperties | not declared | CSS style of card body |
| header-class | string | not declared | custom class name of card header |
| body-class | string | not declared | custom class name of card body |
| footer-class | string | not declared | custom class name of card footer |
| shadow | always / never / hover | always | when to show card shadows |

## Slots

- `default`: customize default content
- `header`: content of the Card header
- `footer`: content of the Card footer
