export interface Layout {
  body: string;
  creationDate: string;
  extension: string;
  id: string;
  lastModified: string;
  name: string;
  online: boolean;
  version: number;
  enableDiffChecker: boolean;
}

export interface PostLayout {
  id: string;
  name: string;
  body: string;
  version: number;
  enableDiffChecker: boolean;
}

export interface ListItem {
  id: string;
  keyHash: string;
  lastModified: string;
  name: string;
  relations: Record<string, number>;
  page: number;
  storeNameHash: number;
}
