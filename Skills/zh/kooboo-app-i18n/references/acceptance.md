# 验收与排障

读回受影响对象。安装脚本成功只证明独立资产已经复制，不能证明应用完成翻译。按本次语言和页面范围检查：

- 读回 ENV.i18n 和 Services.i18n 的导入；即使资产安装成功，也已核对原样保留的 ENV 配置。其他 ENV 字段保持不变，不输出给浏览器。站点默认语言和应用支持语言一致；配置前缀只包含本应用公开文案；接口严格读取 Label，不误用地区/任意语言回退，读取不新增词条。
- import map 只用一份 Vue，挂载前注册 vue-i18n，匿名启动能加载界面文案；首次失败有重试入口。
- 引用的 key 均有默认语言，已有翻译未被迁移覆盖。命名参数、字面量 @/竖线/花括号、使用到的复数正确。
- 保存非默认语言后刷新，只发一次翻译请求；切回该响应已包含的默认语言不再请求。
- 无刷新切换且不改变路由/认证；菜单、标题、placeholder、校验、组件库文字、document.lang、日期数字更新；业务内容和时区保留各自契约。
- 切换失败保留旧语言且可重试，快速多次切换不会被旧请求覆盖。存储不可用不影响运行。切换后的 URL lang 和偏好一致。
- 刷新后能看到 Label 更新和删除；请求/缓存/偏好不会混站点或前缀；非法显式 locale 返回 400。
- 重复安装跳过同内容，保留不同内容和冲突路由；部分失败如实报告，不声称事务回滚。
- 定制 main/layout 和设计系统适配保留。遵守 kooboo-coding 的浏览器显式启用规则，没有浏览器验证就如实注明视觉和交互未验证。

| 症状 | 检查位置 |
| --- | --- |
| 显示 key | 默认语言的确切值、配置前缀、import map、t 参数 |
| 回退到意外语言 | 接口是否使用 Services.i18n 严格读取，而非 k.label/get_label 或 /_spa/lang |
| 菜单/标题不变 | 翻译是否在 computed/watch 内；标题辅助是否在旧 router hook 后注册 |
| 控件仍为中文 | Element locale 映射、外壳 label props、所用 Volcengine 组件内部文案 |
| 刷新丢失选择 | hash 前 URL lang、站点隔离的偏好、外部语言域名/路径策略 |
| Label 修改不出现 | 刷新页面会话缓存，检查响应头和外部缓存 |
| 安装冲突 | 读取所报对象和路由，不盲目重跑 create |

仓库开发测试（不是托管站点内命令）：

```sh
node --test Kooboo.Sites/AI/Skills/packages/kooboo-app-i18n/scripts/runtime.spec.mjs
dotnet test tests/Kooboo.SitesTests/Kooboo.SitesTests.csproj --no-restore --filter FullyQualifiedName~AppI18nTest -m:1 -nodeReuse:false
```

Node 使用随包 Vue/vue-i18n 验证服务行为；C# 在实际 KScript 引擎中运行安装和路由 API。两者不替代交付 App 的浏览器交互验证。
