# YlPopover

Import and register `{ YlPopover }` from `/yuanli.js`.
Props: `modelValue: boolean` via v-model, `title: string` required, `width: number = 320`, `id?: string`. Event: `update:modelValue`.
The `trigger` slot receives `{ open, toggle, triggerAttrs }`; bind triggerAttrs to a YlButton for aria-haspopup/expanded/controls. The default slot receives `{ close }`.

```html
<YlPopover v-model="accountOpen" title="账户" :width="240">
  <template #trigger="{triggerAttrs, toggle}"><YlButton icon="user-round" v-bind="triggerAttrs" @click="toggle">账户</YlButton></template>
  <YlButton icon="log-out" @click="signOut">退出登录</YlButton>
</YlPopover>
```

Use anchored non-modal panels for header notifications/account, not YlDialog. Opening focuses the panel; Tab follows normal controls and leaving dismisses it. Outside click dismisses; Escape closes and returns focus to the trigger. Window scroll/resize repositions it. No focus trap or background mask. Width is clamped to viewport; do not place under transformed or paint-contained ancestors. This is a content panel with role=dialog, not a menu with arrow-key menu-item semantics. Use it in the Shell actions slot; keep only one header panel open.

`persistent=false`: when true, suppress trigger toggling and automatic outside/Escape dismissal while a caller-owned operation is busy. Parent modelValue remains authoritative.

`align="end"` aligns the panel to the trigger end; `align="start"` aligns to its start. Both clamp to the viewport and flip above when needed. Dropdown and Popconfirm use start alignment.
