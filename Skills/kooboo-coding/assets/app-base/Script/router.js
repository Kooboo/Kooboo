/* kooboo-app-base:v2 */
import { createRouter, createWebHashHistory } from 'vue-router'
import DefaultLayout from '/layout.default.js'
import PlannedScreen from '/component.planned-screen.js'
import { buildLoginUrl, buildRouteReturnUrl } from '/service.auth.js'

// Without planning, create_app_base seeds a neutral landing placeholder. Add business
// routes as persisted Screen modules are implemented. With an App Plan, it seeds one
// route for every planned Screen. Only Screens referenced by
// foundation.navigation appear in the shell; workflow, detail, and action-target Screens
// remain route-only. Implement and persist one Screen module first, then immediately
// replace only that Screen's PlannedScreen component. Never modify the shared managed
// placeholder and never import a Script that does not exist yet. Set meta.public = true
// only for an explicitly public Screen. Access flags do not select a layout.
// Put pages under the parent layout that owns them. Additional layouts are sibling
// routes with their own children; a top-level Screen has no surrounding layout.
// Keep parent layout routes free of public:true; mark anonymous leaf pages explicitly.
export const appRoutes = [
  {
    path: '/',
    name: 'app-layout',
    component: DefaultLayout,
    children: [
      // Existing absolute page URLs are preserved within this layout.
__APP_ROUTES__
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
    meta: { navigation: false }
  }
]

export function createAppRouter(bootstrap) {
  const appTitle = document.title
  const router = createRouter({
    history: createWebHashHistory(),
    routes: appRoutes,
    scrollBehavior: () => ({ top: 0 })
  })

  router.beforeEach(to => {
    if (to.meta.public === true || bootstrap.authenticated) return true

    window.location.assign(buildLoginUrl(buildRouteReturnUrl(to.fullPath)))
    return false
  })

  router.afterEach(to => {
    const title = typeof to.meta.title === 'string' ? to.meta.title : ''
    document.title = title ? `${title} · ${appTitle}` : appTitle
  })

  return router
}
