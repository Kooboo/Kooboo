// import { createItem } from "../../basic";
// import { TEXT } from "@/common/lang";
// import context from "@/common/context";
// import { setGuid, clearKoobooInfo } from "@/kooboo/utils";
// import { KOOBOO_ID } from "@/common/constants";
// import { ConverterLog } from "@/operation/recordLogs/ConverterLog";
// import { ConvertUnit } from "@/operation/recordUnits/ConvertUnit";
// import { operationRecord } from "@/operation/Record";
// import { addDisableEditShade, removeDisableEditShade } from "./disableEditShade";
// import { setInlineEditor } from "@/components/richEditor";

// export function createConvertToView() {
//   const item = createItem(`${TEXT.CONVERTTOVIEW}`);

//   item.el.addEventListener("click", convertToViewClick);

//   return item;
// }

// function convertToViewClick() {
//   let { element } = context.lastSelectedDomEventArgs;

//   let convertEditor = new ConvertEditor(element);
//   convertEditor.setInlineEditor();
// }

// class ConvertEditor {
//   constructor(public element: HTMLElement) {
//     this.initHtml = element.outerHTML;
//     this.oldHtml = element.outerHTML;
//   }

//   private initHtml: string;

//   private oldHtml: string;

//   private log: ConverterLog | null = null;

//   setLog() {
//     let viewNameorid = `${new Date().valueOf()}`; // 以时间戳作为naneorid

//     // 生成log
//     let convertResult = {
//       convertToType: "View",
//       name: viewNameorid,
//       koobooId: this.element.getAttribute(KOOBOO_ID),
//       htmlBody: clearKoobooInfo(this.element.outerHTML)
//     };

//     this.log = ConverterLog.create(JSON.stringify(convertResult));
//   }

//   onSave() {
//     let guid = setGuid(this.element);

//     this.setLog();

//     // 生成unit
//     let units = [
//       ConvertUnit.CreateViewConvertUnit(this.initHtml, el => {
//         removeDisableEditShade(el);
//         this.addShade(el);
//       })
//     ];

//     // 为元素添加禁止编辑遮罩
//     this.addShade(this.element);

//     let operation = new operationRecord(units, [this.log!], guid);
//     context.operationManager.add(operation);
//   }

//   onCancel() {
//     let div = document.createElement("div");
//     div.innerHTML = this.oldHtml;
//     this.element.parentElement!.replaceChild(div.children[0], this.element);
//   }

//   addShade(element: HTMLElement) {
//     let oldHtml = element.outerHTML;
//     const onSave = () => {
//       // 更新convertResult的值
//       this.log!.convertResult = clearKoobooInfo(element.outerHTML);
//       this.addShade(element);
//       console.log(this.log!);
//     };

//     const onCancel = () => {
//       let div = document.createElement("div");
//       div.innerHTML = oldHtml;
//       element.parentElement!.replaceChild(div.children[0], element);
//       this.addShade(div.children[0] as HTMLElement);
//     };

//     addDisableEditShade(element, e => {
//       removeDisableEditShade(element);
//       setInlineEditor({ selector: element, onSave, onCancel });
//     });
//   }

//   setInlineEditor() {
//     setInlineEditor({ selector: this.element, onSave: this.onSave, onCancel: this.onCancel });
//   }
// }
