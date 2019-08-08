import EditItem from "./EditItem";
import { createDiv } from "@/dom/element";
import { getMatchedColorGroups, CssColorGroup } from "@/dom/style";

export default class EditElement {
  constructor(public element: HTMLElement) {
    let groups = getMatchedColorGroups(element);

    this.editItems = new Array<EditItem>();
    this.noPseudo = new PseudoItemList("", "", true);
    this.editItems = this.editItems.concat(this.noPseudo.createEditItems(groups, element));

    this.hoverPseudo = new PseudoItemList(":hover", ":hover");
    this.editItems = this.editItems.concat(this.hoverPseudo.createEditItems(groups, element));

    this.focusPseudo = new PseudoItemList(":focus", ":focus");
    this.editItems = this.editItems.concat(this.focusPseudo.createEditItems(groups, element));

    this.activePseudo = new PseudoItemList(":active", ":active");
    this.editItems = this.editItems.concat(this.activePseudo.createEditItems(groups, element));
  }

  noPseudo: PseudoItemList;

  hoverPseudo: PseudoItemList;

  focusPseudo: PseudoItemList;

  activePseudo: PseudoItemList;

  editItems: Array<EditItem>;

  // 点击取消
  cancel() {
    this.editItems.forEach(item => item.CancelChange());
  }

  // 点击ok
  ok() {}

  // 返回类的Html元素
  render() {
    let itemContainer = createDiv();
    this.editItems.forEach(item => itemContainer.appendChild(item.render()));

    return itemContainer;
  }
}

class PseudoItemList {
  constructor(
    protected pseudo: string,
    protected pseudoName: string,
    protected isNoExistShow: boolean = false // 不存在对应的CssColorGroup时，是否依然返回该编辑项（只作用于颜色和背景颜色）
  ) {}

  createColor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "color" && item.pseudo == this.pseudo);
    if (group == undefined && this.isNoExistShow == false) {
      return null;
    }

    let cssColors = group == undefined ? [] : group.cssColors;

    let editItem = new EditItem(`颜色${this.pseudoName}`, element, cssColors, "color");
    return editItem;
  }

  createBackgroupColor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "background" && item.pseudo == this.pseudo);
    if (group == undefined && this.isNoExistShow == false) {
      return null;
    }

    let cssColors = group == undefined ? [] : group.cssColors;

    let editItem = new EditItem(`背景颜色${this.pseudoName}`, element, cssColors, "background-color");
    return editItem;
  }

  createBoxShadow(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "box-shadow" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return null;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`盒子阴影${this.pseudoName}`, element, cssColors, "box-shadow");
    return editItem;
  }

  createBodercolor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "border" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return null;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`边框颜色${this.pseudoName}`, element, cssColors, "boder-color");
    return editItem;
  }

  createOutlinecolor(groups: CssColorGroup[], element: HTMLElement) {
    let group = groups.find(item => item.prop == "outline" && item.pseudo == this.pseudo);
    if (group == undefined) {
      return null;
    }

    let cssColors = group.cssColors;

    let editItem = new EditItem(`Outline${this.pseudoName}`, element, cssColors, "outline-color");
    return editItem;
  }

  createEditItems(groups: CssColorGroup[], element: HTMLElement) {
    let currentGroups = groups.filter(item => item.pseudo == this.pseudo);
    let editItems: EditItem[] = [];

    let color = this.createColor(currentGroups, element);
    if (color != null) {
      editItems.push(color);
    }

    let backgroupColor = this.createBackgroupColor(currentGroups, element);
    if (backgroupColor != null) {
      editItems.push(backgroupColor);
    }

    let boxShadow = this.createBoxShadow(currentGroups, element);
    if (boxShadow != null) {
      editItems.push(boxShadow);
    }

    let bodercolor = this.createBodercolor(currentGroups, element);
    if (bodercolor != null) {
      editItems.push(bodercolor);
    }

    let outlinecolor = this.createOutlinecolor(currentGroups, element);
    if (outlinecolor != null) {
      editItems.push(outlinecolor);
    }

    return editItems;
  }
}
