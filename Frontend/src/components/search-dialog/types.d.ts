import type { InfiniteResponse } from "@/global/types";

export type SearchHandler = (
  req: SearchRequest
) => Promise<InfiniteResponse<any>>;
