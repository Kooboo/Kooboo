# ATypographyText

Ant Design Vue 4.2.6; native export `TypographyText`. Import `ATypographyText` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `code`: boolean; default false. Code style
- `content(v-model)`: string; default -. When using ellipsis or editable, use content instead of children
- `copyable`: boolean | [copyable](#copyable); default false. Whether to be copyable, customize it via setting an object
- `delete`: boolean; default false. Deleted line style
- `disabled`: boolean; default false. Disabled content
- `editable`: boolean | [editable](#editable); default false. If editable. Can control edit state when is object
- `ellipsis`: boolean; default false. Display ellipsis when text overflows
- `keyboard`: boolean; default false. Keyboard style
- `mark`: boolean; default false. Marked style
- `strong`: boolean; default false. Bold style
- `type`: `secondary` | `success` | `warning` | `danger`; default -. Content type
- `underline`: boolean; default false. Underlined style

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATypography><ATypographyTitle :level="3">标题</ATypographyTitle><ATypographyParagraph copyable>可复制内容</ATypographyParagraph><ATypographyText type="secondary">辅助文字</ATypographyText><ATypographyLink href="#/overview">返回概览</ATypographyLink></ATypography>
```

Source: pinned upstream es/components.js, es/typography, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
