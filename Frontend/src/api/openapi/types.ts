export interface OpenApi {
  authUrl: string;
  baseUrl: string;
  caches: Cache[];
  creationDate?: string;
  customAuthorization: string;
  useCustomCode: boolean;
  code: string;
  id: string;
  jsonData: string;
  lastModified?: string;
  name: string;
  type: string;
  url: string;
  useCommaArray: boolean;
  templateId: string;
}

export interface Cache {
  expiresIn: number;
  method: string;
  pattern: string;
}

export interface Template {
  authUrl: string;
  baseUrl: string;
  description: string;
  name: string;
  _id: string;
}

export interface Authorize {
  authorizeName: string;
  constType: number;
  creationDate: string;
  id: string;
  lastModified: string;
  lastModifyTick: number;
  name: string;
  online: boolean;
  openApiName: string;
  version: number;
  securities: {
    /* 
    oauth2?: Securities;
    apiKey?: Securities;
    jwt?: Securities; 
    */
    [key: string]: Securities;
  };
}

export interface Securities {
  username: string;
  password: string;
  clientId: string;
  clientSecret: string;
  accessToken: string;
  refreshToken: string;
  expiresIn: string;
  redirectUrl: string;
  name: string;
}
