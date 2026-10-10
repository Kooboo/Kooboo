---
name: sandbox
description: >-
  Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated
  changes, execute KData or KScript, render templates, and verify page output
  without changing the current site. / 使用隐藏的 Kooboo Sandbox 复制指定站点对象、
  隔离修改并验证 KData、KScript、模板和页面输出。
---

# Kooboo Sandbox

当用户明确要求在 Sandbox、测试站或隔离环境中实施和验证站点变更时使用本 skill。Sandbox 是当前组织中的独立 Kooboo `WebSite` 和 `SiteDb`，不是终端、容器或通用操作系统沙箱。

## 1. 当前实现边界

- `create_sandbox` 创建名称为 `__ai_sandbox__{siteName}` 的站点，状态为 `Hidden`，并且 `Published = false`。
- 当前创建流程不添加域名绑定，因此不要报告 Sandbox 域名或 `baseUrl`。运行验证使用 `sandbox_render_template`、`sandbox_request_url` 或其他支持 `sandbox` 参数的 Function。
- Sandbox 与正式站拥有独立的数据库。Sandbox 中的 Page、View、Layout、Style、Script、Content 和设置不会自动合并或同步回正式站。
- Sandbox 列表按当前组织查询，不按来源站点或用户隔离。同一组织中的 Sandbox 名称必须唯一。
- 当前没有自动过期或后台清理。只有用户明确同意删除时才调用 `delete_sandbox`。
- Sandbox 只隔离站点数据。KScript、KData、页面代码或 API 仍可能产生外部网络、邮件、付款或其他站外副作用。

## 2. 标准工作流

1. 调用 `list_sandbox`，确认当前组织已有的 Sandbox 和名称。
2. 创建前确定简短的 `siteName`，并根据任务选择最小但完整的 `contentTypes`。
3. 调用 `create_sandbox`。该调用需要审批。
4. 从返回值读取 `success`、`siteId` 和 `siteName`，然后再次调用 `list_sandbox`，确认相同 `siteId` 已存在。
5. 后续所有 Sandbox 专用执行调用必须显式传入相同的 `sandbox` 名称。
6. 若创建后还需要复制资源，调用 `copy_sandbox_objects`。该调用需要审批。
7. 修改后使用 Sandbox 内的读取 Function，并根据任务调用 `sandbox_eval_kdata`、`sandbox_eval_script`、`sandbox_render_template` 或 `sandbox_request_url` 验证。
8. 报告 Sandbox 名称、站点 ID、复制范围、验证结果和未验证限制。不要把 Sandbox 结果描述为正式站已上线。
9. 用户明确要求清理后调用 `delete_sandbox`，随后再次调用 `list_sandbox` 确认删除。

## 3. Native Function 合同

| Function | 输入 | 返回 | 审批 |
|---|---|---|---|
| `list_sandbox` | 空对象 | `[{ id, siteName, creationDate }]` | 否 |
| `create_sandbox` | `siteName` 必填；`contentTypes` 可选 | `{ success, siteId, siteName }` | 是 |
| `copy_sandbox_objects` | `siteName`、非空 `objectTypes` | `true` | 是 |
| `sandbox_eval_kdata` | `sandbox`、`source` 必填 | export 变量字典 | 否 |
| `sandbox_eval_script` | `sandbox`、`code` 必填 | KScript 返回值 | 否 |
| `sandbox_render_template` | `sandbox`、`templateHtml` 必填；`models` 可选 | 渲染后的字符串 | 否 |
| `sandbox_request_url` | `sandbox`、`relativeUrl` 必填；`httpMethod`、`queryString` 可选 | `{ statusCode, body, executionTimeMs, headers }` | 否 |
| `delete_sandbox` | `siteName` 必填 | 固定字符串 `delete success` | 是 |

`sandbox_eval_kdata`、`sandbox_eval_script`、`sandbox_render_template` 和 `sandbox_request_url` 使用必填 Sandbox 输入。调用前会切换 Context，并验证当前 Context 确实属于目标 Sandbox；名称为空或目标不存在时会报错，不会继续在正式站执行。

不要直接调用 `k.sandbox.enter()` 或 `k.sandbox.exit()`。Native Function 基类会在每次调用前进入 Sandbox，并在 `finally` 中恢复原站 Context。Context 切换只对当前 Function 调用有效，下一次调用仍需重新传入 `sandbox`。

## 4. 创建与复制

`create_sandbox.contentTypes` 和 `copy_sandbox_objects.objectTypes` 支持以下值，匹配不区分大小写：

`Layout`、`View`、`Style`、`Script`、`Image`、`File`、`Menu`、`HtmlBlock`、`Label`、`Form`、`ContentFolder`、`ContentType`、`ContentCategory`、`TextContent`、`Code`、`OpenApi`、`Page`。

复制规则：

- 只复制显式选择的对象类型；不会自动补齐依赖。
- 复制来源是调用时的当前站点 Context，目标由 `siteName` 指定；不要把目标 Sandbox 先作为当前 Context 再调用复制。
- 每个选中 repository 中的全部对象都会通过 `AddOrUpdate` 写入目标 Sandbox。
- 重复复制会更新同 ID 对象，但不会删除目标 Sandbox 中额外存在的对象。
- 当前没有显式复制 Route、Commerce 数据、站点设置、权限或未列出的 repository。
- 修改现有页面时，通常需要一起考虑 `Page`、`Layout`、`View`、`Style`、`Script`、`Image`、`File`、`Label`、`HtmlBlock` 及相关内容类型。
- `TextContent` 不会自动带上 `ContentType`、`ContentFolder` 或 `ContentCategory`；需要时必须显式选择。
- 传入不支持的类型会抛出错误。

`create_sandbox` 先创建站点，再执行复制。若复制参数无效或复制中途失败，已创建的 Sandbox 不会自动回滚。此时先调用 `list_sandbox` 确认实际状态，再经用户审批后决定是否调用 `delete_sandbox`。

创建示例：

```json
{
  "siteName": "preview",
  "contentTypes": ["Layout", "View", "Style", "Script", "Page"]
}
```

追加复制示例：

```json
{
  "siteName": "preview",
  "objectTypes": ["Image", "File", "HtmlBlock", "Label"]
}
```

## 5. 在 Sandbox 中调用其他 Function

许多 Site、Commerce、读取和写入 Function 的输入继承可选的 `SandboxInput`，因此 schema 中会出现可选 `sandbox` 字段：

```json
{
  "sandbox": "preview",
  "nameOrId": "home"
}
```

对这类通用 Function：

- 每次调用都必须重新传入 `sandbox`。
- 先通过 `function_discover` 或 `use_function` 检查真实 schema；没有 `sandbox` 字段时，不得假设调用会被隔离。
- 可选 Sandbox 的通用 Function 只负责尝试进入目标站点。当前 `KSandbox.Enter` 在名称不存在时不会抛错，因此错误名称可能使调用继续使用正式站 Context。
- 在通用写入前，先用 `list_sandbox` 确认名称，再用带相同 `sandbox` 的 `get_site_info` 核对站点 ID。无法确认时停止写入。
- Sandbox 不改变 Function 原有的审批要求。

对 KScript 验证优先使用 `sandbox_eval_script`，因为它要求并验证 Sandbox。只有需要现有通用脚本接口时才使用带 `sandbox` 的 `run_kscript` 或 `run_skill_script`。

## 6. KData 验证

`sandbox_eval_kdata` 的 `source` 必须包含完整的 `<k-data>...</k-data>` 元素：

```json
{
  "sandbox": "preview",
  "source": "<k-data><query as=\"products\" source=\"content\" resource=\"Product\" action=\"list\" export></query></k-data>"
}
```

只返回显式 export 的变量：

- 带 `export` 或 `export="true"` 的 `<query>`。
- 带 `export` 或 `export="true"` 的 `<let>`。
- 通过 `<export var="name"></export>` 导出的已有变量。

返回空字典时，先检查 export 声明、查询条件以及目标 Sandbox 中是否存在相关数据。不要自动改到正式站重试。

## 7. KScript 与模板验证

KScript：

```json
{
  "sandbox": "preview",
  "code": "return 1 + 1;"
}
```

`sandbox_eval_script` 的执行超时为三分钟。它的 Native Function 本身不要求审批，但脚本仍可能写入 Sandbox 或触发站外副作用；只有任务已授权该执行时才调用。

模板：

```json
{
  "sandbox": "preview",
  "templateHtml": "<h1>Sandbox preview</h1>"
}
```

`sandbox_render_template` 直接在 Sandbox Context 中渲染输入 HTML，不保存 Page、View 或 Layout。

## 8. URL 验证

`sandbox_request_url` 通过 Sandbox `SiteDb.Pages.GetByUrl` 查找并渲染页面：

```json
{
  "sandbox": "preview",
  "relativeUrl": "/products",
  "httpMethod": "GET",
  "queryString": {
    "category": "featured"
  }
}
```

- `httpMethod` 默认是 `GET`。
- `queryString` 可省略。
- 找不到页面时返回 `statusCode: 404` 和 `body: null`。
- 当前实现没有模拟请求 headers、body、session 或 user。
- `RequestUrlResult.Headers` 当前不会被填充，通常为 `null`。
- 如果复制的 Page 缺少可用 URL/Route，可能返回 404；这不等同于页面对象不存在，应同时读取 Page 和 Route 状态确认。

## 9. 删除与正式站处理

`delete_sandbox` 无论目标是否存在都返回固定字符串 `delete success`。删除后必须重新调用 `list_sandbox`，不要仅凭返回字符串判断成功。

Sandbox 当前没有自动发布、合并或回写机制。用户确认 Sandbox 结果后，如需应用到正式站：

1. 明确列出要应用的对象和数据变更。
2. 重新获得正式站写入授权，不沿用 Sandbox 创建、复制或修改时的审批。
3. 不传 `sandbox`，在正式站重新实施并验证。
4. 对付款、邮件、账号、订单、删除或其他站外副作用重新执行相应审批。

## 10. 失败处理

- Sandbox 专用执行 Function 报目标不存在时，调用 `list_sandbox` 重新确认 `siteName`，不要去掉 `sandbox` 后重试。
- 通用 Function 使用可选 `sandbox` 时，若不能确认 Context 已切换，立即停止后续写入。
- 创建后复制失败时，Sandbox 可能已经存在；先列出并检查，再决定保留、补充复制或审批删除。
- `copy_sandbox_objects` 返回错误时，检查目标名称、`objectTypes` 是否为空以及是否包含不支持的类型。
- `sandbox_request_url` 返回 404 时，检查 Page URL、Route 和复制范围，不要直接判定渲染引擎失败。
- 运行时验证无法完成时，可以报告静态读回结果，但必须明确标记为未完成运行时验证。
