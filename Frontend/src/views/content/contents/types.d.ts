import type { TextContentItem } from "@/api/content/textContent";

export type TableRowItem = Pick<
  TextContentItem,
  "id" | "online" | "lastModified" | "usedBy" | "order"
> &
  Record<string, any>;
