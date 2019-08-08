import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups, CssColorGroup } from "@/dom/style";
import { parentBody } from "@/kooboo/outsideInterfaces";
import { createDiv, createCheckboxInput } from "@/dom/element";
import { createColorPicker } from "../common/colorPicker";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { CssUnit } from "@/operation/recordUnits/CssUnit";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { setGuid } from "@/kooboo/utils";

export function createColorEditor(el: HTMLElement) {
  var startStyle = el.getAttribute("style");
  var groups = getMatchedColorGroups(el);
  let container = createDiv();
  for (const i of groups) {
    let row = createItem(i, el);
    container.appendChild(row);
  }

  const getUnits = () => [new AttributeUnit(startStyle || "", "style"), new CssUnit("", groups)];

  let { modal, close, setCancelHandler, setOkHandler } = createModal(TEXT.EDIT_COLOR, container, "600px");
  setCancelHandler(() => {
    getUnits().map(m => m.undo(el));
    close();
  });

  setOkHandler(() => {
    let guid = setGuid(el);
    let operation = new operationRecord(getUnits(), [], guid);
    context.operationManager.add(operation);
    close();
  });
  parentBody.appendChild(modal);
}

function createItem(colorGroup: CssColorGroup, el: HTMLElement) {
  let container = createDiv();
  container.style.display = "flex";
  let color = colorGroup.cssColors[0];
  let globalUpdate = !color.inline;
  let inlineColor = colorGroup.cssColors.find(f => f.inline);
  let cssColor = colorGroup.cssColors.find(f => !f.inline);
  let updateColor = inlineColor || color;
  let label = colorGroup.prop + colorGroup.pseudo;
  let oldValue = color.prop.getColor(color.value);
  let picker = createColorPicker(label, oldValue, s => {
    if (globalUpdate) {
      if (!cssColor) return;
      let important = cssColor.cssStyleRule!.style.getPropertyPriority(cssColor.prop.prop);
      let value = cssColor.prop.replaceColor(cssColor.value, s);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, value, important);
      cssColor.newValue = value;
      cssColor.newImportant = important;
    } else {
      let value = el.style.getPropertyValue(updateColor.prop.prop);
      let important = el.style.getPropertyPriority(updateColor.prop.prop);
      value = updateColor.prop.replaceColor(value, s);
      el.style.setProperty(updateColor.prop.prop, value, important);
    }
  });
  picker.style.width = "200px";
  container.appendChild(picker);

  const onChecked = (checked: boolean) => {
    if (!cssColor) return;
    if (checked) {
      let value = el.style.getPropertyValue(updateColor.prop.prop);
      value = updateColor.prop.getColor(value);
      let important = el.style.getPropertyPriority(updateColor.prop.prop);
      value = cssColor.prop.replaceColor(cssColor.value, value);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, value, important);
      cssColor.newValue = value;
      cssColor.newImportant = important;
      el.style.removeProperty(updateColor.prop.prop);
      globalUpdate = true;
    } else {
      let important = cssColor.cssStyleRule!.style.getPropertyPriority(cssColor.prop.prop);
      let value = cssColor.cssStyleRule!.style.getPropertyValue(cssColor.prop.prop);
      value = cssColor.prop.getColor(value);
      value = updateColor.prop.replaceColor(updateColor.value, value);
      el.style.setProperty(updateColor.prop.prop, value, important);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, cssColor.value, cssColor.important ? "important" : "");
      cssColor.newValue = undefined;
      cssColor.newImportant = undefined;
      globalUpdate = false;
    }
  };
  let { checkbox } = createCheckboxInput(TEXT.GLOBAL_UPDATE, !color.inline, onChecked);
  if (!cssColor || colorGroup.pseudo) {
    checkbox.style.display = "none";
  }
  container.appendChild(checkbox);

  return container;
}
