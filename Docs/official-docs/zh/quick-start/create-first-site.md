# 创建第一个站点

> 从登录后台到进入站点管理页。

## 登录并确认入口

| 项目 | 说明 |
|------|------|
| 后台入口 | `/_Admin/` |
| 旧路径兼容 | `/_Admin/Sites` 会回到站点列表 |
| 主要页面 | 站点列表、文件夹、新建站点 |

登录后会进入站点列表。这里是账户级页面，不带 `SiteId`。如果你已经有站点，点击站点卡片上的 **管理** 进入站点后台。

## 新建站点

后台提供三种常用方式：

| 方式 | 深链 | 适合场景 |
|------|------|----------|
| 空白站点 | `/_Admin/create` | 想从最小结构开始 |
| 导入站点 | `/_Admin/create/import` | 已有站点包、ZIP 或迁移来源 |
| 克隆站点 | `/_Admin/create/clone` | 复制已有站点作为新项目起点 |

如果你想从模板开始，也可以先打开 [市场](/product/market.md)，选择模板后再创建站点。

## 第一次进入站点后台

进入站点后台后，URL 通常会带上站点 ID：

```text
/_Admin/site/dashboard?SiteId={站点GUID}
```

后续 CMS 文档里的站点级深链都会使用同样格式。你可以把 `{站点GUID}` 替换成当前站点地址栏里的 `SiteId`。

## 建议先确认

| 检查项 | 位置 |
|--------|------|
| 站点名称、默认语言、Base URL | [基础设置](../cms/settings/basic/general.md) |
| 是否需要多语言 | [基础设置：基础](../cms/settings/basic/general.md) 与 [SPA 多语言](../cms/development/spa-multilingual.md) |
| 是否需要团队协作 | [站点用户](../cms/settings/site-users.md) 与 [角色权限](../cms/settings/roles.md) |
| 是否需要 AI 能力 | [AI 服务](../cms/settings/ai-service.md) |

## 下一步

继续 [做第一个页面](./build-first-page.md)。
