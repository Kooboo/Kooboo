# [🔙](./index.md) k-page

k-page 有作为**全局状态**的载体和配置页面的全局设置两个职能,并且只作为页面的根标签提供.

```html
<k-page title="Page demo">
  <k-text>This is page demo</k-text>
</k-page>
```

## API

| 属性  | 类型                    | 可空 | 备注               |
| ----- | ----------------------- | ---- | ------------------ |
| title | string                  | 是   |                    |
| back  | string                  | 是   | 填写相对或绝对 url |
| size  | [查看](./types.md#size) | 是   |                    |
