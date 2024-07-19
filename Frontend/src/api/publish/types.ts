import type { KeyValue } from "@/global/types";

export interface Publish {
  difference: number;
  id: string;
  remoteServerUrl: string;
  remoteSiteName: string;
}

export interface PushItem {
  changeType: string;
  id: number;
  koobooType: number;
  lastModified: string;
  logId: number;
  name: string;
  objectType: string;
  selected: false;
  size: string;
  user: string;
  syncItemId: string;
  thumbnail: string;
  version: number;
  keyHash: string;
  storeNameHash: number;
  tableNameHash: number;
}

export interface UserPublish {
  id: string;
  name: string;
  reserved: boolean;
  serverUrl: string;
  orgId: string;
}

export interface ServerHost {
  organizationId: string;
  serverId: string;
  host: string;
  dataCenter: string;
}

export interface Difference {
  name: string;
  value: string;
  remoteValue: string;
}

export interface LogOptions {
  editType: KeyValue[];
  storeName: KeyValue[];
  user: KeyValue[];
}

export interface SyncSetting {
  id: string;
  orgId: string;
  serverName: string;
  remoteWebSiteId: string;
  remoteSiteName: string;
  remoteServerUrl: string;
  ignoreOutStoreNames: string[];
  ignoreInStoreNames: string[];
  creationDate: string;
  lastModified: string;
}

export interface PushFeedBack {
  success: boolean;
  isFinish: boolean;
  siteLogId: number;
  remoteVersion: number;
  hasConflict: boolean;
  remoteUserName: string;
  localUserName: string;
  remoteTime: string;
  localTime: string;
  remoteBody: string;
  localBody: string;
  isImage: boolean;
}

export interface PullFeedBack {
  success: boolean;
  isFinish: boolean;
  senderVersion: number;
  currentSenderVersion: number;
  localLogId: number;
  hasConflict: boolean;
  remoteUserName: string;
  localUserName: string;
  remoteTime: string;
  localTime: string;
  remoteBody: string;
  localBody: string;
  isImage: boolean;
}
