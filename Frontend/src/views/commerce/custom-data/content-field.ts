export interface TextContentDetails {
  columns: TextContentColumn[];
  list: TextContentListItem[];
}

export interface TextContentColumn {
  name: string;
  displayName: string;
  controlType: string;
  multipleValue: boolean;
}

export interface TextContentListItem {
  embedded: any;
  textValues: Record<string, string>;
  id: string;
  userKey: string;
  folderId: string;
  parentId: string;
  contentTypeId: string;
  order: number;
  summaryField: string;
  usedBy: any;
  lastModified: string;
  creationDate: string;
  online: boolean;
  values: Record<string, string>;
  source: string;
}
