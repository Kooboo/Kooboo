# 模块（Modules）

> 菜单：**开发 → 模块**  
> 深链：`/_Admin/development/modules?SiteId={站点GUID}`

**模块**是 Kooboo 的可安装扩展单元：在独立目录中打包 **视图、API、脚本、静态资源** 与 `module.config`，通过站点路由对外提供能力，并可在 **开发模式** 中编辑。适合电商插件、表单组件、后台管理面板等可复用功能包。

::: tip 权限
列表与进入开发模式：`module`（查看）。**创建**、导入、上线开关、设置、导出、分享需 `module·edit`；多选 **删除** 需 `module·delete`。
:::

::: info 与站点 Code / View 的区别
| | **模块** | **开发 → 代码 / 组件** |
|--|----------|-------------------------|
| 结构 | 独立目录（`view/`、`api/`、`code/` 等）+ `module.config` | 单条站点资源入库 |
| 路由 | 自动注册 `/_模块名/{part}` | 页面 / Code 各自路由 |
| 配置 | `module.config` + 后台 **设置** 弹窗 | 无统一模块配置 |
| 后台入口 | `module.config` 的 **`menu`** → 站点左侧菜单（iframe） | 无 |
| 脚本 | `k.module.*`（须在模块上下文中） | `k.site.codes` 等 |

脚本 API 见 **[k.module](../../api/module/index.md)**。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 模块**。
2. 左侧 **开发 → 模块**。

<DocImage src="/cms/development/modules-overview.png" alt="模块列表总览" width="1120" />

## 模块列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **创建** | `module·edit` | [新建模块弹窗](#新建模块弹窗) |
| **导入** | `module·edit` | 上传模块 **zip** 包（见 [导入弹窗](#导入弹窗)） |
| **搜索** | `module·edit` | 打开 Kooboo **应用商店** 在线搜索模块包并安装 |

<DocImage src="/cms/development/modules-toolbar.png" alt="模块列表工具栏" width="1120" />

### 列表表格

| 列 / 操作 | 说明 |
|-----------|------|
| **名称** | 点击名称进入 **开发模式** 编辑该模块（`dev-mode?activity=modules&moduleId=...`） |
| **上线** | 开关控制模块是否对外生效（`ScriptModule/UpdateStatus`） |
| **后台** | 若模块含 `backend/` 视图，显示 **管理** 按钮（与下方 **`menu` 侧栏** 不同，见 [挂载站点后台菜单](#挂载站点后台菜单-menu)） |
| **设置** | [模块设置弹窗](#模块设置弹窗)（读取 `module.config` 的 `settingDefines`） |
| **说明** | 渲染根目录 `Readme.md`（Markdown） |
| **导出** | 下载当前模块 zip |
| **分享** | 将模块分享到 Kooboo 包仓库（需确认） |

支持多选 **删除** 模块。

<DocImage src="/cms/development/modules-list-table.png" alt="模块列表" width="1120" />

## 新建模块弹窗

填写 **名称**（1～50 字符，字母或数字开头）。创建后会在站点中注册模块记录与默认目录结构（含 `module.config`、`Readme.md` 等），并可在列表中点击名称进入开发模式继续添加文件。

<DocImage src="/cms/development/modules-add-dialog.png" alt="新建模块弹窗" width="1120" />

## 导入弹窗

| 字段 | 说明 |
|------|------|
| **名称** | 导入后的模块名；若与已有模块重名会提示 **替换** 确认 |
| **文件** | 模块 **zip** 包 |

导入成功后刷新列表；Monaco 模块缓存会清理以便加载新代码。

<DocImage src="/cms/development/modules-import-dialog.png" alt="导入模块弹窗" width="1120" />

## 模块设置弹窗

**设置** 读取模块根目录 **`module.config`**（JSON）。其中 `settingDefines` 描述后台可填项（`input`、`number`、`select`、`switch`、`textarea` 等），保存后写入站点侧模块 **Settings**（运行时通过 `k.module.config` 读取）。

同一文件中的 **`menu`** 字段用于把模块挂到站点后台左侧菜单（见下节），与 **设置** 弹窗无关，需在开发模式中直接编辑 `module.config`。

未保存过设置时，使用 `settingDefines` 中的 `defaultValue` 初始化表单。

<DocImage src="/cms/development/modules-setting-dialog.png" alt="模块设置弹窗" width="1120" />

## 说明（Readme）

**说明** 图标打开 `Readme.md` 的 Markdown 预览，供安装者了解模块用途与用法。

<DocImage src="/cms/development/modules-readme-dialog.png" alt="模块 Readme 说明" width="1120" />

## 在开发模式中编辑

点击列表 **名称**（或开发模式侧栏 **模块** 活动）进入编辑。左侧按资源类型分组，常见目录包括：

| 类型 | 目录 | 用途 |
|------|------|------|
| **root** | 根文件 | `module.config`、`Readme.md`、`event.js`、任务脚本等（根文件一般不可删） |
| **view** | `view/` | 模块对外 **页面**（SSR/模板） |
| **api** | `api/` | 模块 **API** 脚本（`ModuleApi:` 引用） |
| **code** | `code/` | 模块内 **可复用脚本**（`module:` 引用，如 `main` / `index`） |
| **css** / **js** | 静态资源 | 样式与脚本 |
| **img** / **file** | 二进制 | 图片与其它文件；支持上传 |
| **backend** | `backend/` | 模块 **管理后台** 界面（列表 **管理** 按钮入口） |

文本类文件在右侧 **标签页** 中编辑（Monaco）；可 **保存**、**刷新**、对 view 等 **预览**。各类型可通过分组标题旁 **+** 新建文件；二进制类型可 **上传**。

<DocImage src="/cms/development/modules-devmode.png" alt="开发模式：编辑模块文件" width="1120" />

开发模式顶栏 **更多** 可返回本列表页；**返回** 退出当前模块文件树。

## 挂载站点后台菜单（`menu`）

在 **`module.config`** 中增加 **`menu`** 对象后，模块可在 **站点后台左侧菜单** 中显示为独立入口（与 **开发 → 模块** 列表里的 **管理** 按钮不是同一机制）。登录站点后台时，Kooboo 会扫描所有 **已上线** 模块的 `module.config`；同时存在 **`name`** 与 **`menu`** 的模块会注入到侧栏。

点击该菜单项后，主内容区通过 **iframe** 加载模块页面（路由 `module-menu/:module`，查询参数 `url` 为解析后的模块 view 地址）。

### 生效条件

| 条件 | 说明 |
|------|------|
| 模块 **上线** | 列表中 **上线** 开关为开 |
| `module.config` 含 **`name`** | 包名，与模块资源名一致 |
| 配置 **`menu`** | 见下表 |
| 刷新后台 | 修改 `menu` 后重新进入站点后台或刷新页面，以重新拉取 `moduleMenus` |

### `menu` 字段说明

| 字段 | 必填 | 说明 |
|------|------|------|
| `name` | 是 | 侧栏显示名称 |
| `nameTranslation` | 否 | 多语言文案，键为语言代码（如 `zh`、`en`），对应当前后台用户语言 |
| `parent` | 否 | 挂到某一 **顶级后台菜单** 下；值为该菜单在路由里的 `menu.name`（如 `content`、`commerce`），见 [菜单总览](../navigation.md) |
| `url` | 否 | 模块内 **view** 相对路径（如 `dashboard.html`）；留空则使用模块默认起始 view |
| `icon` | 否 | 图标：一般为 `img/` 目录下的文件名，解析为 `/_模块名/img/{文件名}?SiteId=...` |
| `children` | 否 | 子菜单数组，每项含 `name`、`nameTranslation`、`url`（相对 view 路径） |

子菜单存在时，侧栏显示为可展开的父项，`children` 各占一行；子项 `url` 同样解析为 `/_模块名/{路径}?SiteId={站点GUID}`。

### 配置示例

```json
{
  "name": "my_shop_plugin",
  "version": "1.0.0",
  "description": "店铺扩展",
  "menu": {
    "name": "店铺插件",
    "nameTranslation": {
      "zh": "店铺插件",
      "en": "Shop Plugin"
    },
    "parent": "commerce",
    "url": "admin/dashboard.html",
    "icon": "plugin-icon.svg",
    "children": [
      {
        "name": "订单同步",
        "url": "admin/orders.html"
      },
      {
        "name": "库存",
        "url": "admin/stock.html"
      }
    ]
  },
  "settingDefines": []
}
```

- 上例在 **电商**（`parent: "commerce"`）分组下增加 **店铺插件**，默认打开 `view/admin/dashboard.html`。  
- `view/admin/*.html` 可在模块内使用 `k.module.config` 读取 **设置** 中保存的参数。  
- 若 `url` 以 `http://` 或 `https://` 开头，则不再拼接模块路径（用于外链后台）。

### 与列表「管理」按钮的区别

| | **`menu` 侧栏** | 列表 **管理** |
|--|-----------------|---------------|
| 配置位置 | `module.config` → `menu` | 模块 `backend/` 视图 + 后台 API 解析的 `backendViewUrl` |
| 入口 | 站点日常后台左侧菜单 | **开发 → 模块** 列表行内 |
| 典型用途 | 运营人员日常使用模块功能 | 开发期快速打开模块后台页 |

二者可同时存在：运营走侧栏，开发者在模块列表点 **管理** 调试 `backend/`。

## 路由与访问

创建模块时，Kooboo 为模块注册路由，默认形如：

```text
/_模块名/{part}
```

`{part}` 对应请求的 view、api、静态资源等路径（由模块渲染器解析）。若名称冲突，系统会自动尝试 `模块名2`、`模块名3`… 直至可用。

模块脚本中当前模块根 URL 为 **`k.module.baseUrl`**（仅模块执行上下文）。

## 典型工作流

1. **创建** 或 **导入** / **搜索** 安装模块。  
2. 在 **开发模式** 中编写 `view/`、`api/`、`code/`，并维护 `module.config`、`Readme.md`。  
3. 需要运营后台入口时，在 `module.config` 配置 **`menu`** 与对应 `view/` 页面。  
4. 在列表打开 **设置**，填写 `settingDefines` 运营参数（映射到 `k.module.config`）。  
5. 将 **上线** 设为开启；刷新站点后台查看侧栏菜单，并在前台 `/_模块名/...` 验证。  
6. 需要分发给其它站点时 **导出** zip 或 **分享** 到包仓库。

## 相关

| 文档 | 说明 |
|------|------|
| [k.module](../../api/module/index.md) | `config`、`task`、`localFile`、`module.config` 约定 |
| [站点后台菜单总览](../navigation.md) | `menu.parent` 可挂接的顶级菜单 `name` |
| [代码](./code.md) | 站点级 CodeBlock / API |
| [开发概述](./index.md) | 开发分组索引 |
| [开发 · 定时任务](./jobs.md) | 站点级 `k.site.runJob`（与模块 `k.module.task` 不同） |
