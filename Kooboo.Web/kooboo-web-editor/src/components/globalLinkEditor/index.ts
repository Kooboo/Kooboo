import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createTabs } from "../common/tabs";
import { createDomLinkPanel } from "./domLinkPanel";
import { createContentLinkPanel } from "./contentLinkPanel";
import { parentBody } from "@/kooboo/outsideInterfaces";

export function createGlobalLinkEditor() {
  const options = [
    {
      title: TEXT.DOM_LINKS,
      selected: true,
      panel: createDomLinkPanel()
    },
    {
      title: TEXT.CONTENT_LINKS,
      panel: createContentLinkPanel()
    }
  ];
  let tabs = createTabs(options);
  const { modal, hideCancel } = createModal(TEXT.EDIT_LINKS, tabs, "500px", "500px");
  hideCancel();
  parentBody.appendChild(modal);
}
