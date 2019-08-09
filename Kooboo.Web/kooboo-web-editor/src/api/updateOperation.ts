import { getPageId } from "@/kooboo/utils";
import { Log } from "@/operation/recordLogs/Log";
import { Update } from "@/kooboo/outsideInterfaces";

export default async (operationLogs: Array<Log>) => {
  let pageId = getPageId();
  await Update(
    JSON.stringify({
      pageId: pageId,
      updates: operationLogs
    })
  );
};
