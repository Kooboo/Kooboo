export class KoobooId {
  constructor(public id: string) {}
  get value() {
    return Number(this.getArr().pop());
  }

  get next() {
    var arr = this.getArr();
    var next = Number(arr.pop()) + 1;
    arr.push(next.toString());
    return arr.join("-");
  }

  private getArr() {
    return this.id.split("-");
  }
}
