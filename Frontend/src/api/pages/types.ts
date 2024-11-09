export interface All {
  baseUrl: string;
  layouts: { id: string; name: string }[];
  pages: Page[];
}

export interface Page {
  id: string;
  inlineUrl: string;
  lastModified: string;
  layoutId: string;
  linked: number;
  name: string;
  online: boolean;
  path: string;
  previewUrl: string;
  type: string;
  startPage: boolean;
  relations: { htmlBlock: number; layout: number; menu: null; view: number };
  hasParameter?: boolean;
  title: string;
}

export interface Meta {
  charset?: string;
  property?: string;
  content?: Record<string, string>;
  httpequiv?: string;
  name?: string;
  el?: Element;
}

export interface DefaultRoute {
  startPage: string;
  notFound: string;
  error: string;
}

export interface PostRichPage {
  url: string;
  title: string;
  body: string;
  name: string;
  id: string;
  enableCache: boolean;
  cacheByVersion: boolean;
  cacheMinutes: number;
  cacheQueryKeys: string;
  published?: boolean;
}

export interface PostPage {
  previewUrl?: string;
  contentTitle: Record<string, string>;
  urlPath: string;
  metas: Meta[];
  parameters: Record<string, string>;
  body: string;
  designConfig: string;
  name: string;
  id: string;
  enableCache: boolean;
  disableUnocss: boolean;
  cacheByVersion: boolean;
  cacheMinutes: number;
  cacheQueryKeys: string;
  title?: string;
  baseUrl?: string;
  scripts?: string[];
  styles?: string[];
  version: number;
  enableDiffChecker: boolean;
  layoutName?: string;
  published?: boolean;
  placeholderContents?: string;
  metaBindings?: string[];
  urlParamsBindings?: string[];
  type?: "Normal" | "Layout" | "RichText" | "Designer";
  layoutId?: string;
}

export function toRichPost(postPage: PostPage): PostRichPage {
  return {
    id: postPage.id,
    name: postPage.name,
    body: postPage.body,
    url: postPage.urlPath,
    title: postPage.title,
    cacheByVersion: postPage.cacheByVersion,
    cacheMinutes: postPage.cacheMinutes,
    cacheQueryKeys: postPage.cacheQueryKeys,
    enableCache: postPage.enableCache,
    published: postPage.published,
  } as PostRichPage;
}

export interface StructureNode {
  type: string;
  name: string;
  summary: string;
  newName: string;
  selected: boolean;
  children: StructureNode[];
  invalidMessage?: string;
  guid?: string;
}

export enum PageNodeType {
  Element = 0,
  Text = 1,
  TextBlock = 2,
}

export type LinkedPage = {
  pageId: string;
  koobooId: string;
};

export type PageNodeBase = {
  nodeType: PageNodeType;
  hash: number;
};

export type ConvertToTypes = "View" | "HtmlBlock";

export type NodeGroup = {
  id: string;
  type: PageNodeType;
  title: string;
  alt: string;
  node: PageNodeBase;
  pageCount: number;
  sameSub: boolean;
  pages: LinkedPage[];
  children?: NodeGroup[];
};

export type PageName = {
  pageId: string;
  name: string;
  url: string;
  previewUrl: string;
};

export type TaskId = string;

export type GroupTask = {
  id: string;
  totalPage: number;
  currentStatus: number;
  siteDb: any;
  pages: LinkedPage[];
  isFinish: boolean;
  lastModified: Date;
};

export type GroupTaskStatus = {
  totalPages: number;
  currentStatus: number;
  isFinish: boolean;
};

export enum SearchType {
  TagName = "TagName",
  Id = "Id",
  Class = "Class",
}

export enum Operator {
  Equal = "Equal",
  StartWith = "StartWith",
  Contains = "Contains",
}

export type SearchParam = {
  searchType: SearchType;
  operator: Operator;
  value: string;
};
