# ElInputTag

The InputTag component allows users to add content as tags.

Kind: component. Import `ElInputTag` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/input-tag.html#inputtag

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string[] | not declared | binding value |
| max | number | not declared | max number tags that can be enter |
| tag-type | '' \| 'success' \| 'info' \| 'warning' \| 'danger' | info | tag type |
| tag-effect | '' \| 'light' \| 'dark' \| 'plain' | light | tag effect |
| effect | 'dark' \| 'light' / string | light | tooltip theme, built-in theme: `dark` / `light` |
| trigger | 'Enter' \| 'Space' | Enter | the key to trigger input tag |
| draggable | boolean | false | whether tags can be dragged |
| delimiter | string / regex | not declared | add a tag when a delimiter is matched |
| size | 'large' \| 'default' \| 'small' | not declared | input box size |
| collapse-tags | boolean | false | whether to collapse tags to a text when multiple selecting |
| collapse-tags-tooltip | boolean | false | whether show all selected tags when mouse hover text of collapse-tags. To use this, collapse-tags must be true |
| save-on-blur | boolean | true | whether to save the input value when the input loses focus |
| clearable | boolean | false | whether to show clear button |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| disabled | boolean | false | whether to disable input-tag |
| validate-event | boolean | true | whether to trigger form validation |
| readonly | boolean | false | same as `readonly` in native input |
| autofocus | boolean | false | same as `autofocus` in native input |
| id | string | not declared | same as `id` in native input |
| tabindex | string / number | not declared | same as `tabindex` in native input |
| max-collapse-tags | number | 1 | the max tags number to be shown. To use this, collapse-tags must be true |
| maxlength | string / number | not declared | same as `maxlength` in native input |
| minlength | string / number | not declared | same as `minlength` in native input |
| placeholder | string | not declared | placeholder of input |
| autocomplete | string | off | same as `autocomplete` in native input |
| aria-label | string | not declared | native `aria-label` attribute |

## Events

- `change`: triggers when the modelValue change
- `input`: triggers when the input value change
- `add-tag`: triggers when a tag is added
- `remove-tag`: triggers when a tag is removed
- `drag-tag`: triggers when a tag is dragged
- `focus`: triggers when InputTag focuses
- `blur`: triggers when InputTag blurs
- `clear`: triggers when the clear icon is clicked

## Slots

- `tag`: content as tag
- `prefix`: content as InputTag prefix
- `suffix`: content as InputTag suffix
