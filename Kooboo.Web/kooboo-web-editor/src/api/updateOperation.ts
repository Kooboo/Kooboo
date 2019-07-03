import request from "./request";
import { getPageId } from "@/kooboo/utils";
import qs from "query-string";
import { Log } from "@/operation/recordLogs/Log";

export default async (operationLogs: Array<Log>) => {
  let pageId = getPageId();
  let parsed = qs.parse(parent.location.search);
  let url = `_api/InlineEditor/Update?SiteId=${parsed["SiteId"]}`;
  await request.post(url, {
    pageId: pageId,
    updates: operationLogs
  });
};
