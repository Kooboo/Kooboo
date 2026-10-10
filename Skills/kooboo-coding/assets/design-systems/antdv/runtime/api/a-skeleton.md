# ASkeleton

Ant Design Vue 4.2.6; native export `Skeleton`. Import `ASkeleton` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `active`: boolean; default false. Show animation effect
- `avatar`: boolean | [SkeletonAvatarProps](#skeletonavatarprops); default false. Show avatar placeholder
- `loading`: boolean; default -. Display the skeleton when `true`
- `paragraph`: boolean | [SkeletonParagraphProps](#skeletonparagraphprops); default true. Show paragraph placeholder
- `title`: boolean | [SkeletonTitleProps](#skeletontitleprops); default true. Show title placeholder

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASwitch v-model:checked="busy"/><ACard><ASkeleton :loading="busy" active><p>已加载的真实内容</p></ASkeleton></ACard><ASpace><ASkeletonAvatar active/><ASkeletonInput active/><ASkeletonButton active/></ASpace>
```

Source: pinned upstream es/components.js, es/skeleton, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
