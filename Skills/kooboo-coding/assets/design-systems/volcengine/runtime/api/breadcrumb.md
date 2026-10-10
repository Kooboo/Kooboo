# YlBreadcrumb

Import and register `{ YlBreadcrumb }` from `/yuanli.js` after installation.

Props: items=[] with {label,path?,disabled?}, label=面包屑, size=md (md/lg), maxItems=0. maxItems>=3 collapses long paths; the ellipsis button expands all items. The last item is current. Disabled items do not navigate. navigate(path) handles ordinary clicks; modified clicks retain native links. Use trusted same-App paths. No slots.
