import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createTabs } from "../common/tabs";
import { createDomLinkPanel } from "./domLinkPanel";
import { createContentLinkPanel } from "./contentLinkPanel";

export function createGlobalLinkEditor() {
  const options = [
    {
      title: TEXT.DOM_LINK,
      selected: true,
      panel: createDomLinkPanel()
    },
    {
      title: TEXT.CONTENT_LINK,
      panel: createContentLinkPanel()
    }
  ];
  let tabs = createTabs(options);
  const { modal, hideCancel } = createModal(TEXT.EDIT_LINK, tabs, "500px");
  hideCancel();
  getEditorContainer().appendChild(modal);
}
