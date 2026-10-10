/* kooboo-app-base:v2; object-type: API */
// Route: /api/app/notifications/{action}. Named operations precede base callbacks.
import {
  getNotificationUser, countUnreadNotifications, listAppNotifications,
  markAppNotificationRead, markAllAppNotificationsRead
} from './Services.notification'
import { ENV } from './ENV'
import { successResponse, errorResponse } from './Utils.response'

k.api.get('unread-count', () => {
  const user = getNotificationUser()
  if (!user) return errorResponse(401, 'UNAUTHENTICATED', '请先登录')

  const unreadCount = countUnreadNotifications(user)
  const result = { unreadCount }

  return successResponse(result)
})

k.api.put('mark-read', (body: unknown) => {
  const user = getNotificationUser()
  if (!user) return errorResponse(401, 'UNAUTHENTICATED', '请先登录')

  if (!body || typeof body !== 'object' || Array.isArray(body)) {
    return errorResponse(400, 'INVALID_ARGUMENT', '请求体须为对象')
  }

  if (Object.keys(body).some(key => key !== 'id')) {
    return errorResponse(400, 'INVALID_ARGUMENT', '仅允许提供 id')
  }

  if (!('id' in body) || typeof body.id !== 'string') {
    return errorResponse(400, 'INVALID_ARGUMENT', 'id 须为字符串')
  }

  const id = body.id.trim()
  if (!id || id.length > 200) {
    return errorResponse(400, 'INVALID_ARGUMENT', '请提供有效的通知 ID')
  }

  // Both notification ID and current recipient constrain the update.
  markAppNotificationRead(user, id)

  return successResponse()
})

k.api.put('mark-all-read', () => {
  const user = getNotificationUser()
  if (!user) return errorResponse(401, 'UNAUTHENTICATED', '请先登录')

  markAllAppNotificationsRead(user)
  return successResponse()
})

k.api.get('/', (
  page: string | null,
  pageSize: string | null,
  unreadOnly: string | null
) => {
  const user = getNotificationUser()
  if (!user) return errorResponse(401, 'UNAUTHENTICATED', '请先登录')

  const pageNumber = page == null ? ENV.notifications.defaultPage : Number(page)
  const pageSizeNumber = pageSize == null ? ENV.notifications.defaultPageSize : Number(pageSize)

  if (!Number.isInteger(pageNumber) || pageNumber < 1 || pageNumber > ENV.notifications.maxPage) {
    return errorResponse(400, 'INVALID_ARGUMENT', 'page 超出允许范围或不是整数')
  }

  if (!Number.isInteger(pageSizeNumber) || pageSizeNumber < 1 || pageSizeNumber > ENV.notifications.maxPageSize) {
    return errorResponse(400, 'INVALID_ARGUMENT', 'pageSize 超出允许范围或不是整数')
  }

  if (unreadOnly != null && unreadOnly !== 'true' && unreadOnly !== 'false') {
    return errorResponse(400, 'INVALID_ARGUMENT', 'unreadOnly 须为 true 或 false')
  }

  const result = listAppNotifications(user, {
    page: pageNumber,
    pageSize: pageSizeNumber,
    unreadOnly: unreadOnly === 'true'
  })

  return successResponse(result)
})

k.api.get(() => k.api.notFound())
