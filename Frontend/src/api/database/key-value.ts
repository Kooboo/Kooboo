import type { KeyValue } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getList = () =>
  request.get<Record<string, string>>(useUrlSiteId("KeyValue/list"));

export const get = (key: string) =>
  request.get<string>(useUrlSiteId("KeyValue/get"), { key });

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("KeyValue/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const update = (data: KeyValue) =>
  request.post(useUrlSiteId("KeyValue/Update"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("KeyValue/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
