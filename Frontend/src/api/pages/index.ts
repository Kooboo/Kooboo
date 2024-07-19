import type {
  ConvertToTypes,
  DefaultRoute,
  LinkedPage,
  PostPage,
  PostRichPage,
  NodeGroup,
  Page,
  PageName,
  StructureNode,
  TaskId,
  GroupTask,
  GroupTaskStatus,
  SearchParam,
} from "./types";

import { i18n } from "@/modules/i18n";
import { isUniqueRoute } from "@/api/route";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getAll = () => request.get<Page[]>(useUrlSiteId("Page/all", true));

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Page/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getDefaultRoute = () =>
  request.get<DefaultRoute>(useUrlSiteId("Page/DefaultRoute"));

export const defaultRouteUpdate = (body: DefaultRoute) =>
  request.post(useUrlSiteId("Page/DefaultRouteUpdate"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const convertFile = (body: unknown) =>
  request.post(useUrlSiteId("Page/ConvertFile"), body, undefined, {
    headers: { "Content-Type": "multipart/form-data" },
    hiddenError: true,
    successMessage: $t("common.importSuccess"),
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Page/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );

export const pageUrlIsUniqueName = (name: string, oldName?: string) =>
  isUniqueRoute(name, oldName);

export const copy = (body: unknown) =>
  request.post(useUrlSiteId("Page/Copy"), body, undefined, {
    successMessage: $t("common.copySuccess"),
  });

export const post = (body: PostPage) =>
  request.post<string>(useUrlSiteId("Page/post"), body);

export const postRichText = (body: PostRichPage) =>
  request.post<string>(useUrlSiteId("Page/PostRichText"), body);

export const getEdit = (id: string, args?: Record<string, string>) =>
  request.get<PostPage>(useUrlSiteId("Page/GetEdit"), { id, ...args });

export const getPageStructure = (id: string) =>
  request.get<StructureNode[]>(useUrlSiteId("Page/PageStructure"), { id });

export const getDesignTemplate = () =>
  request.get<string>(useUrlSiteId("Page/GetDesignTemplate"), {
    env: import.meta.env.MODE,
  });

export const getPageTree = (includeScript: boolean) =>
  request.get<NodeGroup>(useUrlSiteId("GroupEdit/Tree"), {
    includeScript,
  });

export const searchPageTree = (
  includeScript: boolean,
  SearchParams: SearchParam[]
) =>
  request.post<NodeGroup>(useUrlSiteId("GroupEdit/Search"), {
    SearchParams,
    includeScript,
  });

export const getSiteRunningTask = () =>
  request.get<GroupTask>(useUrlSiteId("GroupEdit/GetSiteRunningTask"));

export const getViewNames = () =>
  request.get<string[]>(useUrlSiteId(`GroupEdit/ViewNames`));

export const getPageNames = () =>
  request.get<PageName[]>(useUrlSiteId(`GroupEdit/PageNames`));

export const getBlockNames = () =>
  request.get<string[]>(useUrlSiteId(`GroupEdit/BlockNames`));

export const getOuterHtml = (pages: LinkedPage[]) =>
  request.post<string>(useUrlSiteId(`GroupEdit/GetOuterHtml`), { pages });

export const getInnerHtml = (pages: LinkedPage[]) =>
  request.post<string>(useUrlSiteId(`GroupEdit/GetInnerHtml`), { pages });

export const getViewBody = (name: string) =>
  request.get<string>(useUrlSiteId(`GroupEdit/GetViewBody?name=${name}`));

export const getHtmlBlockBody = (name: string) =>
  request.get<string>(useUrlSiteId(`GroupEdit/GetHtmlBlockBody?name=${name}`));

export const editOuterHtml = (pages: LinkedPage[], NewBody: string) =>
  request.post<TaskId>(useUrlSiteId("GroupEdit/EditOuterHtml"), {
    pages,
    NewBody,
  });

export const editInnerHtml = (pages: LinkedPage[], NewBody: string) =>
  request.post<TaskId>(useUrlSiteId("GroupEdit/EditInnerHtml"), {
    pages,
    NewBody,
  });

export const deleteDom = (pages: LinkedPage[]) =>
  request.post<TaskId>(useUrlSiteId("GroupEdit/DeleteDom"), { pages });

export const toHtmlBlock = (
  pages: LinkedPage[],
  name: string,
  body: string,
  UseExisting: boolean
) =>
  request.post<TaskId>(useUrlSiteId("GroupEdit/ToHtmlBlock"), {
    pages,
    name,
    body,
    UseExisting,
  });

export const toView = (
  pages: LinkedPage[],
  viewName: string,
  body: string,
  UseExisting: boolean
) =>
  request.post<TaskId>(useUrlSiteId("GroupEdit/ToView"), {
    pages,
    viewName,
    body,
    UseExisting,
  });

export const getTaskStatus = (TaskId: TaskId) =>
  request.get<GroupTaskStatus>(
    useUrlSiteId(`GroupEdit/GetTaskStatus?TaskId=${TaskId}`)
  );
