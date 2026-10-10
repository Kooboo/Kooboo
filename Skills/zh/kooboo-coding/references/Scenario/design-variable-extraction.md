# 设计变量格式与提取

kooboo-var.css 的格式规范与提取替换流程。格式节为 DesignPanel 和 AI 建站的共用基准，提取节为已有站点分析的操作步骤。

---

# 格式规范

`kooboo-var.css` 同时作为浏览器可用的 CSS 变量文件和 DesignPanel 的配置来源。

```css
:root {
  /* @group colors */
  --color-primary: #1C1917;        /* 主色 */
  --color-cta: #CA8A04;            /* 强调色 */

  /* @group fonts */
  --font-heading: 'Cormorant', Georgia, serif;        /* 标题字体 */
  --font-body: 'Montserrat', 'Helvetica Neue', sans-serif; /* 正文字体 */
}
```

## 格式规则

**生成时必须逐条核对：**

| # | 要求 | 必须 | 说明 |
|---|------|------|------|
| ① | `:root { ... }` 包裹 | 是 | 所有变量定义在 `:root` 内 |
| ② | `/* @group xxx */` 分组标记 | 是 | 每个分组前一行必须写；group key 固定为 `colors` / `fonts` / `borderRadius` / `spacing` / `shadows` |
| ③ | 每行末尾 `/* 变量名 */` 注释 | **是** | 每行声明末尾必须有 `/* 中文或英文变量名 */`，DesignPanel 据此渲染显示名 |
| ④ | 变量以 `--` 开头 | 是 | 如 `--color-primary`、`--font-heading` |
| ⑤ | 尽量使用 DesignPanel 字典中的变量名 | **是** | 见下方「变量命名」节，优先使用字典中的 key，超出部分才自定义 |

**③ 是 AI 最容易遗漏的项**，每行 CSS 变量声明末尾都必须有注释，示例：

```css
--color-primary: #1C1917;        /* 主色 */       ← 必须有
--color-cta: #CA8A04;              /* 强调色 */      ← 必须有
```

- 分组 key 固定：`colors` / `fonts` / `borderRadius` / `spacing` / `shadows`
- 变量名根据站点语言命名：中文站点用中文名，英文站点用英文名，混合站点以主要内容语言为准

## 变量命名

DesignPanel 的变量名字典如下，生成 `kooboo-var.css` 时应优先使用字典中的变量名，保证 DesignPanel 能正确显示中英文名。只有字典未覆盖时才自定义变量名（字典外的变量会降级显示注释名或 key）。

| 变量 key | 中文显示 | 英文显示 |
|----------|----------|----------|
| `--color-primary` | 主色 | Primary |
| `--color-primary-light` | 主色-浅 | Primary Light |
| `--color-primary-dark` | 主色-深 | Primary Dark |
| `--color-accent` | 强调色 | Accent |
| `--color-accent-light` | 强调色亮 | Accent Light |
| `--color-secondary` | 辅色 | Secondary |
| `--color-cta` | CTA 色 | CTA |
| `--color-cta-light` | CTA 色浅 | CTA Light |
| `--color-cta-dark` | CTA 色深 | CTA Dark |
| `--color-bg` | 背景色 | Background |
| `--color-text` | 文字色 | Text |
| `--color-text-secondary` | 次要文字色 | Secondary Text |
| `--color-text-muted` | 浅文字色 | Muted Text |
| `--color-border` | 边框色 | Border |
| `--color-border-light` | 边框浅色 | Border Light |
| `--color-surface` | 表面色 | Surface |
| `--color-warning` | 警告色 | Warning |
| `--color-error` | 错误色 | Error |
| `--color-success` | 成功色 | Success |
| `--font-heading` | 标题字体 | Heading Font |
| `--font-body` | 正文字体 | Body Font |
| `--font-size-sm` | 小字号 | Small |
| `--font-size-base` | 字号 | Base |
| `--font-weight-heading` | 标题字重 | Heading Weight |
| `--radius-sm` | 小圆角 | Small Radius |
| `--radius-md` | 中圆角 | Medium Radius |
| `--radius-lg` | 大圆角 | Large Radius |
| `--spacing-4` | 间距 4 | Spacing 4 |
| `--spacing-6` | 间距 6 | Spacing 6 |
| `--spacing-sm` | 小间距 | Small Spacing |
| `--spacing-base` | 基础间距 | Base Spacing |
| `--spacing-lg` | 大间距 | Large Spacing |
| `--shadow-sm` | 小阴影 | Small Shadow |
| `--shadow-base` | 基础阴影 | Base Shadow |
| `--shadow-lg` | 大阴影 | Large Shadow |
| `--shadow-xl` | 特大阴影 | XL Shadow |

**重要：字典是「如何命名」的参考，不是「必须生成所有」的清单。** 只生成站点实际用到的变量，未使用的变量不写进 `kooboo-var.css`。例如站点只用了一个粉色调色板（primary / accent / bg / text / border），就不要额外生成 `--color-success`、`--color-warning` 等未使用的状态色。

## 变量选取原则

**放入 kooboo-var.css 的应该是品牌标识级设计决策**，即用户改了这个值会看到「另一种品牌表达」。

| 分类 | 提取范围 | 说明 |
|------|----------|------|
| **颜色** | `--color-primary` 系列、`--color-accent` 系列、`--color-secondary`、`--color-cta`、`--color-bg`、`--color-text` 系列、`--color-border` 系列、`--color-surface`、`--color-*` 状态色 | 用户通过 DesignPanel 可调的核心色，字典外的颜色用注释兜底 |
| **字体** | `--font-heading`、`--font-body`、`--font-size-base`、`--font-weight-heading` | 品牌字体改动影响全站 |
| **间距** | `--spacing-sm` / `--spacing-base` / `--spacing-lg` | 品牌间距，非 Tailwind 内置尺度的情况下提取 |
| **圆角** | `--radius-sm` / `--radius-md` / `--radius-lg` | 品牌圆角风格 |
| **阴影** | `--shadow-sm` / `--shadow-base` / `--shadow-lg` / `--shadow-xl` | 品牌阴影风格 |

**判断标准**：这个变量改了之后，用户看到的是「另一种品牌表达」，还是「页面布局碎了」？前者放，后者不放。

## Tailwind 优先

### 有 Tailwind config 时以 config 为准

如果站点通过 `window.tailwind.config` 或 `<script>` 定义了 Tailwind 主题，**必须** 从 config 中提取变量，而不是从页面 HTML 中猜测。

**变量来源优先级：**

| 优先级 | 来源 | 提取方式 |
|--------|------|----------|
| 1 | Tailwind config 中的 `theme.extend.colors` | 每个颜色 key → `--color-{key}` |
| 2 | Tailwind config 中的 `theme.extend.fontFamily` | 每个字体 key → `--font-{key}` |
| 3 | Tailwind config 中的 `theme.extend.borderRadius` | 每个圆角 key → `--radius-{key}` |
| 4 | Tailwind config 中的 `theme.extend.spacing` | 每个间距 key → `--spacing-{key}` |
| 5 | Tailwind config 中的 `theme.extend.boxShadow` | 每个阴影 key → `--shadow-{key}` |
| 6 | 页面 HTML 中的硬编码值 | 仅在 config 中未定义时提取 |

**提取后 Tailwind config 必须改为引用 var()：**

```html
<!-- ✅ 正确：引用 CSS 变量 -->
<script>
window.tailwind.config = {
  theme: {
    extend: {
      colors: {
        primary: 'var(--color-primary)',
        cta: 'var(--color-cta)',
      },
      fontFamily: {
        heading: 'var(--font-heading)',
        body: 'var(--font-body)',
      },
    },
  },
};
</script>
```

**映射规则**：`--color-*` → `theme.extend.colors`，`--font-*` → `theme.extend.fontFamily`。

### 无 Tailwind config 时从页面代码提取

从 Style 对象 、`<style>` 标签或页面硬编码中提取品牌级变量。提取后应创建 `tailwind.config` 引用 `var(--xxx)`，并在页面 class 中使用 Tailwind 主题类替代任意值。

---

# 提取与替换流程

对已有站点分析并提取设计变量，替换站点代码中的硬编码值。

## 何时加载

- 独立任务：用户要求提取/更新设计变量、替换硬编码
- 日常任务分流表中「设计变量提取/更新」命中时
- 建站流程中由 SOP 步骤 ② 自动执行（格式节为必读）

## 前置准备

1. 已读取本文档的「格式规范」节
2. 通过 `function_discover` 发现 Style、Script、Page、View、Layout 相关函数
3. 调用 `use_function` 获取：`list_site_object` / `read_object_text` / `update_object_text` / `create_style`

## 执行步骤

### 1. 检测是否为重复分析

查询是否已存在 `/kooboo-var.css`（`list_site_object` 过滤 Style，名称匹配）。无论是否存在都执行完整分析流程。已有文件可作参考，但最终以站点当前实际值为准。

### 2. 提取变量来源

按优先级查找：
1. `window.tailwind.config` 中的 `theme.extend`（有 Tailwind 时以此为准）
2. Style 对象中的 CSS 自定义属性（`:root { --xxx }` 块）
3. `<style>` 标签中的设计变量定义
4. 页面中的硬编码品牌值

### 3. 提取变量

- **有 Tailwind config**：从 config 的 colors / fontFamily / borderRadius / spacing / boxShadow 中提取，转换为 `--xxx` 命名。优先使用「变量命名」字典中的 key。
- **无 Tailwind config**：从页面提取品牌色（primary / accent / cta / text / bg / border 等）和品牌字体（heading / body），优先使用字典 key。
- **硬编码值**：仅提取多处出现的品牌级颜色/字体值，字典外的变量名用注释兜底。

### 4. 写入 kooboo-var.css

按上文「格式规则」生成内容，调用 `create_style`：

| 参数 | 值 |
|------|-----|
| `name` | `"kooboo-var.css"` |
| `url` | `"/kooboo-var.css"` |
| `content` | 按格式规范生成的 CSS |

### 5. 替换硬编码值

对 Page、View、Layout、Style、Script 中已定义变量对应的硬编码值进行替换。**仅替换** `kooboo-var.css` 中已定义的值，不匹配的不动。

| 上下文 | 替换方式 | 示例 |
|--------|----------|------|
| class 中的 Tailwind 任意值 | Tailwind 主题类 | `text-[#0C4A6E]` → `text-primary` |
| style 属性或 CSS 中的值 | `var(--xxx)` | `color: #0C4A6E` → `color: var(--color-primary)` |

- 先替换值较长的，再替换较短的（避免短值误匹配长值前缀）
- 已替换过的跳过
- 同一值多处出现时确保全部替换

### 6. 完成

调用 `ai_task_finish`，Message：`写入 /kooboo-var.css（X 个变量），替换 Y 个文件 Z 处硬编码`。

## 相关文档

- [建站流程入口](site-building/index.md)
- [前端对象创建](../Functions/frontend-create.md)
