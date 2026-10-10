# ADropdown

Ant Design Vue 4.2.6; native export `Dropdown`. Import `ADropdown` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `align`: Object; default -. this value will be merged into placement's config, please refer to the settings [dom-align](https://github.com/yiminghe/dom-align)
- `arrow`: boolean | { pointAtCenter: boolean }; default false. Whether the dropdown arrow should be open
- `destroyPopupOnHide`: boolean; default false. Whether destroy dropdown when hidden
- `disabled`: boolean; default -. whether the dropdown menu is disabled
- `getPopupContainer`: Function(triggerNode); default `() => document.body`. to set the container of the dropdown menu. The default is to create a `div` element in `body`, you can reset it to the scrolling area and make a relative reposition. [example](https://codepen.io/afc163/pen/zEjNOy?editors=0010)
- `overlay(v-slot)`: [Menu](/components/menu); default -. the dropdown menu
- `overlayClassName`: string; default -. Class name of the dropdown root element
- `overlayStyle`: object; default -. Style of the dropdown root element
- `placement`: String; default `bottomLeft`. placement of pop menu: `bottomLeft` `bottom` `bottomRight` `topLeft` `top` `topRight`
- `trigger`: Array&lt;`click`|`hover`|`contextmenu`>; default `['hover']`. the trigger mode which executes the drop-down action, hover doesn't work on mobile device
- `open(v-model)`: boolean; default -. whether the dropdown menu is open

## Events

- `openChange`: a callback function takes an argument: `open`, is executed when the open state is changed. Not trigger when hidden by click item

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ADropdown><AButton>打开菜单</AButton><template #overlay><AMenu @click="event = $event.key"><AMenuItem key="编辑">编辑</AMenuItem><AMenuItem key="复制">复制</AMenuItem></AMenu></template></ADropdown><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/dropdown, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
