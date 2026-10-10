# YlSegmentedPicker

Props: modelValue string|number; options=[] with {value,label,disabled?}; size=md (lg/md/sm/mini); disabled=false; label required. Source covers 2–8 items; use unique primitive values. Controlled update:modelValue(value). A radiogroup: Arrow keys wrap and skip disabled options; Home/End jump. Native buttons support Space/Enter. No slots.

```html
<YlSegmentedPicker v-model="period" :options="periods" label="统计周期" />
```
