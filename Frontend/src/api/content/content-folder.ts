import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type RelationFolder = {
  [key: string]: any;
  alias: string;
  display?: string;
  folderId: string;
  multiple?: boolean;
};

export type ContentFolder = {
  id: string;
  name: string;
  displayName: string;
  contentTypeId: string;
  hidden: boolean;
  sortable: boolean;
  ascending: boolean;
  sortField: string;
  pageSize: number;
  relations: Record<string, number>;
  category: RelationFolder[];
  embedded: RelationFolder[];
  isContent?: boolean;
};

export interface ContentFolderColumn {
  name: string;
  displayName: string;
  dataType: number;
  operators: string[];
}

export const getList = () =>
  request.get<ContentFolder[]>(useUrlSiteId("ContentFolder/list"));

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("ContentFolder/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getFolderInfoById = (params: { id: string }) =>
  request.get<ContentFolder>(useUrlSiteId("ContentFolder/Get"), params);

export const post = (data: Omit<ContentFolder, "relations">) =>
  request.post(useUrlSiteId("ContentFolder/post"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("ContentFolder/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );

export const getFolderColumns = (folderId: string) =>
  request.get<ContentFolderColumn[]>(useUrlSiteId("ContentFolder/Columns"), {
    id: folderId,
  });
