import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { Info, Panel } from "./types";

export const getData = () =>
  request.get<Record<string, Panel>>(useUrlSiteId(`dashboard/all`));

export const getItem = (name: string, dialog?: string) =>
  request.get<Panel>(
    useUrlSiteId(`dashboard/item`),
    { name, dialog },
    { hiddenLoading: !dialog, hiddenError: !dialog }
  );

export const getDashboardInfo = () =>
  request.get<Info>(useUrlSiteId(`dashboard/info`));
