/* kooboo-app-base:v2 */
import { appConfig } from '/app-config.js';
import { computed, inject, onBeforeUnmount, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { KbAntShell, KbAntIcon, AButton, APopover, AAlert, AConfigProvider, antZhCN } from '/antdv.js';
import { useServices } from '/request-context.js';
import { authService } from '/service.auth.js';
import { notificationService } from '/service.notification.js';
const appName = __APP_NAME_JSON__;

export default {
  name: 'DefaultLayout',
  koobooSource: { moduleUrl: import.meta.url },
  components: { KbAntShell, KbAntIcon, AButton, APopover, AAlert, AConfigProvider },
  setup() {
    const bootstrap = inject('app.bootstrap');
    const route = useRoute();
    const router = useRouter();
    const { auth, notification } = useServices(import.meta.url, {
      auth: authService, notification: notificationService
    }, { currentRoute: true });
    const accountOpen = ref(false), notificationsOpen = ref(false), loading = ref(false), loggingOut = ref(false);
    const notifications = ref([]), error = ref(''), logoutError = ref('');
    let requestId = 0;
    const userName = computed(() => bootstrap.user?.fullName || bootstrap.user?.displayName || bootstrap.user?.userName || '账户');
    const navigation = computed(() => router.getRoutes().filter(item => item.meta?.navigation !== false && item.meta?.navigationLabel &&
      router.resolve(item.path).matched.includes(route.matched[0])).map(item => ({ path: item.path, label: item.meta.navigationLabel, group: item.meta.navigationGroup || '', icon: item.meta.navigationIcon || 'layout-grid' })));
    async function loadNotifications() {
      const request = ++requestId;
      loading.value = true; error.value = '';
      try { const result = await notification.listNotifications(); if (request === requestId) notifications.value = result.items || []; }
      catch (failure) { if (request === requestId) error.value = failure.message || '通知加载失败'; }
      finally { if (request === requestId) loading.value = false; }
    }
    // Let Popover handle click, keyboard and outside-click events. Toggling in
    // the reference button as well would handle the same interaction twice.
    function setNotificationsOpen(visible) {
      if (notificationsOpen.value === visible) return;
      notificationsOpen.value = visible;
      if (visible) {
        accountOpen.value = false;
        loadNotifications();
      } else {
        requestId++;
        loading.value = false;
      }
    }
    function setAccountOpen(visible) {
      accountOpen.value = visible;
      if (visible) setNotificationsOpen(false);
    }
    async function signOut() {
      if (loggingOut.value) return;
      loggingOut.value = true; logoutError.value = '';
      try { await auth.logout(); } catch (failure) { logoutError.value = failure.message || '退出失败，请重试'; }
      finally { loggingOut.value = false; }
    }
    watch(() => route.fullPath, () => { accountOpen.value = false; setNotificationsOpen(false); });
    onBeforeUnmount(() => { requestId++; });
    return { notificationsEnabled: appConfig.notificationsEnabled, appName, antZhCN, route, router, userName, navigation, accountOpen, notificationsOpen, loading, notifications, error, loggingOut, logoutError, loadNotifications, setNotificationsOpen, setAccountOpen, signOut };
  },
  template: `<AConfigProvider :locale="antZhCN">
    <KbAntShell :app-name="appName" :navigation="navigation" :navigation-loading="!navigation.length && route.meta.planned === true" :active-path="route.path" @navigate="router.push($event)">
      <template #actions>
        <APopover v-if="notificationsEnabled" :open="notificationsOpen" @update:open="setNotificationsOpen" trigger="click" placement="bottomRight">
          <AButton type="text" aria-label="通知" :aria-expanded="notificationsOpen"><template #icon><KbAntIcon name="bell" /></template></AButton>
          <template #content><section class="kb-ant-header-panel"><strong>通知</strong>
            <p v-if="loading" role="status">正在加载通知…</p>
            <div v-else-if="error" role="alert"><p>{{ error }}</p><AButton @click="loadNotifications">重新加载</AButton></div>
            <ul v-else-if="notifications.length"><li v-for="item in notifications" :key="item.id"><strong>{{ item.title }}</strong><p>{{ item.body }}</p></li></ul>
            <p v-else role="status">暂无通知</p>
          </section></template>
        </APopover>
        <APopover :open="accountOpen" @update:open="setAccountOpen" trigger="click" placement="bottomRight">
          <AButton type="text" :aria-label="'账户：' + userName" :aria-expanded="accountOpen"><template #icon><KbAntIcon name="user-round" /></template>{{ userName }}</AButton>
          <template #content><strong>{{ userName }}</strong><p><AButton :loading="loggingOut" @click="signOut">退出登录</AButton></p></template>
        </APopover>
      </template>
      <AAlert v-if="logoutError" :message="logoutError" type="error" show-icon closable @close="logoutError = ''" />
      <RouterView :key="route.fullPath" />
    </KbAntShell>
  </AConfigProvider>`
};
