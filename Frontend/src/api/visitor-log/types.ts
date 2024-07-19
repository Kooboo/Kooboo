export interface Error {
  clientIP: string;
  id: string;
  message: string;
  objId: string;
  previewUrl: string;
  startTime: Date;
  statusCode: number;
  url: string;
}

export interface TopImage {
  count: number;
  name: string;
  previewUrl: string;
  thumbNail: string;
  size: number;
}

export interface TopPage {
  count: number;
  name: string;
  size: number;
}

export interface VisitorLog {
  begin: string;
  clientIP: string;
  constType: number;
  country: string;
  end: string;
  entries: [];
  millionSecondTake: number;
  objectId: string;
  pageName: string;
  referer: string;
  size: number;
  state: string;
  statusCode: number;
  timeSpan: number;
  url: string;
  userAgent: string;
  userId: string;
}
