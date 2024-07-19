import type { Component, Source, TagObject } from "./types";

import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

export const getSource = (tag: string, id: string) =>
  request.get<Source>(useUrlSiteId("Component/GetSource"), { tag, id });

export const getList = () =>
  request.get<Component[]>(useUrlSiteId("Component/list"));

export const getTagObjects = (tag: string, hiddenLoading?: boolean) =>
  request.get<TagObject[]>(
    useUrlSiteId("Component/TagObjects"),
    { tag },
    {
      hiddenLoading,
    }
  );
