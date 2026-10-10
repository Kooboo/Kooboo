/* kooboo-app-base:v2; object-type: CodeBlock */
/* app-base:start */
import { getCurrentAppUser } from './Services.user'
/* app-base:end */

export interface NotificationUser {
  id: string
  /* mock:start */
  isMock?: boolean
  /* mock:end */
}

export interface NotificationQuery {
  page: number
  pageSize: number
  unreadOnly: boolean
}

export interface AppNotification {
  id: string
  type: string
  title: string
  message: string | null
  actionUrl: string | null
  metadataJson: string | null
  readAt: string | null
  createdAt: string
}

export interface NotificationPage {
  page: number
  pageSize: number
  items: AppNotification[]
  unreadCount: number
}

export interface CreateNotificationInput {
  recipientUserId: string
  title: string
  type?: string
  message?: string | null
  actionUrl?: string | null
  metadataJson?: string | null
  createdAt?: string
}

// Current-recipient integration belongs to this service, not a separate module.
export function getNotificationUser(): NotificationUser | null {
  /* app-base:start */
  return getCurrentAppUser()
  /* app-base:end */
  /* standalone:start */
  if (!k.account.isLogin) return null

  return { id: String(k.account.user.current.id) }
  /* standalone:end */
}

export function countUnreadNotifications(user: NotificationUser): number {
  /* mock:start */
  if (user.isMock) return 0
  /* mock:end */

  const recipientUserId = user.id
  const row = k.DB.sqlite.query(`
    SELECT COUNT(*) AS count
    FROM AppNotifications
    WHERE RecipientUserId = @recipientUserId AND ReadAt IS NULL
  `, { recipientUserId })[0]
  return Number(row?.count || 0)
}

export function listAppNotifications(user: NotificationUser, options: NotificationQuery): NotificationPage {
  /* mock:start */
  if (user.isMock) return { page: options.page, pageSize: options.pageSize, items: [], unreadCount: 0 }
  /* mock:end */

  const recipientUserId = user.id
  const page = options.page
  const pageSize = options.pageSize
  const offset = (page - 1) * pageSize
  const unreadOnly = options.unreadOnly === true
  const unreadSql = unreadOnly ? ' AND ReadAt IS NULL' : ''
  const items = k.DB.sqlite.query(`
    SELECT Id AS id,
           Type AS type,
           Title AS title,
           Message AS message,
           ActionUrl AS actionUrl,
           MetadataJson AS metadataJson,
           ReadAt AS readAt,
           CreatedAt AS createdAt
    FROM AppNotifications
    WHERE RecipientUserId = @recipientUserId${unreadSql}
    ORDER BY CreatedAt DESC
    LIMIT @pageSize OFFSET @offset
  `, { recipientUserId, pageSize, offset }) as AppNotification[]

  return {
    page,
    pageSize,
    items,
    unreadCount: countUnreadNotifications(user)
  }
}

export function markAppNotificationRead(user: NotificationUser, notificationId: string): number {
  /* mock:start */
  if (user.isMock) return 0
  /* mock:end */

  const recipientUserId = user.id
  return k.DB.sqlite.execute(`
    UPDATE AppNotifications
    SET ReadAt = COALESCE(ReadAt, @readAt)
    WHERE Id = @notificationId AND RecipientUserId = @recipientUserId
  `, {
    notificationId,
    recipientUserId,
    readAt: new Date().toISOString()
  })
}

export function markAllAppNotificationsRead(user: NotificationUser): number {
  /* mock:start */
  if (user.isMock) return 0
  /* mock:end */

  const recipientUserId = user.id
  return k.DB.sqlite.execute(`
    UPDATE AppNotifications
    SET ReadAt = @readAt
    WHERE RecipientUserId = @recipientUserId AND ReadAt IS NULL
  `, {
    recipientUserId,
    readAt: new Date().toISOString()
  })
}

export function createAppNotification(input: CreateNotificationInput): string {
  const id = k.security.newGuid()
  k.DB.sqlite.execute(`
    INSERT INTO AppNotifications (
      Id, RecipientUserId, Type, Title, Message,
      ActionUrl, MetadataJson, ReadAt, CreatedAt
    ) VALUES (
      @id, @recipientUserId, @type, @title, @message,
      @actionUrl, @metadataJson, NULL, @createdAt
    )
  `, {
    id,
    recipientUserId: input.recipientUserId,
    type: input.type || 'info',
    title: input.title,
    message: input.message || null,
    actionUrl: input.actionUrl || null,
    metadataJson: input.metadataJson || null,
    createdAt: input.createdAt || new Date().toISOString()
  })
  return id
}
