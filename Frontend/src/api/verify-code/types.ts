export interface VerifyResult {
  requireinfo: boolean;
  verifyid: string;
  access_token: string;
  error: string;
}

export interface BindResult {
  error: string;
  access_token: string;
}

export interface BindParam {
  verifyid: string;
  userName: string;
  password: string;
}
