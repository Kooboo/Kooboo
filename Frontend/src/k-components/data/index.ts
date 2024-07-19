import KData from "./k-data.vue";
import KDataQuery from "./k-data-query.vue";
import KDataHeader from "./k-data-header.vue";
import KDataItem from "./k-data-item.vue";
import KDataInit from "./k-data-init.vue";
import KDataStorage from "./k-data-storage.vue";
import type { Ref } from "vue";
import { updateQueryString } from "@/utils/url";
import { GetModuleUrl } from "../utils";

export default {
  KData,
  KDataQuery,
  KDataHeader,
  KDataItem,
  KDataInit,
  KDataStorage,
};

export function getDefaultValue(type: string, content?: string) {
  switch (typeMap.get(type)) {
    case "array":
      return tryToParse(content) ?? [];
    case "boolean":
      return tryToParse(content?.toLowerCase()) ?? false;
    case "number":
      return tryToParse(content) ?? 0;
    case "object":
      return tryToParse(content) ?? {};
    default:
      return content ?? "";
  }
}

function tryToParse(content?: string) {
  try {
    return JSON.parse(content!.toLowerCase());
  } catch (error) {
    //
  }
}

export const types = {
  string: "string",
  number: "number",
  boolean: "boolean",
  array: "array",
  object: "object",
};

export const typeMap = {
  string: types.string,
  text: types.string,
  textarea: types.string,
  html: types.string,
  image: types.string,
  date: types.string,
  time: types.string,
  datetime: types.string,
  number: types.number,
  switch: types.boolean,
  boolean: types.boolean,
  bool: types.boolean,
  array: types.array,
  list: types.array,
  object: types.object,
  get(value: string) {
    if (!value) return typeMap.string;
    return (typeMap as any)[value.trim().toLowerCase()];
  },
};

export type Type = keyof typeof types;
export type DataType = Exclude<keyof typeof typeMap, "get">;
export type DataSource =
  | "query"
  | "id"
  | "code"
  | "cookie"
  | "local-storage"
  | "session-storage";
export type StorageType = "local-storage" | "session-storage";

export interface DataStorage {
  name: string;
  from: "header";
  to: StorageType;
}

export interface DataSchema {
  type: DataType;
  data: Ref;
  name?: string;
  children?: DataSchema[];
  hidden?: boolean;
  label?: string;
  from?: string;
  options?: string;
  width?: number;
  disabled?: boolean;
  source?: DataSource;
}

export async function request(
  url: string,
  options?: {
    method?: "GET" | "POST";
    query?: Record<string, string>;
    body?: any;
    authUrl?: string;
    headers?: Record<string, string>;
    storages?: DataStorage[];
  }
) {
  if (options?.query && Object.keys(options.query)) {
    url = updateQueryString(url, options.query);
  }

  const response = await fetch(url, {
    method: options?.method ?? "GET",
    body: options?.body ? JSON.stringify(options?.body) : undefined,
    headers: options?.headers,
  });

  if (options?.storages) {
    for (const item of options.storages) {
      if (item.from == "header") {
        const value = response.headers.get(item.name);
        if (value) saveStorage(item.name, value, item.to);
      }
    }
  }

  if (response.status === 401 && options?.authUrl) {
    location.href = GetModuleUrl(options.authUrl);
    throw Error("auth abort");
  }

  return await response.json();
}

function saveStorage(key: string, value: string, type: StorageType) {
  switch (type) {
    case "local-storage":
      localStorage.setItem(key, value);
      break;
    case "session-storage":
      sessionStorage.setItem(key, value);
      break;

    default:
      break;
  }
}
