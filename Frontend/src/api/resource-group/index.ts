import type { Group, PostGroup } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getGroups = (type: "Script" | "Style") =>
  request.get<Group[]>(useUrlSiteId(`ResourceGroup/${type}`, true));

export const updateGroup = (body: PostGroup) =>
  request.post(useUrlSiteId("ResourceGroup/Update"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("ResourceGroup/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const isUniqueName = (name: string, type: "script" | "style") =>
  request.get(
    useUrlSiteId("ResourceGroup/isUniqueName"),
    { name, type },
    { hiddenLoading: true, hiddenError: true }
  );

export const getEdit = (id: string) =>
  request.get<PostGroup>(useUrlSiteId(`ResourceGroup/Get`), { id });
