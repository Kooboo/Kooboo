# 写第一个 KScript API

> 在 Kooboo 中加入一个服务端接口，让前台页面可以读取 JSON 或执行业务逻辑。

## 先选入口

| 入口 | 适合场景 | 文档 |
|------|----------|------|
| API 页面 | 有独立 URL，需要返回 JSON 或处理表单 | [CMS：Open API](../cms/development/openapis.md) · [k.api](../api/api/index.md) |
| CodeBlock | 多个页面或 API 复用同一段逻辑 | [CMS：代码](../cms/development/code.md) · [k.site.codes](../api/site/code.md) |
| 定时任务 | 定期同步、清理、发送通知 | [CMS：定时任务](../cms/development/jobs.md) · [k.site.runJob](../api/site/job.md) |

第一次建议从 API 页面开始：输入 URL、保存脚本，然后在浏览器或前台 `fetch` 中调用。

## 一个最小 API 需要考虑什么

| 项目 | 说明 |
|------|------|
| 路由 | 给 API 一个稳定 URL，例如 `/api/news` |
| 输入 | 从 [k.request](../api/request/index.md) 读取 query、body、headers 或 files |
| 业务数据 | 用 [k.content](../api/content/index.md)、[k.DB](../api/database/index.md) 或外部接口取数 |
| 输出 | 用 [k.response](../api/response/index.md) 返回 JSON、重定向或状态码 |
| 权限 | 需要登录时读取 [k.session](../api/session/index.md) 或账户相关 API |
| 日志 | 关键失败路径写入 [k.logger](../api/logger/index.md) |

## 与后台配置配合

| 你要做 | 先配哪里 |
|--------|----------|
| 读取内容列表 | [数据类型](../cms/content/content-types.md) 与 [内容文件夹](../cms/content/contents-folders-list.md) |
| 发送邮件 | [邮箱](/product/mail.md) 与 [k.mail](../api/mail/index.md) |
| 发起支付 | [Payment 集成](../cms/settings/integrations/payment.md) 与 [k.payment](../api/payment/index.md) |
| 调第三方接口 | [Open API](../cms/development/openapis.md) 或 [k.net.httpClient](../api/net/httpclient.md) |
| 保存业务数据 | [数据库](../cms/database/index.md) 与 [k.DB](../api/database/index.md) |

## 调试建议

1. 先用固定返回值确认路由可以访问。
2. 再接入真实数据查询。
3. 给错误路径返回明确状态码与 JSON。
4. 最后在页面中调用 API，并检查浏览器 Network。
