import type {
  SearchPackageFileItem,
  SearchProvider,
  SearchRequest,
} from "./types";

import type { InfiniteResponse } from "@/global/types";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

export type OnlineModule = "script" | "code" | "style" | "module";

export function useOnlineSearchApi(module: OnlineModule) {
  const onlineSearch = (req: SearchRequest) =>
    request.get<InfiniteResponse<SearchPackageFileItem>>(
      useUrlSiteId(`online-${module}/SearchOnline`),
      req
    );

  const tops = () =>
    request.get<any>(useUrlSiteId(`online-${module}/TopPackages`));

  const getSearchProviders = () =>
    request.get<SearchProvider[]>(useUrlSiteId(`online-${module}/Providers`));

  const download = (path: string, file: SearchPackageFileItem) =>
    request.post(
      useUrlSiteId(`online-${module}/Download`),
      { file },
      { path },
      { hiddenLoading: true, hiddenError: true }
    );

  return {
    onlineSearch,
    getSearchProviders,
    download,
    tops,
  };
}
