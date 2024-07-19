import type { KeyValue, PaginationResponse } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type {
  Difference,
  LogOptions,
  Publish,
  ServerHost,
  PushItem,
  UserPublish,
  SyncSetting,
  PushFeedBack,
  PullFeedBack,
} from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getList = () =>
  request.get<Publish[]>(useUrlSiteId("publish/list"));

export const getOptions = (id: string, type: string) =>
  request.get<LogOptions>(useUrlSiteId("publish/Options"), { id, type });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("publish/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const post = (body: unknown) =>
  request.post(useUrlSiteId("publish/post"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const importBearer = (body: unknown) =>
  request.post(useUrlSiteId("publish/import"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const removeServer = (id: string) =>
  request.post(useUrlSiteId("UserPublish/DeleteServer"), { id }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const updateServer = (body: unknown) =>
  request.post(useUrlSiteId("UserPublish/UpdateServer"), body, undefined, {
    successMessage: $t("common.serverAddSuccess"),
  });

export const getServers = () =>
  request.get<UserPublish[]>(useUrlSiteId("UserPublish/list"));

export const getCooperationBearer = (
  role: string,
  expire: Date,
  siteId: string
) =>
  request.get<string>("UserPublish/CreateCooperationBearer", {
    role,
    expire,
    siteId,
  });

export const getServerHosts = (orgId: string) =>
  request.get<ServerHost[]>(useUrlSiteId("Publish/OrgServerList"), { orgId });

export const getRemoteSites = (remoteUrl: string, orgId: string) =>
  request
    .post<{ id: string; name: string }[]>(
      useUrlSiteId("publish/RemoteSiteList"),
      {
        remoteUrl,
        orgId,
      }
    )
    .then((rsp) => rsp.map<KeyValue>((m) => ({ key: m.id, value: m.name })));

export const getRemoteDomains = (serverUrl: string, orgId: string) =>
  request
    .post<{ id: string; domainName: string }[]>(
      useUrlSiteId("UserPublish/RemoteDomains"),
      {
        serverUrl,
        orgId,
      }
    )
    .then((rsp) =>
      rsp.map<KeyValue>((m) => ({ key: m.id, value: m.domainName }))
    );

export const getPushItems = (params: Record<string, unknown>) =>
  request.get<PushItem[]>(useUrlSiteId("publish/LocalChanges"), params);

export const getLogItems = (
  type: "InItem" | "OutItem",
  params: Record<string, unknown>
) =>
  request.get<PaginationResponse<PushItem>>(
    useUrlSiteId(`publish/${type}`),
    params
  );

export const getIgnoreList = (params: Record<string, unknown>) =>
  request.get<PaginationResponse<PushItem>>(
    useUrlSiteId(`publish/IgnoreList`),
    params
  );

export const push = (id: string, logId?: number) =>
  request.post<PushFeedBack>(
    useUrlSiteId("publish/PushItem"),
    { settingId: id, logId },
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const forcePush = (id: string, logId: number, ignoreCurrent: boolean) =>
  request.post<PushFeedBack>(
    useUrlSiteId("publish/ForcePush"),
    { settingId: id, logId, ignoreCurrent },
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const ignore = (id: string, logs: number[]) =>
  request.post(
    useUrlSiteId("publish/IgnoreLog"),
    { settingId: id, logs },
    undefined,
    {
      successMessage: $t("common.ignoreSuccess"),
    }
  );

export const unIgnore = (id: string, ids: string[]) =>
  request.post(
    useUrlSiteId("publish/UnIgnore"),
    { settingId: id, objectIds: ids },
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const pull = (id: string, senderVersion?: number) =>
  request.post<PullFeedBack>(
    useUrlSiteId("publish/pull"),
    { id, senderVersion },
    undefined
  );

export const forcePull = (
  id: string,
  currentSenderVersion: number,
  senderVersion: number,
  localLogId: number,
  ignoreCurrent: boolean
) =>
  request.post<PullFeedBack>(
    useUrlSiteId("publish/ForcePull"),
    { id, currentSenderVersion, senderVersion, localLogId, ignoreCurrent },
    undefined
  );

export const getSettingDifferences = (id: string) =>
  request.get<Difference[]>(useUrlSiteId("publish/getSettingDifferences"), {
    id,
  });

export const pullSettings = (body: any) =>
  request.post<void>(useUrlSiteId("publish/PullSettings"), body);

export const pushSettings = (id: string, body: any) =>
  request.post<void>(useUrlSiteId("publish/PushSettings"), body, { id });

export const getSyncSetting = (id: string) =>
  request.get<SyncSetting>(useUrlSiteId("publish/GetSyncSetting"), {
    id,
  });

export const getStoreNames = () =>
  request.get<KeyValue[]>(useUrlSiteId("publish/StoreNames"));

export const getProgress = (SyncSettingId: string) =>
  request.get<any>(useUrlSiteId("publish/GetProgress"), { SyncSettingId });

export const setProgress = (SyncSettingId: string, body: any) =>
  request.post<void>(useUrlSiteId("publish/SetProgress"), body, {
    SyncSettingId,
  });

export const updateSyncSetting = (id: string, body: any) =>
  request.post<void>(useUrlSiteId("publish/UpdateSyncSetting"), body, { id });
