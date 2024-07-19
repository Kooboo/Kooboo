export interface Form {
  formType: string;
  fullUrl: string;
  id: string;
  isEmbedded: boolean;
  keyHash: string;
  lastModified: string;
  name: string;
  references: Record<string, number>;
  routeId: string;
  routeName: string;
  size: number;
  source: string;
  storeNameHash: number;
  type: string;
  valueCount: number;
}

export interface PostForm {
  body: string;
  id: string;
  name: string;
  isEmbedded: boolean;
}

export interface Setting {
  allowAjax: boolean;
  availableSubmitters: availableSubmitter[];
  enable: boolean;
  failedCallBack: null;
  formId: string;
  formSubmitter: string;
  id: string;
  method: string;
  redirectUrl: string;
  setting: Record<string, string>;
  successCallBack: null;
}

interface availableSubmitter {
  name: string;
  settings: {
    controlType: string;
    defaultValue: string;
    name: string;
    selectionValues: Record<string, string>;
  }[];
}

export interface PostSetting {
  id: string;
  formId: string;
  method: string;
  redirectUrl: string;
  setting: Record<string, string>;
  formSubmitter: string;
  enable: boolean;
}

export interface Values {
  id?: string;
  list: Value[];
  pageNr: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface Value {
  id: string;
  formId: string;
  values: {
    [key: string]: any;
  };
  constType: number;
  creationDate: string;
  lastModified: string;
  lastModifyTick: number;
  name: null;
}
