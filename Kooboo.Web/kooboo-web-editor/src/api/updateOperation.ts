import { getPageId } from "@/kooboo/utils";
import { Update } from "@/kooboo/outsideInterfaces";
import { Log } from "@/operation/Log";

export default async (operationLogs: Log[]) => {
  let pageId = getPageId();
  let logs = Log.simplify(operationLogs);
  // eslint-disable-next-line no-console
  console.log(logs);
  await Update(
    JSON.stringify({
      pageId: pageId,
      updates: logs
    })
  );
};
