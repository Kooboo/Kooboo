# ElMention

Used to mention someone or something in an input.

Kind: component. Import `ElMention` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/mention.html#mention

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| options | MentionOption[] | [] | mention options list |
| props | MentionOptionProps | {value: 'value', label: 'label', disabled: 'disabled'} | configuration options |
| prefix | string / string[] | '@' | prefix character to trigger mentions. The string length must be exactly 1 |
| split | string | ' ' | character to split mentions. The string length must be exactly 1 |
| filter-option | false / (pattern: string, option: MentionOption) => boolean | not declared | customize filter option logic |
| placement | 'bottom' \| 'top' | 'bottom' | set popup placement |
| show-arrow | boolean | false | whether the dropdown panel has an arrow |
| offset | number | 0 | offset of the dropdown panel |
| whole | boolean | false | when backspace is pressed to delete, whether the mention content is deleted as a whole |
| check-is-whole | (pattern: string, prefix: string) => boolean | not declared | when backspace is pressed to delete, check if the mention is a whole |
| loading | boolean | false | whether the dropdown panel of mentions is in a loading state |
| model-value | string | not declared | input value |
| popper-class | string / object | '' | custom class name for dropdown panel |
| popper-style | string / object | not declared | custom style for dropdown panel |
| popper-options | object refer to | not declared | [popper.js](https://popper.js.org/docs/v2/) parameters |
| [input props] |  | not declared |  |

## Events

- `search`: trigger when prefix hit
- `select`: trigger when user select the option
- `whole-remove`: trigger when a whole mention is removed and `whole` is `true` or `check-is-whole` is `true`
- `[input events]`:

## Slots

- `label`: content as option label
- `loading`: content as option loading
- `header`: content at the top of the dropdown
- `footer`: content at the bottom of the dropdown
- `[input slots]`:
