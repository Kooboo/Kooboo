---
name: kooboo-app-i18n
description: "使用 Kooboo Label 配合 vue-i18n 为 Vue AI App 添加或维护多语言，包括语言切换、菜单与标题翻译、组件库语言。用户要求 App 国际化，或修改已接入该能力的 App 时使用；单语言 App 无需安装。"
---

# Kooboo App 多语言

站点写入前先加载 `kooboo-coding`，遵守已有规划、授权和验证流程。本技能提供可选能力，加载正文不会安装代码。站点开启多语言也不代表其 App 已使用 vue-i18n。

1. 读取当前站点语言设置和受影响的 index/main/router/layout/service。通过实际 main 注册与 service 判断接入状态，不只依赖记忆。安装、迁移时读取[接入指南](references/integration.md)。
2. Kooboo Label 是应用文案的持久化来源。key 使用稳定的应用前缀，总长不超过 64 个 ASCII 字母、数字、点、下划线或连字符。调用 `set_label` 前读取旧值；除非要求修改，保留已有翻译。浏览器读取不得新增 Label。
3. App i18n 配置统一放在共享服务端 `ENV.i18n`；按接入指南保留 ENV 已有字段，并核对安装回执中的配置项。执行 `scripts/install-runtime.js` 安装独立资产，不把第三方库源码读入上下文再重新输出。安装成功只代表资产已存在；仍需读取并局部接入 import map/main/layout/router。单语言默认行为不变。
4. 界面统一使用 Composition API 的 `useI18n().t`。菜单、规则和标题随语言变化。Label 模板使用 vue-i18n 语法，不先调用 k.label 插值，也不作为 HTML 渲染。历史通知、业务内容是另外的数据。
5. 验收或排障时读取[验收指南](references/acceptance.md)。保留现有路由和认证逻辑，遵守 `kooboo-coding` 的浏览器验证显式启用规则，如实说明未验证的交互。

后端仅输出配置前缀的公开界面文案。前端加载目标语言及默认语言，在当前页面会话内缓存，资源就绪后才提交切换。不要复用另一套存储的 `/_spa/lang`、Label 隐式回退或第二套 JSON 翻译库。

成功接入后，在站点记忆记录实际接入、语言和 key 约定；后续修改受影响 App 时继续加载本技能。可使用现有规划描述和验收项记录语言范围，不添加未声明的 App Plan 字段。
