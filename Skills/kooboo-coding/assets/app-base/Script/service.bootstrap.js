/* kooboo-app-base:v2 */
import { bindServices } from '/request-context.js'
import { authService } from '/service.auth.js'
import { notificationService } from '/service.notification.js'

const { auth, notifications } = bindServices(
  { auth: authService, notifications: notificationService },
  { source: new URL(import.meta.url).pathname }
)

// Frontend composition only; user and notifications have independent APIs.
export async function loadAppBootstrap() {
  const user = await auth.getCurrentUser()
  return {
    authenticated: Boolean(user), user,
    notifications: { unreadCount: user ? await notifications.getUnreadCount() : 0 }
  }
}
