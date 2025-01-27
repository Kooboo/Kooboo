import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface Country {
  code: string;
  name: string;
  currency: string;
  currencySymbol: string;
  emojiFlag: string;
  nameTranslations: Record<string, string>;
}

export const getCountries = () =>
  request.get<Country[]>(useUrlSiteId("Address/countries"));

export const getProvinces = (country: string) =>
  request.get<any[]>(useUrlSiteId("Address/Provinces"), { country });

export const getRegions = (country: string) =>
  request.get<any[]>(useUrlSiteId("Address/Regions"), { country });

export const getCities = (country: string, province: string) =>
  request.get<any[]>(useUrlSiteId("Address/Cities"), {
    country,
    province,
  });

export const getDetails = (body: unknown) =>
  request.post<any[]>(useUrlSiteId("Address/Details"), body);
