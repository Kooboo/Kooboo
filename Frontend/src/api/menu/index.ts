import type { Menu, MenuItem } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getList = () =>
  request.get<MenuItem[]>(useUrlSiteId("Menu/list", true));
export const getEdit = (id: string, name?: string) =>
  request.get<Menu>(useUrlSiteId("Menu/getMenu"), { id, name });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Menu/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const deleteItem = (rootId: string, id: string) =>
  request.post(useUrlSiteId("Menu/Delete"), { rootId, id }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const create = (name: string) =>
  request.post<Menu>(useUrlSiteId("Menu/Create"), { name }, undefined, {
    successMessage: $t("common.addSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Menu/isUniqueName"),
    { name },
    { hiddenError: true, hiddenLoading: true }
  );

export const createSub = (body: unknown) =>
  request.post(useUrlSiteId("Menu/CreateSub"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const updateInfo = (body: unknown) =>
  request.post(useUrlSiteId("Menu/updateInfo"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const updateTemplate = (body: unknown) =>
  request.post(useUrlSiteId("Menu/UpdateTemplate"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const swap = (rootId: string, ida: string, idb: string) =>
  request.post(useUrlSiteId("Menu/Swap"), { rootId, ida, idb }, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
