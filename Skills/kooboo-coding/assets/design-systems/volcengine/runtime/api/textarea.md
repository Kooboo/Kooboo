# YlTextarea

Import and register `{ YlTextarea }` from `/yuanli.js`.

String v-model with IME guard. Native rows, maxlength, required and label attributes. Resize vertically; caller owns validation.

Props/events:
```js
{modelValue:{type:String,default:''},disabled:Boolean,readonly:Boolean,invalid:Boolean,rows:{type:Number,default:3}},emits:['update:modelValue']
```
