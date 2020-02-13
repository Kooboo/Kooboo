import { KOOBOO_GUID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";

export class KoobooComment {
  infos!: kvInfo[];

  constructor(comment: string | Node | null) {
    let str = "";

    if (comment instanceof Node) {
      str = comment.nodeValue!;
    } else if (comment) {
      str = comment;
    }

    if (comment) {
      this.infos = str
        .split("--")
        .map(m => m.replace(/'/g, ""))
        .filter(f => f.indexOf("=") != -1)
        .map(m => {
          let arr = m.split("=");
          return new kvInfo(arr[0], arr[1]);
        });
    }
  }

  get source() {
    return this.getValue("source");
  }

  get uid() {
    return this.getValue("uid");
  }

  getValue(key: string) {
    var item = this.infos.find(f => f.key == key);
    if (item) return item.value;
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

  static getComments(node: Node) {
    let comments: KoobooComment[] = [];

    while (node) {
      comments.push(...this.getAroundComments(node));
      node = node.parentElement!;
    }

    return comments;
  }

  static getAroundComments(node: Node) {
    let comments: KoobooComment[] = [];

    while (node && !(node instanceof HTMLElement)) {
      if (KoobooComment.isComment(node)) comments.push(new KoobooComment(node));
      node = node.previousSibling!;
    }

    return comments;
  }
}
