# ElConfigProvider

Config Provider is used for providing global configurations, which enables your entire application to access these configurations everywhere.

Kind: component. Import `ElConfigProvider` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/config-provider.html#config-provider

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| locale | {name: string, el: TranslatePair} | [en](https://github.com/element-plus/element-plus/blob/dev/packages/locale/lang/en.ts) | Locale Object |
| size | 'large' \| 'default' \| 'small' | default | global component size |
| z-index | number | not declared | global Initial zIndex |
| namespace | string | el | global component className prefix (cooperated with [$namespace](https://github.com/element-plus/element-plus/blob/dev/packages/theme-chalk/src/mixins/config.scss#L1)) |
| button | {autoInsertSpace?: boolean, type?: string, plain?: boolean, text?: boolean, round?: boolean, dashed?: boolean} | see the following table | button related configuration, [see the following table](#button-attribute) |
| link | {type?: string, underline?: boolean \| string} | see the following table | link related configuration, [see the following table](#link-attribute) |
| dialog | {alignCenter?: boolean, draggable?: boolean, overflow?: boolean, transition?: DialogTransition} | see the following table | dialog related configuration, [see the following table](#dialog-attribute) |
| message | {max?: number} | see the following table | message related configuration, [see the following table](#message-attribute) |
| experimental-features | object | not declared | features at experimental stage to be added, all features are default to be set to false |
| empty-values | array | not declared | global empty values of components |
| value-on-clear | string / number / boolean / Function | not declared | global clear return value |
| table | {showOverflowTooltip?: boolean \| object, tooltipEffect?: string, tooltipOptions?: object, tooltipFormatter?: Function} | see the following table | table related configuration, [see the following table](#table-attribute) |

## Slots

- `default`: customize default content
