export interface Site {
  permissionLevel: string;
  siteUrl: string;
}

export interface Sitemap {
  errors: number;
  isPending: boolean;
  isSitemapsIndex: boolean;
  lastDownloaded: string;
  lastSubmitted: string;
  path: string;
  warnings: number;
}
