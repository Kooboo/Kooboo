export interface Permission {
  feature: string;
  action: string;
  access: boolean;
}

export interface Role {
  name: string;
  permissions: Permission[];
}
