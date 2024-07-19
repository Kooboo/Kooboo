import request from "@/utils/request";
import type { MemberOption } from "./types";

export const getMemberOptions = () =>
  request.get<MemberOption[]>("/member/options");
export const purchaseMemberShip = (servicelevel: number) =>
  request.post("/member/NewMemberShip", { servicelevel });
