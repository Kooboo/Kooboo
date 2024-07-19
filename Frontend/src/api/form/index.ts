import type { PostForm, PostSetting, Setting, Values } from "./types";

import type { Form } from "./types";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getExternal = () =>
  request.get<Form[]>(useUrlSiteId("Form/External", true));

export const getEmbedded = () =>
  request.get<Form[]>(useUrlSiteId("Form/Embedded", true));

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Form/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getEdit = (id: string) =>
  request.get<PostForm>(useUrlSiteId("Form/GetEdit"), { id });

export const post = (body: unknown) =>
  request.post<string>(useUrlSiteId("Form/post"), body);

export const updateSetting = (body: PostSetting) =>
  request.post(useUrlSiteId("Form/UpdateSetting"), body);

export const getSetting = (id: string) =>
  request.get<Setting>(useUrlSiteId("Form/GetSetting"), { id });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Form/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );

export const getValues = (id: string, pageNr: number) =>
  request.get<Values>(useUrlSiteId("Form/FormValues"), {
    id,
    pageNr,
    pageSize: 30,
  });

export const deleteValues = (ids: string[]) =>
  request.post(useUrlSiteId("Form/DeleteFormValues"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
