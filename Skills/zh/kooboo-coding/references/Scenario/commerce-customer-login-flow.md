# Commerce 顾客登录与下单流程

适用于需要登录、注册、账户、购物车或订单的 Commerce 站点。目标是让所有消费者共享 Kooboo 已维护的 `kb_commerce`，避免登录 API、Account、Header 和 Cart 各自维护不同状态。

## 1. 唯一默认登录状态

Commerce 顾客流程默认只使用：

- `k.commerce.customer.login(email, password)`：验证身份并写入当前 `ECommerceState.CustomerId`；响应会维护 `kb_commerce`。
- `k.commerce.customer.currentId`：读取当前 Commerce 顾客 ID。
- `k.commerce.customer.get(currentId)`：读取当前顾客资料。
- `k.commerce.customer.logout()`：清除当前 Commerce customer/cart/order 上下文。

不要默认再创建 `commerce_token`，也不要把 token 放进 `localStorage` 作为 Account/Header 的另一套登录事实源。只有用户明确要求独立应用鉴权时才额外签发 token；该 token 不能替代 `customer.login/currentId/logout` 和 `kb_commerce`。

`customer.login(...)` 还会把当前 `ECommerceState.CartId` 对应的购物车绑定到登录顾客。登录后仍出现 `cart.customerId !== customer.currentId`，优先排查会话/购物车状态漂移、客户端保存了旧 cartId、登录响应未落到当前浏览器，或登录后又切换了购物车；不要默认重建购物车，也不要凭空发明绑定 API。

禁止：

- `k.request.cookies.get(...)`：Cookie API 是 `k.cookie.get(...)`，但默认 Commerce 登录状态甚至不需要手工读取 cookie。
- `k.cookie.delete(...)`：删除 Cookie 使用 `k.cookie.remove(...)`。
- `k.request.query...`：URL 参数使用 `k.request.queryString.get(...)`。
- 仅通过 `getByEmail(...)`、自定义 JWT 或 `localStorage` 判断 Commerce 登录。

## 2. 四个最小端点

### 登录

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''
    if (!email || !password) {
        return { success: false, authenticated: false, error: 'email and password are required' }
    }

    const customer = k.commerce.customer.login(email, password)
    if (!customer) {
        return { success: false, authenticated: false, error: 'email or password is incorrect' }
    }

    return {
        success: true,
        authenticated: true,
        customerId: customer.id,
        email: customer.email,
        firstName: customer.firstName
    }
})
```

### 注册并建立登录态

注册成功不等于已登录。创建顾客后必须立即调用 `login(...)`，让 Account/Header/Cart 在下一次请求中读取到同一个 Commerce 状态。

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''
    if (!email || !password) {
        return { success: false, authenticated: false, error: 'email and password are required' }
    }

    k.commerce.customer.create({
        email,
        password,
        firstName: body.firstName || '',
        lastName: body.lastName || '',
        phone: body.phone || ''
    })

    const customer = k.commerce.customer.login(email, password)
    if (!customer) {
        return { success: false, authenticated: false, error: 'customer created but login state was not established' }
    }

    return { success: true, authenticated: true, customerId: customer.id, email: customer.email }
})
```

### 当前顾客

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) {
        return { success: true, authenticated: false, customer: null }
    }

    const customer = k.commerce.customer.get(customerId)
    if (!customer) {
        return { success: true, authenticated: false, customer: null }
    }

    return {
        success: true,
        authenticated: true,
        customer: {
            id: customer.id,
            email: customer.email,
            firstName: customer.firstName,
            lastName: customer.lastName,
            phone: customer.phone
        }
    }
})
```

### 退出

```ts
k.api.post(() => {
    k.commerce.customer.logout()
    return { success: true, authenticated: false }
})
```

## 3. 前端消费者合同

登录或注册成功后导航到 Account；Account 和共享 Header 通过同一个“当前顾客”API（`/api/auth/me`）恢复状态。登录成功后 cookie / `kb_commerce` 已有 `CustomerId`，双显通常是 guest 与 authenticated 节点首屏同时可见、再靠异步 fetch 纠正。

当前阶段默认采用 **API + 客户端显隐**，不要为 Header 首屏引入 `env="server"`。

### Header / Account（唯一推荐实现）

```html
<header>
  <div class="hidden md:block">
    <div data-auth-guest class="hidden">
      <div class="flex items-center gap-4">
        <a href="/login">Login</a>
        <a href="/register">Register</a>
      </div>
    </div>
    <div data-auth-user class="hidden">
      <div class="flex items-center gap-4">
        <a href="/account" data-auth-name></a>
        <a href="/orders">Orders</a>
        <button type="button" data-logout>Logout</button>
      </div>
    </div>
  </div>

  <div class="md:hidden">
    <div data-auth-guest class="hidden">
      <a href="/login">Login</a>
      <a href="/register">Register</a>
    </div>
    <div data-auth-user class="hidden">
      <a href="/account" data-auth-name></a>
      <a href="/orders">Orders</a>
      <button type="button" data-logout>Logout</button>
    </div>
  </div>
</header>

<script>
function applyAuthState(state) {
  const authenticated = !!state.authenticated
  document.querySelectorAll('[data-auth-guest]').forEach((el) => {
    el.classList.toggle('hidden', authenticated)
  })
  document.querySelectorAll('[data-auth-user]').forEach((el) => {
    el.classList.toggle('hidden', !authenticated)
  })

  if (!authenticated || !state.customer) return
  document.querySelectorAll('[data-auth-name]').forEach((el) => {
    el.textContent = state.customer.firstName || state.customer.email || ''
  })
}

async function refreshAuth() {
  try {
    const response = await fetch('/api/auth/me', { credentials: 'same-origin' })
    if (!response.ok) throw new Error('Unable to load auth state')
    applyAuthState(await response.json())
  } catch (error) {
    applyAuthState({ authenticated: false, customer: null })
  }
}

document.addEventListener('click', async (event) => {
  const button = event.target.closest('[data-logout]')
  if (!button) return
  await fetch('/api/auth/logout', { method: 'POST', credentials: 'same-origin' })
  await refreshAuth()
})

refreshAuth()
</script>
```

要求：

- guest 与 authenticated **两侧初始都必须 `hidden`**，等 `/api/auth/me` 返回后再只打开一侧；禁止首屏两侧同时可见。
- `data-auth-guest` / `data-auth-user` 所在的动态显隐节点只能使用基础 `hidden`，不得同时带 `md:flex`、`lg:flex`、`sm:block` 等响应式 display class。响应式布局放到外层断点容器或内层 `flex/grid` 容器，否则断点样式会覆盖运行时 `hidden`。
- Desktop 与 Mobile 可以使用不同 DOM，但必须复用同一个 `applyAuthState`；所有 `data-auth-guest` 和 `data-auth-user` 节点都遵循同一互斥状态。
- 切换时按 `authenticated` 互斥设置，不要只 `remove('hidden')` 一侧而忘了隐藏另一侧。
- Account 与 Header 共用同一 auth-status 端点；不要用 `localStorage` / `commerce_token` 替代。
- 不要手工解析 `kb_commerce` cookie。
- 登录、注册、退出后必须刷新 Header（整页导航或再次调用 `refreshAuth`）。
- Cart/Order 使用同一个 `kb_commerce` 上下文，不在下单 API 中再次接收邮箱密码。
- 刷新 Header 时不要用 `innerHTML` 整段替换（尤其不要连带换掉购物车节点）。
- Account 包含地址簿或 Checkout 复用保存地址时，按 [Commerce 顾客地址与收货地址契约](commerce-customer-address-flow.md) 读取完整 `AddressModel`，不得用页面可见字段覆盖隐藏字段。

## 4. 交付前必测路径

登录功能只有完成以下同路径验证后才能标记 `runtime verified`：

1. 注册新顾客 -> 自动进入已登录 Account。
2. 退出 -> Account 变为 guest，Header 恢复登录/注册。
3. 已有顾客登录 -> Account 显示顾客资料，Header 显示 Account/Orders/Logout，且**不再显示**登录/注册。
4. 刷新 Account 和任一含 Header 的页面 -> 登录状态保持一致；加载过程中**不得**登录与账户入口同时可见。
5. 登录后创建/读取 Cart -> Cart 的 `customerId` 与当前顾客一致。

Preflight PASS 只能证明结构没有命中已知错误，不能替代以上浏览器运行证据。无法运行 Preview/Browser 时必须标记 `structurally verified`，不得直接说登录完成。

## 相关参考

- 顾客 API：[Commerce Customer](../K-Script/commerce/customer.md)
- 顾客地址与收货地址：[Commerce 顾客地址与收货地址契约](commerce-customer-address-flow.md)
- 购物车：[Commerce Cart](../K-Script/commerce/cart.md)
- 订单：[Commerce Order](../K-Script/commerce/order.md)
- 结算支付：[Commerce 结算与支付黄金路径](commerce-checkout-payment-flow.md)
- Commerce 整站动态化：[Commerce 站点动态化契约](site-building/commerce-dynamicization.md)
