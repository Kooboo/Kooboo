export class KoobooComment {
  private _infos!: string[];
  private _comment!: string;

  /**
   * dom kooboo info
   */
  constructor(comment: string | null) {
    if (comment) {
      this._comment = comment;
      this._infos = comment.split("--").map(m => m.replace(/'/g, ""));
    }
  }

  get objecttype() {
    return this.getValue("objecttype");
  }

  get nameorid() {
    return this.getValue("nameorid");
  }

  get folderid() {
    return this.getValue("folderid");
  }

  get bindingvalue() {
    return this.getValue("bindingvalue");
  }
  get boundary() {
    return this.getValue("boundary");
  }
  get fieldname() {
    return this.getValue("fieldname");
  }
  get koobooid() {
    return this.getValue("koobooid");
  }

  get attributename() {
    return this.getValue("attributename");
  }

  private getValue(key: string) {
    var item = this._infos.find(f => f.startsWith(key));
    if (item && item.indexOf("=") != -1) {
      return item.split("=")[1];
    }
  }
}
