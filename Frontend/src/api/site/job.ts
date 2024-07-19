import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface Job {
  id: string;
  startTime: string;
  name: string;
  frequence: number;
  frequenceUnit: number;
  repeat: boolean;
  code: string;
  active: boolean;
  finish: boolean;
}

export interface Log {
  description: string;
  executionTime: string;
  jobName: string;
  message: string;
  success: boolean;
  webSiteId: string;
}

export const getList = () => request.get<Job[]>(useUrlSiteId("Job/list"));

export const getLogs = (success: boolean) =>
  request.get<Log[]>(useUrlSiteId("Job/Logs"), { success });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Job/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const post = (body: unknown) =>
  request.post(useUrlSiteId("Job/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const run = (id: string) =>
  request.get(
    useUrlSiteId("Job/Run"),
    { id },
    {
      successMessage: $t("common.jobStartRunning"),
    }
  );

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Job/isUniqueName"),
    { name },
    { hiddenError: true, hiddenLoading: true }
  );
