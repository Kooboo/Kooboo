import { getPageId } from "@/kooboo/utils";
import { Update } from "@/kooboo/outsideInterfaces";
import { kvInfo } from "@/common/kvInfo";

export default async (operationLogs: kvInfo[]) => {
  let pageId = getPageId();
  await Update(
    JSON.stringify({
      pageId: pageId,
      updates: operationLogs
    })
  );
};
