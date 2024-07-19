import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type TableRelation = {
  fieldA: string;
  fieldB: string;
  id: string;
  name: string;
  relation: string;
  relationName: string;
  tableA: string;
  tableB: string;
};
export const getList = () =>
  request.get<TableRelation[]>(useUrlSiteId("TableRelation/list"));

export const deleteRelation = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("TableRelation/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export type TableFields = {
  name: string;
  fields: string[];
};
export const getTablesAndFields = () =>
  request.get<TableFields[]>(useUrlSiteId("TableRelation/getTablesAndFields"));

export type RelationType = {
  type: string;
  displayName: string;
};
export const getRelationTypes = () =>
  request.get<RelationType[]>(useUrlSiteId("TableRelation/getRelationTypes"));

export const postRelation = (data: {
  name: string;
  tableA: string;
  fieldA: string;
  tableB: string;
  fieldB: string;
  relation: string;
}) =>
  request.post(useUrlSiteId("TableRelation/post"), data, undefined, {
    successMessage: $t("common.createSuccess"),
  });
export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("TableRelation/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
