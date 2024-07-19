export interface Position {
  x: number;
  y: number;
}

export interface State {
  event?: Event;
  element?: HTMLElement;
  hoverElement?: HTMLElement;
  clickPosition?: Position;
  editing?: boolean;
}
