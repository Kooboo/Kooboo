import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createTabs } from "../../dom/tabs";
import { createOutLinkPanel } from "./outLinkPanel";
import { createPageLinkPanel } from "./pageLinkPanel";

export async function createLinkPicker(oldValue: string) {
  let el = document.createElement("div");

  const {
    outLinkPanel,
    getContent: getOutLinkContent,
    setContent
  } = createOutLinkPanel();

  const {
    pageLinkPanel,
    getContent: getPageLinkContent
  } = await createPageLinkPanel();

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
  console.log(oldValue);
  setContent(oldValue);
  el.appendChild(createTabs(options));
  let container = getEditorContainer();
  let { modal, setOkHandler } = createModal(TEXT.EDIT_LINK, el, "400px");
  container.appendChild(modal);

  return new Promise<string>(rs => {
    setOkHandler(async () => {
      let option = options.find(f => f.selected);
      if (option) {
        rs(option.getContent());
      } else rs("");
    });
  });
}
