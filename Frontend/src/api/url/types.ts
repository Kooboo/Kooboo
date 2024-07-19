export interface UrlItem {
  fullUrl: string;
  hasObject: boolean;
  id: string;
  lastModified: string;
  name: string;
  objectId: string;
  previewUrl: string;
  relations: Record<string, number>;
  resourceType: string;
}
