# YlDateRangePicker

Two labeled YlDatePickers; independent from the original TimePicker.

- `v-model`: `[start, end]` strings, default `['', '']`. Updates replace the array.
- `mode` date/month/datetime-local (date default); `size` lg/md/sm/mini (md default); `min`/`max` constrain both native inputs.
- `disabled`, `readonly`, `invalid`, `clearable`: false.
- `startLabel` 开始日期, `endLabel` 结束日期, `orderError` 结束日期不能早于开始日期.
- Emits `update:modelValue(range)` and `change(range)`; no slots. Root attributes target the group, not both inputs.

```html
<YlDateRangePicker v-model="range" clearable aria-label="报告日期范围" />
```

Use same-mode canonical strings. A reversed complete range marks both inputs invalid and displays an associated error; values are preserved, not silently reordered. Partial ranges are allowed while editing. Business submission must validate completeness, chronological order and bounds; the displayed range error alone does not block form submission. Native calendar appearance and support vary by browser. No dual-calendar popup or presets.
