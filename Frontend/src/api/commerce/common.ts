export interface PagingParams {
  pageIndex: number;
  pageSize: number;
}

export interface PagingResult<T> extends PagingParams {
  list: T[];
  count: number;
}

export interface CustomerInfo {
  id: string;
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
}

export interface Condition {
  isAny: boolean;
  items: {
    option: string;
    method: string;
    value: any;
  }[];
}

export interface ConditionSchema {
  name: string;
  display: string;
  methods: { name: string; display: string }[];
  selections: string[];
}
