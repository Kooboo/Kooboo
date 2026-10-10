# YlTimePicker

Props: modelValue='' string (or [start,end] strings when range=true); range=false; size=md (lg/md/sm); disabled=false; readonly=false; invalid=false; min/max optional local time strings; step=1 seconds; placeholder=请选择时间; startLabel=开始时间; endLabel=结束时间.

Native time controls, no timezone conversion. Values are browser canonical HH:mm or HH:mm:ss; empty input is ''. Emits update:modelValue(value). Class/style apply to the wrapper; input attributes apply to the native fields (range end id appends -end). Use name only for a single field or serialize range explicitly. min/max/step provide native validity; the parent validates range order and business rules. Placeholder text hides during editing; native picker appearance depends on the browser. No slots.

```html
<YlTimePicker v-model="hours" range aria-label="营业时间" />
```

Default width is 168px for single time and 352px for a range, bounded by the container. Override wrapper style.width for full-width form layouts. Native indicator stays at the field end, outside placeholder text.
