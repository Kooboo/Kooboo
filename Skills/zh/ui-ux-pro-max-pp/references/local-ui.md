# Local UI

局部替换 section 使用 `prompt-recommendations`：

```json
{
  "mode":"prompt-recommendations",
  "query":"editorial portfolio modern",
  "sectionType":"hero",
  "pageName":"home",
  "limit":5,
  "excludePromptIds":[]
}
```

建站上下文可用 `brief` 替代 `query`。普通资料检索使用 `search` + `query` + 可选 `domain` / `limit`；技术栈规则使用 `stack` + `query` + `stack`。`limit` 范围为 1–20。

局部 UI 结果仍须服从当前站点 Design System、真实内容、可访问性、响应式和交互能力；不编造数据、外链图片或提交行为。
