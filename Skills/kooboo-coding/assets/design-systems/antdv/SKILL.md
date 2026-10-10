---
name: antdv-runtime
description: Consume the bundled Ant Design Vue runtime with native APIs and themes.
---

# Ant Design Vue

Use design-system ID `antdv`. `create_app_base` installs the complete fixed runtime. Optional `components` selects API guides only; discover names in [runtime/component-selection.json](runtime/component-selection.json). Reuse returned guides before reading additional resources.

Follow [runtime/usage.md](runtime/usage.md) for native exports, registration, styles, ConfigProvider themes, dates and version upgrades. Optional `shellTemplate="antdv-workspace"` installs a sidebar workspace when it fits the business. Read [runtime/icons.md](runtime/icons.md) for native icons and [runtime/extensions.md](runtime/extensions.md) for business composition. Component APIs live in `runtime/api/`.

The maintained distribution is prebuilt; generated Apps do not run npm or bundle another Vue. Business code owns content, requests, validation and authorization. Runtime installation does not establish business or browser acceptance.
