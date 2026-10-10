# ALocaleProvider

Ant Design Vue 4.2.6; native export `LocaleProvider`. Import `ALocaleProvider` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Legacy upstream export retained for compatibility; use AConfigProvider for new Apps.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ALocaleProvider :locale="antEnUS"><APagination :total="50" show-size-changer/></ALocaleProvider><p>兼容旧版；新代码使用 ConfigProvider。</p>
```

Source: pinned upstream es/components.js, es/locale-provider, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
