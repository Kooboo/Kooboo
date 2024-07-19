export interface Group {
  childrenCount: number;
  id: string;
  lastModified: string;
  name: string;
  previewUrl: string;
  references: { page: number };
  relativeUrl: string;
}

export interface PostGroup {
  id: string;
  name: string;
  typeName: string;
  children: { name: string; routeId: string }[];
}
