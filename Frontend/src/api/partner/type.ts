export interface partnerServer {
  id: string;
  ip: string;
  region: string;
  requireICP: boolean;
  useCount: number;
  domain: string;
  displayName: string;
}

export interface partnerDNS {
  name: string;
  region: string;
  domainCount: number;
}

export interface partnerUser {
  id: string;
  partnerName: string;
  userName: string;
  joinDate: Date;
  remark: string;
  phone: string;
  email: string;
}

export interface AddUser {
  username: string;
  password: string;
  remark: string;
  serverId: string;
  phone: string;
  email: string;
}
