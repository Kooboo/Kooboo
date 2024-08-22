export interface TransferSingle {
  pageUrl: string;
  name: string;
}

export interface Domain {
  subDomain: string;
  rootDomain: string;
}

export interface ByLevelBody extends Domain {
  siteName: string;
  url: string;
  TotalPages?: number;
  Depth?: number;
  headless?: boolean;
  convertToRoot?: boolean;
}

export interface TransferReponse {
  success?: boolean;
  messages?: string[];
}
