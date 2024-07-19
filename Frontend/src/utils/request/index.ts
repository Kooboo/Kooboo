/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import instance from "./instance";
import type { RequestConfig } from "./types";

export default {
  async get<T>(
    url: string,
    params?: Record<string, unknown>,
    config: RequestConfig = {}
  ): Promise<T> {
    const response = await instance.get<T>(url, {
      params,
      ...config,
    });

    return response.data;
  },
  async post<T>(
    url: string,
    data?: any,
    params?: Record<string, unknown>,
    config: RequestConfig = {}
  ): Promise<T> {
    const response = await instance.post<T>(url, data, {
      params,
      ...config,
    });

    return response.data;
  },
  async put<T>(
    url: string,
    data?: any,
    params?: Record<string, unknown>,
    config: RequestConfig = {}
  ): Promise<T> {
    const response = await instance.put<T>(url, data, {
      params,
      ...config,
    });

    return response.data;
  },
  async delete<T>(
    url: string,
    data?: unknown,
    params?: Record<string, unknown>,
    config: RequestConfig = {}
  ): Promise<T> {
    const response = await instance.delete<T>(url, {
      params,
      data,
      ...config,
    });

    return response.data;
  },
};

export const api = instance;
