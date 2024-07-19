export type SearchRequest = {
  provider: string;
  keyword: string;
  pageNr: number;
};

export type SearchProvider = {
  value: string;
  label: string;
  url: string;
};

export type SearchPackageFileItem = {
  id: string;
  name: string;
  fullUrl: string;
  type: string;
  description: string;
  version: string;
  installed: boolean;
  loading?: boolean;
};
