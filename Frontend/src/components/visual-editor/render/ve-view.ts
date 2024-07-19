import type { Meta, VeRenderContext } from "../types";

export default async function (
  meta: Meta,
  classic: boolean,
  rootMeta: Meta,
  ctx: VeRenderContext
): Promise<string> {
  const { baseUrl } = ctx;
  const url: URL = new URL(`/__kb/View/${meta.name}`, baseUrl);
  for (const def of meta.propDefines) {
    const key = def.name;
    if (!key || key.toLowerCase().startsWith("vecontainer")) {
      continue;
    }
    url.searchParams.set(key, (meta.props[key] ?? "").toString());
  }

  meta.htmlStr = await fetch(url.toString()).then((data) => data.text());
  return meta.htmlStr;
}
