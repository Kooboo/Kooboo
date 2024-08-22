export interface Template {
  id: string;
  userName: string;
  thumbNail: string;
  name: string;
  category: string;
  sizeString: string;
  description: string;
  score: 2824;
  tags: string;
  lastModified: string;
  downloadCount: number;
  pageCount: number;
  contentCount: number;
  imageCount: number;
  layoutCount: number;
  menuCount: number;
  viewCount: number;
  price: number;
  thunbnailUrl: string;
  thumbNailFileName: string;
  previewUrl: string;
  downloadUrl: string;
  theme: { name: string; value: number };
  thumbnailMime: string;
  type: { name: string; value: number };
  screenShotUrl: string | null;
}

export interface PagedTemplate {
  facets: Facet[];
  list: Template[];
  pageNr: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ShareValidator {
  isSecure: boolean;
  message: string | null;
  showScreenShot: boolean;
  violation: string | null;
}

export interface SearchOptions {
  keyword: string;
  pageNr: number;
  pageSize: number;
  typeName: number | string;
  color: string;
  category: string;
}

export interface Facet {
  name: string;
  labels: {
    name: string;
    count: number;
  }[];
}
