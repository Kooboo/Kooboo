import type { SiteItem as SiteItemAPI } from "@/api/site";
import type { KeyValue } from "@/global/types";

export type SiteItem = SiteItemAPI & {
  selected?: boolean;
  isHover?: boolean;
};

export type FolderItem = { key: string; value: number } & {
  visibleMore?: boolean;
};
