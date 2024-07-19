import request from "@/utils/request";
import type { BindParam, BindResult, VerifyResult } from "./types";

export const smsCode = (tel: string) => {
  return request.post("/VerifyCode/SmsCode", { tel });
};

export const verifyTelCode = (tel: string, code: string, returnurl: string) => {
  return request.post<VerifyResult>("/VerifyCode/verifyTelCode", {
    tel,
    code,
    returnurl,
  });
};

export const bind = (body: BindParam) => {
  return request.post<BindResult>("/VerifyCode/Bind", body);
};

export const register = (body: BindParam) => {
  return request.post<BindResult>("/VerifyCode/Register", body);
};
