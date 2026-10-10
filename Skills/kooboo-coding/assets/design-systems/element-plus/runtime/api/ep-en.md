# epEn

English locale object for ElConfigProvider.

Kind: locale. Import `epEn` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Pass :locale=epEn to ElConfigProvider for English control text. It does not translate business copy or choose a timezone.

```js
h(ElConfigProvider, { locale: epEn }, { default: () => h(CurrentScreen) });
```
