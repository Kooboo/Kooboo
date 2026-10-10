# ElPagination

If you have too much data to display in one page, use pagination.

Kind: component. Import `ElPagination` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/pagination.html#pagination

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| size | 'large' \| 'default' \| 'small' | 'default' | pagination size |
| background | boolean | false | whether the buttons have a background color |
| page-size | number | not declared | item count of each page |
| default-page-size | number | not declared | default initial value of page size, not setting is the same as setting 10 |
| total | number | not declared | total item count |
| page-count | number | not declared | total page count. Set either `total` or `page-count` and pages will be displayed; if you need `page-sizes`, `total` is required |
| pager-count | 5 / 7 / 9 / 11 / 13 / 15 / 17 / 19 / 21 | 7 | number of pagers. Pagination collapses when the total page count exceeds this value |
| current-page | number | not declared | current page number |
| default-current-page | number | not declared | default initial value of current-page, not setting is the same as setting 1 |
| layout | string | prev, pager, next, jumper, ->, total | layout of Pagination, elements separated with a comma |
| page-sizes | number[] | [10, 20, 30, 40, 50, 100] | options of item count per page |
| append-size-to | string | not declared | which element the size dropdown appends to |
| popper-class | string | '' | custom class name for the page size Select's dropdown |
| popper-style | string / object | not declared | custom style for the page size Select's dropdown |
| prev-text | string | '' | text for the prev button |
| prev-icon | string / Component | ArrowLeft | icon for the prev button, has a lower priority than `prev-text` |
| next-text | string | '' | text for the next button |
| next-icon | string / Component | ArrowRight | icon for the next button, has a lower priority than `next-text` |
| disabled | boolean | false | whether Pagination is disabled |
| teleported | boolean | true | whether Pagination select dropdown is teleported to the body |
| hide-on-single-page | boolean | false | whether to hide when there's only one page |
| small | boolean | false | whether to use small pagination |

## Events

- `size-change`: triggers when `page-size` changes
- `current-change`: triggers when `current-page` changes
- `change`: triggers when `current-page` or `page-size` changes
- `prev-click`: triggers when the prev button is clicked and current page changes
- `next-click`: triggers when the next button is clicked and current page changes

## Slots

- `default`: custom content. To use this, you need to declare `slot` in `layout`
