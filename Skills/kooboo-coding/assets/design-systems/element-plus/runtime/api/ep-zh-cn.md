# epZhCn

Chinese locale object for ElConfigProvider.

Kind: locale. Import `epZhCn` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Pass :locale=epZhCn to ElConfigProvider. A locale object is neither a component nor an installation plugin. Keep locale consistent with current App content. It does not set date serialization or business timezone.

```js
// Select ElConfigProvider and epZhCn.
export default { components: { ElConfigProvider }, setup: () => ({ epZhCn }), template: '<ElConfigProvider :locale="epZhCn"><RouterView /></ElConfigProvider>' };
```
