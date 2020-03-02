export class kvInfo {
  constructor(public key: string, public value: string | null) {}

  static koobooId(value: string | null) {
    return new kvInfo("koobooid", value);
  }

  static attribute(value: string | null) {
    return new kvInfo("attribute", value);
  }

  static value(value: string | null) {
    return new kvInfo("value", value);
  }

  static property(value: string | null) {
    return new kvInfo("property", value);
  }

  static important(value: string | null) {
    return new kvInfo("important", value);
  }

  static mediaRuleList(value: string | null) {
    return new kvInfo("mediarulelist", value);
  }

  static source(value: string | null) {
    return new kvInfo("source", value);
  }

  static get delete() {
    return new kvInfo("action", "delete");
  }

  static get copy() {
    return new kvInfo("action", "copy");
  }
}
