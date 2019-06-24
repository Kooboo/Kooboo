import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";
import { createBody } from "./body";

export function createModal(
  title: string,
  content: string,
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
    setOkHandler: (h: () => Promise<void>) => {
      ok.onclick = async () => {
        await h();
        recovery();
      };
    },
    setCancelHandler: async (h: () => Promise<void>) => {
      cancel.onclick = async () => {
        await h();
        recovery();
      };
    }
  };
}
