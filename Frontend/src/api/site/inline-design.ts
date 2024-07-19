import type { KeyValue } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;
export interface Update {
  infos: KeyValue[];
  source: string;
}

export async function update(url: string, updates: Update[]) {
  await request.post(
    useUrlSiteId("/InlineEditor/Update"),
    updates,
    { url },
    {
      successMessage: $t("common.saveSuccess"),
    }
  );
}

export async function checkEnterLink(url: string) {
  return await request.post<any>(useUrlSiteId("/InlineEditor/canEnter"), {
    url,
  });
}
