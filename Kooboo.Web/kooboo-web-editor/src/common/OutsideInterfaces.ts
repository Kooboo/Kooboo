import { createModal } from "@/components/modal";
import { TEXT } from "./lang";
import { createIframe } from "@/dom/Iframe";
const Kooboo = (document as any).Kooboo;
const mediaDialogData = (document as any).mediaDialogData;
const parentBody = (document as any).parentBody as HTMLBodyElement;
const gl = (document as any).__gl;

export function pickImg(callBack: (path: string) => void) {
  Kooboo.Media.getList().then(function(res: any) {
    if (res.success) {
      res.model["show"] = true;
      res.model["onAdd"] = (selected: any) => {
        callBack(selected.url);
      };
    }
    mediaDialogData(res.model);
  });
}

export async function editHtmlBlock(nameOrId: string) {
  let url = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DialogPage, {
    nameOrId: nameOrId
  });
  let iframe = createIframe(url);
  iframe.style.height = "600px";
  const { modal, setOkHandler, close } = createModal(
    TEXT.EDIT_HTML_BLOCK,
    iframe.outerHTML
  );
  parentBody.appendChild(modal);
  return new Promise<string>(rs => {
    setOkHandler(async () => {
      gl.saveHtmlblockFinish = (content: string) => {
        rs(content);
        close();
      };
      gl.saveHtmlblock();
    });
  });
}

export function editRepeat(nameOrId: string, folderId: string) {
  let url = Kooboo.Route.Get(Kooboo.Route.TextContent.DialogPage, {
    id: nameOrId,
    folder: folderId
  });

  let iframe = createIframe(url);
  iframe.style.height = "600px";
  const { modal, setOkHandler, close } = createModal(
    TEXT.EDIT_REPEAT,
    iframe.outerHTML
  );
  parentBody.appendChild(modal);
  return new Promise<any>(rs => {
    setOkHandler(() => {
      gl.saveContentFinish = (content: any) => {
        rs(content);
        close();
      };
      gl.saveContent();
    });
  });
}

export function editMenu(
  nameOrId: string,
  callback: (content: string) => void
) {
  var url = Kooboo.Route.Get(Kooboo.Route.Menu.DialogPage, {
    nameOrId: nameOrId
  });
  let iframe = createIframe(url);
  iframe.style.height = "600px";
  const { modal, hideCancel } = createModal(TEXT.EDIT_MENU, iframe.outerHTML);
  hideCancel();
  gl.saveMenuFinish = callback;
  parentBody.appendChild(modal);
}

export async function getPageUrls() {
  return new Promise<string[]>((rs, rj) => {
    Kooboo.Link.SyncAll().then((data: any) => {
      rs(data.model.pages.map((m: any) => m.url));
    });
  });
}
