export class BaseEvent<T> {
  handlers: Array<(e: T) => void> = [];
  addEventListener(handler: (e: T) => void) {
    this.handlers.push(handler);
  }
  emit(e: T) {
    this.handlers.forEach(i => i(e));
  }
}
