import { STANDARD_Z_INDEX } from "@/common/constants";
import Color from "color-string";
import { TEXT } from "@/common/lang";
import { createDiv } from "@/dom/element";
import "@/components/colorPicker/nano.min.css";
import { Pickr } from "@/components/colorPicker/pickr";

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
      button.style.border = "1px solid #ccc";
    });
    pickr.on("save", (e: any) => {
      let c = e.toRGBA();
      onsave(`rgba(${c[0]},${c[1]},${c[2]},${c[3]})`);
    });
  });

  return el;
}