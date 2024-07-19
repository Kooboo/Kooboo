export interface StyleItem {
  fullUrl: string;
  id: string;
  keyHash: string;
  lastModified: string;
  name: string;
  references: { resourceGroup: number; page: number };
  routeId: string;
  routeName: string;
  size: number;
  ownerObjectId: string;
}

export interface Style {
  body: string;
  extension: string;
  id: string;
  name: string;
  isEmbedded: boolean;
  version?: number;
  enableDiffChecker?: boolean;
  ownerObjectId: string;
}
