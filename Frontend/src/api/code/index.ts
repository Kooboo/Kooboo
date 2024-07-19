import type { BreakLine, Code, DebugSession, PostCode } from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getListByType = (codeType: string) =>
  request.get<Code[]>(useUrlSiteId("Code/ListByType", true), { codeType });

export const getTypes = () =>
  request.get<Record<string, string>>(useUrlSiteId("Code/CodeType"));

export const getEdit = (codeType: string, id: string) =>
  request.get<PostCode>(useUrlSiteId("Code/GetEdit"), { codeType, id });

export const getByName = (name: string) =>
  request.get<PostCode>(
    useUrlSiteId("Code/GetByName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );

export const post = (body: unknown) =>
  request.post<string>(useUrlSiteId("Code/post"), body);

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Code/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Code/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );

export const getSession = () =>
  request.get<DebugSession>(useUrlSiteId("Debugger/GetSession"), undefined, {
    hiddenLoading: true,
    hiddenError: true,
  });

export const startSession = () =>
  request.get(useUrlSiteId("Debugger/StartSession"), undefined, {
    hiddenLoading: true,
  });

export const setBreakPoint = (body: unknown) =>
  request.post<BreakLine[]>(
    useUrlSiteId("Debugger/SetBreakPoint"),
    body,
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const stepCode = (action: string) =>
  request.get<DebugSession>(
    useUrlSiteId("Debugger/Step"),
    { action },
    {
      hiddenLoading: true,
    }
  );

export const executeCode = (jsStatement: string) =>
  request.post(useUrlSiteId("Debugger/execute"), { jsStatement }, undefined, {
    hiddenLoading: true,
  });
