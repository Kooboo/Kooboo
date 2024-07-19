export interface MenuItem {
  id: string;
  keyHash: string;
  lastModified: string;
  name: string;
  relations: Record<string, number>;
  storeNameHash: number;
}

export interface Menu {
  children: Menu[];
  id: string;
  lastModified: string;
  name: string;
  online: true;
  parentId: string;
  rootId: string;
  subItemContainer: string;
  subItemTemplate: string;
  template: string;
  url: string;
  values: Record<string, string>;
  urls: Record<string, string>;
}
