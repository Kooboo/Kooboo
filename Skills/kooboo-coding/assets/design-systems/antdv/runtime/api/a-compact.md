# ACompact

Ant Design Vue 4.2.6; native export `Compact`. Import `ACompact` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

Refer to the parent composition `space` and its primary API guide; upstream metadata does not list a separate complete contract for this export. Do not infer props from another library.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><AButton>第一项</AButton><AButton>第二项</AButton></ASpace><ACompact><AInput v-model:value="text"/><AButton @click="event = text">提交</AButton></ACompact><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/space, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
