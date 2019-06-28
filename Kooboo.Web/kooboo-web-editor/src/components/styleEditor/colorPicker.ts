import "@simonwep/pickr/dist/themes/nano.min.css";
import Pickr from "@simonwep/pickr/dist/pickr.min";
import { STANDARD_Z_INDEX } from "@/common/constants";
import { createLabel } from "@/dom/label";
import { getEditorContainer } from "@/dom/utils";
import Color from "color-string";
import { TEXT } from "@/common/lang";

type ononsave = (color: string) => void;

export function createColorPicker(text: string, old: string, onsave: ononsave) {
  old = Color.to.hex(Color.get(old)!.value);
  let el = document.createElement("div");
  let label = createLabel(text);
  label.style.marginRight = "5px";
  let picker = document.createElement("span");
  el.appendChild(label);
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
      let root = e._root.root as HTMLElement;
      let button = e._root.button as HTMLElement;
      app.style.zIndex = STANDARD_Z_INDEX + 5 + "";
      getEditorContainer().appendChild(app);
      root.style.display = "inline-block";
      root.parentElement!.style.display = "inline-block";
      button.style.border = "1px solid #ccc";
    });
    pickr.on("save", (e: any) => {
      let c = e.toRGBA();
      onsave(`rgba(${c[0]},${c[1]},${c[2]},${c[3]})`);
    });
  });

  return el;
}
