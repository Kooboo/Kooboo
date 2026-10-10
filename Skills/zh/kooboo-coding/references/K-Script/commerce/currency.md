# Commerce Currency

> `k.commerce.currency` 负责站点 commerce 货币配置读取与汇率换算。它的前提不是“Kooboo 支持哪些货币”，而是“当前站点在 Commerce 后台真正启用了哪些货币”。

## 使用前提

在站点侧使用多货币前，先去 Commerce 后台配置：

- 默认货币
- 附加货币列表
- 每个附加货币的汇率或 `AutoUpdate`

如果后台没有启用某个币种：

- `k.commerce.currency.list()` 不会返回它；
- `k.commerce.order.create(..., { currency })` 不会真正创建成这个币种，而会回退到默认货币；
- 第三方支付即使 provider 支持该币种，你也不应直接在站点里启用它。

## default

获取站点 commerce 默认货币代码。

```ts
k.api.get(() => {
    return k.commerce.currency.default
})
```

**返回：** `string`（如 `"CNY"`）

## list()

获取当前站点可用的货币列表。

```ts
k.api.get(() => {
    return k.commerce.currency.list()
})
```

**返回：** `CurrencyItem[]`

运行时行为要点：

- 返回结果一定优先包含默认货币，且默认货币的 `exchangeRate` 固定为 `1`。
- 之后才是后台配置的附加货币。
- 若某个附加货币开启了 `AutoUpdate`，`list()` 会在读取时尝试用最新汇率刷新 `exchangeRate`。

推荐先看当前站点真正启用了哪些货币：

```ts
k.api.get(() => {
    const currencies = k.commerce.currency.list()
    return {
        defaultCurrency: k.commerce.currency.default,
        currencies
    }
})
```

## convert()

货币汇率转换。

```ts
k.api.get(() => {
    const usdAmount = 2
    const cnyAmount = k.commerce.currency.convert("USD", "CNY", usdAmount)
    return { usdAmount, cnyAmount }
})
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| from | `string` | 源货币代码 |
| to | `string` | 目标货币代码 |
| value | `number` | 转换金额 |

**返回：** `number`

运行时行为要点：

- 当 `from` 等于站点默认货币时，Kooboo 会优先读取当前站点对 `to` 的本地配置汇率。
- 如果 `to` 这个附加货币配置了固定汇率，就使用固定汇率。
- 如果 `to` 开启了 `AutoUpdate`，则会使用远程汇率结果。
- 当 `from` 不是站点默认货币时，会直接走通用汇率服务，而不是站点本地配置表。

## 支付场景怎么配合使用

`k.commerce.currency` 适合做三类事：

1. 决定站点应该向用户展示哪些币种；
2. 与 `k.payment.xxx.supportedCurrency` 做交集，筛选当前 provider 真正可用的币种；
3. 做展示价、报价或 provider 选择前的换算。

推荐交集写法：

```ts
k.api.get(() => {
    const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
    const provider = k.payment.stripeCheckout.supportedCurrency.map(code => code.toUpperCase())
    const available = provider.filter(code => configured.includes(code))
    return { available }
})
```

如果目标是“这张 commerce 订单最终要按某个币种支付”，优先在下单阶段直接创建那个币种的订单，而不是只做 `convert(...)`：

```ts
k.api.post((body) => {
    const order = k.commerce.order.create(body.cartId, {
        address: body.address,
        note: body.note,
        currency: body.currency
    })

    return {
        orderId: order.id,
        currency: order.currency,
        totalAmount: order.totalAmount
    }
})
```

## CurrencyItem 类型定义

```ts
interface CurrencyItem {
    name: string;
    nameNative: string;
    code: string;
    symbol: string;
    exchangeRate: number;
}
```
