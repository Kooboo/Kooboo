# AInputPassword

Ant Design Vue 4.2.6; native export `InputPassword`. Import `AInputPassword` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Text inputs use v-model:value, not plain v-model. Textarea is ATextarea; InputPassword and InputSearch are native auxiliary components. Do not serialize passwords into logs.

## Props and defaults

- `visible(v-model)`: boolean; default false. password visibility
- `iconRender`: slot; default -. Custom toggle button
- `visibilityToggle`: boolean; default true. Whether show toggle button or control password visible

## Events

No events listed in the pinned upstream metadata.

## Slots

- `iconRender`: Custom toggle button

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace direction="vertical"><AInput v-model:value="text" allow-clear placeholder="姓名"/><AInputPassword v-model:value="password" placeholder="密码"/><AInputSearch v-model:value="text" @search="event = $event"/><ATextarea v-model:value="text" :rows="3"/><AInput disabled value="禁用"/></ASpace><p>{{ text }} {{ event }}</p>
```

Source: pinned upstream es/components.js, es/input, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
