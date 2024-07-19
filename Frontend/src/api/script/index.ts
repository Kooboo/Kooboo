import type { Script, ScriptItem } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getExternal = () =>
  request.get<ScriptItem[]>(useUrlSiteId("Script/External", true));

export const getEmbedded = () =>
  request.get<ScriptItem[]>(useUrlSiteId("Script/Embedded", true));

export const getEdit = (id: string) =>
  request.get<Script>(useUrlSiteId("Script/Get"), { id });

export const post = (body: Script) =>
  request.post<string>(useUrlSiteId("Script/Update"), body);

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Script/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const upload = (body: unknown) =>
  request.post(useUrlSiteId("Upload/Script"), body, undefined, {
    headers: { "Content-Type": "multipart/form-data" },
    successMessage: $t("common.uploadSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Script/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );
