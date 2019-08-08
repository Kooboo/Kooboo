import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups, CssColorGroup } from "@/dom/style";
import { parentBody } from "@/kooboo/outsideInterfaces";
import { createDiv, createCheckboxInput } from "@/dom/element";
import { createColorPicker } from "../common/colorPicker";
import EditElement from "./EditElement";

export function createColorEditor(el: HTMLElement) {
  console.log(getMatchedColorGroups(el));

  let editElement = new EditElement(el);

  let modal = createModal(TEXT.EDIT_COLOR, editElement.render(), "600px");

  parentBody.appendChild(modal.modal);

  return new Promise<EditElement>((rs, rj) => {
    modal.setOkHandler(() => {
      editElement.ok();
      rs(editElement);
      modal.close();
    });
    modal.setCancelHandler(() => {
      editElement.cancel();
      rj();
      modal.close();
    });
  })
}

function createItem(cssColorGroup: CssColorGroup, el: HTMLElement) {
  let container = createDiv();
  container.style.display = "flex";
  let color = cssColorGroup.cssColors[0];
  let globalUpdate = !color.inline;
  let label = cssColorGroup.prop + cssColorGroup.pseudo;
  let oldValue = color.prop.getColor(color.value);
  let picker = createColorPicker(label, oldValue, s => {
    if (globalUpdate) {
      let notInlineColor = cssColorGroup.cssColors.find(f => !f.inline)!;
      let important = notInlineColor.cssStyleRule!.style.getPropertyPriority(color.prop.prop);
      let value = notInlineColor.prop.replaceColor(notInlineColor.value, s);
      notInlineColor.cssStyleRule!.style.setProperty(notInlineColor.prop.prop, value, important);
    } else {
      let value = el.style.getPropertyValue(color.prop.prop);
      let important = el.style.getPropertyPriority(color.prop.prop);
      value = color.prop.replaceColor(value, s);
      el.style.setProperty(color.prop.prop, value, important);
    }
  });
  picker.style.width = "200px";
  container.appendChild(picker);
  const onChecked = (checked: boolean) => {
    let notInlineColor = cssColorGroup.cssColors.find(f => !f.inline)!;
    if (checked) {
      let value = el.style.getPropertyValue(color.prop.prop);
      let important = el.style.getPropertyPriority(color.prop.prop);
      notInlineColor.cssStyleRule!.style.setProperty(notInlineColor.prop.prop, value, important);
      el.style.removeProperty(color.prop.prop);
      globalUpdate = true;
    } else {
      let value = notInlineColor.cssStyleRule!.style.getPropertyValue(color.prop.prop);
      let important = notInlineColor.cssStyleRule!.style.getPropertyPriority(color.prop.prop);
      el.style.setProperty(color.prop.prop, value, important);
      notInlineColor.cssStyleRule!.style.removeProperty(notInlineColor.prop.prop);
      globalUpdate = false;
    }
  };
  let { checkbox } = createCheckboxInput(TEXT.GLOBAL_UPDATE, !color.inline, onChecked);
  if (cssColorGroup.cssColors.every(e => e.inline) || cssColorGroup.pseudo) {
    checkbox.style.display = "none";
  }
  container.appendChild(checkbox);
  return container;
}
