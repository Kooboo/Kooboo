# YlSlider

Props: modelValue=0 (number, or two-number array when range=true); range=false; min=0; max=100; step=1 positive; size=md (md/sm); disabled=false; label required; startLabel=最小值; endLabel=最大值.

Emits update:modelValue(value) while adjusting and change(value) on native commit. Native range inputs support pointer drag, arrows and Home/End. Two handles cannot cross; values are displayed clamped and ordered without silently rewriting the parent on mount. Supply finite valid bounds and models. Both range handles have accessible labels; disabled values cannot change. No slots.

```html
<YlSlider v-model="budget" range :min="0" :max="1000" :step="10" label="预算范围" />
```
