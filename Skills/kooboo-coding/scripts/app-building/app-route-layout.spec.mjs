import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import test from 'node:test';

const resource = name => readFileSync(new URL(`../../assets/app-base/Script/${name}`, import.meta.url), 'utf8');
const moduleUrl = source => `data:text/javascript;base64,${Buffer.from(source).toString('base64')}`;
// Use the shipped Vue Router matcher/guards offline, with memory history instead of a browser.
const vueUrl = moduleUrl(resource('vue.js'));
const routerSource = resource('vue-router.js').replace(/from\s*(['"])vue\1/g, `from '${vueUrl}'`);
const { createRouter, createMemoryHistory } = await import(moduleUrl(routerSource));
const DefaultLayout = { template: '<RouterView />' };
const PlannedScreen = { template: '<p>Pending</p>' };

function scaffold() {
  const source = resource('router.js')
    .replace(/^import .*$/gm, '')
    .replace('__APP_ROUTES__', "{ path: '/', name: 'app-home', component: PlannedScreen, meta: { planned: true } },")
    .replace('export const appRoutes', 'const appRoutes')
    .replace('export function createAppRouter', 'function createAppRouter');
  return new Function('createRouter', 'createWebHashHistory', 'DefaultLayout', 'PlannedScreen', 'buildLoginUrl', 'buildRouteReturnUrl',
    `${source}\nreturn { appRoutes, createAppRouter };`)(
    createRouter, createMemoryHistory, DefaultLayout, PlannedScreen,
    path => `/login?returnUrl=${encodeURIComponent(path)}`, path => path
  );
}

// Browser globals are introduced after the vendored router has selected its server environment.
globalThis.document = { title: 'Test App' };
const redirects = [];
globalThis.window = { location: { assign: url => redirects.push(url) } };

test('Default page retains its URL and belongs to the installed layout', async () => {
  const { createAppRouter } = scaffold();
  const router = createAppRouter({ authenticated: true });
  await router.push('/');
  assert.deepEqual(router.currentRoute.value.matched.map(item => item.name), ['app-layout', 'app-home']);
  assert.equal(router.currentRoute.value.matched[0].components.default, DefaultLayout);
});

test('Sibling layouts and a standalone page do not inherit the default Shell', async () => {
  const { appRoutes, createAppRouter } = scaffold();
  const ReaderLayout = { template: '<main><RouterView /></main>' };
  const reader = { path: '/', name: 'reader', component: ReaderLayout, children: [
    { path: '', name: 'home', component: PlannedScreen, meta: { public: true } },
    { path: 'articles/:id', name: 'article', component: PlannedScreen, meta: { public: true } }
  ] };
  appRoutes[0].path = '/admin';
  appRoutes[0].children = [
    { path: '', name: 'admin-home', component: PlannedScreen },
    { path: 'settings', name: 'admin-settings', component: PlannedScreen }
  ];
  appRoutes.splice(1, 0, reader, { path: '/login', name: 'login', component: PlannedScreen, meta: { public: true } });
  const router = createAppRouter({ authenticated: true });
  await router.push('/admin');
  const workspace = router.currentRoute.value.matched[0];
  await router.push('/admin/settings');
  assert.equal(router.currentRoute.value.matched[0], workspace);
  await router.push('/articles/42');
  assert.deepEqual(router.currentRoute.value.matched.map(item => item.name), ['reader', 'article']);
  assert.equal(router.currentRoute.value.matched[0].components.default, ReaderLayout);
  await router.push('/login');
  assert.equal(router.currentRoute.value.matched.length, 1);
});

test('A page outside the default layout still requires authentication unless explicitly public', async () => {
  redirects.length = 0;
  const { appRoutes, createAppRouter } = scaffold();
  appRoutes.unshift(
    { path: '/read', name: 'read', component: PlannedScreen, meta: { public: true } },
    { path: '/private', name: 'private', component: PlannedScreen }
  );
  const router = createAppRouter({ authenticated: false });
  await router.push('/read');
  assert.equal(router.currentRoute.value.name, 'read');
  await router.push('/private');
  assert.deepEqual(redirects, ['/login?returnUrl=%2Fprivate']);
  assert.equal(router.currentRoute.value.name, 'read');
});
