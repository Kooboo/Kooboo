export class kvInfo {
  constructor(public key: string, public value: string | null) {}

  static koobooId(value: string | null) {
    return new kvInfo("koobooid", value);
  }

  static value(value: string | null) {
    return new kvInfo("value", value);
  }
}
