import type { Meta } from "../../types";
import { initMeta } from "../../utils/widget";
import { preview } from "../../render";
import { useInjectGlobalStore } from "./inject-global-store";

export async function beforeDrop(widget: Meta) {
  initMeta(widget);

  const { renderContext } = useInjectGlobalStore();
  widget.htmlStr = await preview(widget, renderContext.value);
}
