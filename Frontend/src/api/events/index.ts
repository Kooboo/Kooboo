import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { EventItem, Rule, Option, Setting, EventTypeItem } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getEventList = () =>
  request.get<EventTypeItem[]>(useUrlSiteId("BusinessRule/EventList"));

export const getList = () =>
  request.get<EventItem[]>(useUrlSiteId("BusinessRule/list"));

export const getListByEvent = (eventname: string) =>
  request.get<Rule[]>(useUrlSiteId("BusinessRule/ListByEvent"), { eventname });

export const getAvailableCodes = (eventname: string) =>
  request.get<Record<string, string>>(
    useUrlSiteId("BusinessRule/GetAvailableCodes"),
    {
      eventname,
    }
  );

export const getSetting = (id: string) =>
  request.get<Setting[]>(useUrlSiteId("BusinessRule/GetSetting"), {
    id,
  });

export const getConditionOptions = (eventname: string) =>
  request.get<Option[]>(useUrlSiteId("BusinessRule/ConditionOption"), {
    eventname,
  });

export const post = (body: unknown) =>
  request.post(useUrlSiteId("BusinessRule/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
