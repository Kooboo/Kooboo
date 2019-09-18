import { TEXT } from "@/common/lang";
import context from "@/common/context";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getViewComment } from "../utils";
import { createColorEditor } from "@/components/colorEditor";
import { isDirty } from "@/kooboo/utils";

export default class EditColorItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = this.createItem(TEXT.EDIT_COLOR);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
    this.setReadonly = setReadonly;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  setReadonly: (readonly: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    this.setReadonly(isDirty(args.element));
    if (!getViewComment(comments)) return this.setVisiable(false);
    if (!args.koobooId) return this.setVisiable(false);
  }

  async click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    createColorEditor(args.element, getViewComment(comments)!, args.koobooId!);
    this.parentMenu.hidden();
  }
}

// async function useEditElementEditColor(args: SelectedDomEventArgs){
//   let editElement = await createColorEditor(args.element);

//     let guid = setGuid(args.element);
//     let colorEditUnit = new ColorEditUnit("", editElement);

//     let comments = KoobooComment.getComments(args.element);
//     let comment = getViewComment(comments)!;
//     let nameorid = comment.nameorid!;
//     let objectType = comment.objecttype!;
//     let koobooId = args.koobooId!;
//     let logs: Log[] = [];
//     editElement.getEditItems().forEach(item=>{
//       if(item.localEditData != null){
//         let colorEditLog = new ColorEditLog();
//         colorEditLog.nameOrId = nameorid;
//         colorEditLog.objectType = objectType;
//         colorEditLog.KoobooId = koobooId;
//         colorEditLog.property = item.localEditData.propName;
//         colorEditLog.value = item.localEditData.value;
//         colorEditLog.pseudo = "";
//         colorEditLog.selector = null;

//         logs.push(colorEditLog);
//       }

//       if(item.globalEditData != null){
//         let colorEditLog = new ColorEditLog();
//         colorEditLog.nameOrId = nameorid;
//         colorEditLog.objectType = objectType;
//         // colorEditLog.KoobooId = koobooId;    // 全局更新不能全koobooid不然服务器会以为局部更新
//         colorEditLog.property = item.globalEditData.propName;
//         colorEditLog.value = item.globalEditData.value;
//         colorEditLog.pseudo = item.globalEditData.pseudo;
//         colorEditLog.selector = item.globalEditData.selector == "" ? null : item.globalEditData.selector;
//         colorEditLog.styleTagKoobooId = item.globalEditData.styleTagKoobooId;
//         colorEditLog.styleSheetUrl = item.globalEditData.styleSheetUrl;

//         logs.push(colorEditLog);
//       }
//     });

//     console.log(logs);

//     let record = new operationRecord([colorEditUnit], logs, guid);
//     context.operationManager.add(record);
// }
