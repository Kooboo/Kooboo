# YlAlert

Import and register `{ YlAlert }` from `/yuanli.js`. Install it with sync_design_runtime before importing.

Persistent inline context or load/retry errors. close emits but does not hide; parent controls v-if. For transient mutation results use YlMessage.

Props and events (Vue declaration; types/defaults are authoritative):
```js
{ title: { type: String, required: true }, description: String, type: { type: String, default: 'info', validator: oneOf(['info','success','warning','error']) }, closable: Boolean, closeLabel: { type: String, default: '关闭提示' } }, emits: ['close']
```

`banner=false` enables the source full-width banner style when true; closing remains parent-controlled through close.
