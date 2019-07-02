export enum Type {
  innerHtml,
  copy
}

export abstract class Unit {
  newValue!: string;

  constructor(public oldValue: string) {}

  abstract undo(node: Node): void;

  abstract redo(node: Node): void;
}
