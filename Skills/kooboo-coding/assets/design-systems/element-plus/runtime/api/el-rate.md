# ElRate

Used for rating

Kind: component. Import `ElRate` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/rate.html#rate

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number | 0 | binding value |
| max | number | 5 | max rating score |
| size | 'large' \| 'default' \| 'small' | not declared | size of Rate |
| disabled | boolean | false | whether Rate is read-only |
| allow-half | boolean | false | whether picking half start is allowed |
| low-threshold | number | 2 | threshold value between low and medium level. The value itself will be included in low level |
| high-threshold | number | 4 | threshold value between medium and high level. The value itself will be included in high level |
| colors | string[] / Record<number, string> | ['#f7ba2a', '#f7ba2a', '#f7ba2a'] | colors for icons. If array, it should have 3 elements, each of which corresponds with a score level, else if object, the key should be threshold value between two levels, and the value should be corresponding color |
| void-color | string | #c6d1de | color of unselected icons |
| disabled-void-color | string | #eff2f7 | color of unselected read-only icons |
| icons | string[] / Component[] / Record<number, string \| Component> | [StarFilled, StarFilled, StarFilled] | icon components. If array, it should have 3 elements, each of which corresponds with a score level, else if object, the key should be threshold value between two levels, and the value should be corresponding icon component |
| void-icon | string / Component | Star | component of unselected icons |
| disabled-void-icon | string / Component | StarFilled | component of unselected read-only icons |
| show-text | boolean | false | whether to display texts |
| show-score | boolean | false | whether to display current score. show-score and show-text cannot be true at the same time |
| text-color | string | '' | color of texts |
| texts | string[] | ['Extremely bad', 'Disappointed', 'Fair', 'Satisfied', 'Surprise'] | text array |
| score-template | string | {value} | score template |
| clearable | boolean | false | whether value can be reset to `0` |
| id | string | not declared | native `id` attribute |
| aria-label | string | not declared | same as `aria-label` in Rate |
| label | string | not declared | same as `aria-label` in Rate |

## Events

- `change`: Triggers when rate value is changed
