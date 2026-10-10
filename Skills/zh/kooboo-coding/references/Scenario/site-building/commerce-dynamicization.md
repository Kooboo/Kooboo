# Commerce 站点动态化契约

本文用于 `commerceModel = catalog-contact | online-checkout` 的公开商城前台动态化。它补充 Site Building S7-S10，不创建第二份 Storefront Blueprint，也不替代 Page Plan。

## 1. 数据源归属

电商业务实体默认以 Commerce 为唯一事实源：

| 实体 | 默认数据源 |
| --- | --- |
| product / category / variant | `commerce` |
| cart / customer / order | `commerce` |
| discount / shipping / membership / loyalty | `commerce` |
| Blog / FAQ / Lookbook / 品牌故事 | `content` |
| Commerce 无模型的站点专属扩展数据 | `sqlite` 或已确认的其它数据源 |

不得为了“后台可维护”就在 Content 中复制 Product、Category、Variant、Cart 或 Order。只有用户明确要求独立于 Commerce 的展示目录时，才允许把该目录建为 Content，并须说明它不参与 Commerce 库存、变体、购物车和订单链路。

数据源确认后必须：

1. 调用 `function_discover`，`tags` 包含 `Commerce`。
2. 对准备调用的 Commerce 函数先执行 `use_function`，读取真实 InputSchema / ReturnSchema。
3. SSR 商品/分类读取优先 `<k-data>` + `<query source="commerce">`。
4. 已有内置 Commerce 写入函数时，不用 `run_kscript` 重新实现同一写入。

## 2. Storefront 数据质量

AI 为商城前台创建的种子数据必须达到可展示、可路由标准。

Product 最小字段：

- `title`
- `seoName`
- `featuredImage`
- `price`
- `inventory`
- `active`
- `categoryIds`（使用 `create_product_category` 返回的分类 id；只有明确需要未分类商品时传 `[]`）

Category 最小字段：

- `title`
- `seoName`
- `image`
- `active`

这里的图片要求只适用于 AI 创建的 storefront 数据，不改变 Commerce runtime 对其它商品/分类的通用可选字段。图片使用真实站内媒体 URL 或 ImagePlaceholder；不得编造外链。

创建后必须调用详情函数读回关键字段，并确认 `product.categories` 包含预期分类、分类详情/后台计数不再为 0。`Save success`、函数调用成功或源代码中存在写入语句，都不能单独证明数据完整。

## 3. 商品价格与变体

- `product.price` 是当前商品变体价格的最小值，适合列表和“起售价”展示。
- `product.price` 不保证等于 `product.variants[0].price`。
- PDP 在用户选择具体变体后，展示选中 `variant.price`。
- 加入购物车、愿望清单和精确库存判断使用选中 `variant.id`。
- 只有确认商品仅有一个默认变体时，示例代码才可使用 `variants[0].id`；多变体商品必须先完成选项到 variant 的匹配。

## 4. 商品详情页 URL

需要独立访问的商品详情页应使用路径参数表达商品身份。优先使用 `/product/{seoName}`，链接使用 `/product/{item.seoName}`；PDP 从路由上下文读取 `seoName` 并传给 Commerce `item` 查询。列表状态或其它非身份值才使用 query string；只有需要兼容既有公开路由时才保留 query-string 商品身份。

## 5. Products 分类筛选

分类选择器必须使用同一个 `selectedCategory` 事实值驱动查询与 active UI。

- 分类列表只显示 `active = true`。
- 商品列表只显示 `active = true`。
- `All` 表示没有 category 条件；不得把 `all` 当成 category id 或 seoName。
- 切换分类时将分页重置到第一页。
- active 样式从当前 queryString 与 category `seoName` 比较得出，不写死在首个按钮上。
- active 可通过服务端动态 class 绑定，或由前端读取 `URLSearchParams` 后同步 `class` / `aria-current`；两种方式都必须以当前 category 参数为事实值。
- 支持 All、有商品分类、空分类、刷新、前进/返回和无结果状态。

推荐的 k-data 查询结构：

```html
<k-data>
  <let as="selected_category" source="queryString" from="category" default="''" export></let>
  <let as="page_number" source="queryString" from="page" default="1"></let>
  <query as="categories" source="commerce" resource="category" action="list" export
    where="{active:{$eq:true}}"></query>
  <when test="{selected_category:{$ne:''}}">
    <query as="product_paged" source="commerce" resource="product" action="paged" export
      where="{category:selected_category,active:{$eq:true}}"
      page-number="page_number" page-size="12"></query>
    <else>
      <query as="product_paged" source="commerce" resource="product" action="paged" export
        where="{active:{$eq:true}}"
        page-number="page_number" page-size="12"></query>
    </else>
  </when>
</k-data>
```

分类选择器和商品网格可以位于两个 View，但它们必须作为同一个 Products 页面能力验收：

- 页面内只保留一个分类选择器和一组权威 category 查询；
- 两个 View 均从同名 `selected_category` 读取当前 queryString，不各自维护第二份前端状态；
- 分类链接省略 `page` 参数，从而自然重置到第一页；
- Preflight 必须把该 Page 引用的相关 Views 一起传入，不能分别扫描后要求每个 View 都同时包含分类和商品查询。

### Products 运行验收

按顺序执行，不允许只观察初始页面：

1. 打开 `/products`，确认 All active 且只显示 active 商品；
2. 点击一个有商品的 active 分类，确认 URL、active 样式和结果集合一致；
3. 点击空分类，确认出现真实空状态而不是回退全部商品；
4. 再点 All，确认 URL 不携带 `category=all`；
5. 刷新并执行浏览器后退/前进，确认 active 与结果保持一致；
6. 检查 DOM 中不存在重复分类选择器。

## 6. 原地动态化

动态化默认修改现有 View，不重新生成一个通用 Commerce 模板。

必须保留：

- 已选择的 layout archetype / desktop topology；
- 主要 DOM 分区与信息顺序；
- 视觉层级、响应式行为和可访问性；
- Cart、Checkout、Account 等任务页面的现有设计差异。

允许为真实状态增加必要的 loading、empty、error、disabled、authenticated 和 cart-count 区域，但不得把原有视觉结构整体替换成无差异表格或简单卡片。

## 7. 跨页能力合同

### catalog-contact

必须完成 Product/Category 列表、详情和联系/询价入口。除非用户明确要求，不加入登录、购物车、订单或支付。

### online-checkout

必须沿同一 Commerce 上下文完成：

| Surface | 必须消费的状态 |
| --- | --- |
| Products | 分类、搜索、排序、分页、商品结果 |
| PDP | 媒体、变体、价格、数量、库存、加购 |
| Header | 顾客状态、登录/注册或账户/订单/退出、购物车数量 |
| Cart | 行项增减删、空状态、金额、优惠/配送状态 |
| Checkout | 联系、地址、配送、支付方式、订单确认 |
| Order Result | 成功、处理中、失败、取消和恢复行动 |
| Account / Orders | 当前顾客、订单列表和订单详情 |

登录或加购不能只更新当前局部组件：Header 必须立即更新，并在刷新/导航后从服务端 `kb_commerce` 上下文恢复。

登录实现默认采用 `k.commerce.customer.login/currentId/logout`。不要用 `commerce_token + localStorage` 替代 `kb_commerce`；完整四端点与验收路径见 `../commerce-customer-login-flow.md`。

Account 地址簿、Checkout 收货地址和订单地址共享 Commerce `Address` 字段语义。涉及任一地址表单时读取 [Commerce 顾客地址与收货地址契约](../commerce-customer-address-flow.md)，不能让不同页面各自裁剪并覆盖同一地址对象。

### Header 登录态

当前阶段 Header / Account 用共享 `/api/auth/me`（或等价 auth-status）恢复登录态，不要为 Header 引入 `env="server"`。

- guest 与 authenticated 两侧初始都必须 `hidden`，`fetch` 返回后再按 `authenticated` 互斥显示一侧。
- 动态显隐节点不得同时使用 `hidden` 与 `md:flex` / `lg:flex` / `sm:block` 等响应式 display class。断点控制放外层，`flex/grid` 布局放内层；Desktop 与 Mobile 的 auth 节点共享同一状态更新函数。
- 切换时必须同时处理两侧，禁止只显示账户入口却未隐藏登录/注册。
- 登录/注册/退出后刷新同一 auth-status；不要用 `innerHTML` 整段替换 Header（尤其不要连带换掉购物车节点）。

### PDP / Cart 客户端边界

- SSR/`k-data` 负责读取商品并确定默认 variant；
- 把 `productId`、默认 `variantId` 等客户端所需值渲染到 `data-*` 或安全 JSON；
- 客户端脚本只读取 DOM，不直接引用 `product_detail.variants[0].id` 等服务端模板变量；
- 没有 variant radio/select 时使用按钮上的默认 `data-variant-id`，不能查询不存在的控件后提交空值；
- 加购按钮必须用 `try/finally` 恢复 loading/disabled 状态；
- 加购成功统一触发 `kooboo:cart-changed`，Header 监听该事件后重新读取权威 cart detail；
- Header 的 cart 节点与 auth 节点分离，auth 刷新不得用 `innerHTML` 替换掉购物车 DOM。

### Account / Orders 页面最低产品契约

Account：

- 当前客户身份摘要、账户导航、退出入口和最近订单；
- guest、loading、authenticated、empty、error 状态；
- 不把整个页面缩减成一个通用欢迎卡片。

Orders：

- 日期、订单号、商品摘要、总金额、支付/履约状态和查看详情操作；
- loading、empty、error、pending、paid、canceled 状态；
- 移动端使用可扫描的订单块，不把桌面宽表格强行压缩。

Order Detail：

- 商品行、数量、单价、小计、折扣、运费、税费和总计；
- 收货地址、支付状态、配送状态和合法恢复操作；
- 读取详情、继续支付、取消等入口都验证 `order.customer.id === k.commerce.customer.currentId`；
- Order Result 只表达本次结算结果，不替代可长期访问的 Order Detail。

三个页面须沿用当前站点字体、色彩、间距、按钮和图片比例，不得用同一套无差异卡片模板覆盖不同站点。

### Checkout / Payment / Order Result

完整步骤、cart 归属校验、支付页服务端出表单、回跳只读状态与 Account 订单入口，见 [Commerce 结算与支付黄金路径](../commerce-checkout-payment-flow.md)。

硬约束摘要：

- 下单前 `customer.currentId` 与 `cart.currentId` 都在，且 `cart.customerId === currentId`；
- 创建后确认 `order.customer.id === currentId`，再跳转 `/api/payment/alipay?orderId=...`；
- 支付端点服务端 `charge({ order })` 并用 `k.response.write(renderHtml)` 直接输出，禁止 View 中的 `<script env="server">` 和客户端 innerHTML 注入；
- `returnUrl` 不是支付成功证据；Account 必须能列出并打开该顾客订单。

## 8. 配置入口

- 支付渠道密钥：服务集成 `Payment`。
- Commerce 支付开关/字段模型：Commerce Settings。
- 货币：Commerce Currencies。

需要用户打开后台完成配置时，不要猜测后台路径或手写 SiteId。若当前工具链没有专用且已验证的支付配置入口，明确提示用户在后台完成支付集成配置，并将该项保留为未完成证据；不要用通用 `show_action_button` 伪装成可信的配置入口。

## 9. 完成与证据

完成状态分开表述：

- `modified`：对象或数据已写入。
- `structurally verified`：最终写入后 Preflight 通过。
- `runtime verified`：原路径交互已在 Preview/Browser 中验证。
- `business verified`：订单、支付回调、后台结果等真实业务结果已确认。

Products All/分类、登录后 Header、加购后 Header 数量、Cart 增减删和 Checkout 配置入口均需要同路径运行证据。Preview 不可用时明确标记未验证，不得把 Preflight PASS 当作运行完成。

Preflight 返回的 `completionGate` 是机器可读上限：

- `claimAllowed=false` 时最终回复不得写“已完成”或“运行正常”；
- `verifiedArtifacts[].contentHash` 只证明对应版本的结构检查；
- 任一后续写入都会使旧哈希证据失效；
- Commerce `missingEvidence` 未完成时，最高只能表述为 `structurally verified`。

Commerce Preflight 按交付阶段调用：

- 静态首版：`deliveryStage: "static-first"`，动态规则只进入 `deferred`。
- 商品/顾客/购物车/结算动态化：`deliveryStage: "commerce-runtime"`，完成 Products、PDP、登录/Header、Cart、Checkout 与订单归属的同路径验收。
- 支付集成：`deliveryStage: "payment-runtime"`，完成 PaymentRequest、provider callback/checkStatus 与前后台 paid order state 证据。

不得在静态阶段用未来动态化规则制造 BLOCKER，也不得在 `commerce-runtime` 阶段把支付回跳当作支付验收完成。
