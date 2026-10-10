/* kooboo-app-base:v2; object-type: CodeBlock */
// One application-owned server configuration. Never expose this object wholesale to the browser.
interface AppEnvironment {
  authMode: 'mock' | 'kooboo'
  defaultLanguage: string
  files: {
    maxBytes: number
    temporaryHours: number
    cleanupBatchSize: number
    allowedExtensions: string[]
  }
  notifications: {
    defaultPage: number
    defaultPageSize: number
    maxPage: number
    maxPageSize: number
  }
}

export const ENV: AppEnvironment = {
  authMode: 'kooboo',
  defaultLanguage: 'zh-CN',
  files: {
    maxBytes: 10 * 1024 * 1024,
    temporaryHours: 24,
    cleanupBatchSize: 10,
    allowedExtensions: ['jpg', 'jpeg', 'png', 'gif', 'webp', 'pdf', 'txt', 'csv', 'xlsx', 'docx', 'zip']
  },
  notifications: {
    defaultPage: 1,
    defaultPageSize: 20,
    maxPage: 100000,
    maxPageSize: 100
  }
}
