# YlInput

Import `{ YlInput } from '/yuanli.js'`; register in `components`.

Props: `modelValue: string = ''` (`v-model`); `size: 'lg'|'md'|'sm'|'mini' = 'md'`; `invalid`, `disabled`, `readonly`: booleans, default false.
Event: `update:modelValue(string)` on input after IME composition. An undecorated control has a native input root; decorated controls forward these attributes to their inner input. `id`, `name`, `type`, `required`, `maxlength`, `placeholder`, `aria-describedby`, `@blur`, `@keydown` pass through. Values remain strings, including number inputs; convert/validate in the business layer.

```html
<label for="record-name">名称</label>
<YlInput id="record-name" v-model="name" required :invalid="!!nameError" aria-describedby="name-error" />
<p id="name-error" v-if="nameError" role="alert">{{ nameError }}</p>
```

Optional prefixIcon/suffixIcon use Lucide names; insetLabel renders an internal label. Named prefix/suffix slots can replace icons. Class/style target the wrapper when decorated; input id and native attributes remain on the focusable input. Supply an accessible label through a visible outer label or aria-label. Validation and query requests belong to the caller. Use YlTextarea for multiline input.
