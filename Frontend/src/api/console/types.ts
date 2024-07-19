export interface DomainMenu {
  name: string;
  displayName: string;
  icon: string;
  url: string;
  actionRights: number;
  items: DomainMenu[];
}

export interface Domain {
  dataCenter?: string;
  nameServer?: string;
  domainName: string;
  emails: number;
  expires: string;
  id: string;
  records: number;
  sites: number;
  useEmail: boolean;
  enableDns: boolean;
  enableEmail: boolean;
}

export interface AvailableDomain {
  id: string;
  domainName: string;
  organizationId: string;
  expirationDate: string;
  isKooboo: boolean;
}

export interface ServerInfo {
  dnsServers: string[];
  ipAddress: string;
  cName: string;
}

export interface ListByDomain {
  webSiteId: string;
  id: string;
  organizationId: string;
  siteName: string;
  subDomain: string;
  domainId: string;
  fullName: string;
  device: null;
  port: number;
}

export interface ListBySite {
  webSiteId: string;
  enableSsl: boolean;
  id: string;
  organizationId: string;
  subDomain: string;
  domainId: string;
  fullName: string;
  device: null;
  ipAddress: null;
  port: number;
  defaultPortBinding: boolean;
}

export interface SiteBinding {
  name: string;
  id: string;
  bindingCount: number;
}

export interface DNS {
  id: string;
  domain: string;
  host: string;
  type: string;
  value: string;
  priority: number;
  TTL: number;
}

export interface dataCenterType {
  dataCenter: string;
  description: string;
  isChosen: boolean;
}

export interface CDN {
  id: string;
  domainName: string;
  siteCDN: boolean;
  resourceCDN: boolean;
  icpCert: string;
}

export interface ResourceCDN {
  id: string;
  name: string;
  displayName: string;
  enable: boolean;
}

export interface PurchaseDomain {
  domain: string;
  status: string;
  price: {
    product: {
      currency: string;
      price: number;
    };
    reseller: {
      currency: string;
      price: number;
    };
  };
}

export interface DomainOrder {
  id: string;
  organizationId: string;
  type: number;
  title: string;
  body: string;
  extraInfo: string;
  totalAmount: number;
  creationDate: string;
  isPaid: boolean;
  isDelivered: boolean;
  isDeleted: boolean;
}

export interface EmailDomain {
  dataCenter: string;
  domainName: string;
  expiration: Date;
  icpCert: string;
  id: string;
  isKoobooDns: boolean;
  nameServer: string;
  organizationId: string;
  resourceCDN: boolean;
  siteCDN: boolean;
}

export interface OrderInfo {
  id: string;
  items: {
    name: string;
    price: number;
  }[];
  title: string;
  summary: string;
  subTotal: number;
  tax: number;
  total: number;
  currency: string;
}

export interface Card {
  id: string;
  display: string;
  type: string;
  isDefault: boolean;
}

export interface SubmitOrderResult {
  redirectUrl: string;
  errorMessage: string;
  success: boolean;
}

export interface CreditCard {
  name: string;
  cardNumber: string;
  year: string;
  month: string;
  cvc: string;
  orderId?: string;
}

export interface Recent {
  name: string;
  date: Date;
  item: string;
  isUp: string;
  amount: number;
  currency: string;
}
export interface Balance {
  amount: number;
  currency: string;
}
