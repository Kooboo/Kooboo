export interface TableColumn {
  name: string;
  label?: string;
  width?: string | number;
  hidden?: boolean;
}

export type Size = "default" | "small" | "large";
export type Format = "json" | "string";
