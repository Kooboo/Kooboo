import type { Layout, ListItem, PostLayout } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getLayout = (id: string, args?: Record<string, any>) =>
  request.get<Layout>(useUrlSiteId("Layout/get"), { id, ...args });

export const getList = () =>
  request.get<ListItem[]>(useUrlSiteId("Layout/list", true));

export const copy = (id: string, name: string) =>
  request.post(useUrlSiteId("Layout/Copy"), { id, name }, undefined, {
    successMessage: $t("common.copySuccess"),
  });

export const post = (body: PostLayout) =>
  request.post<string>(useUrlSiteId("Layout/post"), body);

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Layout/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Layout/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );
