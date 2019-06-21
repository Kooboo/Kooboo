import { createModal } from "@/components/modal";
import { createIframe } from "@/dom/utils";
const Kooboo = (document as any).Kooboo;
const mediaDialogData = (document as any).mediaDialogData;
const parentBody = (document as any).parentBody as HTMLBodyElement;

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

export function editHtmlBlock(nameOrId: string, save: () => {}) {
  let url = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DialogPage, {
    nameOrId: nameOrId
  });

  let iframe = createIframe(url);
  let modal = createModal({
    body: iframe,
    title: "编辑html"
  });
  parentBody.appendChild(modal);
}
