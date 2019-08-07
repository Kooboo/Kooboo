import EditItem from "./EditItem";
import { createDiv } from "@/dom/element";
import { getMatchedColors } from "@/dom/style";

export default class EditElement {
  constructor(public element: HTMLElement) {
    let style = getComputedStyle(element);
    let matchedColors = getMatchedColors(element);

    this.editItems = new Array<EditItem>();
    this.editItems.push(new EditItem("颜色", style.color!, c => (element.style.color = c)));
    this.editItems.push(new EditItem("背景颜色", style.backgroundColor!, c => (element.style.backgroundColor = c)));
    this.editItems.push(new EditItem("颜色：hover", "#ffffff"));
    this.editItems.push(new EditItem("背景颜色：hover", "#ffffff"));
    this.editItems.push(new EditItem("颜色：focus", "#ffffff"));
    this.editItems.push(new EditItem("背景颜色：focus", "#ffffff"));
    this.editItems.push(new EditItem("颜色：active", "#ffffff"));
    this.editItems.push(new EditItem("背景颜色：active", "#ffffff"));
  }

  editItems: Array<EditItem>;

  // 返回类的Html元素
  render() {
    let itemContainer = createDiv();
    this.editItems.forEach(item => itemContainer.appendChild(item.render()));

    return itemContainer;
  }
}
