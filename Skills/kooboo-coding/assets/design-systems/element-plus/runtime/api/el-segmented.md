# ElSegmented

Display multiple options and allow users to select a single option.

Kind: component. Import `ElSegmented` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/segmented.html#segmented

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / boolean | not declared | binding value |
| options | Option[] | [] | data of the options |
| [props] | object | not declared | configuration options, see the following table |
| size | '' \| 'large' \| 'default' \| 'small' | '' | size of component |
| block | boolean | false | fit width of parent content |
| disabled | boolean | false | whether segmented is disabled |
| validate-event | boolean | true | whether to trigger form validation |
| name | string | not declared | native `name` attribute |
| id | string | not declared | native `id` attribute |
| aria-label | string | not declared | native `aria-label` attribute |
| direction | 'horizontal' \| 'vertical' | horizontal | display direction |

## Events

- `change`: triggers when the selected value changes, the param is current selected value

## Slots

- `default`: option renderer
