# ElPageHeader

If path of the page is simple, it is recommended to use PageHeader instead of the Breadcrumb.

Kind: component. Import `ElPageHeader` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/page-header.html#page-header

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| icon | string / Component | Back | icon component of page header |
| title | string | '' | main title of page header, default is Back that built-in a11y |
| content | string | '' | content of page header |

## Events

- `back`: triggers when right side is clicked

## Slots

- `icon`: content as icon
- `title`: content as title
- `content`: content
- `extra`: extra
- `breadcrumb`: content as breadcrumb
- `default`: main content
