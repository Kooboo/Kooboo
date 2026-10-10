# ElDescriptions

Display multiple fields in list form.

Kind: component. Import `ElDescriptions` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/descriptions.html#descriptions

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| border | boolean | false | with or without border |
| column | number | 3 | numbers of `Descriptions Item` in one line |
| direction | 'vertical' \| 'horizontal' | horizontal | direction of list |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | size of list |
| title | string | '' | title text, display on the top left |
| extra | string | '' | extra text, display on the top right |
| label-width | string / number | not declared | label width of every column |

## Slots

- `default`: customize default content
- `title`: custom title, display on the top left
- `extra`: custom extra area, display on the top right
