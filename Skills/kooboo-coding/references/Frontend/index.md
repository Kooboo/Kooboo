# Frontend References

Choose a reference based on the current object and runtime model; do not mix the rules for server-rendered Layout/View, client-rendered Vue, and ordinary scoped scripts.

## Technical routes

| Task | Required document |
|---|---|
| Client-rendered Vue, Vue 3 CDN, no-build Vue, or import-map Vue application | [Client-rendered Vue development guide](cdn-vue/index.md) |
| Site and App interactions, forms, editing, feedback, and navigation | [Frontend Interaction Guide](interaction-guide.md) |
| General HTML, CSS, and Script style | [Frontend code style](code-style.md) |
| Page | [Page](page.md) |
| Layout | [Layout](layout.md) |
| View | [View](view.md) |
| Template binding | [Template Binding Syntax](template-binding-syntax.md) |
| `k-data` queries | [k-data basics](k-data/basic.md) |
| `k-label` multilingual labels | [k-label](k-label.md) |
| Custom login UI | [Custom Login UI](custom-login-ui.md) |
| Motion | [Motion](motion.md) |

## Client-rendered Vue detection rules

When any of the following intents appears, go directly to the [Client-rendered Vue development guide](cdn-vue/index.md):

- Client-rendered Vue, Vue CDN, or no-build Vue;
- “Vue 3 + import map”;
- “do not build with Vite/Webpack/npm”;
- “run Vue directly with Kooboo Page, Style, and Script”;
- Creating or modifying an existing client-rendered Vue Page, store, service, or component.

This reference is a general client-rendering technical guide; it does not require a Blueprint and is not limited to prototypes. If API, Database, Permission, or authentication is also involved, read the corresponding Backend, KScript, or scenario references in parallel.
