import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";
import { createBody } from "./body";

export function createModal(title: string, content: string) {
  document.body.style.overflow = "hidden";
  const [body, setBodyContent] = createBody();
  const [footer, setOkHandler, setCancelHandler] = createFooter();
  setBodyContent(content);
  let [container, addContainerItem] = createContainer();
  const recovery = () => {
    document.body.style.overflow = "auto";
    container.parentElement!.removeChild(container);
  };
  addContainerItem(createHeader(title));
  addContainerItem(body);
  addContainerItem(footer);

  return container;
}
