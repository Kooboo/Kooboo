import type { KeyValue, PaginationWithColumnsResponse } from "@/global/types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type TextContentUsedByContent = {
  id: string;
  title: string;
  url: string;
};

export type TextContentUsedBy = {
  folderId: string;
  type: number; // 0=>Unknown;1=>Embedded;2=>Category
  folderName: string;
  displayName: string;
  contents: TextContentUsedByContent[];
};

export type TextContentItem = {
  contentTypeId: string;
  folderId: string;
  id: string;
  lastModified: string;
  usedBy: TextContentUsedBy[];
  online: boolean;
  order: number;
  summaryField: string;
  textValues: Record<string, any>;
};

export type TextContentColumn = Pick<
  ContentFieldItem,
  "name" | "displayName" | "controlType" | "multipleValue"
>;

export type TextContentCategoryOptionItem = {
  id: string;
  alias: string;
  display?: string;
  multipleChoice: boolean;
  options: any[];
};

export type ByFolderResponse = PaginationWithColumnsResponse<
  TextContentItem,
  TextContentColumn
> & {
  categories: TextContentCategoryOptionItem[];
};

export type MoveTextContent = {
  source: string;
  folderId: string;
  prevId?: string;
  nextId?: string;
};

type QueryParams = {
  folderId: string;
  pageNr: number;
  sortField?: string;
  ascending?: boolean;
  exclude?: string[];
};

export type ByFolderQueryParams = QueryParams & {
  pageSize?: number;
  categories?: Record<string, string[]>;
};

export type SearchQueryParams = QueryParams & {
  keyword: string;
  pageSize?: number;
  categories?: Record<string, string[]>;
};

export const getByFolder = (params: ByFolderQueryParams) =>
  request.post<ByFolderResponse>(useUrlSiteId("textContent/ByFolder"), params);

export const getByIds = (ids: string[], folderId: string) =>
  request.post<any>(useUrlSiteId("textContent/GetByIds"), {
    ids,
    folderId,
  });

export const moveContent = (changes: MoveTextContent) =>
  request.put(useUrlSiteId("textContent/Move"), { changes });

export const Search = (params: SearchQueryParams) =>
  request.post<ByFolderResponse>(useUrlSiteId("textContent/Search"), params);

export type ContentFieldItem = {
  controlType: string;
  displayName: string;
  isMultilingual: boolean;
  multipleValue: boolean;
  name: string;
  selectionOptions: string;
  toolTip: string;
  validations: string;
  settings: string;
  values: Record<string, any>;
};

export type ContentCategory = {
  alias: string;
  display: string;
  contents: TextContentItem[];
  categoryFolder: {
    id: string;
  };
  columns: any;
  multipleChoice: boolean;
};
export type ContentEmbedded = {
  alias: string;
  display: string;
  contents: TextContentItem[];
  columns: any;
  embeddedFolder: {
    id: string;
  };
};
export type EditContentResponse = {
  categories: ContentCategory[];
  embedded: ContentEmbedded[];
  properties: ContentFieldItem[];
};
export const getEditContent = (params: {
  folderId: string;
  id?: string;
  typeId?: string;
}) =>
  request.get<EditContentResponse>(useUrlSiteId("textContent/GetEdit"), params);

export const deletes = (data: { ids: string[]; parentId?: string }) =>
  request.post(useUrlSiteId("textContent/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const langupdate = (
  data: {
    id?: string;
    folderId: string;
    typeId?: string;
    values: Record<string, Record<string, any>>;
    categories: Record<string, string[]>;
    embedded: Record<string, string[]>;
  },
  hideMessage?: boolean
) =>
  request.post<string>(
    useUrlSiteId("textContent/langupdate"),
    data,
    undefined,
    {
      successMessage: hideMessage ? undefined : $t("common.saveSuccess"),
    }
  );
