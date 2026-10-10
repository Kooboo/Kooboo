# ElAutocomplete

Get some recommended tips based on the current input.

Kind: component. Import `ElAutocomplete` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/autocomplete.html#autocomplete

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string | not declared | binding value |
| placeholder | string | not declared | the placeholder of Autocomplete |
| clearable | boolean | false | whether to show clear button |
| disabled | boolean | false | whether Autocomplete is disabled |
| value-key | string | value | key name of the input suggestion object for display |
| debounce | number | 300 | debounce delay when typing, in milliseconds |
| placement | 'top' \| 'top-start' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' | bottom-start | placement of the popup menu |
| fetch-suggestions | array / (queryString: string, callback: callbackfn) => void | not declared | a method to fetch input suggestions. When suggestions are ready, invoke `callback(data:[])` to return them to Autocomplete |
| trigger-on-focus | boolean | true | whether show suggestions when input focus |
| select-when-unmatched | boolean | false | whether to emit a `select` event on enter when there is no autocomplete match |
| name | string | not declared | same as `name` in native input |
| aria-label | string | not declared | native `aria-label` attribute |
| hide-loading | boolean | false | whether to hide the loading icon in remote search |
| popper-class | string / object | '' | custom class name for autocomplete's dropdown |
| popper-style | string / object | not declared | custom style for autocomplete's dropdown |
| popper-options | objectrefer to  doc | {} | [popper.js](https://popper.js.org/docs/v2/) parameters |
| show-arrow | boolean | true | whether the dropdown has an arrow |
| teleported | boolean | true | whether select dropdown is teleported to the body |
| append-to | CSSSelector / HTMLElement | not declared | which select dropdown appends to |
| highlight-first-item | boolean | false | whether to highlight first item in remote search suggestions by default |
| fit-input-width | boolean | false | whether the width of the dropdown is the same as the input |
| popper-append-to-body | boolean | false | whether to append the dropdown to body. If the positioning of the dropdown is wrong, you can try to set this prop to false |
| loop-navigation | boolean | true | whether keyboard navigation loops from end to start |
| [input props] |  | not declared |  |

## Events

- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `input`: triggers when the Input value change
- `clear`: triggers when the Input is cleared by clicking the clear button
- `select`: triggers when a suggestion is clicked
- `change`: triggers when the icon inside Input value change

## Slots

- `default`: custom content for input suggestions
- `header`: content at the top of the dropdown
- `footer`: content at the bottom of the dropdown
- `prefix`: content as Input prefix
- `suffix`: content as Input suffix
- `prepend`: content to prepend before Input
- `append`: content to append after Input
- `loading`: override loading content
