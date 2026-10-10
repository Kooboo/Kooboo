# YlPageHeader

Import and register `{ YlPageHeader }` from `/yuanli.js` after installation.

Props: title required, description optional, breadcrumbs=[], tabs=[] (YlTabs items), modelValue optional selected tab key, status optional YlStatusTag type, statusText optional visible label. Emits navigate(path) from breadcrumbs and update:modelValue(key) from tabs. actions slot supplies title-level buttons; named tab slots supply the respective content. Each region is optional; omit tabs or breadcrumbs when not needed. This is a page header, not an App shell.

Use once at the top of business content, for example a purchase-request detail: breadcrumbs above; title/status/description at the left, actions at the right; optional tabs and their panels below. It is separate from the global TopNav/Header. Omit unused regions; narrow containers may wrap actions. Avoid duplicating the title inside a YlListPage.
