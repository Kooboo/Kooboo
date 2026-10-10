// Application-owned response helpers. Safe to edit, replace, or remove.
export function successResponse<T = null>(data: T | null = null, message: string = "", httpStatus: number = 200): ReturnType<typeof k.api.httpCode> {
    return k.api.httpCode(httpStatus, { success: true, code: "OK", message, data });
}

export function errorResponse<T = null>(httpStatus: number, code: string, message: string, data: T | null = null): ReturnType<typeof k.api.httpCode> {
    return k.api.httpCode(httpStatus, { success: false, code, message, data });
}
