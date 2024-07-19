import { cloneMeta, findWidget } from "@/components/visual-editor/utils/widget";

import { Completer } from "@/utils/lang";
import type { Meta } from "@/components/visual-editor/types";
import WidgetDialog from "./widget-dialog.vue";
import { cloneDeep } from "lodash-es";
import { ref } from "vue";
import { render } from "@/components/visual-editor/render";
import { usePageDesigner } from "@/components/visual-editor/main";

const { rootMeta, onSave } = usePageDesigner();

const show = ref(false);
const styleDialogRef = ref();
const showStylesDialog = ref(false);
let completer: Completer<void>;
const widget = ref<Meta>();

const editWidget = async (id: string) => {
  completer = new Completer();
  const [meta] = findWidget(rootMeta.value!, (it) => it.id === id);
  if (!meta) {
    completer.reject();
    return;
  }
  widget.value = cloneDeep(meta);
  show.value = true;
  return await completer.promise;
};

const copyWidget = async (id: string) => {
  const [meta, parent] = findWidget(rootMeta.value!, (it) => it.id === id);
  if (!meta || !parent) {
    return;
  }

  const ix = parent.children.findIndex((it) => it.id === meta.id);
  if (ix > -1) {
    const clonedMeta = cloneMeta(meta);
    // id changed, so it should be re-rendered
    clonedMeta.htmlStr = await render(clonedMeta);
    parent.children.splice(ix + 1, 0, clonedMeta);
    await onSave();
  }

  return parent;
};

const deleteWidget = async (id: string) => {
  const [meta, parent] = findWidget(
    rootMeta.value!,
    (it) => it.id === id,
    rootMeta.value
  );
  if (!meta || !parent) {
    return;
  }

  const ix = parent.children.findIndex((it) => it.id === meta.id);
  if (ix > -1) {
    parent.children.splice(ix, 1);
    await onSave();
  }

  return parent;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

const closeStylesDialog = () => {
  showStylesDialog.value = false;
};

export {
  show,
  editWidget,
  copyWidget,
  deleteWidget,
  rootMeta,
  widget,
  close,
  WidgetDialog,
  showStylesDialog,
  styleDialogRef,
  closeStylesDialog,
};
