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

export function pickLink(callBack: (path: string) => void, oldValue: string) {
  Kooboo.plugins.EditLink.dialogSetting.beforeSave = function() {
    var url = Kooboo.plugins.EditLink.getLinkUrl();
    if (callBack) {
      callBack(url);
    }
  };
  Kooboo.PluginManager.click(Kooboo.plugins.EditLink, {});
  if (oldValue) {
    Kooboo.plugins.EditLink.setLinkUrl(oldValue);
  } else {
    Kooboo.plugins.EditLink.setLinkUrl("");
  }
}

export function editHtmlBlock(
  nameOrId: string,
  saveCallback: (result: string) => void
) {
  let url = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DialogPage, {
    nameOrId: nameOrId
  });
  let iframe = createIframe(url);
  iframe.style.height = "600px";
  const { modal, setOkHandler } = createModal(
    TEXT.EDIT_HTML_BLOCK,
    iframe.outerHTML
  );
  setOkHandler(
    () =>
      new Promise((rs, rj) => {
        gl.saveHtmlblockFinish = (content: string) => {
          saveCallback(content);
          rs();
        };
        gl.saveHtmlblock();
      })
  );

  parentBody.appendChild(modal);
}

export async function getPageUrls() {
  return new Promise<string[]>((rs, rj) => {
    Kooboo.Link.SyncAll().then((data: any) => {
      rs(data.model.pages.map((m: any) => m.url));
    });
  });
}
