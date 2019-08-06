// import { MenuItem, createItem } from "../../basic";
// import { TEXT } from "@/common/lang";
// import { KoobooComment } from "@/kooboo/KoobooComment";
// import context from "@/common/context";
// import { isDynamicContent, isDirty } from "@/kooboo/utils";
// import { OBJECT_TYPE, KOOBOO_ID } from "@/common/constants";

// import "./index.css";
// import { isBody } from "@/dom/utils";
// import { createConvertToView } from "./convertToView";

// export function createConvert(): MenuItem {
//   const convertItem = createItem(TEXT.CONVERT);
//   convertItem.el.classList.add("floatmenu-convert");

//   const update = (comments: KoobooComment[]) => {
//     convertItem.setVisiable(true);
//     let args = context.lastSelectedDomEventArgs;

//     if (
//       comments.length == 0 ||
//       comments[0].objecttype != OBJECT_TYPE.page ||
//       isDynamicContent(args.element) || // 如果元素下面有其他kooboo类型
//       !args.element.getAttribute(KOOBOO_ID) ||
//       isBody(args.element) ||
//       isDirty(args.element) // 如果元素是脏的，则说明该元素已被其他项操作过
//     ) {
//       return convertItem.setVisiable(false);
//     }
//   };

//   let childsItemContainerObject = createChildMenus();
//   convertItem.el.appendChild(childsItemContainerObject.el);

//   convertItem.el.addEventListener("mouseover", () => {
//     childsItemContainerObject.show();
//   });
//   convertItem.el.addEventListener("mouseout", () => {
//     childsItemContainerObject.hidden();
//   });

//   return { el: convertItem.el, update };
// }

// function createChildMenus() {
//   let childsItemContainer = document.createElement("div");

//   let div = document.createElement("div");

//   let htmlBlockItem = createConvertToHtmlBlock();
//   div.appendChild(htmlBlockItem.el);

//   let viewItem = createConvertToView();
//   div.appendChild(viewItem.el);

//   childsItemContainer.appendChild(div);

//   childsItemContainer.classList.add("floatmenu-convert-childscontainer");
//   let show = () => {
//     childsItemContainer.classList.add("floatmenu-convert-childscontainer-hover");

//     let rect = div.getBoundingClientRect();
//     // 如果元素最右边的位置大于body宽度
//     if (rect.right > document.body.clientWidth) {
//       childsItemContainer.classList.add("floatmenu-convert-childscontainer-floatleft");
//     }
//   };
//   let hidden = () => {
//     childsItemContainer.classList.remove("floatmenu-convert-childscontainer-hover");
//     childsItemContainer.classList.remove("floatmenu-convert-childscontainer-floatleft");
//   };

//   return { el: childsItemContainer, show, hidden };
// }

// function createConvertToHtmlBlock() {
//   const item = createItem(`${TEXT.CONVERTTOHTMLBLOCK}`);

//   item.el.addEventListener("click", () => {});

//   return item;
// }
