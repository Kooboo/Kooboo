# ElDescriptionsItem

ElDescriptionsItem: native Element Plus component.

Kind: component. Import `ElDescriptionsItem` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/descriptions.html#descriptionsitem

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| label | string | '' | label text |
| span | number | 1 | colspan of column |
| rowspan | number | 1 | the number of rows a cell should span |
| width | string / number | '' | column width, the width of the same column in different rows is set by the max value (If no `border`, width contains label and content) |
| min-width | string / number | '' | column minimum width, columns with `width` has a fixed width, while columns with `min-width` has a width that is distributed in proportion (If no`border`, width contains label and content) |
| label-width | string / number | not declared | column label width, if not set, it will be the same as the width of the column. Higher priority than the `label-width` of `Descriptions` |
| align | 'left' \| 'center' \| 'right' | left | column content alignment (If no `border`, effective for both label and content) |
| label-align | 'left' \| 'center' \| 'right' | not declared | column label alignment, if omitted, the value of the above `align` attribute will be applied (If no `border`, please use `align` attribute) |
| class-name | string | '' | column content custom class name |
| label-class-name | string | '' | column label custom class name |

## Slots

- `default`: customize default content
- `label`: custom label
