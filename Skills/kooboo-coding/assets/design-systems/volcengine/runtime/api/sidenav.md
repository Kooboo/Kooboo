# YlSidenav

Import and register `{ YlSidenav }` from `/yuanli.js` after installation.

Props: navigation=[] (same recursive NavigationItem contract as YlShell), activePath="", navigationLabel=主导航, title optional, collapsed=false. Full navigation supports flat groups, icons, nested disclosures, disabled subtrees, active ancestors and keyboard navigation. collapsed uses a 56px icon rail; branch icons open an accessible panel with the full nested navigation. The App owns the collapsed state and any toggle placement. navigate(path) handles ordinary route clicks; modified links retain native behavior. footer slot is optional. Independent from YlShell and viewport height; place in YlAside for application layout.
