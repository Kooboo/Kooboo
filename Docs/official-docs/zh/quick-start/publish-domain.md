# 发布与域名

> 确认站点可以被外部访问，并完成基础上线配置。

## 先确认基础设置

| 项目 | 位置 |
|------|------|
| 站点名称、Base URL | [基础设置：基础](../cms/settings/basic/general.md) |
| 访问控制 | [基础设置：访问控制](../cms/settings/basic/access.md) |
| 性能缓存 | [基础设置：性能](../cms/settings/basic/performance.md) |
| SEO 默认值 | [基础设置：SEO](../cms/settings/basic/seo.md) |

Base URL 会影响站点生成的绝对地址、AI 服务 Endpoint、SEO 与部分集成回调地址。上线前建议先把它设置成最终域名。

## 绑定域名

| 项目 | 说明 |
|------|------|
| 菜单路径 | 站点后台 -> 设置 -> 域名管理 |
| 深链 | `/_Admin/system/domains?SiteId={站点GUID}` |
| 详细文档 | [CMS：域名管理](../cms/settings/domains.md) |

域名配置完成后，回到浏览器直接访问域名，确认首页、子页面、静态资源与表单提交都能正常工作。

## 发布前检查

| 检查项 | 去哪里 |
|--------|--------|
| 页面 URL 与标题 | [页面列表](../cms/site/pages.md) |
| 图片与附件 | [媒体库](../cms/site/media.md) |
| 内容条目 | [内容](../cms/content/index.md) |
| Cookie 合规横幅 | [Cookie](../cms/settings/cookie.md) |
| 支付、短信、OAuth、存储等外部服务 | [服务集成](../cms/settings/integrations/index.md) |
| 请求或操作前后的自动逻辑 | [请求 Hook](../cms/settings/request-hooks.md) · [操作 Hook](../cms/settings/action-hooks.md) |

## 下一步

如果站点需要表单、业务查询、Webhook 或第三方接口代理，继续 [写第一个 KScript API](./add-script-api.md)。
