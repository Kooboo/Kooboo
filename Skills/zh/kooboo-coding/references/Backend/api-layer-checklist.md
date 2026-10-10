# Api Layer Checklist API 层自检清单

在开始编写 API 代码前，逐项确认：

## 1. 模块判断

- [ ] 这个 API 属于哪个业务模块？（commerce / 其他）
- [ ] 如果是 commerce 模块，先 [Commerce 模块](../K-Script/commerce/index.md) 确认对应的子模块 skill

## 2. create_api 参数

- [ ] `Name` 是否为**简洁 API 标识**（如 `product`、`auth`、`init-sqlite`）？**不要**把 API 的 `Name` 写成 `Services.xxx`（后者仅用于 `create_codeblock` 的 Service / Utils CodeBlock）
- [ ] `Url` 是否以 `/` 开头？
- [ ] `Url` 是否包含 `{action}`？（如果需要多个子路由）
- [ ] `Content` 中换行是否用 `\\n` 转义？

## 3. 路由设计

- [ ] 如果需要动态子路由，`Url` 是否包含 `{action}`？
- [ ] 通配路由 `k.api.method(()=>{})` 是否放在文件最后？

## 4. 参数获取

- [ ] GET 参数用函数参数获取（如 `(page, size)`）
- [ ] POST/PUT 参数用函数参数获取（如 `(body)`）
- [ ] 必填参数是否已校验？

## 5. Service 调用

- [ ] 业务逻辑是否仅通过 **`import { ... } from "./Services.xxx"`**（或 `./Utils.xxx`）调用？**禁止** `k.req.services`、`k.request.services` 及任何未在文档中出现的「请求侧按名调 Service」写法
- [ ] Service 中对外函数是否已 **`export function`**，且 import 路径以 `./` 开头且与实际 CodeBlock 名称一致？

## 6. 响应

- [ ] 是否有错误处理（404 / 400 / 401）？
- [ ] 响应格式是否一致？

---

**通过全部检查项后，再调用 `create_api` 写入代码。**
