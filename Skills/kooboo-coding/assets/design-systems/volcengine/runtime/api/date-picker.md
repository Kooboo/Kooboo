# YlDatePicker

Native browser date control styled with Yuanli Input. Import from `/yuanli.js`.

- `v-model`: string, default `''`; `mode`: `date` (YYYY-MM-DD, default), `month` (YYYY-MM), `datetime-local` (YYYY-MM-DDTHH:mm). No implicit Date/UTC conversion.
- `size`: lg/md/sm/mini, default md. `min`/`max`: strings in the same format. Native input attributes (`id`, `name`, `required`, `step`, `aria-*`) are forwarded to the input; class/style target the wrapper.
- `disabled`, `readonly`, `invalid`, `clearable`: false. `clearLabel`: 清除日期.
- Emits `update:modelValue(value)` and `change(value)` on edits or clear; clear emits `''`. No slots.

```html
<label>交付日期<YlDatePicker v-model="date" min="2026-01-01" clearable /></label>
```

The calendar and localized display are native, browser-dependent UI; month may fall back to text in unsupported browsers. No custom calendar, shortcuts, week selector, disabled-date callback or timezone handling. Use canonical values. min/max/required participate in native form validity; they do not sanitize external models or replace business/server validation. `change` reports edits, not a separate confirmation action. Source basis: Yuanli Input styles plus platform input behavior, not the original TimePicker component.

The optional clear action is inside the field. On hover or focus it replaces the native calendar indicator; empty, readonly and disabled fields retain the native indicator. Clicking the editable field can open the native picker. Browsers without a styleable calendar indicator omit the custom clear action and retain native editing.

The native calendar indicator and clear action share one suffix position; hover/focus swaps them without adding input padding. In browsers without the WebKit indicator selector, the custom clear is hidden and native editing remains available. Native picker rendering remains browser-owned.
