export interface Position {
  top?: number;
  left?: number;
  bottom?: number;
  right?: number;
  width?: number;
  height?: number;
}

export const updatePositon = (positon: Position, el: HTMLElement) => {
  const { top, left, height, width, bottom, right } = positon;
  if (top != undefined) el.style.top = top + "px";
  if (left != undefined) el.style.left = left + "px";
  if (bottom != undefined) el.style.bottom = bottom + "px";
  if (right != undefined) el.style.right = right + "px";
  if (width != undefined) el.style.width = width + "px";
  if (height != undefined) el.style.height = height + "px";
};
