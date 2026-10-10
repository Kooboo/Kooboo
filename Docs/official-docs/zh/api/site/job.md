# k.site.runJob

> 对应后台 [开发 → 定时任务](../../cms/development/jobs.md)

## 概述

站点 **定时任务（SiteJob）** 在后台配置名称、调度与 **内联 KScript**。运行时由 `JobService` 在服务端执行，不经过前台 HTTP。

脚本侧仅提供 **触发** 与（部分版本）**状态查询**；任务的增删改请在后台完成。

::: tip 先配后台
在 [任务列表](../../cms/development/jobs.md#任务列表) 中 **添加任务** 并保存后，方可在其它 Code 中按 **名称** 调用 `k.site.runJob`。
:::

## k.site.runJob(name)

异步执行指定任务一次（与列表 **运行** 等效，日志说明为 `Manual trigger from kscript`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 任务 **名称** 或任务 Id（GUID 字符串） |

**返回：** `void`。

```typescript
k.site.runJob("nightly_sync")
```

任务不存在时抛出 `Job not found`。

::: warning 与调度器的关系
`runJob` **不** 替代周期调度；重复任务仍按后台 **开始时间** 与 **间隔** 自动执行。手动运行可在站点未发布时用于调试。
:::

## 执行环境

- 使用当前站点的服务端执行上下文。
- 代码来自 `SiteJob.Code` 字段（后台 Monaco 保存），以 **Module** 脚本类型、**长任务** 选项执行。
- 可使用与其它服务端 Code 相同的 `k.*` API（`k.DB`、`k.content`、`k.mail` 等）。

执行结果写入中心 **JobLog**（后台 **已完成** / **失败** 页签）。

## 与 k.site.codes 的区别

| | **SiteJob（定时任务）** | **k.site.codes** |
|--|---------------------------|------------------|
| 配置 | 开发 → 定时任务 | 开发 → 代码 |
| 触发 | 调度器 / `runJob` | HTTP 路由、事件、页面引用 |
| 代码存储 | `SiteJob.Code` 内联 | `Code` 资源 `body` |

若希望复用同一段逻辑，可将公共函数放在 Code 中，在任务脚本里通过约定调用（需自行 `import` 或内联，Kooboo 任务默认不自动绑定 Code Id）。

## 模块任务

模块包内 `module.config` 的 **task** 与事件由 `k.module` / 模块调度处理，**不是** `k.site.runJob`。见 [k.module](../module/index.md)。

## 相关

- [定时任务（CMS）](../../cms/development/jobs.md)
- [k.site](./index.md)
- [k.site.codes](./code.md)
