import { removeFolder } from "@/api/site";
import { showDeleteConfirm } from "@/components/basic/confirm";
import type { FolderItem } from "../type";

export type ListEmits = {
  (e: "select", value: FolderItem): void;
  (e: "remove-success", value: FolderItem): void;
  (e: "rename", value: FolderItem): void;
};

export function useFolderList(emits: ListEmits) {
  function handleRename(item: FolderItem) {
    emits("rename", item);
  }
  function handleSelectFolder(item: FolderItem) {
    emits("select", item);
  }
  async function handleDelete(item: FolderItem) {
    try {
      await showDeleteConfirm();
      await removeFolder({ name: item.key });
      emits("remove-success", item);
    } catch {
      void 0;
    }
  }
  function handleActionVisible(visible: boolean, item: FolderItem) {
    item.visibleMore = visible;
  }
  return {
    handleRename,
    handleSelectFolder,
    handleDelete,
    handleActionVisible,
  };
}
