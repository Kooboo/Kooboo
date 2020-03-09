import { KOOBOO_GUID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";
import { getAllNode } from "@/dom/utils";

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
    return this.getValue("source")!;
  }

  get uid() {
    return this.getValue("uid")!;
  }

  get id() {
    return this.getValue("id")!;
  }

  get path() {
    return this.getValue("path")!;
  }

  get attribute() {
    return this.getValue("attribute")!;
  }

  get scope() {
    return this.getValue("scope")!;
  }

  getValue(key: string) {
    var item = this.infos.find(f => f.key == key);
    if (item) return item.value;
  }

  setValue(key: string, value: string) {
    var item = this.infos.find(f => f.key == key);
    if (item) return (item.value = value);
  }

  ToComment() {
    return new Comment(`#kooboo--${this.infos.map(m => `${m.key}=${m.value}`).join("--")}`);
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
      comments.push(...this.getAroundComments(node, false));
      node = node.parentElement!;
    }

    return comments;
  }

  static getAroundComments(node: Node, strict: boolean = true) {
    let comments: KoobooComment[] = [];
    let skipUid: string[] = [];

    while (node) {
      if (KoobooComment.isComment(node)) {
        var comment = new KoobooComment(node);
        if (KoobooComment.isEndComment(node)) {
          skipUid.push(comment.uid);
        } else if (skipUid.every(e => e != comment.uid)) {
          if (!strict || (!comment.scope && !comment.source.startsWith("repeat"))) {
            comments.push(new KoobooComment(node));
          }
        }
      }
      node = node.previousSibling!;
      if (strict && node instanceof HTMLElement) break;
    }

    return comments;
  }

  static getAroundScopeComments(el: HTMLElement) {
    let aroundComments = KoobooComment.getAroundComments(el, false);
    return aroundComments.find(f => f.scope);
  }

  static getInnerComments(nodes: Node[]) {
    const comments = [];

    for (const node of nodes) {
      for (const i of getAllNode(node, true)) {
        if (this.isComment(i) && !this.isEndComment(i)) {
          comments.push(new KoobooComment(i));
        }
      }
    }

    return comments;
  }
}
