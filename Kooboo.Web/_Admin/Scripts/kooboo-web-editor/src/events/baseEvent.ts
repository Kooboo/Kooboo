export class BaseEvent<T> {
  constructor(name: string) {
    this.name = name;
  }
  handlers: Array<(e: T) => void> = [];
  name: string | null = null;
  addEventListener(handler: (e: T) => void) {
    this.handlers.push(handler);
  }
  emit(e: T) {
    this.handlers.forEach(i => i(e));
  }
}
