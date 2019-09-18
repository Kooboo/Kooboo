import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createTabs } from "../common/tabs";
import { createContentImagePanel } from "./contentImagePanel";
import { createStyleImagePanel } from "./styleImagePanel";
import { createDomImagePanel } from "./domImagePanel";
import context from "@/common/context";

export function createGlobalImageEditor() {
  const options = [
    {
      title: TEXT.DOM_IMAGES,
      selected: true,
      panel: createDomImagePanel()
    },
    {
      title: TEXT.CONTENT_IMAGES,
      panel: createContentImagePanel()
    },
    {
      title: TEXT.STYLE_IMAGES,
      panel: createStyleImagePanel()
    }
  ];
  let tabs = createTabs(options);
  const { modal, hideCancel } = createModal(TEXT.EDIT_IMAGES, tabs, "600px", "500px");
  hideCancel();
  context.container.appendChild(modal);
}
