import type { DialogInfo, MediaFileItem, MediaFolderItem } from "./";
import {
  combineUrl,
  getQueryString,
  isAbsoluteUrl,
  updateQueryString,
} from "@/utils/url";

import type { ComputedRef } from "vue";
import { inject } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useRouter } from "vue-router";
import { useSiteStore } from "@/store/site";

export interface ListEmits {
  (e: "clickFolder", value: MediaFolderItem): void;
  (e: "editImage", file: MediaFileItem, provider: string): void;
}

export function useMedia(emits: ListEmits) {
  const router = useRouter();
  const dialogInfo = inject<ComputedRef<DialogInfo>>("dialogInfo");
  const siteStore = useSiteStore();

  function getReferencesCount(references?: Record<string, number>): number {
    if (references) {
      const sum = Object.values(references).reduce((a, b) => a + b, 0);
      return sum;
    }
    return 0;
  }
  function handleClickFolder(folder: MediaFolderItem) {
    emits("clickFolder", folder);
  }
  function handleEditImage(file: MediaFileItem, provider: string) {
    if (dialogInfo?.value) {
      emits("editImage", file, provider);
      return;
    }
    router.push(
      useRouteSiteId({
        name: "editImage",
        query: { ...router.currentRoute.value.query, id: file.key, provider },
      })
    );
  }

  function handleCheckFile(file: MediaFileItem, files: MediaFileItem[]) {
    if (dialogInfo?.value.multiple === false) {
      files.forEach((item) => {
        if (item.selected && item !== file) {
          item.selected = false;
        }
      });
    }
  }

  function getPreviewUrl(url: string) {
    if (url) {
      if (isAbsoluteUrl(url)) {
        return url;
      }
      url = updateQueryString(url, {
        siteId: getQueryString("siteId"),
      });
      return combineUrl(siteStore.site.prUrl!, url);
    }
    return url;
  }

  return {
    getReferencesCount,
    handleClickFolder,
    handleEditImage,
    dialogInfo,
    handleCheckFile,
    getPreviewUrl,
  };
}
