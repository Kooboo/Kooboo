import type { Style, StyleItem } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getExternal = () =>
  request.get<StyleItem[]>(useUrlSiteId("Style/External", true));

export const getEmbedded = () =>
  request.get<StyleItem[]>(useUrlSiteId("Style/Embedded", true));

export const getEdit = (id: string) =>
  request.get<Style>(useUrlSiteId("Style/Get"), { id });

export const post = (body: Style) =>
  request.post<string>(useUrlSiteId("Style/Update"), body);

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Style/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const upload = (body: unknown) =>
  request.post(useUrlSiteId("Upload/Style"), body, undefined, {
    headers: { "Content-Type": "multipart/form-data" },
    successMessage: $t("common.uploadSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Style/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
