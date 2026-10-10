/* kooboo-app-base:v2 */
import { appConfig } from '/app-config.js'
import { defineService } from '/service.http.js'

const NOTIFICATION_URL = '/api/app/notifications'

async function notificationData(request, url, options) {
  const response = await request(url, options)
  if (!response || response.success !== true) throw new Error(response?.message || 'Unable to load notifications')
  return response.data
}

export const notificationService = defineService(import.meta.url, {
  async listNotifications(request, { page = 1, pageSize = 20, unreadOnly = false } = {}) {
    if (!appConfig.notificationsEnabled) return { page, pageSize, items: [], unreadCount: 0 }

    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize), unreadOnly: String(unreadOnly) })
    return notificationData(request, `${NOTIFICATION_URL}?${params.toString()}`)
  },

  async getUnreadCount(request) {
    if (!appConfig.notificationsEnabled) return 0

    const data = await notificationData(request, `${NOTIFICATION_URL}/unread-count`)
    return Number(data.unreadCount)
  },

  async markNotificationRead(request, id) {
    if (!appConfig.notificationsEnabled) return null

    return notificationData(request, `${NOTIFICATION_URL}/mark-read`, { method: 'PUT', body: { id } })
  },

  async markAllNotificationsRead(request) {
    if (!appConfig.notificationsEnabled) return null

    return notificationData(request, `${NOTIFICATION_URL}/mark-all-read`, { method: 'PUT' })
  }
})

export const { listNotifications, getUnreadCount, markNotificationRead, markAllNotificationsRead } = notificationService
