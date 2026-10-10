# YlRadioGroup

Import and register `{ YlRadioGroup }` from `/yuanli.js`.

options: Array<{value:string|number,label:string,disabled?:boolean}>. Native radio group with legend and browser keyboard navigation. Value types are preserved. Source radio styling is in the original Form section, outside the source component index.

Props/events:
```js
{modelValue:{type:[String,Number],default:null},options:{type:Array,default:()=>[]},disabled:Boolean,label:{type:String,required:true}},emits:['update:modelValue']
```
