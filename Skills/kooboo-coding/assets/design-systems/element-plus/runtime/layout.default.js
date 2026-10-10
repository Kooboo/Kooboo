/* kooboo-app-base:v2 */
import { appConfig } from '/app-config.js';
import { computed, inject, onBeforeUnmount, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { KbElementShell, KbLucideIcon, ElButton, ElPopover, ElAlert, ElConfigProvider, epZhCn } from '/element-plus.js';
import { useServices } from '/request-context.js';
import { authService } from '/service.auth.js';
import { notificationService } from '/service.notification.js';
const appName = __APP_NAME_JSON__;

export default {
  name: 'DefaultLayout',
  koobooSource: { moduleUrl: import.meta.url },
  components: { KbElementShell, KbLucideIcon, ElButton, ElPopover, ElAlert, ElConfigProvider },
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
    return { notificationsEnabled: appConfig.notificationsEnabled, appName, epZhCn, route, router, userName, navigation, accountOpen, notificationsOpen, loading, notifications, error, loggingOut, logoutError, loadNotifications, setNotificationsOpen, setAccountOpen, signOut };
  },
  template: `<ElConfigProvider :locale="epZhCn">
    <KbElementShell :app-name="appName" :navigation="navigation" :navigation-loading="!navigation.length && route.meta.planned === true" :active-path="route.path" navigation-label="主导航" menu-label="打开导航" @navigate="router.push($event)">
      <template #actions>
        <ElPopover v-if="notificationsEnabled" :visible="notificationsOpen" @update:visible="setNotificationsOpen" :width="360" trigger="click" placement="bottom-end" :show-arrow="false" :offset="10" popper-class="kb-ep-header-popover">
          <template #reference>
            <ElButton text class="kb-ep-header-action" :class="{ 'is-active': notificationsOpen }" aria-label="通知" :aria-expanded="notificationsOpen">
              <KbLucideIcon name="bell" :size="18" /><span class="kb-ep-action-label">通知</span>
            </ElButton>
          </template>
          <div class="kb-ep-popover-heading"><strong>通知</strong><span v-if="!loading && !error && notifications.length" class="kb-ep-notification-count">{{ notifications.length }}</span></div>
          <div v-if="loading" class="kb-ep-popover-state" role="status" aria-live="polite">正在加载通知…</div>
          <div v-else-if="error" class="kb-ep-popover-state" role="alert"><p>{{ error }}</p><ElButton plain size="small" @click="loadNotifications">重新加载</ElButton></div>
          <ul v-else-if="notifications.length" class="kb-ep-notification-list"><li v-for="item in notifications" :key="item.id"><strong>{{ item.title }}</strong><p>{{ item.body }}</p></li></ul>
          <div v-else class="kb-ep-popover-state" role="status"><KbLucideIcon name="bell" :size="28" /><strong>暂无通知</strong><p>新的通知会显示在这里</p></div>
        </ElPopover>
        <span v-if="notificationsEnabled" class="kb-ep-actions-divider" aria-hidden="true"></span>
        <ElPopover :visible="accountOpen" @update:visible="setAccountOpen" :width="260" trigger="click" placement="bottom-end" :show-arrow="false" :offset="10" popper-class="kb-ep-header-popover">
          <template #reference>
            <ElButton text class="kb-ep-header-action kb-ep-account-trigger" :class="{ 'is-active': accountOpen }" :aria-label="'账户：' + userName" :aria-expanded="accountOpen">
              <span class="kb-ep-account-avatar"><KbLucideIcon name="user-round" :size="18" /></span>
              <span class="kb-ep-account-name">{{ userName }}</span><KbLucideIcon name="chevron-down" :size="14" />
            </ElButton>
          </template>
          <div class="kb-ep-account-summary"><span class="kb-ep-account-avatar"><KbLucideIcon name="user-round" :size="20" /></span><div><span>当前账户</span><strong>{{ userName }}</strong></div></div>
          <ElButton text class="kb-ep-sign-out" :loading="loggingOut" @click="signOut"><KbLucideIcon v-if="!loggingOut" name="log-out" :size="16" /><span>退出登录</span></ElButton>
        </ElPopover>
      </template>
      <ElAlert v-if="logoutError" :title="logoutError" type="error" show-icon @close="logoutError = ''" />
      <RouterView :key="route.fullPath" />
    </KbElementShell>
  </ElConfigProvider>`
};
