import type { Addon } from "@/global/types";
import type { Source } from "@/api/component/types";
import { getSource } from "@/api/component";

export interface AddonSource {
  id: string;
  tag: string;
  source: Source;
}

export async function tryAddSource(sources: AddonSource[], addon: Addon) {
  let found = sources.find((f) => f.id === addon.id && f.tag === addon.type);

  if (!found) {
    const source = await getSource(addon.type, addon.id);
    found = { id: addon.id, tag: addon.type, source: source };
    sources.push(found);
  }

  return found;
}
