/* kooboo-app-base:v2 */
// Temporary loading skeleton only. Replace the entire template immediately after design reads.
// Keep the existing RouterView in the generated content area; only the surrounding layout is temporary.
// Preserve and extend setup when authoring business navigation and layout.
// Follow the installer's DesignRuntime usage and API guides before editing.
// Write/read back app.css before mounting the generated layout. Keep app-root and
// data-app-region="content"; neither imposes a navigation position or page recipe.
import { computed, inject, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useServices } from '/request-context.js'
import { authService } from '/service.auth.js'
import { notificationService } from '/service.notification.js'

export default {
  name: 'DefaultLayout',
  koobooSource: { moduleUrl: import.meta.url },
  setup() {
    const bootstrap = inject('app.bootstrap')
    const route = useRoute()
    const router = useRouter()
    const { auth, notification } = useServices(import.meta.url, {
      auth: authService, notification: notificationService
    }, { currentRoute: true })
    const navigationOpen = ref(false)
    const accountMenuOpen = ref(false)
    const notificationsOpen = ref(false)
    const notificationsLoading = ref(false)
    const notificationsError = ref('')
    const notifications = ref([])
    const unreadCount = ref(Number(bootstrap.notifications?.unreadCount || 0))
    const currentUser = computed(() => bootstrap.user || {})
    const userName = computed(() => currentUser.value.fullName || currentUser.value.displayName || currentUser.value.userName || 'User')
    const navigation = computed(() => router.getRoutes().filter(item =>
      item.meta?.navigation !== false && item.meta?.navigationLabel &&
      router.resolve(item.path).matched.includes(route.matched[0])
    ))
    const go = destination => router.push(destination)

    function closeShellPanels() {
      navigationOpen.value = false
      accountMenuOpen.value = false
      notificationsOpen.value = false
    }

    function toggleAccountMenu() {
      notificationsOpen.value = false
      accountMenuOpen.value = !accountMenuOpen.value
    }

    function closeAccountMenu() {
      accountMenuOpen.value = false
    }

    function closeNotifications() {
      notificationsOpen.value = false
    }

    async function openNotifications() {
      accountMenuOpen.value = false
      notificationsOpen.value = true
      notificationsLoading.value = true
      notificationsError.value = ''
      try {
        const result = await notification.listNotifications()
        notifications.value = result.items || []
        unreadCount.value = Number(result.unreadCount || 0)
      } catch (error) {
        notificationsError.value = error.message || 'Unable to load notifications.'
      } finally {
        notificationsLoading.value = false
      }
    }

    watch(() => route.fullPath, closeShellPanels)

    // Every identifier used by the template must be exposed here or use $route/$router.
    return {
      route, router, go, navigation, navigationOpen, currentUser, userName, logout: auth.logout,
      accountMenuOpen, toggleAccountMenu, closeAccountMenu, closeShellPanels,
      notificationsOpen, notificationsLoading, notificationsError, notifications,
      unreadCount, openNotifications, closeNotifications
    }
  },
  template: `
    <div class="app-root app-base-skeleton" data-design-system="__DESIGN_SYSTEM_ID__">
      <header class="app-base-skeleton__header" aria-hidden="true">
        <div class="app-base-skeleton__identity">
          <span class="app-base-skeleton__block app-base-skeleton__icon"></span>
          <span class="app-base-skeleton__bar app-base-skeleton__brand"></span>
        </div>
        <div class="app-base-skeleton__actions">
          <span class="app-base-skeleton__block app-base-skeleton__search"></span>
          <span class="app-base-skeleton__block app-base-skeleton__avatar"></span>
        </div>
      </header>
      <div class="app-base-skeleton__workspace">
        <aside class="app-base-skeleton__sidebar" aria-hidden="true">
          <div class="app-base-skeleton__menus">
            <div v-for="index in 5" :key="index" class="app-base-skeleton__menu">
              <span class="app-base-skeleton__block app-base-skeleton__icon"></span>
              <span class="app-base-skeleton__bar app-base-skeleton__label"></span>
            </div>
          </div>
          <div class="app-base-skeleton__menu">
            <span class="app-base-skeleton__block app-base-skeleton__icon"></span>
            <span class="app-base-skeleton__bar app-base-skeleton__label"></span>
          </div>
        </aside>
        <main class="app-base-skeleton__content" data-app-region="content">
          <RouterView :key="$route.fullPath"></RouterView>
        </main>
      </div>
    </div>
  `
}
