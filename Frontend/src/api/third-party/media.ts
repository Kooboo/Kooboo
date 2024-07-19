import type { SearchProvider, SearchRequest } from "./types";

import type { InfiniteResponse } from "@/global/types";
import type { MediaFileItem } from "../content/media";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

export type SearchMediaFileItem = MediaFileItem & {
  alt?: string;
  loading?: boolean;
  status?: boolean;
  selected?: boolean;
};

export type DownloadItem = Pick<
  SearchMediaFileItem,
  "id" | "name" | "url" | "alt"
>;

export const onlineSearch = (req: SearchRequest) =>
  request.get<InfiniteResponse<SearchMediaFileItem>>(
    useUrlSiteId("online-media/SearchOnline"),
    req
  );

export const getSearchProviders = () =>
  request.get<SearchProvider[]>(useUrlSiteId("online-media/Providers"));

export const download = (path: string, file: DownloadItem, provider: string) =>
  request.post(
    useUrlSiteId(`online-media/Download?provider=${provider}`),
    { file },
    { path },
    { hiddenLoading: true }
  );
