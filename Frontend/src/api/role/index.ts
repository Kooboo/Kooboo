import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { Role } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getEdit = (name: string) =>
  request.get<Role>(useUrlSiteId("Role/GetEdit"), { name: name });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Role/isUniqueName"),
    { name },
    { hiddenError: true, hiddenLoading: true }
  );

export const post = (body: unknown) =>
  request.post(useUrlSiteId("Role/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
