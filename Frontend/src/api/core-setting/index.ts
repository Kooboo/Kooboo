import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { CoreSetting } from "./types";
import type { Field } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getCoreSetting = () =>
  request.get<CoreSetting[]>(useUrlSiteId("CoreSetting/list"));

export const getFields = (name: string) =>
  request.get<Field[]>(useUrlSiteId("CoreSetting/get"), { name });

export const update = (body: unknown) =>
  request.post(useUrlSiteId("CoreSetting/update"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
