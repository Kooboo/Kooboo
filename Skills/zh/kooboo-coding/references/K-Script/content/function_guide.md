# Content 内置函数指南

> 指导 AI 通过 **Kooboo 内置函数**（`BuiltIn/Core/Content`）创建内容类型、内容夹、以及用 JSON 读写内容。**运行时**在 KScript 里对内容做 CRUD 请读同目录 [k.content 模块参考](index.md)。

## 何时阅读本文

- [建站 SOP](../../Scenario/site-building/index.md) 数据源映射含 `content`，需先建模再落数据。
- 需要调用 `create_content_type`、`get_content_schema`、`create_content` 等函数，且不确定参数或顺序时。

## 与 `index.md`（`k.content`）的分工


| 场景                                                                 | 使用                 |
| -------------------------------------------------------------------- | -------------------- |
| 定义/调整 ContentType、ContentFolder；用工具函数按 schema 写入或列举 | 本文 + 对应内置函数  |
| 在 API / CodeBlock / SSR KScript 中访问 `k.content.文件夹名`         | [index.md](index.md) |


## 推荐调用顺序

1. **`create_content_type`** — 创建数据类型。
2. 显式调用 **`create_content_folder`**，通常使用与类型相同的名称，并传入已有类型的 `ContentTypeNameOrId`。当前实现只持久化 ContentType，不会创建文件夹。
3. 写入或校验字段前：**`get_content_schema`**，得到字段名、控件类型、只读信息。
4. **`create_content`** / **`update_content`** — `DataJson` 的键名与取值须符合 schema（见下文契约）。
5. 只读列举或拉全字段：**`list_content`** / **`get_content`**。
6. 演进模型：**`update_content_type`**（追加或删除属性）。

## 审批（Approval）

以下函数 `ApprovalRequired == true`，执行前通常需用户确认：

- `create_content_type`
- `create_content_folder`
- `create_content`
- `update_content`
- `update_content_type`

以下函数 `ApprovalRequired == false`：

- `get_content_schema`
- `get_content`
- `list_content`

## `create_content_type`

创建 ContentType。当前实现**不会**创建 ContentFolder；创建内容前必须显式创建文件夹。


| 参数       | 类型              | 必填 | 说明                 |
| ---------- | ----------------- | ---- | -------------------- |
| Name       | string            | 是   | 类型名称（英文标识） |
| Properties | ContentProperty[] | 是   | 字段定义数组         |


## `create_content_folder`

在已有 ContentType 上创建文件夹。若目标名称已存在，函数直接返回且不修改现有文件夹。


| 参数                | 类型   | 必填 | 说明                                |
| ------------------- | ------ | ---- | ----------------------------------- |
| Name                | string | 是   | 文件夹标识（后续 `k.content.Name`） |
| DisplayName         | string | 否   | 后台显示名                          |
| ContentTypeNameOrId | string | 是   | 已存在类型的名称或 Id               |


## `get_content_schema`

获取某内容夹对应的字段 schema（用于表单或拼 `DataJson`）。


| 参数           | 类型   | 必填 | 说明            |
| -------------- | ------ | ---- | --------------- |
| FolderNameOrId | string | 是   | 内容夹名称或 Id |


**输出要点：** `Name`（夹名）、`Properties` 列表，每项含 `Name`、`ControlType`、`ReadOnly`（系统字段或不可编辑时为 true）。

## `create_content`

在指定文件夹中 **新建** 一条内容。


| 参数           | 类型   | 必填 | 说明                                                                                                 |
| -------------- | ------ | ---- | ---------------------------------------------------------------------------------------------------- |
| FolderNameOrId | string | 是   | 内容夹名称或 Id                                                                                      |
| DataJson       | string | 是   | 静态 JSON 字符串，键为 **用户字段名**，须符合 `get_content_schema`；例如 `{"title":"x","views":"0"}` |


实现会对每个用户字段执行默认语言下的赋值（具体以站点默认 culture 为准）。

成功返回字符串 `Save success`。

## `update_content`

按 Id 更新已有内容；`DataJson` 可只包含要改的字段。


| 参数           | 类型   | 必填 | 说明                                                                                                     |
| -------------- | ------ | ---- | -------------------------------------------------------------------------------------------------------- |
| ContentId      | string | 是   | 内容 Id                                                                                                  |
| FolderNameOrId | string | 是   | 内容夹名称或 Id                                                                                          |
| Culture        | string | 否   | 指定语言时，仅写入该 culture；为空时行为与实现一致（会写默认 culture 路径，详见源码 `UpdateContent.cs`） |
| DataJson       | string | 是   | 要修改的字段 JSON，须符合 schema                                                                         |


## `get_content`

读取 **单条** 内容的所有类型字段当前值。


| 参数      | 类型   | 必填 | 说明                                                        |
| --------- | ------ | ---- | ----------------------------------------------------------- |
| ContentId | string | 是   | 内容 Id 或可按名称解析的标识（与实现 `GetByNameOrId` 一致） |
| Culture   | string | 否   | 指定语言取值                                                |


## `list_content`

列举某夹下所有内容；可限制返回字段。


| 参数           | 类型     | 必填 | 说明                                                  |
| -------------- | -------- | ---- | ----------------------------------------------------- |
| FolderNameOrId | string   | 是   | 内容夹名称或 Id                                       |
| Culture        | string   | 否   | 各字段取值语言                                        |
| Skip           | int      | 否   | 非负偏移；负数按 `0` 处理                            |
| Take           | int      | 否   | 返回数量上限；未传返回剩余全部，`0` 不返回条目，负数按 `0` 处理 |
| Fields         | string[] | 否   | 要返回的用户字段名；**若为空或未传，仅返回每条 `id`** |
| Keyword        | string   | 否   | 不区分大小写的包含搜索；有 `Fields` 时只搜指定字段，否则搜索全部文本字段 |

返回值包含 `List`、`Skip`、`Take`、`Total`。未传 `Take` 时，`Total` 会统计全部匹配项；传入 `Take` 后当前实现可能提前停止扫描，因此 `Total` 不能可靠表示分页前的全部匹配数。


## `update_content_type`

按类型名追加或删除属性。


| 参数             | 类型     | 必填 | 说明                                                                        |
| ---------------- | -------- | ---- | --------------------------------------------------------------------------- |
| Name             | string   | 是   | ContentType 名称                                                            |
| AppendProperties | object[] | 否   | 每项含 `Name`、`DisplayName`、`ControlType`（字符串，与下表枚举名一致） |
| DeleteProperties | string[] | 否   | 要删除的属性 **Name** 列表                                                  |

当前实现追加属性时会忽略 `SelectionOptions`。


## ContentProperty（`create_content_type` 用）


| 字段             | 类型          | 必填 | 说明                                |
| ---------------- | ------------- | ---- | ----------------------------------- |
| Name             | string        | 是   | 字段标识                            |
| DisplayName      | string        | 否   | 显示名                              |
| ControlType      | 枚举          | 是   | 见下表                              |
| MultipleLanguage | bool          | 否   | 默认 false                          |
| IsSummary        | bool          | 否   | 列表摘要字段                        |
| SelectionOptions | {Key,Value}[] | 否   | `ControlType` 为 `Selection` 时使用 |


### ControlType 枚举值

`TextBox`、`TextArea`、`RichEditor`、`Selection`、`CheckBox`、`RadioBox`、`Switch`、`MediaFile`、`File`、`DateTime`、`Number`、`ColorPicker`、`KeyValues`、`ValueList`

**Selection：** `SelectionOptions` 属于输入结构，但当前 `create_content_type` 和 `update_content_type` 实现不会持久化它。

## 常见错误

1. **未调用 `get_content_schema`** 就凭猜测拼 `DataJson`，导致缺字段或控件类型不匹配。
2. 未向 **`list_content`** 传 `Fields` 却期望拿到全文——实现只返回 `id`；需要有界或筛选读取时使用 `Skip`、`Take`、`Keyword`。
3. **`update_content_type`** 的 `AppendProperties.ControlType` 须为 **字符串枚举名**（与 `create_content_type` 用的枚举名一致），与 `create_content_type` 的强类型 `ContentProperty` 区分。

## 实现溯源

逻辑以源码为准：`Kooboo.Sites/AI/Functions/BuiltIn/Core/Content/*.cs`。
