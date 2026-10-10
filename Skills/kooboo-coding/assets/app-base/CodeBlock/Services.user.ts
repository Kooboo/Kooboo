/* kooboo-app-base:v2; object-type: CodeBlock */
import { ENV } from './ENV'

export interface AppIdentity {
  id: string
  koobooUserId: string | null
  userName: string
  email: string | null
  displayName: string
  fullName?: string
  avatarUrl?: string | null
  firstName: string | null
  lastName: string | null
  language: string
  isAdmin: boolean
  /* mock:start */
  isMock?: boolean
  /* mock:end */
}

export function getCurrentIdentity(): AppIdentity | null {
  /* mock:start */
  // MOCK ONLY: remove this entire branch before using real data or delivering the App.
  if (ENV.authMode === 'mock') {
    return {
      id: 'mock-user',
      koobooUserId: null,
      userName: 'demo',
      email: null,
      displayName: 'Demo User',
      fullName: 'Demo User',
      avatarUrl: null,
      firstName: null,
      lastName: null,
      language: ENV.defaultLanguage,
      isAdmin: false,
      isMock: true
    }
  }
  /* mock:end */

  if (!k.account.isLogin) return null
  const current = k.account.user.current
  const firstName = current.firstName || ''
  const lastName = current.lastName || ''
  const fullName = [firstName, lastName].filter(Boolean).join(' ') || current.userName
  return {
    id: String(current.id),
    koobooUserId: String(current.id),
    userName: current.userName,
    email: current.email || null,
    displayName: fullName,
    firstName: firstName || null,
    lastName: lastName || null,
    language: current.language || ENV.defaultLanguage,
    isAdmin: Boolean(current.isAdmin)
  }
}

export interface AppUser extends AppIdentity {
  createdAt?: string
  updatedAt?: string
  lastSeenAt?: string
}

export interface UpdateProfileInput {
  displayName: string
}

interface AppUserRow {
  id: string
  koobooUserId: string
  userName: string
  email: string | null
  displayName: string | null
  avatarUrl: string | null
  createdAt: string
  updatedAt: string
  lastSeenAt: string
}

export function getCurrentAppUser(): AppUser | null {
  const identity = getCurrentIdentity()
  if (!identity) return null
  /* mock:start */
  if (identity.isMock) return identity
  /* mock:end */

  const timestamp = new Date().toISOString()
  // One atomic upsert handles concurrent first requests without a check/insert race.
  // Preserve App-owned display name and avatar when synchronizing account fields.
  k.DB.sqlite.execute(`
    INSERT INTO AppUsers (
      Id, KoobooUserId, KoobooUserName, Email, DisplayName,
      AvatarUrl, CreatedAt, UpdatedAt, LastSeenAt
    ) VALUES (
      @id, @koobooUserId, @userName, @email, @displayName,
      NULL, @timestamp, @timestamp, @timestamp
    ) ON CONFLICT(KoobooUserId) DO UPDATE SET
      KoobooUserName = excluded.KoobooUserName,
      Email = excluded.Email,
      LastSeenAt = excluded.LastSeenAt
  `, {
    id: k.security.newGuid(), koobooUserId: identity.koobooUserId,
    userName: identity.userName, email: identity.email,
    displayName: identity.displayName, timestamp
  })
  const user = k.DB.sqlite.query(`
    SELECT Id AS id, KoobooUserId AS koobooUserId, KoobooUserName AS userName,
           Email AS email, DisplayName AS displayName, AvatarUrl AS avatarUrl,
           CreatedAt AS createdAt, UpdatedAt AS updatedAt, LastSeenAt AS lastSeenAt
    FROM AppUsers WHERE KoobooUserId = @koobooUserId LIMIT 1
  `, { koobooUserId: identity.koobooUserId })[0] as AppUserRow
  const displayName = user.displayName || identity.displayName
  return { ...identity, ...user, displayName, fullName: displayName }
}

export function updateCurrentAppUser(user: AppUser, patch: UpdateProfileInput): AppUser {
  /* mock:start */
  // Preview response only: mock edits are not persisted across requests.
  if (user.isMock) return { ...user, displayName: patch.displayName, fullName: patch.displayName }
  /* mock:end */

  const updatedAt = new Date().toISOString()
  k.DB.sqlite.execute(`
    UPDATE AppUsers SET DisplayName = @displayName, UpdatedAt = @updatedAt
    WHERE Id = @id AND KoobooUserId = @koobooUserId
  `, { displayName: patch.displayName, updatedAt, id: user.id, koobooUserId: user.koobooUserId })
  return { ...user, displayName: patch.displayName, fullName: patch.displayName, updatedAt }
}
