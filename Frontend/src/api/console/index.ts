import request from "@/utils/request";
import type {
  DomainMenu,
  AvailableDomain,
  Domain,
  ServerInfo,
  ListByDomain,
  ListBySite,
  SiteBinding,
  DNS,
  dataCenterType,
  CDN,
  PurchaseDomain,
  ResourceCDN,
  EmailDomain,
  Card,
  OrderInfo,
  SubmitOrderResult,
  CreditCard,
  Recent,
  Balance,
} from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getDomainMenu = () => request.get<DomainMenu[]>("/Bar/domainMenu");

export const getAvailableDomain = () =>
  request.get<AvailableDomain[]>("/Domain/Available");

export const Search = (keyword: string) =>
  request.get<Domain[]>("/Domain/search", { keyword });

export const getDomainList = () =>
  request.get<EmailDomain[]>("/EmailAddress/domains");

export const getList = () => request.get<Domain[]>("/Domain/list");

export const getDataCenterList = (domain: string) =>
  request.get<dataCenterType[]>("/Domain/DataCenterList", { domain });
export const assignDataCenter = (domain: string, dataCenter: string) =>
  request.post("/Domain/AssignDataCenter", { domain, dataCenter }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getDnsRecords = (domain: string) =>
  request.get<DNS[]>("/Domain/DnsRecords", { domain });

export const getDnsType = () => request.get<string[]>("/Domain/DnsType");

export const getTTL = () =>
  request.get<{ name: string; value: number }[]>("/Domain/TTL");

export const addDns = (body: DNS) =>
  request.post("/Domain/AddDns", body, undefined, {
    successMessage: $t("common.addSuccess"),
  });
export const deleteDns = (ids: string[]) =>
  request.post("/Domain/DeleteDns", { ids: ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getDomain = (id: string) =>
  request.get<Domain>("/Domain/get", { id });

export const postDomain = (domainName: string) =>
  request.post("/Domain/Create", { domainname: domainName }, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const deleteDomains = (ids: string[]) =>
  request.post("/Domain/Deletes", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getServerInfo = () =>
  request.get<ServerInfo>("/Domain/ServerInfo");

export const getBindingListByDomain = (id: string) =>
  request.get<ListByDomain[]>("/Binding/ListByDomain", { domainid: id });

export const getBindingListBySite = (id: string) =>
  request.get<ListBySite[]>("/Binding/listbysite", { SiteId: id });

/* {"SubDomain":"sds","RootDomain":"kooboo.site","SiteId":"a77f8475-c977-43eb-d19d-0471a400bad0"}:  */
export const postBinding = (
  subDomain: string,
  rootDomain: string,
  SiteId: string
) =>
  request.post<ListByDomain[]>("/Binding/post", {
    subDomain,
    rootDomain,
    SiteId,
  });

export const getSiteBinding = () =>
  request.get<SiteBinding[]>("/Binding/SiteBinding");

export const deleteBinding = (ids: string[]) =>
  request.post("/Binding/Deletes", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getPurchaseDomain = (tlds: string[], keyword: string) =>
  request.post<{
    results: PurchaseDomain[];
  }>("domain/search", { tlds, keyword });

export const getCDNList = () => request.get<CDN[]>("/CDN/List");
export const getResourceCDNList = () =>
  request.get<ResourceCDN[]>("/ResourceCDN/List");

export function enableResourceCDN(siteId: string) {
  return request.post("ResourceCDN/Enable", { siteId }, undefined, {
    successMessage: $t("common.enableResourceCDNTips"),
  });
}

export function disableResourceCDN(siteId: string) {
  return request.post("ResourceCDN/Disable", { siteId }, undefined, {
    successMessage: $t("common.disableResourceCDNTips"),
  });
}
export const updateCDN = (cdn: CDN) =>
  request.post("/CDN/Update", cdn, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const IPaymentResponse = (id: string) =>
  request.post("/MarketOrder/WeChatPay", { id });

export const getCards = () => request.get<Card[]>("/MarketOrder/CreditCards");
export const getInfo = (orderId: string) =>
  request.get<OrderInfo>("/MarketOrder/Info", { orderId });

export const submitPaymentMethod = (paymentMethodId: string, orderId: string) =>
  request.post<SubmitOrderResult>("/MarketOrder/SubmitPaymentMethod", {
    paymentMethodId,
    orderId,
  });

export const submitCreditCard = (body: CreditCard) =>
  request.post<CreditCard>("/MarketOrder/SubmitCreditCard", body);

export const getRecentList = () => request.get<Recent[]>("/balance/recentlist");
export const getCurrentBalance = () => request.get<Balance>("/balance/current");
export const newTopup = (amount: number) =>
  request.post("/marketorder/newtopup", { amount });
