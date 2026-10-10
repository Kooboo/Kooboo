# Motion and Static Interaction Guidelines

Kooboo 站点动效与静态 JS 交互实现参考。用于创建或修改 Layout、Page、View、Style、Script 时，将 `ui-ux-pro-max-pp` 返回的 motion / animation / reveal / hover / tabs / accordion / carousel / menu 等设计意图落成可维护、可访问、不会阻塞内容的前端代码。

## 适用范围

- 静态站点首屏、View section、产品卡、作品卡、FAQ、评价、价格卡、表单状态等轻交互动效
- 不依赖后端数据/API 的展示交互：移动导航、dropdown、accordion、tabs、gallery/carousel、静态筛选 chips、展开收起、hover/focus/active 状态
- `ui-ux-pro-max-pp` 的 `ux-guidelines.csv` / `anti-slop.csv` 检索到 motion 相关规则时
- Page Plan block 的 `preflightTags`、`variantBrief`、`visualTreatment` 或 `Layout Intent` 提到 reveal、stagger、hover、motion、animation 时

不适用：

- 依赖后端数据、账号、订单、支付、表单提交、持久化或业务 API 的交互；这些属于 S6-S9 动态化。
- 复杂运行时状态动画、拖拽、实时可视化、视频播放器、3D 交互。这些属于专项前端任务，应单独设计。

## 基本原则

1. **动效服务叙事**：动效必须强化进入顺序、视觉层级、状态反馈、内容比较或品牌语气；不要为了“有动画”而动画。
2. **Kooboo HTML 优先 CSS-first**：静态建站阶段优先使用 CSS keyframes、transition、`animation-delay`、CSS custom properties。需要状态切换时使用轻量原生 JS，不要引入 Framer Motion、GSAP 或其它前端框架依赖。
3. **内容默认可见**：不得让核心内容永久依赖 JS 才显示。不要把所有 section 初始写成 `opacity: 0` 再等待脚本解锁。
4. **性能优先 transform / opacity**：动画属性优先使用 `transform`、`opacity`、`filter` 的轻量用法；禁止动画 `width`、`height`、`top`、`left`、`margin` 等会触发布局抖动的属性。
5. **可访问性必备**：必须提供 `prefers-reduced-motion` fallback；减少动效时仍保留最终布局、内容顺序和视觉层级。
6. **一页一种 motion language**：同一页面复用 1 个入场 reveal、1 个 hover/active 反馈即可。避免每个组件都使用不同动画。
7. **静态 JS 属于 S4**：移动菜单、FAQ、tabs、gallery、静态筛选等不依赖 API 的交互必须在首轮静态实现中完成。静态 JS 禁止 `fetch`、`k.*`、真实提交、持久化或业务 API。

## 推荐模式

### 1. 编排式首屏加载

适合 Hero、首个 proof strip、首屏 CTA、首批产品卡。一个精心编排的页面加载比到处零散微交互更有效。

```html
<section class="kb-motion-scope">
  <div class="kb-reveal" style="--kb-delay: 0ms">...</div>
  <div class="kb-reveal" style="--kb-delay: 90ms">...</div>
  <div class="kb-reveal" style="--kb-delay: 180ms">...</div>
</section>
```

```css
.kb-motion-scope .kb-reveal {
  animation: kb-reveal-up 560ms cubic-bezier(.16, 1, .3, 1) both;
  animation-delay: var(--kb-delay, 0ms);
}

@keyframes kb-reveal-up {
  from {
    opacity: 0;
    transform: translateY(18px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (prefers-reduced-motion: reduce) {
  .kb-motion-scope .kb-reveal {
    animation: none;
  }
}
```

约束：

- 单组 stagger 总时长建议控制在 700ms 内。
- 延迟建议 40-90ms 递增，不要出现 1s 以上等待。
- 骨架 Page 阶段不要写复杂动效；真实 View 完成后再加入。

### 2. 叙事型 Hover

适合作品卡、商品卡、服务卡、项目卡。Hover 应展示有用信息或强化焦点，不应只是晃动。

```html
<article class="kb-project-card group">
  <img src="/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=interior%20project&orientation=landscape&size=regular" alt="Interior project" class="kb-project-image">
  <div class="kb-project-meta">
    <p>Residential / 2026</p>
    <h3>Courtyard renovation</h3>
  </div>
</article>
```

```css
.kb-project-card {
  overflow: hidden;
}

.kb-project-image,
.kb-project-meta {
  transition:
    transform 220ms cubic-bezier(.16, 1, .3, 1),
    opacity 220ms cubic-bezier(.16, 1, .3, 1);
}

.kb-project-card:hover .kb-project-image {
  transform: scale(1.035);
}

.kb-project-meta {
  opacity: .88;
}

.kb-project-card:hover .kb-project-meta {
  opacity: 1;
  transform: translateY(-2px);
}

@media (hover: none) {
  .kb-project-card:hover .kb-project-image,
  .kb-project-card:hover .kb-project-meta {
    transform: none;
  }
}
```

约束：

- 价格、CTA、关键说明不能只在 hover 后出现；移动端没有 hover。
- 网格卡片不要使用会挤压邻居的 `hover:scale-105`。如果要缩放，只缩放内部图片或 overlay。
- 交互元素保留 `focus-visible` 样式，不能只做 hover。

### 3. Scroll Reveal 渐进增强

适合长页面中后段，但静态建站阶段默认不用复杂 JS。若需要滚动触发，优先 CSS / 轻量渐进增强，并保证内容默认可见。

```css
@supports (animation-timeline: view()) {
  @media (prefers-reduced-motion: no-preference) {
    .kb-scroll-reveal {
      animation: kb-reveal-up both;
      animation-timeline: view();
      animation-range: entry 10% cover 28%;
    }
  }
}
```

约束：

- 禁止 scroll-jacking。
- 禁止必须依赖外部 JS 才能看到内容。
- 不要在医疗、法律、金融表单等高信任场景使用强 parallax 或炫技 reveal。

### 4. 静态 JS 展示交互

适合移动菜单、FAQ accordion、tabs、gallery/carousel、静态 filter chips。只改变当前 DOM 的 class、ARIA 状态、焦点和可见性，不请求数据、不提交、不持久化。

```html
<section class="kb-faq" data-kb-faq>
  <button class="kb-faq-trigger" type="button" aria-expanded="false" aria-controls="kb-faq-panel-1">
    Delivery options
  </button>
  <div id="kb-faq-panel-1" class="kb-faq-panel" hidden>
    <p>Choose pickup, local courier, or scheduled delivery at checkout.</p>
  </div>
</section>
```

```html
<script>
document.querySelectorAll('[data-kb-faq]').forEach(function (root) {
  root.querySelectorAll('.kb-faq-trigger').forEach(function (trigger) {
    trigger.addEventListener('click', function () {
      var panel = document.getElementById(trigger.getAttribute('aria-controls'));
      var expanded = trigger.getAttribute('aria-expanded') === 'true';
      trigger.setAttribute('aria-expanded', String(!expanded));
      if (panel) panel.hidden = expanded;
    });
  });
});
</script>
```

约束：

- JS 必须按 View 或组件命名空间选择元素，避免全站误伤。
- 关键内容不要只在 hover 后出现；触屏和键盘用户也要能访问。
- 使用 `aria-expanded`、`aria-controls`、`hidden`、`focus-visible` 等基础可访问性状态。
- 若多个页面复用同一交互，优先放入共享 Script；单个 View 独享时可放在 View 底部。

## Kooboo 组织方式

### View 内局部动效

页面专属动效优先和对应 View 一起封装：

- class 加命名空间，例如 `kb-home-hero-*`、`kb-product-grid-*`
- keyframes 使用唯一前缀，例如 `kb-home-hero-reveal`
- 不要写通用 `.card`、`.btn`、`.reveal` 影响全站

### 全站通用动效

只有全站需要复用时，才放入 `create_style` 创建的全局 CSS：

- 通用 reveal 工具类
- 通用 easing / duration token
- reduced-motion fallback
- 按钮 active / focus / hover 基础反馈

示例：

```css
:root {
  --kb-ease-out: cubic-bezier(.16, 1, .3, 1);
  --kb-motion-fast: 160ms;
  --kb-motion-base: 240ms;
}

@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 1ms !important;
    animation-iteration-count: 1 !important;
    scroll-behavior: auto !important;
    transition-duration: 1ms !important;
  }
}
```

## 建站流程中的使用

- S3：`ui-ux-pro-max-pp` 只决定是否需要动效、动效强度和适用组件。
- S4 步骤 ④：Page 骨架只放轻量占位，不写最终动效。
- S4 步骤 ⑤：生成真实 View 时，若 block 的设计意图包含 motion 或静态交互，按本文添加局部 CSS / 原生 JS，或使用全局 motion token / 共享 Script。
- S4 步骤 ⑥：精修时检查动效是否统一、是否过量、是否有 reduced-motion fallback，以及移动菜单、FAQ、tabs、gallery 等静态交互是否已可用。
- S9：只处理依赖真实数据、服务端 API、提交、持久化、鉴权、订单/支付等业务动态；不得把 S4 应完成的静态展示交互延后到 S9。

## 禁止模式

| 禁止 | 原因 |
|------|------|
| 引入 Framer Motion / GSAP 到静态 Kooboo View | 静态建站阶段不需要框架依赖，增加失败面 |
| `opacity: 0` 等待 JS 解锁全部内容 | 脚本失败会出现空白页面 |
| 大量 `animate-bounce` / `animate-pulse` 装饰元素 | 廉价且干扰阅读 |
| 动画 `width` / `height` / `top` / `left` | 触发布局和重绘，移动端容易卡顿 |
| hover 才显示价格、CTA、FAQ 答案等关键信息 | 触屏和键盘用户无法稳定访问 |
| 所有行业使用同一套炫技动效 | 动效强度应匹配业务信任感和品牌语气 |
| 将 FAQ/tabs/menu/gallery 等静态展示交互推迟到动态化 | 静态版应是可体验页面，不是无交互截图 |
| 静态 JS 中调用 `fetch`、`k.*` 或真实提交 | 这已进入数据/API 动态化，应走 S6-S9 |

## 自检清单

- [ ] 动效是否服务于内容进入顺序、状态反馈或视觉叙事？
- [ ] 是否只使用了 transform / opacity 等低成本属性？
- [ ] 是否有 `prefers-reduced-motion` fallback？
- [ ] 内容在 JS 失败或 reduced motion 下是否仍完整可见？
- [ ] 移动端和触屏场景是否不依赖 hover？
- [ ] 同一页面是否保持统一的 easing、duration 和 reveal 语言？
- [ ] 不依赖后端的菜单、FAQ、tabs、gallery、静态筛选等交互是否已在 S4 完成？
- [ ] 静态 JS 是否没有 `fetch`、`k.*`、真实提交、持久化或业务 API？
