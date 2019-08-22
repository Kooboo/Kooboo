import { CssColorGroup, CssColor } from "@/dom/style";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv, createCheckboxInput } from "@/dom/element";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { createColorPicker } from "../common/colorPicker";
import { Log } from "@/operation/recordLogs/Log";
import { TEXT } from "@/common/lang";

export function createColorEditItem(colorGroup: CssColorGroup, el: HTMLElement, comment: KoobooComment, koobooId: string) {
  let container = createDiv();
  container.style.display = "flex";
  container.style.alignItems = "center";
  container.style.height = "38px";

  // 颜色文本标签
  let colorTextSpan = document.createElement("span");
  colorTextSpan.style.width = "160px";

  // 内联日志
  let inlineCssLog: StyleLog | undefined;
  // 类日志
  let classCssLog: StyleLog | undefined;

  // 最高优先级cssColor
  let firstCssColor = colorGroup.cssColors[0];
  // 是否全局更新
  let isGlobalUpdate = !firstCssColor.inline;
  // 更新使用的内联样式
  let inlineCssColor = colorGroup.cssColors.find(f => f.inline);
  // 更新使用的类样式
  let classCssColor = colorGroup.cssColors.find(f => !f.inline);

  let updateColor = inlineCssColor || firstCssColor;
  colorTextSpan.innerText = firstCssColor.prop.getColor(firstCssColor.value);
  let picker = createColorPicker(colorGroup.prop + colorGroup.pseudo, firstCssColor.prop.getColor(firstCssColor.value), s => {
    colorTextSpan.innerText = s;
    if (isGlobalUpdate) {
      if (!classCssColor) return;
      let value = updateClassCss(classCssColor, s);
      classCssLog = StyleLog.createCssUpdate(
        value,
        classCssColor.prop.prop,
        classCssColor.rawSelector,
        classCssColor.url ? "" : classCssColor.koobooId,
        classCssColor.url!,
        !!classCssColor.newImportant,
        classCssColor.nameorid!,
        classCssColor.objecttype!,
        classCssColor.mediaRuleList
      );
    } else {
      let { important, value } = updateInlineCss(updateColor, s, el);
      inlineCssLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, value, updateColor.prop.prop, koobooId, !!important);
    }
  });
  picker.style.width = "200px";
  picker.style.justifyContent = "flex-end";
  container.appendChild(picker);

  container.appendChild(colorTextSpan);

  const onChecked = (checked: boolean) => {
    if (!classCssColor) return;

    if (checked) {
      let value = el.style.getPropertyValue(updateColor.prop.prop);
      value = updateColor.prop.getColor(value);
      value = updateClassCss(classCssColor, value);
      el.style.removeProperty(updateColor.prop.prop);
      inlineCssLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, "", updateColor.prop.prop, koobooId);
      classCssLog = StyleLog.createCssUpdate(
        value,
        classCssColor.prop.prop,
        classCssColor.rawSelector,
        classCssColor.url ? "" : classCssColor.koobooId,
        classCssColor.url!,
        !!classCssColor.newImportant,
        classCssColor.nameorid!,
        classCssColor.objecttype!,
        classCssColor.mediaRuleList
      );
      isGlobalUpdate = true;
    } else {
      let important = classCssColor.cssStyleRule!.style.getPropertyPriority(classCssColor.prop.prop);
      let value = classCssColor.cssStyleRule!.style.getPropertyValue(classCssColor.prop.prop);

      value = classCssColor.prop.getColor(value);
      value = updateColor.prop.replaceColor(updateColor.value, value);
      el.style.setProperty(updateColor.prop.prop, value, important);

      classCssColor.cssStyleRule!.style.setProperty(classCssColor.prop.prop, classCssColor.value, classCssColor.important ? "important" : "");
      classCssColor.newValue = undefined;
      classCssColor.newImportant = undefined;

      classCssLog = undefined;
      inlineCssLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, value, updateColor.prop.prop, koobooId, !!important);
      isGlobalUpdate = false;
    }
  };

  let { checkbox } = createCheckboxInput(TEXT.GLOBAL_UPDATE, !firstCssColor.inline, onChecked);
  if (!classCssColor || colorGroup.pseudo) {
    checkbox.style.display = "none";
  }
  container.appendChild(checkbox);

  const getLogs = () => {
    let logs: Log[] = [];
    if (inlineCssLog) logs.push(inlineCssLog);
    if (classCssLog) logs.push(classCssLog);
    return logs;
  };

  return {
    item: container,
    getLogs
  };
}

function updateClassCss(classCssColor: CssColor, color: string) {
  let important = classCssColor.cssStyleRule!.style.getPropertyPriority(classCssColor.prop.prop);
  let value = classCssColor.prop.replaceColor(classCssColor.value, color);

  classCssColor.cssStyleRule!.style.setProperty(classCssColor.prop.prop, value, important);
  classCssColor.newValue = value;
  classCssColor.newImportant = important;

  return value;
}

function updateInlineCss(inlineCssColor: CssColor, color: string, el: HTMLElement) {
  let value = el.style.getPropertyValue(inlineCssColor.prop.prop);
  let important = el.style.getPropertyPriority(inlineCssColor.prop.prop);

  value = inlineCssColor.prop.replaceColor(value, color);
  el.style.setProperty(inlineCssColor.prop.prop, value, important);

  return { value, important };
}
