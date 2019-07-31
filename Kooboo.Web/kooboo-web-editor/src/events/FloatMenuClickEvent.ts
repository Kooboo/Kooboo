import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  expand,
  edit,
  copy,
  delete,
  editImage,
  editLink,
  enterLink,
  editHtmlBlock,
  editRepeat,
  editMenu,
  editStyle,
  copyRepeat,
  deleteRepeat,
  replaceToImg,
  replaceToText,
  click,
  editForm,
  convert,
  convertToHtmlBlock,
  convertToView
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
