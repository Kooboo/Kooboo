import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";

export interface Tag {
  name: string;
  description: string;
  attributes: Attribute[];
}

export interface Attribute {
  name: string;
  description: string;
  values: { name: string; description: string }[];
}

export interface Suggestions {
  name: string;
  tags: Tag[];
  globalAttributes: Attribute[];
}

export enum Reference {
  Script,
  Style,
}

export interface ClassDefineItem {
  name: string;
  description: string;
  repeat?: string[];
}

export interface ClassDefine {
  name: string;
  references: { type: Reference; url: string }[];
  classes: ClassDefineItem[];
}

export const getDefine = () =>
  request.get<string>(useUrlSiteId("KScript/GetDefine"));

export const getKViewSuggestions = () =>
  request.get<Suggestions[]>(useUrlSiteId("KScript/GetKViewSuggestions"));

export const getClassSuggestions = () =>
  request.get<ClassDefine[]>(useUrlSiteId("KScript/GetClassSuggestions"));

export const getColumns = (
  database: string,
  table: string,
  moduleId?: string
) =>
  request.get<string[]>(
    useUrlSiteId("KScript/getcolumns"),
    { database, table, moduleId },
    {
      hiddenLoading: true,
      hiddenError: true,
    }
  );

export const getTables = (database: string, moduleId?: string) =>
  request.get<string[]>(
    useUrlSiteId("KScript/gettables"),
    { database, moduleId },
    {
      hiddenLoading: true,
      hiddenError: true,
    }
  );
