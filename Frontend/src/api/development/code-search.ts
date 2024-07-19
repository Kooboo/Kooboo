import type { KeyValue } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";

export type CodeSearchLine = { lineNumber: number; summary: string };
export type CodeSearch = {
  id: string;
  lines: CodeSearchLine[];
  name: string;
  type: string;
  url: string;
  pageType: string;
  layoutId: string;
  folderId: string;
};

export interface SearchResult {
  name: string;
  type: string;
  id: string;
  matched: {
    lineNumber: number;
    summary: string;
    start?: number;
    end?: number;
  }[];
  params: Record<string, any>;
}

export const getList = (params: { keyword: string }) =>
  request.get<CodeSearch[]>(useUrlSiteId("CodeSearch/list"), params);

export const search = (
  keyword: string,
  isRegex: boolean,
  ignoreCase: boolean
) =>
  request.get<SearchResult[]>(useUrlSiteId("CodeSearch/Search"), {
    keyword,
    isRegex,
    ignoreCase,
  });
