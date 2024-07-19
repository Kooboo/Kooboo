import type { PostView, View } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getList = () =>
  request.get<View[]>(useUrlSiteId("View/list", true));

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("View/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const copy = (id: string, name: string) =>
  request.post(useUrlSiteId("View/Copy"), { id, name }, undefined, {
    successMessage: $t("common.copySuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("View/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );

export const post = (body: PostView) =>
  request.post<string>(useUrlSiteId("View/post"), body);

export const getEdit = (id: string) =>
  request.get<PostView>(useUrlSiteId("View/get"), { id });
