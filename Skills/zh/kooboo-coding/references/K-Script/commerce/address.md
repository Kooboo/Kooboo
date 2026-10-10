# Commerce Address

## countries()

获取所有国家信息。

```ts
k.api.get(() => {
    const countries = k.commerce.address.countries()
    return { count: countries.length, items: countries.slice(0, 3) }
})
```
**返回：** `Country[]`

## provinces()

根据国家名称获取省份列表。

```ts
k.api.get(() => {
    const provinces = k.commerce.address.provinces('China')
    return { count: provinces.length, items: provinces.slice(0, 3) }
})
```
**参数：** `country: string`（英文名称）| **返回：** `Province[]`

## cities()

根据国家与省份获取城市列表。

```ts
k.api.get(() => {
    const cities = k.commerce.address.cities('China', 'Fujian')
    return { count: cities.length, items: cities.slice(0, 3) }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| country | `string` | 国家名称（英文） |
| province | `string` | 省份名称（英文） |

**返回：** `City[]`

## addressDetail()

将地址中保存的国家、省份和城市名称解析为对应的行政区划对象。

```ts
k.api.get(() => {
    return k.commerce.address.addressDetail({
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1'
    })
})
```
**参数：** `address: Address` | **返回：** `AddressDetail`

## 类型定义

```ts
interface Country {
    code: string;
    name: string;
    nameTranslations: Record<string, string>;
    currency: string;
    currencySymbol: string;
    emojiFlag: string;
}

interface Province {
    country: string;
    name: string;
    nameTranslations: Record<string, string>;
}

interface City {
    country: string;
    province: string;
    name: string;
    nameTranslations: Record<string, string>;
}

interface Address {
    address1: string;
    address2?: string;
    city: string;
    country: string;
    firstName?: string;
    lastName?: string;
    phone?: string;
    province: string;
    zip?: string;
}

interface AddressDetail extends Address {
    countryDetail: Country | null;
    provinceDetail: Province | null;
    cityDetail: City | null;
}
```

## 边界说明

- 表单展示可以使用 `nameTranslations`，写入 `country/province/city` 时使用返回对象的英文 `name`。
- 顾客地址管理和 Checkout 收货地址 → [Commerce 顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md)
- 顾客地址覆盖写入 → [Customer Guide](customer.md) 的 `updateAddresses()`
- 订单收货地址 → [Order Guide](order.md) 的 `create()`
