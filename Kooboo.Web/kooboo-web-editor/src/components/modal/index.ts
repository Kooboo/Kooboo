import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";
import { createBody } from "./body";

export function createModal(title: string, content: string | HTMLElement, width?: string, height?: string) {
  let { shade, win } = createContainer(width);
  const [body, setBodyContent] = createBody(height);
  const { footer, ok, cancel } = createFooter();
  const { header, close } = createHeader(title, win);
  setBodyContent(content);

  const recovery = () => {
    shade.parentElement!.removeChild(shade);
  };

  ok.onclick = recovery;
  cancel.onclick = recovery;
  close.onclick = recovery;
  win.appendChild(header);
  win.appendChild(body);
  win.appendChild(footer);

  return {
    modal: shade,
    setOkHandler: (h: () => void) => (ok.onclick = h),
    setCancelHandler: (h: () => void) => {
      cancel.onclick = h;
      close.onclick = h;
    },
    close: recovery,
    hideCancel: () => (cancel.style.display = "none")
  };
}
