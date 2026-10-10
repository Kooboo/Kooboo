# YlInputNumber

Import and register `{ YlInputNumber }` from `/yuanli.js`.

Number|null v-model; native number control. min/max/step use native constraint validation, never silently clamp typed values. Empty emits null; bad intermediate input emits nothing. Validate custom API submissions.

Props/events:
```js
{modelValue:{type:Number,default:null},min:Number,max:Number,step:{type:Number,default:1},disabled:Boolean,readonly:Boolean,invalid:Boolean},emits:['update:modelValue']
```
