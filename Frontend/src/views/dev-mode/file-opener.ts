import PagePanel from "@/views/dev-mode/tab-panels/page-panel.vue";
import type { ResolvedRoute } from "@/api/route/types";
import RichPagePanel from "@/views/dev-mode/tab-panels/rich-page-panel.vue";
import { open as codeOpen } from "@/views/dev-mode/side-panels/code-panel.vue";
import { open as formOpen } from "@/views/dev-mode/side-panels/form-panel.vue";
import { open as layoutOpen } from "@/views/dev-mode/side-panels/layout-panel.vue";
import { open as menuOpen } from "@/views/dev-mode/side-panels/menu-panel.vue";
import { open as pageOpen } from "@/views/dev-mode/side-panels/page-panel.vue";
import { open as scriptOpen } from "@/views/dev-mode/side-panels/script-panel.vue";
import { open as styleOpen } from "@/views/dev-mode/side-panels/style-panel.vue";
import { open as viewOpen } from "@/views/dev-mode/side-panels/view-panel.vue";

export function useFileOpener() {
  return {
    open(item: ResolvedRoute) {
      const type = item.type!.toLowerCase();
      switch (type) {
        case "page":
          pageOpen(
            { id: item.id, name: item.name },
            item.params.type === "RichText" ? RichPagePanel : PagePanel
          );
          break;
        case "script":
          scriptOpen({
            id: item.id,
            name: item.name,
            ownerObjectId: item.params.ownerObjectId,
          });
          break;
        case "style":
          styleOpen({
            id: item.id,
            name: item.name,
            ownerObjectId: item.params.ownerObjectId,
          });
          break;
        case "menu":
          menuOpen({ id: item.id, name: item.name });
          break;
        case "layout":
          layoutOpen({ id: item.id, name: item.name });
          break;
        case "view":
          viewOpen({ id: item.id, name: item.name });
          break;
        case "form":
          formOpen({ id: item.id, name: item.name });
          break;
        case "code":
          codeOpen({
            id: item.id,
            name: item.name,
            codeType: item.params.codeType,
          });
          break;
        default:
          break;
      }
    },
  };
}
