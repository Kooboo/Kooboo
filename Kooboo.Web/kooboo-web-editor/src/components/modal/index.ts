import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";
import { createBody } from "./body";

export function createModal(title: string, content: string | HTMLElement, width?: string, height?: string) {
  let style = document.createElement("style");
  document.head.appendChild(style);
  let { shade, win } = createContainer(width, height);
  const [body, setBodyContent] = createBody();
  const { footer, ok, cancel } = createFooter();
  const header = createHeader(title, win);
  setBodyContent(content);

  const recovery = () => {
    shade.parentElement!.removeChild(shade);
  };

  ok.onclick = recovery;
  cancel.onclick = recovery;
  win.appendChild(header);
  win.appendChild(body);
  win.appendChild(footer);

  return {
    modal: shade,
    setOkHandler: (h: () => void) => (ok.onclick = h),
    setCancelHandler: (h: () => void) => (cancel.onclick = h),
    close: recovery,
    hideCancel: () => (cancel.hidden = true)
  };
}
