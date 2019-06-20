import { OperationLogItem, OperationLog } from "@/models/OperationLog";
import request from "./request";
import { getPageId } from "@/common/koobooInfo";
import qs from "query-string";

export default async (operationLogItems: Array<OperationLogItem>) => {
  let pageId = getPageId();
  let parsed = qs.parse(parent.location.search);
  let url = `_api/InlineEditor/Update?SiteId=${parsed["SiteId"]}`;
  let operationLog = new OperationLog(operationLogItems, pageId);
  await request.post(url, operationLog);
};
