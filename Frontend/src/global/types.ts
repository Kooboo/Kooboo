import type { RouteLocationNormalizedLoaded, RouteParamsRaw } from "vue-router";

import type { WritableComputedRef } from "vue";

export interface IAnyObject {
  [propName: string]: any;
}

export interface KeyValue {
  key: string;
  value: string;
}

export type OptionItemType = KeyValue & { options?: string[] };

export interface SortEvent {
  newIndex: number;
  oldIndex: number;
}

export type SortMovedEvent = {
  element: any;
  oldIndex: number;
  newIndex: number;
};

export type SortAddedEvent = {
  newIndex: number;
  element: any;
};

export type SortRemovedEvent = {
  oldIndex: number;
  element: any;
};

export type SortChangeEvent = {
  added?: SortAddedEvent;
  removed?: SortRemovedEvent;
  moved?: SortMovedEvent;
};

export interface Pagination {
  pageNr: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PaginationResponse<T> extends Pagination {
  list: T[];
}

export interface PaginationWithColumnsResponse<TRow, TColumn>
  extends Pagination {
  columns: TColumn[];
  list: TRow[];
}

export interface InfiniteResponse<T> {
  list: T[];
  pageNr: number;
  pageSize: number;
  hasMore: boolean;
}

export interface Resource {
  el?: Element;
  content: string;
  id: string;
  position?: "head" | "body";
}

export interface Conflict {
  version: number;
  body: string;
}

export interface DomValueWrapper extends WritableComputedRef<any> {
  origin: any;
  name?: string;
}

export type SortSetting<TRow> = {
  prop: keyof TRow | null;
  order: "ascending" | "descending" | null;
};

export interface Placeholder {
  name: string;
  innerHtml: string;
  addons: Addon[];
}

export interface Addon {
  id: string;
  type: string;
  attributes: Record<string, string>;
  content?: string | Placeholder[];
}

export type MenuQueryBuilder = (
  menu: Menu,
  route: RouteLocationNormalizedLoaded
) => any;

export interface Menu {
  display: string;
  items: Menu[];
  name?: string;
  id: string;
  permission?: { feature: string; action?: string };
  advanced?: unknown;
  icon?: unknown;
  queryBuilder?: MenuQueryBuilder;
  routeMenuName?: string;
  params?: RouteParamsRaw;
}

export type AttributesBuilderType<T extends KeyValue> = (
  item: T,
  index: number,
  itemList: T[]
) => Record<string, any>;

export type RichEditorMenuItem = {
  type: "menuitem";
  text: string;
  onAction: () => void;
};

export type RichEditorAddMenuButtonOptions = {
  text: string;
  fetch: (callback: (items: RichEditorMenuItem[]) => void) => void;
};

export type RichEditorInstance = {
  insertContent: (content: string) => void;
  ui: {
    registry: {
      addMenuButton: (
        name: string,
        options: RichEditorAddMenuButtonOptions
      ) => void;
    };
  };
};
