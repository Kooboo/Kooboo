export interface Header {
  mail: { count: 0 };
  menu: { name: string; displayName: string; url: string }[];
  user: { id: string; language: string; name: string; emailAddress: string };
  isOnlineServer: boolean;
  isPrivateServer: boolean;
}
