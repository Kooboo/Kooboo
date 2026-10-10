# address

> 地址管理

## 概述

`k.commerce.address` 提供地址相关操作，用于获取国家、省份、城市等地理信息。

## countries()

获取所有国家信息。

**参数**：无。

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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| country | `string` | 是 | 国家名称（英文） |

**返回：** `Province[]`

## cities()

根据国家与省份获取城市列表。

```ts
k.api.get(() => {
    const cities = k.commerce.address.cities('China', 'Fujian')
    return { count: cities.length, items: cities.slice(0, 3) }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| country | `string` | 是 | 国家名称（英文） |
| province | `string` | 是 | 省份名称（英文） |

**返回：** `City[]`

## 结构

### Country 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| code | 国家代码 | `string` |
| name | 国家名称（英文） | `string` |
| nameTranslations | 国家名称翻译 | [`NameTranslation[]`](#nametranslation-属性) |
| currency | 货币代码 | `string` |
| currencySymbol | 货币符号 | `string` |
| emojiFlag | 国旗 emoji | `string` |

### Province 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| country | 国家名称（英文） | `string` |
| name | 省份名称（英文） | `string` |
| nameTranslations | 省份名称翻译 | [`NameTranslation[]`](#nametranslation-属性) |

### City 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| country | 国家名称（英文） | `string` |
| province | 省份名称（英文） | `string` |
| name | 城市名称（英文） | `string` |
| nameTranslations | 城市名称翻译 | [`NameTranslation[]`](#nametranslation-属性) |

### NameTranslation 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| key | 语言代码 | `string` |
| value | 对应翻译 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [customer](./customer.md) - 客户管理
- [order](./order.md) - 订单管理
