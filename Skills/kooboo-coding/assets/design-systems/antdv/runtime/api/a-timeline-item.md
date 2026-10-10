# ATimelineItem

Ant Design Vue 4.2.6; native export `TimelineItem`. Import `ATimelineItem` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `color`: string; default `blue`. Set the circle's color to `blue`, `red`, `green` or other custom colors
- `dot`: string|slot; default -. Customize timeline dot
- `label`: string | slot; default -. Set the label
- `position`: `left` | `right`; default -. Customize node position

## Events

No events listed in the pinned upstream metadata.

## Slots

- `dot`: Customize timeline dot
- `label`: Set the label

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATimeline><ATimelineItem color="green">已创建</ATimelineItem><ATimelineItem>审核中</ATimelineItem><ATimelineItem color="gray">待完成</ATimelineItem></ATimeline>
```

Source: pinned upstream es/components.js, es/timeline, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
