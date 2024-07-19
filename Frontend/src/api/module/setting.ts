import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getSetting = (id: string) =>
  request.get<string>(useUrlSiteId("ModuleSetting/GetSetting"), {
    id,
  });

export const updateSetting = (id: string, json: string) =>
  request.post(
    useUrlSiteId("ModuleSetting/UpdateSetting"),
    {
      id,
      json,
    },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );
