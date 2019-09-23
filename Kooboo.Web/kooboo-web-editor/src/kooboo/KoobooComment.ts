import { isSingleCommentWrap, previousComment, getWrapDom } from "./utils";
import { getAllNode } from "@/dom/utils";
import { KOOBOO_GUID } from "@/common/constants";

export class KoobooComment {
  private _infos!: string[];

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

  static isEndComment(node: Node) {
    return this.isComment(node) && node.nodeValue!.indexOf("--end") > -1;
  }

  static isGuidComment(node: Node) {
    return this.isComment(node) && node.nodeValue!.indexOf(KOOBOO_GUID) > -1;
  }

  static getComments(el: Element) {
    let self = el;
    let comments: Comment[] = this.getSingleWrapComment(el);
    let comment: Comment | undefined;

    while (el) {
      comment = previousComment(el);
      while (comment) {
        if (this.isEndComment(comment) || this.isGuidComment(comment)) break;
        if (!isSingleCommentWrap(comment) && this.isInWrap(comment, self)) {
          comments.push(comment);
        }
        comment = previousComment(comment);
      }
      el = el.previousElementSibling ? el.previousElementSibling : el.parentElement!;
    }

    return comments.map(m => new KoobooComment(m));
  }

  private static getSingleWrapComment(el: Element) {
    let comments: Comment[] = [];
    let comment: Comment | undefined;
    comment = previousComment(el);
    while (comment && isSingleCommentWrap(comment)) {
      comments.push(comment);
      comment = previousComment(comment);
    }
    return comments;
  }

  private static isInWrap(comment: Comment, self: Element) {
    let koobooComment = new KoobooComment(comment);
    let { nodes } = getWrapDom(comment, koobooComment.objecttype!);
    return nodes.some(s => {
      for (const i of getAllNode(s, true)) {
        if (i == self) return true;
      }
    });
  }
}
