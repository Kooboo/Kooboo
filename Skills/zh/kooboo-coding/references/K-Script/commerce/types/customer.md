# Commerce Customer Types

## Customer

```ts
interface Customer {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    phone: string;
    password: string;
    lastPlaceOrderDate: string | null;
    addresses: AddressModel[];
    tags: string[];
    createdAt: string;
    updatedAt: string;
}
```

## AddressModel

```ts
interface AddressModel {
    id: string;
    isDefault: boolean;
    country: string;
    province: string;
    city: string;
    address1: string;
    address2: string;
    firstName: string;
    lastName: string;
    phone: string;
    zip: string;
}
```

字段角色：

- `password` 是存储的密码哈希，不应暴露给客户端。公开接口不要直接返回完整 `Customer` 对象，而应投影为安全字段。
- `id`、`isDefault` 是地址簿状态；编辑已有地址时保留 `id`，非空列表最多一个默认地址。
- `country`、`province`、`city` 默认保存 Commerce 行政区划对象的英文 `name`；本地化名称只用于展示。
- `address2` 是可选补充地址，`phone` 是收件联系电话。页面未展示这些字段时，编辑操作仍须保留已有值。
- `email` 不属于 `AddressModel`；Checkout 联系邮箱保留在顾客/联系信息层。
- `customer.updateAddresses()` 会整组覆盖地址数组。Account、Checkout 和订单地址的组装与验证见 [Commerce 顾客地址与收货地址契约](../../../Scenario/commerce-customer-address-flow.md)。

## CustomerOAuthModel

```ts
interface CustomerOAuthModel {
    CustomerId: string;
    OpenId: string;
    Type: string;
    CreatedAt: string;
}
```
