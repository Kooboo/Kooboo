import type { KeyValue } from "@/global/types";

export enum VersionDataType {
  String = 0,
  Image = 1,
}

export interface Compare {
  constType: number;
  dataType: VersionDataType;
  id1: number;
  id2: number;
  objectId: string;
  source1: string;
  source2: string;
  title1: string;
  title2: string;
}

export interface Log {
  actionType: string;
  id: number;
  itemName: string;
  keyHash: string;
  lastModified: string;
  storeName: string;
  storeNameHash: number;
  tableName: string;
  tableNameHash: number;
  userName: string;
}

export interface Version {
  id: number;
  lastModified: string;
  userName: string;
}

export interface LogOptions {
  editType: KeyValue[];
  storeName: KeyValue[];
  user: KeyValue[];
}

export interface TaskStatus {
  totalItem: number;
  finishedItems: number;
  isFinished: boolean;
}

export interface CleanLogRunningTask {
  id: string;
  isFinished: boolean;
}
