# ElAutoResizer

Measure available width/height for virtual tables using a scoped default slot.

Kind: component. Import `ElAutoResizer` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Props: disableWidth=false, disableHeight=false, onResize callback. Default slot receives { width, height }. Give its parent a definite size; this is measurement, not an automatic page-height policy. Feed dimensions into ElTableV2; verify actual resize behavior manually.

```js
h(ElAutoResizer, null, { default: ({ width, height }) => h(ElTableV2, { width, height, columns, data }) });
```
