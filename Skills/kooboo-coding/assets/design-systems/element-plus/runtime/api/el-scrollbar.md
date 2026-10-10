# ElScrollbar

Used to replace the browser's native scrollbar.

Kind: component. Import `ElScrollbar` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/scrollbar.html#scrollbar

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| height | string / number | not declared | height of scrollbar |
| max-height | string / number | not declared | max height of scrollbar |
| native | boolean | false | whether to use the native scrollbar style |
| wrap-style | string / CSSProperties / CSSProperties[] / string[] | not declared | style of wrap container |
| wrap-class | string | not declared | class of wrap container |
| view-style | string / CSSProperties / CSSProperties[] / string[] | not declared | style of view |
| view-class | string | not declared | class of view |
| noresize | boolean | false | do not respond to container size changes, if the container size does not change, it is better to set it to optimize performance |
| tag | string | div | element tag of the view |
| always | boolean | false | always show scrollbar |
| min-size | number | 20 | minimum size of scrollbar |
| id | string | not declared | id of view |
| role | string | not declared | role of view |
| aria-label | string | not declared | aria-label of view |
| aria-orientation | 'horizontal' \| 'vertical' | not declared | aria-orientation of view |
| tabindex | number / string | not declared | tabindex of wrap container |
| distance | number | 0 | trigger end-reached event distance(px) |

## Events

- `scroll`: triggers when scrolling, return distance of scrolling
- `end-reached`: triggers when the end of a scroll is triggered

## Slots

- `default`: customize default content
