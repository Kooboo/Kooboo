import type { Component, TagObject } from "@/api/component/types";
import type { Meta, VeWidgetPropDefine, VeWidgetType } from "./types";
import { computed, ref } from "vue";
import { createPropDefine, ensureProp } from "./utils/prop";
import { flatMap, map } from "lodash-es";
import { getList, getTagObjects } from "@/api/component";

import { resourceIconMaps } from "@/constants/icons";
import { useControlTypes } from "@/hooks/use-control-types";

const { getControlType } = useControlTypes();

const componentList = ref<Component[]>([]);
const tagObjectMaps = ref<Record<string, TagObject[]>>({});

const customWidgets = computed<VeWidgetType[]>(() => {
  const result: VeWidgetType[] = flatMap(componentList.value, (group) => {
    const tagObjects = tagObjectMaps.value[group.tagName];
    if (!tagObjects?.length) {
      return [];
    }

    return map(tagObjects, (tagObject) => {
      const propDefines: VeWidgetPropDefine[] = (
        tagObject.propDefines ?? []
      ).map((it) => {
        if (typeof it.selectionOptions === "string") {
          it.selectionOptions = JSON.parse(it.selectionOptions);
        }
        if (typeof it.settings === "string") {
          it.settings = JSON.parse(it.settings);
        }
        if (typeof it.validations === "string") {
          it.validations = JSON.parse(it.validations);
        }

        let value: any = it.defaultValue;
        if (typeof it.controlType === "string") {
          const control = getControlType(it.controlType);
          if (control) {
            if (control.value === "Switch") {
              if (value && typeof value === "string") {
                value = JSON.parse(value.toLowerCase());
              } else {
                value = !!value;
              }
            } else if (
              control.value === "CheckBox" &&
              typeof value === "string"
            ) {
              value = value ? JSON.parse(value) : [];
            } else if (control.value === "Number") {
              value = value === null ? undefined : value;
            }
          }
        }

        it.defaultValue = value;
        return it;
      });
      const meta: Meta = {
        id: tagObject.id,
        name: tagObject.name,
        propDefines,
        children: [],
        htmlStr: "",
        props: {},
        type: group.tagName,
        attribute: group.attribute,
      };

      if (group.requireEngine) {
        ensureProp(
          meta,
          "engine",
          createPropDefine("engine", {
            isSystemField: true,
            defaultValue: group.engineName,
          })
        );
      }
      const widget: VeWidgetType = {
        id: tagObject.id,
        name: tagObject.name,
        settings: tagObject.settings,
        // displayName: tagObject.name,
        tooltip: `${group.displayName}: ${tagObject.name}`,
        // engineName: group.engineName,
        // requireEngine: group.requireEngine,
        // tagName: group.tagName,
        // attribute: group.attribute,
        icon: resourceIconMaps[group.tagName.toLowerCase()] ?? "icon-debugging",
        meta,
      };
      return widget;
    });
  });
  return result;
});

const initCustomWidgets = async (classic?: boolean) => {
  if (classic) {
    return;
  }
  componentList.value = await getList();
  for (const group of componentList.value) {
    if (group.tagName === "Layout") {
      continue;
    }
    tagObjectMaps.value[group.tagName] = await getTagObjects(
      group.tagName,
      true
    );
  }
};
export function useCustomWidgetEffects() {
  return {
    initCustomWidgets,
    customWidgets,
  };
}
