import { cloneDeep } from "lodash-es";

export function postParentMessage(
  data: any,
  event:
    | "sync"
    | "init"
    | "select"
    | "layout"
    | "switch-tab"
    | "copy"
    | "delete"
    | "ready",
  group?: string
) {
  window.parent.postMessage(
    {
      type: `ve-${event}`,
      group: group ?? "",
      item: data ? cloneDeep(data) : null,
    },
    "*"
  );
}
