export interface Code {
  id: string;
  name: string;
  codeType: string;
  url: string;
  previewUrl: string;
  lastModified: string;
  isEmbedded: boolean;
  references: Record<string, number>;
  scriptType: string;
}

export interface PostCode {
  body: string;
  codeType: string;
  scriptType: string;
  config: string;
  eventType: string;
  id: string;
  name: string;
  url: string;
  version: number;
  enableDiffChecker: boolean;
  isEmbedded?: boolean;
  isDecrypted?: boolean;
}

export interface BreakLine {
  source: string;
  line: number;
}

export interface DebugSession {
  breakpoints: BreakLine[];
  currentCode: string;
  debugInfo: {
    currentLine: number;
    variables: [];
  };
  exception: any;
  end: boolean;
}
