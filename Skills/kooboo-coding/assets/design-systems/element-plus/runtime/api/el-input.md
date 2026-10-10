# ElInput

Input data using mouse or keyboard.

Kind: component. Import `ElInput` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/input.html#input

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| type | 'text' \| 'textarea' \| 'number' \| 'password' \| 'email' \| 'search' \| 'tel' \|  'url' | text | type of input, see more in [MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input#Form_%3Cinput%3E_types) |
| model-value | string / number | not declared | binding value |
| model-modifiers | { lazy?: true, number?: true, trim?: true } | not declared | v-model modifiers, reference [Vue modifiers](https://vuejs.org/guide/essentials/forms.html#modifiers) |
| maxlength | string / number | not declared | same as `maxlength` in native input |
| minlength | string / number | not declared | same as `minlength` in native input |
| show-word-limit | boolean | false | whether show word count, only works when `type` is 'text' or 'textarea' |
| word-limit-position | 'inside' \| 'outside' | "inside" | word count position, valid when `show-word-limit` is true |
| placeholder | string | not declared | placeholder of Input |
| clearable | boolean | false | whether to show clear button, only works when `type` is not 'textarea' |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| formatter | (value: string \| number) => string | not declared | specifies the format of the value presented input.(only works when `type` is 'text') |
| parser | (value: string) => string | not declared | specifies the value extracted from formatter input.(only works when `type` is 'text') |
| show-password | boolean | false | whether to show toggleable password input |
| disabled | boolean | false | whether Input is disabled |
| size | 'large' \| 'default' \| 'small' | not declared | size of Input, works when `type` is not 'textarea' |
| prefix-icon | string / Component | not declared | prefix icon component |
| suffix-icon | string / Component | not declared | suffix icon component |
| rows | number | 2 | number of rows of textarea, only works when `type` is 'textarea' |
| autosize | boolean / { minRows?: number, maxRows?: number } | false | whether textarea has an adaptive height, only works when `type` is 'textarea'. Can accept an object, e.g. `{ minRows: 2, maxRows: 6 }` |
| autocomplete | string | off | same as `autocomplete` in native input |
| name | string | not declared | same as `name` in native input |
| readonly | boolean | false | same as `readonly` in native input |
| max |  | not declared | same as `max` in native input |
| min |  | not declared | same as `min` in native input |
| step |  | not declared | same as `step` in native input |
| resize | 'none' \| 'both' \| 'horizontal' \| 'vertical' | not declared | control the resizability |
| autofocus | boolean | false | same as `autofocus` in native input |
| form | string | not declared | same as `form` in native input |
| aria-label | string | not declared | same as `aria-label` in native input |
| tabindex | string / number | not declared | input tabindex |
| validate-event | boolean | true | whether to trigger form validation |
| input-style | string / CSSProperties / CSSProperties[] / string[] | {} | the style of the input element or textarea element |
| label | string | not declared | same as `aria-label` in native input |
| inputmode | string | not declared | same as `inputmode` in native input |
| count-graphemes | (value: string) => number | not declared | custom function to count graphemes; when set, native `maxlength`/`minlength` constraints are bypassed. Component uses `Intl.Segmenter` (Chrome 87+, Firefox 125+, Safari 14.1+) for proper grapheme clustering; older browsers fall back to `Array.from()` for code-point iteration |

## Events

- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `change`: triggers when the input box loses focus or the user presses Enter, only if the modelValue has changed
- `input`: triggers when the Input value change
- `clear`: triggers when the Input is cleared by clicking the clear button
- `keydown`: triggers when a key is pressed down
- `mouseleave`: triggers when the mouse leaves the Input element
- `mouseenter`: triggers when the mouse enters the Input element
- `compositionstart`: triggers when the composition starts
- `compositionupdate`: triggers when the composition is updated
- `compositionend`: triggers when the composition ends

## Slots

- `prefix`: content as Input prefix, only works when `type` is not 'textarea'
- `suffix`: content as Input suffix, only works when `type` is not 'textarea'
- `prepend`: content to prepend before Input, only works when `type` is not 'textarea'
- `append`: content to append after Input, only works when `type` is not 'textarea'
- `password-icon`: content as Input password icon, only works when `show-password` is true. The scope variable is `{ visible: boolean }`

## Usage and state ownership

Business owns modelValue; bind v-model for updates. Use type=textarea instead of inventing an ElTextarea export. size accepts large/default/small, not Yuanli mini/sm. Native form validation triggers follow Element Plus; validate at submission and keep server errors explicit.

```js
export default { components: { ElInput }, setup: () => ({ keyword: ref('') }), template: '<ElInput v-model="keyword" clearable placeholder="Search records" />' };
```
