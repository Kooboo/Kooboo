const Kooboo = (document as any).Kooboo;
const mediaDialogData = (document as any).mediaDialogData;

export function pickImg(callBack: Function) {
  Kooboo.Media.getList().then(function(res: any) {
    if (res.success) {
      res.model["show"] = true;
      res.model["onAdd"] = callBack;
    }
    mediaDialogData(res.model);
  });
}
