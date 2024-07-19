import type { PropertyJsonString } from "@/global/control-type";

export interface PostView {
  body: string;
  dummyLayout: string;
  layouts: Record<string, string>;
  name: string;
  id: string;
  version: number;
  enableDiffChecker: boolean;
  propDefines: PropertyJsonString[];
}

export interface View {
  dataSourceCount: number;
  id: string;
  keyHash: string;
  lastModified: string;
  name: string;
  preview: string;
  relations: Record<string, number>;
  storeNameHash: number;
  version: number;
}
