import { previousComment, isBody } from "@/dom/utils";
import { isSingleCommentWrap } from "./utils";

export class KoobooComment {
  private _infos!: string[];

  /**
   * dom kooboo info
   */
  constructor(comment: string | Node | null) {
    let str = "";

    if (comment instanceof Node) {
      str = comment.nodeValue!;
    } else if (comment) {
      str = comment;
    }

    if (comment) {
      this._infos = str.split("--").map(m => m.replace(/'/g, ""));
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
    let result = this.getValue("fieldname");
    if (!result && this.bindingvalue && this.bindingvalue.indexOf(".") > -1) {
      result = this.bindingvalue.split(".").pop();
      result = result!.replace("}", "");
    }
    return result;
  }
  get koobooid() {
    return this.getValue("koobooid");
  }

  get attributename() {
    return this.getValue("attributename");
  }

  get end() {
    return Boolean(this.getValue("end"));
  }

  private getValue(key: string) {
    var item = this._infos.find(f => f.startsWith(key));
    if (item && item.indexOf("=") != -1) {
      return item.split("=")[1];
    }
  }

  static isComment(node: Node) {
    return node.nodeType == Node.COMMENT_NODE && node.nodeValue && node.nodeValue.startsWith("#kooboo");
  }

  static getComments(el: HTMLElement) {
    let comments: Comment[] = [];
    let comment = previousComment(el);
    while (comment && this.isComment(comment) && isSingleCommentWrap(comment)) {
      comments.push(comment);
      comment = previousComment(comment);
    }

    do {
      comment = previousComment(el);
      if (comment && this.isComment(comment) && !isSingleCommentWrap(comment)) {
        comments.push(comment);
      }
      if (!el.parentElement) break;
      el = el.parentElement;
    } while (el);

    return comments.map(m => new KoobooComment(m));
  }
}
