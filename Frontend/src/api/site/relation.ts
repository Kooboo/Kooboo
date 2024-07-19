import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";

export interface Relation {
  objectId: string;
  remark: string;
  name: string;
  url: string;
}

export const showBy = (id: string, by: string, type: string) =>
  request.get<Relation[]>(
    useUrlSiteId("Relation/ShowBy"),
    {
      id,
      by,
      type,
    },
    { hiddenLoading: true }
  );
