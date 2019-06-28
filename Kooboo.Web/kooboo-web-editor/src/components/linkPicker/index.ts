import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createTabs } from "./tabs";
import { createOutLinkPanel } from "./outLinkPanel";
import { createPageLinkPanel } from "./pageLinkPanel";

export async function createLinkPicker(oldValue: string) {
  let el = document.createElement("div");

  const {
    pageLinkPanel,
    getContent: getPageLinkContent
  } = await createPageLinkPanel(oldValue);

  const {
    outLinkPanel,
    getContent: getOutLinkContent,
    setContent
  } = createOutLinkPanel();

  const options = [
    {
      title: TEXT.PAGE_LINK,
      panel: pageLinkPanel,
      getContent: getPageLinkContent
    },
    {
      title: TEXT.OUT_LINK,
      selected: true,
      panel: outLinkPanel,
      getContent: getOutLinkContent
    }
  ];
  setContent(oldValue);
  el.appendChild(createTabs(options));
  let container = getEditorContainer();
  let { modal, setOkHandler, close } = createModal(TEXT.EDIT_LINK, el, "400px");
  container.appendChild(modal);

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
