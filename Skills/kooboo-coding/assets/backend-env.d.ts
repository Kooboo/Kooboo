// Editor-only subset of Kooboo's generated KScript declarations used by these templates.
// Not installed into sites. CLI projects use their generated kooboo.d.ts instead.
interface AppBaseHttpResponse {
  code: number
  data: string | null
}

interface AppBaseApiMethod {
  <T extends (...args: never[]) => unknown>(handler: T): void
  <T extends (...args: never[]) => unknown>(action: string, handler: T): void
}

interface AppFileInfo {
  name: string
  url: string
  size: number
  lastModified: Date
}

declare const k: {
  api: {
    get: AppBaseApiMethod
    put: AppBaseApiMethod
    post: AppBaseApiMethod
    notFound(): AppBaseHttpResponse
    httpCode(code: number, data: unknown): AppBaseHttpResponse
  }
  request: {
    queryString: { get(name: string): string | null }
  }
  account: {
    isLogin: boolean
    user: {
      current: {
        id: string
        userName: string
        email: string | null
        firstName: string | null
        lastName: string | null
        language: string | null
        isAdmin: boolean
      }
    }
    logout(returnUrl?: string): void
  }
  cookie: { remove(name: string): void }
  security: { newGuid(): string }
  file: {
    exists(path: string): boolean
    get(path: string): AppFileInfo | null
    folderFiles(folder: string): AppFileInfo[]
    writeBinary(path: string, bytes: number[]): unknown
    rename(source: string, destination: string): void
    delete(path: string): void
  }
  DB: {
    sqlite: {
      query(sql: string, parameters?: Record<string, unknown>): { [key: string]: any }[]
      execute(sql: string, parameters?: Record<string, unknown>): number
    }
  }
}
