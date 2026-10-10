# ASkeletonTitle

Ant Design Vue 4.2.6; native export `SkeletonTitle`. Import `ASkeletonTitle` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

Refer to the parent composition `skeleton` and its primary API guide; upstream metadata does not list a separate complete contract for this export. Do not infer props from another library.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASwitch v-model:checked="busy"/><ACard><ASkeleton :loading="busy" active><p>已加载的真实内容</p></ASkeleton></ACard><ASpace><ASkeletonAvatar active/><ASkeletonInput active/><ASkeletonButton active/></ASpace>
```

Source: pinned upstream es/components.js, es/skeleton, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
