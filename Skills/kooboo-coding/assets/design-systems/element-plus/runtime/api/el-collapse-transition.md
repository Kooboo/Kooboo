# ElCollapseTransition

Native Element Plus height transition wrapper.

Kind: component. Import `ElCollapseTransition` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Wrap one element controlled by v-show in the default slot. It supplies transition behavior, not state or layout. Respect reduced-motion needs in the App's accessibility policy.

```js
h(ElCollapseTransition, null, { default: () => contentVNode });
```
