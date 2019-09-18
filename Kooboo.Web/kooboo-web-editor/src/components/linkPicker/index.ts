import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createTabs } from "../common/tabs";
import { createOutLinkPanel } from "./outLinkPanel";
import { createPageLinkPanel } from "./pageLinkPanel";
import { createDiv } from "@/dom/element";
import context from "@/common/context";
import { STANDARD_Z_INDEX } from "@/common/constants";

export async function createLinkPicker(oldValue: string) {
  let el = createDiv();

  const { pageLinkPanel, getContent: getPageLinkContent } = await createPageLinkPanel(oldValue);

  const { outLinkPanel, getContent: getOutLinkContent, setContent } = createOutLinkPanel();

  const options = [
    {
      title: TEXT.PAGE_LINK,
      panel: pageLinkPanel,
      selected: !!getPageLinkContent(),
      getContent: getPageLinkContent
    },
    {
      title: TEXT.OUT_LINK,
      selected: !getPageLinkContent(),
      panel: outLinkPanel,
      getContent: getOutLinkContent
    }
  ];
  setContent(oldValue);
  el.appendChild(createTabs(options));
  let { modal, setOkHandler, close } = createModal(TEXT.EDIT_LINK, el, "400px", "500px");
  modal.style.zIndex = STANDARD_Z_INDEX + 100 + "";
  context.container.appendChild(modal);

  return new Promise<string>((rs, rj) => {
    setOkHandler(async () => {
      let result = "";
      let option = options.find(f => f.selected);
      if (option) result = option.getContent();
      if (result && result != oldValue) rs(result);
      else rj();
      close();
    });
  });
}
