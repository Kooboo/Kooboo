import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { Binding } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getListBySite = () =>
  request.get<Binding[]>(useUrlSiteId("Binding/listbysite"));

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Binding/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const post = (body: unknown) =>
  request.post(useUrlSiteId("Binding/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const verifySSL = (body: unknown) =>
  request.post(useUrlSiteId("Binding/verifySSL"), body);

export const setSsl = (body: unknown) =>
  request.post(useUrlSiteId("Binding/SetSsl"), body);

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Binding/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );
