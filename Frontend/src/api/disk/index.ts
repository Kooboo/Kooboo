import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { Disk } from "./types";

export const getDisk = () => {
  return request.get<Disk>(useUrlSiteId("Disk/list"));
};
