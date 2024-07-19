import type { Meta, VeWidgetPropDefine } from "../types";
import {
  createBackgrounds,
  createBorderProps,
  createJustifyContent,
  createPropDefine,
} from "./prop";

import { get } from "lodash-es";
import { i18n } from "@/modules/i18n";

function _getSkipContainerFields() {
  const skipContainerFields: Record<string, string[]> = {
    divider: [
      "veContainerBackgroundColor",
      "veContainerBackgroundImage",
      "veContainerBackgroundRepeat",
      "veContainerBackgroundPosition",
      "veContainerBorderColor",
      "veContainerBorderRadius",
      "veContainerBorderStyle",
      "veContainerBorderWidth",
      "veContainerMargin",
    ],
    column: ["veContainerJustifyContent"],
    paragraph: ["veContainerJustifyContent"],
    row: [
      "veContainerJustifyContent",
      "veContainerMargin",
      "veContainerPadding",
    ],
  };
  return skipContainerFields;
}

function _overrideContainerPropDefines(meta: Meta, prop: VeWidgetPropDefine) {
  const overrides: Record<
    string,
    Record<string, Partial<VeWidgetPropDefine>>
  > = {
    divider: {
      veContainerJustifyContent: {
        defaultValue: "center",
      },
      veContainerMargin: {
        defaultValue: JSON.stringify({
          moreOptions: true,
          top: 10,
          bottom: 10,
          left: 0,
          right: 0,
        }),
      },
    },
  };
  const overridePropDefine = get(overrides, `${meta.type}.${prop.name}`);
  if (overridePropDefine) {
    return {
      ...prop,
      ...overridePropDefine,
    };
  }

  return prop;
}

function _getAllContainerPropDefines() {
  const t = i18n.global.t;

  const _containerProps: VeWidgetPropDefine[] = [
    ...createBackgrounds({
      backgroundColor: "veContainerBackgroundColor",
      backgroundImage: "veContainerBackgroundImage",
      backgroundRepeat: "veContainerBackgroundRepeat",
      backgroundPosition: "veContainerBackgroundPosition",
    }),
    createJustifyContent("veContainerJustifyContent"),
    createPropDefine("veContainerMargin", {
      controlType: "side-control",
      displayName: t("ve.margin"),
    }),
    createPropDefine("veContainerPadding", {
      controlType: "side-control",
      displayName: t("ve.padding"),
      defaultValue: JSON.stringify({ moreOptions: false, all: 10 }),
    }),
    ...createBorderProps(
      {
        borderColor: "veContainerBorderColor",
        borderRadius: "veContainerBorderRadius",
        borderStyle: "veContainerBorderStyle",
        borderWidth: "veContainerBorderWidth",
      },
      {
        borderColor: "#CCCCCC",
        borderStyle: "solid",
      }
    ),
  ];

  return _containerProps;
}

export function getContainerPropDefines(meta: Meta) {
  const type = meta.type;
  if (["spacer"].includes(type)) {
    return [];
  }
  const skipContainerFields = _getSkipContainerFields();
  return _getAllContainerPropDefines()
    .filter((it) => {
      if (it.isSystemField) return false;
      if (!skipContainerFields[type]) {
        return true;
      }

      return !skipContainerFields[type].includes(it.name);
    })
    .map((it) => _overrideContainerPropDefines(meta, it));
}
