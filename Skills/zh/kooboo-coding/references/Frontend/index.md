# Frontend References 前端参考

按当前对象和运行方式选择 reference，不要把服务端 Layout/View、客户端渲染 Vue 和普通局部脚本的规则混用。

## 技术路线

| 任务 | 必读文档 |
|---|---|
| 客户端渲染 Vue、Vue 3 CDN、无构建 Vue、import map Vue 应用 | [客户端渲染 Vue 开发规范](cdn-vue/index.md) |
| 站点与 App 的页面交互、表单、编辑、反馈及导航 | [前端交互指南](interaction-guide.md) |
| 通用 HTML、CSS、Script 风格 | [前端代码风格](code-style.md) |
| Page | [Page](page.md) |
| Layout | [Layout](layout.md) |
| View | [View](view.md) |
| 模板绑定 | [Template Binding Syntax](template-binding-syntax.md) |
| `k-data` 查询 | [k-data 基础](k-data/basic.md) |
| `k-label` 多语言标签 | [k-label](k-label.md) |
| 自定义登录界面 | [Custom Login UI](custom-login-ui.md) |
| 动效 | [Motion](motion.md) |

## 客户端渲染 Vue 识别规则

出现以下任一意图时，直接进入 [客户端渲染 Vue 开发规范](cdn-vue/index.md)：

- 客户端渲染 Vue、Vue CDN 或无构建 Vue；
- “Vue 3 + import map”；
- “不用 Vite/Webpack/npm 构建”；
- “用 Kooboo Page、Style、Script 直接运行 Vue”；
- 创建或修改现有客户端渲染 Vue Page、store、service、component。

该 reference 是通用客户端渲染技术规范，不要求 Blueprint，也不只用于原型。若同时涉及 API、Database、Permission 或认证，再并行读取对应 Backend、KScript 或场景 reference。
