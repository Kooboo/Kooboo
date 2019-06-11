export class OperationLogItem {
  action!: string;
  editorType!: string;
  koobooId!: string;
  value!: string;
  nameOrId!: string;
  objectType!: string;
  fieldName!: string;
}

export class OperationLog {
  constructor(updates: Array<OperationLogItem>, pageid: string) {
    this.updates = updates;
    this.pageid = pageid;
  }

  updates: Array<OperationLogItem>;
  pageid!: string;
}
