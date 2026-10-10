# AInput

Ant Design Vue 4.2.6; native export `Input`. Import `AInput` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Text inputs use v-model:value, not plain v-model. Textarea is ATextarea; InputPassword and InputSearch are native auxiliary components. Do not serialize passwords into logs.

## Props and defaults

- `addonAfter`: string|slot; default unspecified. The label text displayed after (on the right side of) the input field.
- `addonBefore`: string|slot; default unspecified. The label text displayed before (on the left side of) the input field.
- `allowClear`: boolean; default unspecified. allow to remove input content with clear icon
- `bordered`: boolean; default true. Whether has border style
- `clearIcon`: slot; default `<CloseCircleFilled />`. custom clear icon when allowClear
- `defaultValue`: string; default unspecified. The initial input content
- `disabled`: boolean; default false. Whether the input is disabled.
- `id`: string; default unspecified. The ID for input
- `maxlength`: number; default unspecified. max length
- `prefix`: string|slot; default unspecified. The prefix icon for the Input.
- `showCount`: boolean; default false. Whether show text count
- `status`: 'error' | 'warning'; default -. Set validation status
- `size`: string; default -. The size of the input box. Note: in the context of a form, the `middle` size is used. Available: `large` `middle` `small`
- `suffix`: string|slot; default unspecified. The suffix icon for the Input.
- `type`: string; default `text`. The type of input, see: [MDN](https://developer.mozilla.org/docs/Web/HTML/Element/input#Form_%3Cinput%3E_types)(use `<a-textarea />` instead of `type="textarea"`)
- `value(v-model)`: string; default unspecified. The input content value
- `change`: function(e); default unspecified. callback when user input
- `pressEnter`: function(e); default unspecified. The callback function that is triggered when Enter key is pressed.

## Events

No events listed in the pinned upstream metadata.

## Slots

- `addonAfter`: The label text displayed after (on the right side of) the input field.
- `addonBefore`: The label text displayed before (on the left side of) the input field.
- `clearIcon`: custom clear icon when allowClear
- `prefix`: The prefix icon for the Input.
- `suffix`: The suffix icon for the Input.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace direction="vertical"><AInput v-model:value="text" allow-clear placeholder="姓名"/><AInputPassword v-model:value="password" placeholder="密码"/><AInputSearch v-model:value="text" @search="event = $event"/><ATextarea v-model:value="text" :rows="3"/><AInput disabled value="禁用"/></ASpace><p>{{ text }} {{ event }}</p>
```

Source: pinned upstream es/components.js, es/input, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
