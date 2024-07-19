export interface PanelRow {
  type: string;
  content: string;
}

export interface Panel {
  autoRefresh: boolean;
  refreshIntervalSeconds: number;
  rows: PanelRow[];
}
export interface Banner {
  linkText: string;
  summary: string;
  title: string;
  url: string;
}

export interface Doc {
  url: string;
  ico: string;
  name: string;
}

export interface News {
  date: string;
  image: string;
  summary: string;
  title: string;
  url: string;
}

export interface Visitors {
  count: number;
  name: string;
  size: number;
  sizeString: string;
}

export interface Resource {
  count: number;
  name: string;
  size: number;
  sizeString: string;
}

export interface EditLog {
  actionType: string;
  ago: string;
  displayName: string;
  lastModify: Date;
  logId: string;
  storeName: string;
  updateTick: string;
  userName: string;
}
export interface SiteInfo {
  creationDate: Date;
  domains: string[];
  previewUrl: string;
  siteName: string;
  type: string;
  users: {
    userName: string;
    role: string;
  }[];
}

export interface Info {
  editLog: EditLog[];
  info: {
    banner: Banner;
    news: News[];
    doc: Doc[];
  };
  resource: Resource[];
  site: SiteInfo;
  top: any;
  visitors: Visitors[];
}
