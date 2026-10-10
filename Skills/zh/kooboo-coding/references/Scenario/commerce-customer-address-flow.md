# Commerce 顾客地址与收货地址契约

适用于 Account 保存地址、Checkout 收货地址、订单地址展示/更新，以及需要与后台 Commerce 顾客地址互通的站点实现。地址字段必须来自 Commerce `Address` 契约；不得把某个已生成 View、某次 Checkout 表单或页面当前可见字段反向当成数据模型。

开始实现前同时读取：

- [Commerce Customer](../K-Script/commerce/customer.md) 的 `updateAddresses()`；
- [Commerce Address](../K-Script/commerce/address.md) 的国家、省份、城市查询；
- [Customer Types](../K-Script/commerce/types/customer.md) 的 `AddressModel`；
- 创建订单时再读取 [Commerce Order](../K-Script/commerce/order.md)。

## 1. 权威字段契约

Commerce 顾客保存地址和订单收货地址共享以下字段语义：

| 字段 | 角色 |
|---|---|
| `id` | 保存地址的稳定标识；编辑已有地址时必须保留 |
| `isDefault` | 顾客地址簿状态；Checkout 新建的临时收货地址不需要用户填写 |
| `country` | Commerce 国家英文名称；选项来自 `k.commerce.address.countries()` |
| `province` | Commerce 省/州英文名称；选项来自 `provinces(country)` |
| `city` | Commerce 城市英文名称；选项来自 `cities(country, province)` |
| `address1` | 主要街道地址 |
| `address2` | 可选的补充地址，例如楼栋、单元或房间 |
| `firstName` | 收件人名 |
| `lastName` | 收件人姓 |
| `phone` | 收件联系电话；后台 Commerce 顾客地址会消费该字段 |
| `zip` | 邮政编码 |

`email` 属于顾客/Checkout 联系信息，不属于 `AddressModel`。Checkout 可以把 Contact 区域的电话号码映射到 `address.phone`，但不能因为 UI 把电话放在 Contact 区域就从收货地址 payload 中删除它。

页面可以按业务隐藏可选字段，但“未显示”不等于“从模型删除”。编辑已有地址时，未展示或本次未提交的字段必须保留原值。

## 2. 国家、省份和城市

默认使用 Commerce 行政区划作为选项源：

```ts
const countries = k.commerce.address.countries()
const provinces = k.commerce.address.provinces(country)
const cities = k.commerce.address.cities(country, province)
```

- 展示文本可以使用 `nameTranslations`，保存值使用对象的 `name`。
- 国家改变后重新加载 Province，并清除不再匹配的 Province/City。
- Province 改变后重新加载 City，并清除不再匹配的 City。
- 不从另一个站点 View 复制硬编码国家列表，也不据此声称“与后台一致”。
- 当 Commerce 对某国家没有 Province/City 列表且产品确实需要自由输入时，可以提供文本回退；必须明确这是产品回退，并保留完整字符串，不能把自由输入描述为后台行政区划已验证一致。

## 3. 顾客地址簿更新

`k.commerce.customer.updateAddresses(customerId, addresses)` 是**整组覆盖**：runtime 会直接用传入数组替换 `customer.addresses`。因此新增、编辑、删除和设置默认地址都必须先读取当前完整列表，再提交下一版完整数组。

编辑地址时使用字段白名单合并，不能把缺失字段写成空字符串：

```ts
const addressFields = [
    'country', 'province', 'city', 'address1', 'address2',
    'firstName', 'lastName', 'phone', 'zip'
]

function copyAddress(source) {
    const result = {
        id: source.id || '',
        isDefault: !!source.isDefault
    }
    addressFields.forEach((name) => {
        result[name] = source[name] || ''
    })
    return result
}

function patchAddress(current, patch) {
    const result = copyAddress(current)
    addressFields.forEach((name) => {
        if (Object.prototype.hasOwnProperty.call(patch, name)) {
            result[name] = patch[name] || ''
        }
    })
    if (Object.prototype.hasOwnProperty.call(patch, 'isDefault')) {
        result.isDefault = !!patch.isDefault
    }
    return result
}
```

禁止用以下模式处理部分表单：

```ts
current.phone = body.address.phone || ''
current.address2 = body.address.address2 || ''
```

如果本次表单没有 `phone` 或 `address2`，这种写法会在用户只修改邮编、城市等字段时清空已有数据。

地址簿还必须维持：

- 编辑已有地址时保留 `id`；
- 非空列表最多一个默认地址；
- 删除默认地址后，为剩余地址明确选择新的默认项；
- 调用 `updateAddresses()` 后重新 `customer.get(customerId)` 读回，不用 void 调用成功代替持久化验证。

## 4. Checkout 收货地址

Checkout 的可见布局可以不同于 Account 地址簿，但提交给 `order.create(..., { address })` 的字段语义必须一致：

- 手工填写地址时，把可见字段组装为 `Address`；Contact Phone 映射到 `address.phone`。
- 选择已保存地址时，复制完整地址对象，不重建一个只含当前页面可见字段的子集。
- Checkout 不默认覆盖顾客地址簿。只有产品明确提供“保存到地址簿”或“更新此地址”操作时，才调用 `updateAddresses()`。
- 隐藏 `address2` 等可选字段不影响订单创建，但从保存地址进入 Checkout 时仍应把已有值带入订单地址。

创建订单后读回订单，核对 `shippingAddress` 的关键字段；页面无 JS 错误或 UI audit 通过不能证明地址 payload 正确。

## 5. 完成与验收

地址能力只有完成对应路径后才能声明验证：

1. Account 新增地址，读回完整 `AddressModel`。
2. 用包含 `phone/address2` 的已有地址只修改一个其它字段，确认未提交字段仍保留。
3. Country → Province → City 的选项与保存值使用同一 Commerce `name`。
4. Checkout 使用手填地址和保存地址各创建一次订单，读回 `shippingAddress` 核对字段。
5. 后台顾客地址、Account 地址簿和订单详情能消费同一地址，不以静态源码或通用 UI diagnostics 代替往返验证。

无法执行真实读写时，只能表述为 contract/structurally verified，不得声称与后台字段完全一致或业务闭环已通过。
