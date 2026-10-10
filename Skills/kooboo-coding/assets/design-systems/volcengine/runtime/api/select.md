# YlSelect

Import `{ YlSelect }` from `/yuanli.js` and register it. Use for category, status and sort choices; do not create a native select with business-specific CSS.

Props: `modelValue: string | number | Array<string | number> | null = null` (`v-model`), `options: Array<{ value: string | number, label: string, disabled?: boolean, group?: string }> = []`; values must be unique and retain their type. `placeholder = '请选择'`, `size: 'lg' | 'md' | 'sm' | 'mini' = 'md'`, `disabled`, `readonly`, `invalid`, `clearable` default false. `clearLabel = '清除选择'`, `emptyText = '暂无选项'`.

Event: `update:modelValue` emits the selected value; clearing emits `null` (single) or `[]` (multiple). Caller owns filtering, validation and persistence. Forward `id`, `aria-label`, `aria-labelledby`, `aria-describedby` to the focusable trigger; class/style apply to the wrapper. Always provide a visible label or aria-label.

With a selected value and clearable enabled, the trailing clear button replaces the arrow. The main trigger still opens the list with click/keyboard; Tab can reach the labelled clear button. Empty, disabled or readonly controls show only the arrow; disabled/readonly cannot clear or change selection.

```html
<label for="category">产品分类</label>
<YlSelect id="category" v-model="category" :options="categories" clearable placeholder="全部分类" />
<YlSelect v-model="draft.status" :options="statusOptions" aria-label="产品状态" :disabled="saving" :invalid="!!statusError" />
```

Options example: `[{ value: 'active', label: '启用' }, { value: 'archived', label: '归档', disabled: true }]`.

Arrow keys open/navigate and skip disabled options; Home/End select the active endpoint; Enter/Space commits; typing finds a label prefix. Escape closes only this popup, Tab/outside click dismisses it. Selection remains controlled by the parent. The fixed popup stays inside its dialog DOM and tracks window resize/scroll; avoid transformed/contained ancestors that redefine fixed positioning.

Coverage: single/multiple select, optional local search, four sizes, placeholder, clear, disabled/readonly/invalid, empty options and keyboard/typeahead. group renders labeled option sections. Caller owns async loading and native form serialization/required validation. Validate business fields in the submit handler.

`multiple: boolean = false`: v-model is an array; selecting toggles membership and keeps the popup open. Selected labels render removable chips. maxTagCount=2 controls visible chips, with overflow summarized as +N; readonly/disabled selections have no removal control. insetLabel adds a leading internal label. A multi-select trigger uses a focusable combobox container to avoid nesting chip buttons inside a button. `searchable: boolean = false` enables a focused search input filtering labels locally; `searchLabel = '搜索选项'` labels it. Search does not mutate the selection. Disabled selections are preserved; clear explicitly clears all.

```html
<YlSelect v-model="selectedCategories" :options="categories" multiple searchable clearable aria-label="产品分类" />
```
