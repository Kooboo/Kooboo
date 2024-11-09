import { parseQuery } from "vue-router";
import { template } from "lodash-es";

/**
 * http://abc.com/1.jpg -> ///abc.com/1.jpg
 */
export function toUniversalSchema(url?: string) {
  if (!url) return url;

  if (url.toLowerCase().startsWith("https:")) {
    return `/${url.substring(6)}`;
  }

  if (url.toLowerCase().startsWith("http:")) {
    return `/${url.substring(5)}`;
  }
}

export function getQueryString(name: string, query?: Record<string, string>) {
  const record = query || parseQuery(location.search);
  const keys = Object.keys(record);
  const key = keys.find((f) => f.toLowerCase() === name?.toLocaleLowerCase());
  return key ? record[key]?.toString() : undefined;
}

export function updateQueryString(url: string, params: Record<string, any>) {
  const index = url.indexOf("?");
  let search = index == -1 ? "" : url.substring(index + 1);
  const baseUrl = index == -1 ? url : url.substring(0, index);

  const urlParams: URLSearchParams = new URLSearchParams(search);

  for (const key in params) {
    if (params[key] !== undefined) {
      urlParams.set(key, params[key]);
    }
  }

  search = urlParams.toString();
  return `${baseUrl}?${search}`;
}

export function appendBaseUrl(url: string) {
  if (import.meta.env.PROD) {
    return import.meta.env.BASE_URL + url;
  }
  return url;
}

export function combineUrl(left: string, right: string) {
  left = left.trim();
  right = right.trim();
  if (
    right.startsWith("http://") ||
    right.startsWith("https://") ||
    right.startsWith("//") ||
    right.startsWith("data:")
  ) {
    return right;
  }

  if (left.endsWith("/")) {
    left = left.substring(0, left.length - 1);
  }

  if (right.startsWith("/")) {
    right = right.substring(1, right.length);
  }

  return left + "/" + right;
}

export function openInNewTab(url: string) {
  window.open(url, "_blank");
}

export function openInAnchorTag(url: string, filename?: string) {
  const anchor = document.createElement("a");
  anchor.href = url;
  if (filename) anchor.download = filename;
  anchor.style.display = "none";
  document.body.appendChild(anchor);
  anchor.click();
  setTimeout(() => {
    document.body.removeChild(anchor);
  }, 3000);
}

export function openInHiddenFrame(url: string) {
  const frame = document.createElement("iframe");
  frame.src = url;
  frame.style.display = "none";
  document.body.appendChild(frame);
  setTimeout(() => {
    document.body.removeChild(frame);
  }, 3000);
}

export function searchDebounce(fn: () => void, delay: number) {
  let timer: NodeJS.Timeout | null | undefined = null;
  return () => {
    if (timer) clearTimeout(timer);
    timer = setTimeout(() => {
      fn();
    }, delay);
  };
}

export function toName(url: string) {
  if (!url) return "";
  url = url.trim().replace(/[^\w\d]/g, "_");
  return url.replace(/^_*/, "");
}

export function isAbsoluteUrl(url: string) {
  try {
    const parsedURL = new URL(url);
    return parsedURL.protocol !== "" && parsedURL.host !== "";
  } catch {
    return false;
  }
}

export function renderTemplate(url: string, args: object) {
  const compiled = template(url, {
    interpolate: /{([\s\S]+?)}/g,
  });
  return compiled(args);
}
export function tryGetExtension(url: string) {
  if (!url) return "";
  const index = url.lastIndexOf(".");
  if (index < 1) return "";
  return url.substring(index);
}
