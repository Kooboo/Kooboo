import type { KeyValue, Pagination } from "@/global/types";
import request, { api } from "@/utils/request";

import { downloadFromResponse } from "@/utils/dom";
import { i18n } from "@/modules/i18n";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type DatabaseType = "Database" | "Sqlite" | "MySql" | "SqlServer";

export const getTables = (dbType: DatabaseType) =>
  request.get<string[]>(useUrlSiteId(`${dbType}/Tables`), undefined, {
    hiddenError: true,
  });

export const isUniqueTableName = (dbType: DatabaseType, name: string) =>
  request.get(
    useUrlSiteId(`${dbType}/IsUniqueTableName`),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );

export const createTable = (dbType: DatabaseType, data: { name: string }) =>
  request.post(useUrlSiteId(`${dbType}/CreateTable`), data, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const deleteTables = (
  dbType: DatabaseType,
  data: { names: string[] }
) => {
  return request.post(useUrlSiteId(`${dbType}/DeleteTables`), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
};

export type ColumnSetting = {
  options: KeyValue[];
};

export type DatabaseColumn = {
  [key: string]: any;
  controlType: string | undefined;
  dataType: string;
  isIncremental: boolean;
  isIndex: boolean;
  isPrimaryKey: boolean;
  isSystem: boolean;
  isUnique: boolean;
  length: number;
  name: string;
  displayName?: string;
  scale: number;
  seed: number;
  setting?: ColumnSetting | string;
  value?: any;
};
export const getDatabaseColumns = (
  dbType: DatabaseType,
  params: { table: string }
) => request.get<DatabaseColumn[]>(useUrlSiteId(`/${dbType}/Columns`), params);

export const updateColumn = (
  dbType: DatabaseType,
  data: {
    tableName: string;
    columns: DatabaseColumn[];
  }
) =>
  request.post(useUrlSiteId(`/${dbType}/UpdateColumn`), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export type DatabaseListColumn = Pick<
  DatabaseColumn,
  "name" | "dataType" | "isSystem" | "controlType" | "displayName"
>;

export type DatabaseData = Pagination & {
  primaryKey: string;
  columns: DatabaseListColumn[];
  list: KeyValue[][];
};

export type ReadCsvResponse = {
  columns: DatabaseListColumn[];
  list: Record<string, string>[];
};

export const getDatabaseData = (
  dbType: DatabaseType,
  params: { table: string; pageNr: number; order: string; desc: boolean }
) => request.get<DatabaseData>(useUrlSiteId(`/${dbType}/Data`), params);

export type ImportFieldItem = {
  dbFieldName: string;
  csvFieldName: string;
  unique: boolean;
  required: boolean;
};

export type ImportRequestModel = {
  overwriteExisting: boolean;
  fields: ImportFieldItem[];
  records: Record<string, string>[];
};

export const importData = (
  dbType: DatabaseType,
  table: string,
  data: ImportRequestModel
) =>
  request.post(
    useUrlSiteId(`/${dbType}/importData?table=${table}`),
    data,
    undefined,
    {
      successMessage: $t("common.importSuccess"),
      errorMessage: $t("common.importFailed"),
      keepShowErrorMessage: true,
    }
  );

export const exportData = (
  dbType: DatabaseType,
  params: {
    table: string;
    pageNr: number;
    pageSize: number;
    order: string;
    desc: boolean;
  }
) =>
  api
    .get<Blob>(useUrlSiteId(`/${dbType}/ExportData`), {
      params: params,
      responseType: "blob",
      timeout: 1000 * 60 * 30,
    })
    .then((response) => {
      downloadFromResponse(response);
    });
export const deleteTableData = (
  dbType: DatabaseType,
  data: {
    tableName: string;
    values: string[];
  }
) =>
  request.post(useUrlSiteId(`/${dbType}/DeleteData`), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getDatabaseEdit = (
  dbType: DatabaseType,
  params: { tableName: string; id: string }
) => request.get<DatabaseColumn[]>(useUrlSiteId(`/${dbType}/GetEdit`), params);

export const updateTableData = (
  dbType: DatabaseType,
  data: {
    tableName: string;
    id: string;
    values: { name: string; value: any }[];
  }
) =>
  request.post(useUrlSiteId(`/${dbType}/UpdateData`), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
