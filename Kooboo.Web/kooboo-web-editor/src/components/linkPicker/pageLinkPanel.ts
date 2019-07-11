import { createRadioInput } from "@/dom/input";
import { getPageUrls } from "@/kooboo/outsideInterfaces";
import createDiv from "@/dom/div";

export async function createPageLinkPanel(oldValue: string) {
  let urls = await getPageUrls();
  let el = createDiv();
  let selectedItem!: string;
  let radios = urls.map(m => createRadioInput(m));
  radios.forEach(i => {
    i.radio.onclick = () => {
      radios.filter(f => f != i).forEach(f => f.setChecked(false));
      i.setChecked(true);
      selectedItem = i.getContent();
    };
    if (i.getContent() == oldValue) i.setChecked(true);
    el.appendChild(i.radio);
  });
  return {
    pageLinkPanel: el,
    getContent: () => selectedItem
  };
}
