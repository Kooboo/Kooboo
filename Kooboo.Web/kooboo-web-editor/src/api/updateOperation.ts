import { getPageId } from "@/kooboo/utils";
import { Update } from "@/kooboo/outsideInterfaces";
import { Log } from "@/operation/Log";

export default async (operationLogs: Log[]) => {
  let pageId = getPageId();
  await Update(
    JSON.stringify({
      pageId: pageId,
      updates: operationLogs
    })
  );
};
