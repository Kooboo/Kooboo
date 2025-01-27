import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { EventItem, Rule, Option, Setting, EventTypeItem } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;
export const getEventList = () =>
  request.get<EventTypeItem[]>(useUrlSiteId("BackendRule/EventList"));
export const getList = () =>
  request.get<EventItem[]>(useUrlSiteId("BackendRule/list"));
export const getListByEvent = (eventname: string) =>
  request.get<Rule[]>(useUrlSiteId("BackendRule/ListByEvent"), { eventname });
export const getAvailableCodes = (eventname: string) =>
  request.get<Record<string, string>>(
    useUrlSiteId("BackendRule/GetAvailableCodes"),
    {
      eventname,
    }
  );
export const getSetting = (id: string) =>
  request.get<Setting[]>(useUrlSiteId("BackendRule/GetSetting"), {
    id,
  });
export const getConditionOptions = (eventname: string) =>
  request.get<Option[]>(useUrlSiteId("BackendRule/ConditionOption"), {
    eventname,
  });
export const post = (body: unknown) =>
  request.post(useUrlSiteId("BackendRule/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deletes = (names: string[]) =>
  request.post(useUrlSiteId("BackendRule/DeleteEvents"), { names }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
