# Service Layer Guide Service 层规范

## 定位

Service 层是业务逻辑的核心，负责：

- 数据获取和加工
- 业务规则实现
- 跨模块协调
- 错误处理和边界检查

API 层只负责参数获取和结果返回，不写业务逻辑。

## 命名规范

CodeBlock 的 Name 使用点分文件夹系统：


| 前缀          | 用途   | 示例                                                     |
| ----------- | ---- | ------------------------------------------------------ |
| `Services.` | 业务服务 | `Services.auth`、`Services.product`、`Services.order`    |
| `Utils.`    | 通用工具 | `Utils.formatDate`、`Utils.validation`、`Utils.response` |


## CodeBlock 组织

一个功能模块对应一个 CodeBlock 文件：

```
Services.product    // 产品相关服务
├── listProducts()   // 导出函数
├── getProduct()     // 导出函数
└── searchProducts() // 导出函数
```

```ts
// Services.product (CodeBlock)

export function listProducts(query) {
    const products = k.commerce.product.list(query)
    return products
}

export function getProduct(id) {
    try {
        return k.commerce.product.get(id)
    } catch (e) {
        return null
    }
}
```

## import 模式

Service 层可以 import 其他 Service 或 Utils：

```ts
// Services.order 引用 Services.product
import { getProduct } from "./Services.product"
import { formatDate } from "./Utils.formatDate"

export function createOrder(items) {
    const product = getProduct(items[0].productId)
    return {
        items,
        createdAt: formatDate(new Date())
    }
}
```

import 路径使用相对路径，以 `.` 开头：

- 引用：`./Services.product`

## 错误处理

Service 层负责捕获和处理错误：

```ts
export function getProduct(id) {
    try {
        const product = k.commerce.product.get(id)
        if (!product) {
            return { error: "Product not found" }
        }
        return product
    } catch (e) {
        return { error: e.message }
    }
}
```

## 数据转换

在 Service 层完成数据转换，保持 API 层简洁：

```ts
// Services.product

export function listProducts(query) {
    const products = k.commerce.product.list(query)
    return products.map(p => ({
        id: p.id,
        name: p.name,
        price: p.price,
        image: p.images?.[0]?.url
    }))
}
```

## 组合模式

复杂业务可能需要多个 Service 协作：

```ts
// Services.checkout

import { getCart } from "./Services.cart"
import { calculateDiscount } from "./Services.discount"
import { createOrder } from "./Services.order"

export function checkout(cartId, customerId) {
    const cart = getCart(cartId)
    if (!cart || cart.items.length === 0) {
        return { error: "Cart is empty" }
    }

    const discount = calculateDiscount(cart, customerId)
    const order = createOrder({
        cartId,
        customerId,
        discount
    })

    return { order, discount }
}
```

## 注意事项

1. **不要在 Service 层直接返回 HTTP 响应**，只返回数据
2. **保持函数单一职责**，每个函数只做一件事
3. **避免循环依赖**，Service A 引用 Service B 时，不要让 Service B 也引用 Service A
4. **错误统一处理**，用返回对象 `{ error }` 而非抛出异常
5. **调试输出使用 `k.logger`**，不要使用 `console.log` / `console.error`（Kooboo 服务端不支持 console 方法）
6. **需要被 API 调用的逻辑必须 `export function`**；API 侧通过 `import { ... } from "./Services.xxx"` 引用。**不存在** `k.req.services` 这类从请求对象反射调用 CodeBlock 的语法。

