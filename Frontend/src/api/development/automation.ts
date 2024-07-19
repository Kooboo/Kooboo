import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
export type Automation = {
  description: string;
  id: number | string;
  name: string;
  siteModification: boolean;
};
export const getAutomationTask = () =>
  request.get<Automation[]>("Automation/list");
export const ExecuteTask = (TaskId: string) =>
  request.post<string>(
    useUrlSiteId("Automation/ExecuteTask"),
    { TaskId },
    undefined,
    {
      hiddenLoading: true,
    }
  );
export const getStatus = (sessionId: string) =>
  request.post<{
    idEnd: boolean;
    sessionId: string;
    results: [{ name: string; description: string }];
  }>(
    useUrlSiteId("Automation/GetStatus"),
    {
      sessionId,
    },
    undefined,
    {
      hiddenLoading: true,
    }
  );
