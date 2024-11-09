import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { i18n } from "@/modules/i18n";
import type {
  Condition,
  ConditionSchema,
  PagingParams,
  PagingResult,
} from "./common";
const $t = i18n.global.t;

export interface Member {
  id: string;
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
  status: MembershipStatus;
  points: number;
}

export interface MembershipStatus {
  startedAt: string;
  endAt: string;
  active: boolean;
  membership: MembershipListItem;
}

export type MemberPagingResult = PagingResult<Member>;
export interface Membership {
  name: string;
  description: string;
  price: number;
  durationUnit: "Year" | "Month" | "Day";
  duration: number;
  priority: number;
  condition: Condition;
  allowPurchase: boolean;
  allowAutoUpgrade: boolean;
}

export type MembershipCreate = Membership;

export const getMembershipSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("loyalty/MembershipSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );

export interface MembershipEdit extends Membership {
  id: string;
}

export interface MembershipListItem extends Membership {
  id: string;
}

export const createMembership = (body: unknown) =>
  request.post(useUrlSiteId("loyalty/createMembership"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editMembership = (body: unknown) =>
  request.post(useUrlSiteId("loyalty/EditMembership"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getMembershipEdit = (id: string) =>
  request.get<MembershipEdit>(useUrlSiteId("loyalty/getMembershipEdit"), {
    id,
  });

export const getMemberships = () =>
  request.get<MembershipListItem[]>(useUrlSiteId("loyalty/Memberships"));

export const deleteMemberships = (ids: string[]) =>
  request.post(useUrlSiteId("loyalty/deleteMemberships"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getMembers = (params: PagingParams) =>
  request.post<MemberPagingResult>(useUrlSiteId("loyalty/Members"), params);

export const getMember = (id: string) =>
  request.get<Member>(useUrlSiteId("loyalty/Member"), { id });

export const changeMembership = (id: string, membershipId: string) =>
  request.post(
    useUrlSiteId("loyalty/ChangeMembership"),
    { id, membershipId },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export const renewMembership = (id: string) =>
  request.post(useUrlSiteId("loyalty/RenewMembership"), { id }, undefined, {
    successMessage: $t("common.renewSuccess"),
  });

export const earnPoints = (id: string, points: number, description: string) =>
  request.post(
    useUrlSiteId("loyalty/EarnPoints"),
    { id, points, description },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );
export const redeemPoints = (id: string, points: number, description: string) =>
  request.post(
    useUrlSiteId("loyalty/RedeemPoints"),
    { id, points, description },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export interface CustomerPoint {
  customerId: string;
  points: number;
  createdAt: string;
  type: string;
  description: string;
}

export const pointList = (
  params: PagingParams & {
    id: string;
  }
) =>
  request.post<PagingResult<CustomerPoint>>(
    useUrlSiteId("loyalty/PointList"),
    params
  );

export const customerMemberships = (
  params: PagingParams & {
    id: string;
  }
) =>
  request.post<PagingResult<CustomerPoint>>(
    useUrlSiteId("loyalty/CustomerMemberships"),
    params
  );

export interface EarnPointSettings {
  activeDurationUnit: "Year" | "Month" | "Week" | "Day";
  activeDuration: number;
  orderEarnRules: OrderEarnPointRule[];
  loginEarnRules: LoginEarnPointRule[];
}

export interface RedeemPointSettings {
  exchangeRate: number;
  orderRedeemRules: OrderEarnPointRule[];
}

export interface OrderEarnPointRule {
  value: number;
  isPercent: boolean;
  description: string;
  condition: Condition;
}

export interface LoginEarnPointRule {
  value: number;
  durationUnit: "Year" | "Month" | "Week" | "Day";
  description: string;
  condition: Condition;
}

export const getOrderEarnSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("loyalty/OrderEarnSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const getOrderRedeemSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("loyalty/OrderRedeemSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const getLoginEarnSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("loyalty/LoginEarnSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );
