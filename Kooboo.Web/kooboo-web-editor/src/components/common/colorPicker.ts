import { STANDARD_Z_INDEX } from "@/common/constants";
import Color from "color-string";
import { TEXT } from "@/common/lang";
import { createDiv } from "@/dom/element";
import Pickr from "@simonwep/pickr";
import context from "@/common/context";

export function createColorPicker(text: string, old: string, onsave: (color: string) => void) {
  old = Color.to.hex(Color.get(old)!.value);
  let el = createDiv();
  el.classList.add("kb_web_editor_color_picker");

  let label = createDiv();
  label.innerText = text;
  el.appendChild(label);

  let picker = createDiv();
  el.appendChild(picker);

  setTimeout(() => {
    const pickr = new Pickr({
      el: picker,
      theme: "nano",
      default: old,
      autoReposition: false,
      components: {
        preview: true,
        opacity: true,
        hue: true,
        interaction: {
          cancel: true,
          save: true
        }
      },
      strings: {
        save: TEXT.OK,
        cancel: TEXT.CANCEL
      }
    });
    pickr.on("init", (e: any) => {
      let app = e._root.app as HTMLElement;
      let button = e._root.button as HTMLElement;
      app.style.zIndex = STANDARD_Z_INDEX + 5 + "";
      app.style.fontSize = "14px";
      context.container.appendChild(app);
      button.style.border = "1px solid #ccc";
    });
    pickr.on("save", (e: any) => {
      onsave(e.toRGBA().toString(0));
    });
  });

  return el;
}
