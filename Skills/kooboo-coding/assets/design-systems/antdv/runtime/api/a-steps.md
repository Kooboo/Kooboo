# ASteps

Ant Design Vue 4.2.6; native export `Steps`. Import `ASteps` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `current(v-model)`: number; default 0. to set the current step, counting from 0. You can overwrite this state by using `status` of `Step`, support v-model after 1.5.0
- `direction`: string; default `horizontal`. to specify the direction of the step bar, `horizontal` and `vertical` are currently supported
- `initial`: number; default 0. set the initial step, counting from 0
- `labelPlacement`: string; default `horizontal`. support vertical title and description
- `percent`: number; default -. Progress circle percentage of current step in `process` status (only works on basic Steps)
- `progressDot`: Boolean or v-slot:progressDot="{index, status, title, description, prefixCls, iconDot}"; default false. Steps with progress dot style, customize the progress dot by setting a scoped slot. labelPlacement will be `vertical`
- `responsive`: boolean; default true. change to vertical direction when screen width smaller than `532px`
- `size`: string; default `default`. to specify the size of the step bar, `default` and `small` are currently supported
- `status`: string; default `process`. to specify the status of current step, can be set to one of the following values: `wait` `process` `finish` `error`
- `type`: string; default `default`. Type of steps, can be set to one of the following values: `default`, `navigation`
- `items`: [StepItem](#stepsstep); default []. StepItem content
- `change`: (current) => void; default -. Trigger when Step is changed

## Events

No events listed in the pinned upstream metadata.

## Slots

- `progressDot`: Steps with progress dot style, customize the progress dot by setting a scoped slot. labelPlacement will be `vertical`

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASteps v-model:current="step" :items="[{title:'申请'},{title:'审核'},{title:'完成'}]"/><p>步骤 {{ step + 1 }}</p>
```

Source: pinned upstream es/components.js, es/steps, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
