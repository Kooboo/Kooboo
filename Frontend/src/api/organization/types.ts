export interface IAddUserParam {
  organizationId: string;
  userName: string;
}

export interface IDeleteUserParam {
  organizationId: string;
  userName: string;
}

export interface IOrganization {
  id: string;
  adminUser: string;
  displayName: string;
  name: string;
}

export interface OrganizationList {
  id: string;
  adminUser: string;
  displayName: string;
  isAdmin: boolean;
  members: number;
  name: string;
  serverId: number;
  serviceLevel: number;
}

export interface Organization {
  displayName: string;
  id: string;
  isAdmin: boolean;
  isPartner: boolean;
  name: string;
  serviceLevel: number;
  userId: string;
  userName: string;
}

export interface DataCenter {
  default: boolean;
  description: string;
  enable: boolean;
  name: string;
  navUrl: string;
  primaryDomain: string;
}

export interface UsersList {
  currentOrgId: string;
  currentOrgName: string;
  emailAddress: string;
  firstName: string;
  fullName: string;
  id: string;
  isAdmin: boolean;
  joinDate: Date;
  language: string;
  lastName: string;
  registrationDate: Date;
  userName: string;
}
