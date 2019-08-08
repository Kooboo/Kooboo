import EditItem from "./EditItem";
import { createDiv } from "@/dom/element";
import { getMatchedColorGroups, CssColorGroup } from "@/dom/style";

export default class EditElement {
  constructor(public element: HTMLElement) {
    this.noPseudo = new PseudoItemList("", "", element, true);

    this.hoverPseudo = new PseudoItemList(":hover", ":hover", element);

    this.focusPseudo = new PseudoItemList(":focus", ":focus", element);

    this.activePseudo = new PseudoItemList(":active", ":active", element);

    this.visitedPseudo = new PseudoItemList(":visited", ":visited", element);
  }

  noPseudo: PseudoItemList;

  hoverPseudo: PseudoItemList;

  focusPseudo: PseudoItemList;

  activePseudo: PseudoItemList;

  visitedPseudo: PseudoItemList;

  getEditItems(){
    let editItems:EditItem[]  = [];
    editItems = editItems.concat(this.noPseudo.editItems);
    editItems = editItems.concat(this.hoverPseudo.editItems);
    editItems = editItems.concat(this.focusPseudo.editItems);
    editItems = editItems.concat(this.activePseudo.editItems);
    editItems = editItems.concat(this.visitedPseudo.editItems);

    return editItems;
  }

  // 重新设置元素（在撤销和重做中，元素不在是那个元素）
  resetElement(element: HTMLElement){
    this.element = element;
    this.getEditItems().forEach(item => item.resetElement(element));
  }

  // 点击取消
  cancel() {
    this.getEditItems().forEach(item => item.CancelChange());
  }

  // 点击ok
  ok() {
    this.getEditItems().forEach(item => item.setChange());
  }

  // 返回类的Html元素
  render() {
    let itemContainer = createDiv();
    this.getEditItems().forEach(item => itemContainer.appendChild(item.render()));

    return itemContainer;
  }
}

class PseudoItemList {
  constructor(
    protected pseudo: string,
    protected pseudoName: string,
    protected element: HTMLElement,
    protected isNoExistShow: boolean = false // 不存在对应的CssColorGroup时，是否依然返回该编辑项（只作用于颜色和背景颜色）
  ) {
    let groups = getMatchedColorGroups(element);
    let currentGroups = groups.filter(item => item.pseudo == this.pseudo);

    this.createColor(currentGroups, element);
    this.createBackgroupColor(currentGroups, element);
    this.createBoxShadow(currentGroups, element);
    this.createBodercolor(currentGroups, element);
    this.createOutlinecolor(currentGroups, element);
  }

  editItems: Array<EditItem> = [];

  createColor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "color" && item.pseudo == this.pseudo);
    if (group == undefined && this.isNoExistShow == false) {
      return;
    }

    let cssColors = group == undefined ? [] : group.cssColors;

    let editItem = new EditItem(`颜色${this.pseudoName}`, element, cssColors, "color");
    this.editItems.push(editItem);
  }

  createBackgroupColor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "background" && item.pseudo == this.pseudo);
    if (group == undefined && this.isNoExistShow == false) {
      return;
    }

    let cssColors = group == undefined ? [] : group.cssColors;

    let editItem = new EditItem(`背景颜色${this.pseudoName}`, element, cssColors, "background-color");
    this.editItems.push(editItem);
  }

  createBoxShadow(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "box-shadow" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`盒子阴影${this.pseudoName}`, element, cssColors, "box-shadow");
    this.editItems.push(editItem);
  }

  createBodercolor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "border" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`边框颜色${this.pseudoName}`, element, cssColors, "boder-color");
    this.editItems.push(editItem);
  }

  createOutlinecolor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "outline" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`Outline${this.pseudoName}`, element, cssColors, "outline-color");
    this.editItems.push(editItem);
  }
}
