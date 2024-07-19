export interface Schema {
  name: string;
  display: string;
  type: Type;
  arrayType?: ArrayType;
  children: Schema[];
}
export type ArrayType = "string" | "number" | "boolean" | "object";
export type Type = ArrayType | "array";

export function getDefaultValue(type: string) {
  switch (type) {
    case "string":
      return "";
    case "array":
      return [];
    case "boolean":
      return false;
    case "number":
      return 0;
    case "object":
      return {};
    default:
      break;
  }
}
