import type { Addon, Placeholder } from "@/global/types";

import type { AddonSource } from "./source";
import { layoutToAddon } from "@/utils/page";
import { tryAddSource } from "./source";

export async function mergeAddon(
  left: Addon,
  right: Addon,
  sources: AddonSource[]
) {
  if (left.type === "layout") {
    for (const leftPlaceholder of left.content as Placeholder[]) {
      if (right.type === "layout") {
        const rightPlaceholder = (right.content as Placeholder[]).find(
          (f) => f.name === leftPlaceholder.name
        );
        const rightAddons = rightPlaceholder?.addons;
        if (rightAddons) {
          for (const addon of rightAddons) {
            const source = await tryAddSource(sources, addon);
            if (addon.type === "layout") {
              const layout = layoutToAddon(source.id, source.source.body);
              leftPlaceholder.addons.push(layout);
              mergeAddon(layout, addon, sources);
            } else {
              leftPlaceholder.addons.push(addon);
            }
          }
        }
      }
    }
  }
}
