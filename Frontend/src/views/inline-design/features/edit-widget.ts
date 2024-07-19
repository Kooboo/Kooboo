import { copyWidget, deleteWidget, editWidget } from "../widget";
import { getKoobooBindings, isDesignerWidget } from "../binding";

import type { MenuItem } from "../menus/context-menu.vue";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;

export function active(el: Element) {
  return isDesignerWidget(el);
}

export function getWidgetMenus(el: Element): MenuItem[] | null {
  if (!isDesignerWidget(el)) {
    return null;
  }
  const widgetMenus: MenuItem[] = [
    {
      name: "editWidget",
      display: t("common.edit"),
      icon: "icon-a-writein",
      invoke: invoke,
      immediate: true,
      order: 0,
    },
  ];
  const bindings = getKoobooBindings(el);
  const type = bindings[0]!.type!;
  if (!["section", "column"].includes(type)) {
    widgetMenus.push({
      name: "copyWidget",
      display: t("common.copy"),
      icon: "icon-copy",
      invoke: onCopy,
      immediate: true,
      order: 1,
      confirm: t("common.copyTips"),
    });
    widgetMenus.push({
      name: "deleteWidget",
      display: t("common.remove"),
      icon: "icon-delete",
      invoke: onDelete,
      immediate: true,
      order: 2,
      confirm: t("common.deleteTips"),
    });
  }
  return widgetMenus;
}

export async function onCopy(el?: Element) {
  const bindings = getKoobooBindings(el!);
  const { id } = bindings[0];

  if (!id) {
    return;
  }
  await copyWidget(id);
  location.reload();
}

export async function onDelete(el?: Element) {
  const bindings = getKoobooBindings(el!);
  const { id } = bindings[0];
  if (!id) {
    return;
  }
  await deleteWidget(id);
  location.reload();
}

export async function invoke(el?: Element) {
  const bindings = getKoobooBindings(el!);
  const { id } = bindings[0];
  if (!id) {
    return;
  }
  await editWidget(id);
  location.reload();
}
