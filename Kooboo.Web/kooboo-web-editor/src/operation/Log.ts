import { kvInfo } from "@/common/kvInfo";

export class Log {
  source: string;
  infos: kvInfo[];

  /**
   *
   */
  constructor(infos: kvInfo[]) {
    this.infos = infos;
    this.source = infos.find(f => f.key == "source")!.value!;
  }

  getValue(key: string) {
    var info = this.infos.find(f => f.key == key);
    if (info) return info.value;
  }

  get koobooId() {
    return this.getValue("koobooid");
  }

  get id() {
    return this.getValue("id");
  }

  get attribute() {
    return this.getValue("attribute");
  }

  get url() {
    return this.getValue("url");
  }

  get selector() {
    return this.getValue("selector");
  }

  get property() {
    return this.getValue("property");
  }

  get action() {
    return this.getValue("action");
  }

  get mediaRuleList() {
    return this.getValue("mediarulelist");
  }

  static simplify(logs: Log[]) {
    let deleted: Log[] = [];

    for (const log of logs.reverse()) {
      if (!log.koobooId) continue;
      var index = logs.indexOf(log);
      var children = logs.filter(
        f =>
          logs.indexOf(f) > index &&
          f.source == log.source &&
          f.id == log.id &&
          f.koobooId &&
          f.koobooId!.startsWith(log.koobooId!) &&
          f.attribute == log.attribute &&
          f.property == log.property
      );

      if (log.action == "copy") children = [];

      if (log.action == "delete") {
        children = children.filter(f => f.koobooId!.length > log.koobooId!.length);
      }

      if (log.attribute || log.property) {
        children = children.filter(f => f.koobooId == log.koobooId);
      }

      if (log.url) {
        children = children.filter(f => f.url == log.url);
      }

      if (log.selector) {
        children = children.filter(f => f.selector == log.selector);
      }

      if (log.mediaRuleList) {
        children = children.filter(f => f.mediaRuleList == log.mediaRuleList);
      }

      for (const log of children) {
        if (deleted.indexOf(log) == -1) deleted.push(log);
      }
    }

    return logs.filter(f => deleted.indexOf(f) == -1).reverse();
  }
}
