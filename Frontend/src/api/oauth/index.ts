import request from "@/utils/request";
import type { BindParam, BindResult } from "../verify-code/types";

export const getUrl = (redirectUri: string, type: string) =>
  request.post<string>(`OAuth/url/${type}`, { redirectUri });

export const bind = (body: BindParam) => {
  return request.post<BindResult>("/OAuth/Bind", body);
};

export const register = (body: BindParam) => {
  return request.post<BindResult>("/OAuth/Register", body);
};
