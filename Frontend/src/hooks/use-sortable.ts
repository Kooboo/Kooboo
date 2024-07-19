import type { Ref } from "vue";
import type { SortChangeEvent } from "@/global/types";
import { cloneDeep } from "lodash-es";
import { toRaw } from "vue";

export function useSortable<T>(
  list: Ref<T[]>,
  changedCallback: (value: T[]) => void
) {
  return {
    onChange(e: SortChangeEvent) {
      if (typeof changedCallback !== "function") return;
      const clonedList = cloneDeep(toRaw(list.value));
      if (e.moved) {
        clonedList.splice(e.moved.oldIndex, 1);
        clonedList.splice(e.moved.newIndex, 0, e.moved.element);
      } else if (e.added) {
        clonedList.splice(e.added.newIndex, 0, e.added.element);
      } else if (e.removed) {
        clonedList.splice(e.removed.oldIndex, 1);
      }
      changedCallback(clonedList);
    },
  };
}
