/* kooboo-app-base:v2 */
import { appConfig } from '/app-config.js'
import { computed, inject, onBeforeUnmount, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { YlShell, YlButton, YlIcon, YlPopover, YlMessage } from '/yuanli.js'
import { useServices } from '/request-context.js'
import { authService } from '/service.auth.js'
import { notificationService } from '/service.notification.js'
const appName = __APP_NAME_JSON__

export default {
  name: 'DefaultLayout',
  koobooSource: { moduleUrl: import.meta.url },
  components: { YlShell, YlButton, YlIcon, YlPopover, YlMessage },
  setup() {
    const bootstrap = inject('app.bootstrap');
    const route = useRoute()
    const router = useRouter()
    const { auth, notification } = useServices(import.meta.url, {
      auth: authService, notification: notificationService
    }, { currentRoute: true })
    const accountOpen = ref(false)
    const notificationsOpen = ref(false)
    const loading = ref(false)
    const error = ref('')
    const notifications = ref([])
    const loggingOut = ref(false)
    const message = ref(null)
    let requestId = 0
    const userName = computed(() => bootstrap.user?.fullName || bootstrap.user?.displayName || bootstrap.user?.userName || '账户')
    const navigation = computed(() => router.getRoutes().filter(item => item.meta?.navigation !== false && item.meta?.navigationLabel &&
      router.resolve(item.path).matched.includes(route.matched[0])).map(item => ({
      path: item.path, label: item.meta.navigationLabel, group: item.meta.navigationGroup || '', icon: item.meta.navigationIcon || 'layout-grid'
    })))
    async function openNotifications() {
      const request = ++requestId
      accountOpen.value = false
      notificationsOpen.value = true
      loading.value = true
      error.value = ''
      try {
        const result = await notification.listNotifications()
        if (request === requestId) notifications.value = result.items || []
      } catch (failure) {
        if (request === requestId) error.value = failure.message || '通知加载失败'
      } finally {
        if (request === requestId) loading.value = false
      }
    }
    async function signOut() {
      if (loggingOut.value) return
      loggingOut.value = true
      message.value = null
      try { await auth.logout() } catch (failure) { message.value = { type: 'error', text: failure.message || '退出失败，请重试' } }
      finally { loggingOut.value = false }
    }
    watch(() => route.fullPath, () => { accountOpen.value = false; notificationsOpen.value = false; requestId++ })
    onBeforeUnmount(() => { requestId++ })
    return { notificationsEnabled: appConfig.notificationsEnabled, appName, route, router, userName, navigation, accountOpen, notificationsOpen, loading, error, notifications, openNotifications, loggingOut, message, signOut }
  },
  template: `
    <YlShell class="app-root" :app-name="appName" :navigation="navigation" :navigation-loading="!navigation.length && route.meta.planned === true" :active-path="route.path" @navigate="router.push($event)">
      <template #actions>
        <YlPopover v-if="notificationsEnabled" id="app-notifications-panel" class="yl-header-popover" v-model="notificationsOpen" title="通知" :width="360">
          <template #trigger="{triggerAttrs, open, toggle}">
            <YlButton class="yl-header-action" :class="{ 'is-active': open }" variant="text" icon="bell" aria-label="通知" v-bind="triggerAttrs" @click="open ? toggle() : openNotifications()"><span class="yl-header-action-label">通知</span></YlButton>
          </template>
          <div class="yl-header-panel-heading"><h2>通知</h2><span v-if="!loading && !error && notifications.length" class="yl-header-notification-count">{{ notifications.length }}</span></div>
          <div v-if="loading" class="yl-header-panel-state" role="status" aria-live="polite">正在加载通知…</div>
          <div v-else-if="error" class="yl-header-panel-state" role="alert"><p>{{ error }}</p><YlButton size="sm" icon="refresh-cw" @click="openNotifications">重新加载</YlButton></div>
          <ul v-else-if="notifications.length" class="yl-notifications"><li v-for="item in notifications" :key="item.id"><strong>{{ item.title }}</strong><p>{{ item.body }}</p></li></ul>
          <div v-else class="yl-header-panel-state" role="status"><YlIcon name="bell" :size="28" /><strong>暂无通知</strong><p>新的通知会显示在这里</p></div>
        </YlPopover>
        <span v-if="notificationsEnabled" class="yl-header-divider" aria-hidden="true"></span>
        <YlPopover id="app-account-panel" class="yl-header-popover" v-model="accountOpen" title="账户" :width="260">
          <template #trigger="{triggerAttrs, open, toggle}">
            <YlButton class="yl-header-action" :class="{ 'is-active': open }" variant="text" suffix-icon="chevron-down" :aria-label="'账户：' + userName" v-bind="triggerAttrs" @click="notificationsOpen = false; toggle()">
              <span class="yl-header-avatar"><YlIcon name="user-round" :size="18" /></span><span class="yl-header-user-name">{{ userName }}</span>
            </YlButton>
          </template>
          <div class="yl-header-account-summary"><span class="yl-header-avatar"><YlIcon name="user-round" :size="20" /></span><div><span>当前账户</span><strong>{{ userName }}</strong></div></div>
          <YlButton class="yl-account-action" variant="text" icon="log-out" :loading="loggingOut" @click="signOut">退出登录</YlButton>
        </YlPopover>
      </template>
      <RouterView :key="route.fullPath" />
    </YlShell>
    <YlMessage v-model="message" />
  `
}
