# 用户输入中的代码引用格式

用户在聊天中可能发送 `[type:fileName startLine-endLine]` 格式的文本，这表示用户引用了一段站点代码或模板。

## 格式定义

```
[type:fileName]
[type:fileName|fileId]
[type:fileName startLine-endLine]
[type:fileName|fileId startLine-endLine]
```

| 部分 | 说明 | 示例 |
|------|------|------|
| `type` | 资源类型，见下方对照表 | `page`、`api` |
| `fileName` | 对象名称 | `home`、`author-link` |
| `fileId` | 可选，对象唯一标识符，用于精确定位 | 跟在 `\|` 后 |
| `startLine-endLine` | 可选，起始行-结束行 | `3-8` |

## type → TextObjectType 对照

当需要调用后端 `read_object_text` 时，按以下映射转换：

| 输入 type | read_object_text objectType |
|-----------|---------------------------|
| `page` | `Page` |
| `view` | `View` |
| `layout` | `Layout` |
| `script` | `Script` |
| `style` | `Style` |
| `api` | `Api` |
| `codeblock` | `CodeBlock` |

## AI 处理规则

当用户输入或对话历史中包含 `[type:fileName range]` 格式时：

1. **优先使用 `read_editor_tool`**（前端工具）读取当前打开编辑器中的代码（包含未保存的修改）
2. 如果 `read_editor_tool` 返回 `status: "error"`，且 `code` 为 `editor_file_not_open` 或 `editor_model_unavailable`，说明无法从当前编辑器读取该文件。**不要在此处停止，立即回退使用 `read_object_text`** 从服务端读取当前代码
3. `read_object_text` 的 `nameOrId` 参数使用文件名，`objectType` 使用下方对照表中的对应值
4. 调用方式见下方示例
5. 如果用户给出了行号范围（如 `3-8`），服务端读取使用覆盖该范围及必要周边代码的窗口；上下文不足时扩大范围。编辑器的 `startLine/endLine` 不是后端参数：`read_object_text` 使用中心行 `lineNumber` 和半径 `range`。未保存的编辑器修改可能导致服务端位置偏移，必须核对目标原文。窗口边界和安全补丁规则见[对象读取与更新](Functions/read-update.md)。

### 关于 `read_editor_tool`

`read_editor_tool` 是前端自定义工具，用于读取当前页面上打开的 Monaco 编辑器中的代码。

- **name**: 文件名（从 `[type:name]` 中提取）
- **type**: 资源类型（从 `[type:name]` 中提取）
- **fileId**: 可选，文件唯一标识
- **startLine/endLine**: 可选行号范围

如果文件在编辑器中打开，返回最新代码（可能包含未保存的修改）；
如果文件未打开，返回 `status: "error"`、`code` 和 `message`，此时应改用 `read_object_text`。

### 关于 `write_editor_tool`

`write_editor_tool` 是前端自定义工具，用于将代码写入当前页面上打开的 Monaco 编辑器。

- **name**: 文件名
- **code**: 要写入的代码内容
- **startLine/endLine**: 可选，替换指定行范围（不传则替换整个文件）

**注意**：写入操作仅修改编辑器内容，不会自动保存到服务端。写入后应提示用户手动保存。

### 示例

用户输入：`请修改 [api:author-link 3-8] 中的逻辑`

AI 应执行：
1. 调用 `read_editor_tool({ name: "author-link", type: "api", startLine: 3, endLine: 8 })`
2. 如果 result 的 `status` 为 `error`，且 `code` 为 `editor_file_not_open` 或 `editor_model_unavailable`，**立即回退**调用 `read_object_text({ nameOrId: "author-link", objectType: "Api", lineNumber: 6, range: 10 })`（正文足够长时覆盖第 1–16 行，包含目标 3–8 行及周边代码）
3. 如果成功，获取代码内容
4. 分析代码并给出修改建议或使用 `write_editor_tool` 写入修改

用户输入：`[page:home] 的标题不对`

AI 应执行：
1. 调用 `read_editor_tool({ name: "home", type: "page" })`
2. 如果 result 的 `status` 为 `error`，且 `code` 为 `editor_file_not_open` 或 `editor_model_unavailable`，回退调用 `read_object_text({ nameOrId: "home", objectType: "Page" })`
3. 分析标题标签内容

## 使用场景

此格式由前端 Monaco 编辑器的"添加到对话"功能生成，当用户在编辑器中选中代码并点击"添加到对话"时自动插入。AI 收到此类引用后应：
1. 优先通过 `read_editor_tool` 读取打开编辑器中的最新内容（可能包含未保存修改）
2. 如果编辑器未打开，回退调用 `read_object_text` 从服务端读取
