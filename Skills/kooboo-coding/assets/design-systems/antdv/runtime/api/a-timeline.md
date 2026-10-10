# ATimeline

Ant Design Vue 4.2.6; native export `Timeline`. Import `ATimeline` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `mode`: `left` | `alternate` | `right`; default `left`. By sending `alternate` the timeline will distribute the nodes to the left and right.
- `pending`: boolean|string|slot; default `false`. Set the last ghost node's existence or its content
- `pendingDot`: string|slot; default `<LoadingOutlined />`. Set the dot of the last ghost node when pending is true
- `reverse`: boolean; default false. reverse nodes or not

## Events

No events listed in the pinned upstream metadata.

## Slots

- `pending`: Set the last ghost node's existence or its content
- `pendingDot`: Set the dot of the last ghost node when pending is true

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATimeline><ATimelineItem color="green">已创建</ATimelineItem><ATimelineItem>审核中</ATimelineItem><ATimelineItem color="gray">待完成</ATimelineItem></ATimeline>
```

Source: pinned upstream es/components.js, es/timeline, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
