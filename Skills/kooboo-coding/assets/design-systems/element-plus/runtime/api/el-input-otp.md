# ElInputOtp

Used to enter a one-time password

Kind: component. Import `ElInputOtp` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/input-otp.html#inputotp

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number | undefined | The value of the OTP fields. Since numbers must not have leading zeros, `modelValue` is allowed to be a number only during initialization. |
| length | number | 6 | The OTP fields length |
| validator | (char: string, index: number) => boolean | () => true | Custom validator function |
| inputmode | string | not declared | Native `inputmode` attribute |
| type | 'outlined' \| 'filled' \| 'underlined' | 'outlined' | The type of the OTP fields |
| size | 'large' \| 'default' \| 'small' | not declared | The size of the OTP fields |
| mask | boolean | not declared | Whether to enable password mode |
| disabled | boolean | undefined | Whether the OTP fields are disabled |
| separator | string / VNode / () => string / VNode | not declared | The separator between OTP fields |
| validate-event | boolean | true | Whether to trigger form validation |
| readonly | boolean | false | Same as `readonly` in native input |
| id | string | not declared | Native `id` attribute |
| aria-label | string | not declared | Native `aria-label` attribute |

## Events

- `update:model-value`: Triggers when value updates
- `change`: Triggers when the value changes after input blur
- `finish`: Fires when all fields have been filled
- `focus`: Triggers when input is focused
- `blur`: Triggers when input is blurred

## Slots

- `separator`: The separator between OTP fields
