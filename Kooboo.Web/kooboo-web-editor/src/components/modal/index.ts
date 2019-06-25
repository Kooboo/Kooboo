import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";
import { createBody } from "./body";

export function createModal(
  title: string,
  content: string | HTMLElement,
  width?: string,
  height?: string
) {
  document.body.style.overflow = "hidden";
  const [body, setBodyContent] = createBody();
  const { footer, ok, cancel } = createFooter();
  setBodyContent(content);
  let [container, addContainerItem] = createContainer(width, height);
  const recovery = () => {
    document.body.style.overflow = "auto";
    container.parentElement!.removeChild(container);
  };
  ok.onclick = recovery;
  cancel.onclick = recovery;
  addContainerItem(createHeader(title));
  addContainerItem(body);
  addContainerItem(footer);

  return {
    modal: container,
    setOkHandler: (h: () => void) => (ok.onclick = h),
    setCancelHandler: (h: () => void) => (cancel.onclick = h),
    close: recovery
  };
}
