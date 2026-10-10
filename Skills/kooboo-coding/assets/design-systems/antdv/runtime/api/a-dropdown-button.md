# ADropdownButton

Ant Design Vue 4.2.6; native export `DropdownButton`. Import `ADropdownButton` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `disabled`: boolean; default -. whether the dropdown menu is disabled
- `icon`: vNode | slot; default -. Icon (appears on the right)
- `loading`: boolean | { delay: number }; default false. Set the loading status of button
- `overlay(v-slot)`: [Menu](/components/menu); default -. the dropdown menu
- `placement`: String; default `bottomLeft`. placement of pop menu: `bottomLeft` `bottom` `bottomRight` `topLeft` `top` `topRight`
- `size`: string; default `default`. size of the button, the same as [Button](/components/button)
- `trigger`: Array&lt;`click`|`hover`|`contextmenu`>; default `['hover']`. the trigger mode which executes the drop-down action
- `type`: string; default `default`. type of the button, the same as [Button](/components/button)
- `open(v-model)`: boolean; default -. whether the dropdown menu is open

## Events

- `click`: a callback function, the same as [Button](/components/button), which will be executed when you click the button on the left
- `openChange`: a callback function takes an argument: `open`, is executed when the open state is changed. Not trigger when hidden by click item

## Slots

- `icon`: Icon (appears on the right)

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ADropdown><AButton>打开菜单</AButton><template #overlay><AMenu @click="event = $event.key"><AMenuItem key="编辑">编辑</AMenuItem><AMenuItem key="复制">复制</AMenuItem></AMenu></template></ADropdown><p>{{ event }}</p>
```

Source: pinned upstream es/components.js, es/dropdown, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
