import type { KeyValue } from "@/global/types";

export function noEmptyArray(arr: any): boolean {
  return Array.isArray(arr) && arr.length > 0;
}

export function deepCopy<T = any>(val: any) {
  return JSON.parse(JSON.stringify(val)) as T;
}

export function clearAndPush(arr: Array<any>, newArr: Array<any>) {
  arr.length = 0;
  if (Array.isArray(newArr)) {
    arr.push(...newArr);
  }
}

export function toObject(list: { key: string; value: unknown }[]) {
  const result: Record<string, unknown> = {};

  for (const item of list) {
    if (item.key) {
      result[item.key] = item.value;
    }
  }

  return result;
}

export function toList(obj: Record<string, string>) {
  const result: KeyValue[] = [];

  for (const key in obj) {
    result.push({
      key: key,
      value: obj[key],
    });
  }
  return result;
}

export class Completer<T> {
  public promise: Promise<T>;
  public resolve!: (value: T | PromiseLike<T>) => void;
  public reject!: (reason?: any) => void;

  constructor() {
    this.promise = new Promise((resolve, reject) => {
      this.resolve = resolve;
      this.reject = reject;
    });
  }
}

export function setProperty(obj: any, value: any, ...props: string[]) {
  const last = props.pop();
  if (!last) return;

  for (const i of props) {
    obj = obj[i];
    if (!obj) return;
  }

  obj[last] = value;
}

export function getProperty(obj: any, ...props: string[]) {
  const last = props.pop();
  if (!last) return;

  for (const i of props) {
    obj = obj[i];
    if (!obj) return;
  }

  return obj[last];
}

export function isJson(content: string) {
  try {
    JSON.parse(content);
    return true;
  } catch (error) {
    return false;
  }
}

export function tryParseInt(value?: string) {
  if (!value) return null;
  try {
    return parseInt(value);
  } catch (error) {
    return null;
  }
}
