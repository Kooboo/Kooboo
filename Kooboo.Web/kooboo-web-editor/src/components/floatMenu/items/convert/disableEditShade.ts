// const shadeAttrName = "data-convert-shade";

// export function addDisableEditShade(element: HTMLElement, editBtnClick: (e: MouseEvent) => void) {
//   let shade = document.createElement("div");
//   shade.classList.add("floatmenu-convert-shade");
//   shade.setAttribute(shadeAttrName, "true");
//   shade.style.width = `${element.offsetWidth}px`;
//   shade.style.height = `${element.offsetHeight}px`;

//   // 拦截点击事件
//   shade.onclick = e => {
//     e = e || window.event;
//     e.cancelBubble = true;
//     e.stopPropagation();
//     e.preventDefault();
//     return false;
//   };

//   shade.innerHTML = `
//     <div class="floatmenu-convert-shade-btns">
//     </div>`;

//   let editBtn = document.createElement("button");
//   editBtn.onclick = editBtnClick;
//   editBtn.innerHTML = `编辑`;
//   shade.children[0].append(editBtn);

//   element.prepend(shade);
// }

// export function removeDisableEditShade(element: Element) {
//   for (let n = 0; n < element.children.length; n++) {
//     if (element.children[n].getAttribute(shadeAttrName) != null) {
//       element.children[n].remove();
//       break;
//     }
//   }
// }
