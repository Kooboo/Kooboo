export interface ILoginParam {
  username: string;
  password: string;
  withToken?: boolean;
  returnurl?: string;
}

export interface IRegisterParam {
  username: string;
  password: string;
  email: string;
  tel: string;
  confirmPassword: string;
}

export interface IUser {
  id: string;
  userName: string;
  emailAddress: string;
  tel: string;
  firstName: string;
  lastName: string;
  password: string;
  currency: string;
  fullName: string;
}

export interface IChangePasswordParam {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
  username: string;
}
