import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createTabs } from "../common/tabs";
import { createContentImagePanel } from "./contentImagePanel";
import { createStyleImagePanel } from "./styleImagePanel";
import { createDomImagePanel } from "./domImagePanel";

export function createGlobalImageEditor() {
  const options = [
    {
      title: TEXT.DOM_IMAGE,
      selected: true,
      panel: createDomImagePanel()
    },
    {
      title: TEXT.CONTENT_IMAGE,
      panel: createContentImagePanel()
    },
    {
      title: TEXT.STYLE_IMAGE,
      panel: createStyleImagePanel()
    }
  ];
  let tabs = createTabs(options);
  const { modal, hideCancel } = createModal(TEXT.EDIT_IMAGE, tabs, "600px", "500px");
  hideCancel();
  getEditorContainer().appendChild(modal);
}
