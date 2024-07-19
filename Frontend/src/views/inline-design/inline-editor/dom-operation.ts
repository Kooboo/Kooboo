//!!!   document.execCommand is obsolete  !!!
//document.execCommand已经过时,但是依旧被广泛浏览器支持,目前时间点w3c没有很好的替代方案
//或许将来此标准https://w3c.github.io/input-events/可以进入标准,之后再来重构此代码
import { doc } from "../page";

export function insertImage(src: string) {
  if (!doc.value) return;
  doc.value.execCommand("insertImage", false, src);
}

export function redo() {
  if (!doc.value) return;
  doc.value.execCommand("redo");
}

export function undo() {
  if (!doc.value) return;
  doc.value.execCommand("undo");
}
