import type { Meta, NumberUnit, VeWidgetPropDefine } from "../types";
import { fontFamilies, lengthUnits } from "@/global/style";

import type { Field } from "@/components/field-control/types";
import type { KeyValue } from "@/global/types";
import { computed } from "vue";
import { i18n } from "@/modules/i18n";
import { ignoreCaseEqual } from "@/utils/string";
import { isClassic } from ".";

const t = i18n.global.t;
const actionTypeSelectionOptions = computed(() => {
  const classic = isClassic();
  const linkOptions: KeyValue[] = [
    {
      key: t("ve.openWebsite"),
      value: "openWebsite",
    },
  ];
  if (!classic) {
    linkOptions.push({
      key: t("ve.openPage"),
      value: "openPage",
    });
  }

  linkOptions.push({
    key: t("ve.sendEmail"),
    value: "sendEmail",
  });
  linkOptions.push({
    key: t("ve.callPhone"),
    value: "callPhone",
  });

  if (classic) {
    linkOptions.push({
      key: t("ve.unsubscribe"),
      value: "unsubscribe",
    });
  }
  return linkOptions;
});
export function createPropDefine(
  name: string,
  option?: Partial<VeWidgetPropDefine>
): VeWidgetPropDefine {
  return {
    defaultValue: null,
    required: false,
    name,
    displayName: name,
    controlType: "TextBox",
    dataType: "String",
    isSummaryField: false,
    multipleLanguage: false,
    editable: false,
    order: 0,
    tooltip: null,
    maxLength: 0,
    validations: [],
    isSystemField: false,
    displayInSearchResult: true,
    multipleValue: false,
    selectionOptions: [],
    settings: {},
    ...option,
  };
}

export function createFontWeight(defaultValue = "bold") {
  return createPropDefine("fontWeight", {
    displayName: t("common.fontWeight"),
    controlType: "Selection",
    defaultValue,
    selectionOptions: [
      { key: t("common.normal"), value: "normal" },
      { key: t("common.bold"), value: "bold" },
    ],
    settings: {
      clearable: false,
    },
  });
}

export function createBorderStyle(name?: string, defaultValue?: string) {
  return createPropDefine(name || "borderStyle", {
    displayName: t("ve.borderStyle"),
    controlType: "Selection",
    defaultValue: defaultValue ?? "solid",
    selectionOptions: [
      {
        key: t("common.none"),
        value: "none",
      },
      {
        key: t("ve.solid"),
        value: "solid",
      },
      {
        key: t("ve.dashed"),
        value: "dashed",
      },
      {
        key: t("ve.dotted"),
        value: "dotted",
      },
    ],
    settings: {
      clearable: false,
    },
  });
}

function createFontSize(defaultValue: number | null, defaultUnit?: string) {
  return createPropDefine("fontSize", {
    displayName: t("common.fontSize"),
    controlType: "NumberUnit",
    defaultValue: {
      value: defaultValue,
      unit: defaultUnit ?? "px",
    },
    selectionOptions: lengthUnits,
    settings: {
      value: {
        min: 0,
      },
    },
  });
}

export function createFontFamily() {
  return createPropDefine("fontFamily", {
    displayName: t("common.font"),
    controlType: "Selection",
    defaultValue: ["Arial"],
    selectionOptions: fontFamilies.map((it) => {
      return {
        key: it,
        value: it,
      };
    }),
    settings: {
      "default-first-option": true,
      "allow-create": true,
      filterable: true,
      multiple: true,
    },
  });
}

export function createColorPicker(
  name: string,
  displayName: string,
  options?: Partial<VeWidgetPropDefine>
) {
  return createPropDefine(name, {
    displayName,
    controlType: "VeColorPicker",
    dataType: "String",
    ...options,
    // defaultValue: options?.defaultValue || "rgba(0, 0, 0, 0)",
  });
}

export function createNumber(
  name: string,
  displayName: string,
  option?: Partial<VeWidgetPropDefine>
) {
  return createPropDefine(name, {
    displayName,
    controlType: "Number",
    dataType: "Number",
    settings: {
      min: 0,
    },
    ...option,
  });
}

export function createJustifyContent(name: string, defaultValue = "") {
  return createPropDefine(name, {
    displayName: t("ve.justifyContent"),
    controlType: "Selection",
    defaultValue: defaultValue || "left",
    selectionOptions: [
      {
        key: t("ve.alignLeft"),
        value: "left",
      },
      {
        key: t("ve.alignCenter"),
        value: "center",
      },
      {
        key: t("ve.alignRight"),
        value: "right",
      },
    ],
    settings: {
      clearable: false,
    },
  });
}

export function createTextAlign(defaultValue = "center") {
  return createPropDefine("textAlign", {
    displayName: t("ve.textAlign"),
    controlType: "Selection",
    defaultValue,
    selectionOptions: [
      { key: t("ve.alignLeft"), value: "left" },
      { key: t("ve.alignCenter"), value: "center" },
      { key: t("ve.alignRight"), value: "right" },
    ],
  });
}

export function ensureProp(
  item: Meta,
  name: string,
  propDefine: VeWidgetPropDefine
) {
  if (!item.propDefines) {
    item.propDefines = [];
  }
  const exists = item.propDefines.find((it) => it.name === name);
  if (!exists) {
    item.propDefines.push(propDefine);
  }
}

export function createField(it: VeWidgetPropDefine) {
  const field: Field = {
    lang: "en-US", // TODO
    name: it.name,
    displayName: it.displayName,
    prop: it.name,
    toolTip: it.tooltip ?? "",
    selectionOptions: it.selectionOptions,
    isMultilingual: false,
    // isMultilingual: it.multipleLanguage,
    multipleValue: it.multipleValue,
    controlType: it.controlType,
    settings: it.settings,
    required: it.required,
  };
  return field;
}

export function getLinkUrl(props: Record<string, any>): string | null {
  const {
    actionType,
    link,
    pageLink,
    emailTo,
    phoneNumber,
    emailSubject,
    emailBody,
  } = props;
  let href = link;
  switch (actionType) {
    case "openPage":
      href = pageLink;
      break;
    case "callPhone":
      href = phoneNumber ? `tel:${phoneNumber}` : null;
      break;
    case "sendEmail":
      if (emailTo) {
        const subject = encodeURIComponent(emailSubject || "");
        const body = encodeURIComponent(emailBody || "");
        href = `mailto:${emailTo}?subject=${subject}&body=${body}`;
      } else {
        href = null;
      }
      break;
    case "unsubscribe":
      href = "#";
      break;
    default:
      break;
  }

  return href;
}

export function createLinkElement(
  props: Record<string, any>,
  defaultLink?: string
): HTMLElement {
  const el = document.createElement("a");
  if (ignoreCaseEqual(props.actionType, "unsubscribe")) {
    el.setAttribute("target", "_blank");
    el.setAttribute("href", "Link.Unsubscribe");
    return el;
  }
  const href = getLinkUrl(props) || defaultLink;
  if (href) {
    const { linkTarget } = props;
    el.setAttribute("href", href);
    el.setAttribute("target", linkTarget);
  } else {
    el.setAttribute("href", "#");
  }

  return el;
}

export function createLinkProps() {
  return [
    createPropDefine("actionType", {
      displayName: t("ve.actionType"),
      controlType: "Selection",
      defaultValue: "openWebsite",
      selectionOptions: actionTypeSelectionOptions.value,
      settings: {
        clearable: false,
      },
    }),
    createPropDefine("link", {
      controlType: "LinkValue",
      displayName: t("common.link"),
      settings: {
        "data-type": "actionType",
      },
    }),
    createPropDefine("linkTarget", {
      isSystemField: true,
      defaultValue: "_self",
    }),
    createPropDefine("pageLink", {
      isSystemField: true,
    }),
    createPropDefine("phoneNumber", {
      isSystemField: true,
    }),
    createPropDefine("emailTo", {
      isSystemField: true,
    }),
    createPropDefine("emailSubject", {
      isSystemField: true,
    }),
    createPropDefine("emailBody", {
      isSystemField: true,
    }),
  ];
}

export function createLinkStyleProps(defaultValues?: {
  linkColor?: string;
  linkHoverColor?: string;
  linkBackgroundColor?: string;
  linkHoverBackgroundColor?: string;
  linkUnderline?: boolean;
  linkHoverUnderline?: boolean;
}) {
  return [
    createColorPicker("linkColor", t("ve.linkColor"), {
      defaultValue: defaultValues?.linkColor,
    }),
    createColorPicker("linkHoverColor", t("ve.linkHoverColor"), {
      defaultValue: defaultValues?.linkHoverColor,
    }),
    createColorPicker("linkBackgroundColor", t("ve.linkBackgroundColor"), {
      defaultValue: defaultValues?.linkBackgroundColor,
    }),
    createColorPicker(
      "linkHoverBackgroundColor",
      t("ve.linkHoverBackgroundColor"),
      {
        defaultValue: defaultValues?.linkHoverBackgroundColor,
      }
    ),
    createPropDefine("linkUnderline", {
      displayName: t("ve.linkUnderline"),
      controlType: "Switch",
      defaultValue: defaultValues?.linkUnderline ?? true,
    }),
    createPropDefine("linkHoverUnderline", {
      displayName: t("ve.linkHoverUnderline"),
      controlType: "Switch",
      defaultValue: defaultValues?.linkHoverUnderline ?? true,
    }),
  ];
}

type BorderPropType =
  | "borderStyle"
  | "borderWidth"
  | "borderColor"
  | "borderRadius";
export function createBorderProps(
  names?: {
    borderStyle?: string;
    borderWidth?: string;
    borderColor?: string;
    borderRadius?: string;
  },
  defaultValues?: {
    borderStyle?: string;
    borderWidth?: NumberUnit;
    borderColor?: string;
    borderRadius?: NumberUnit;
  },
  ignoreProps?: BorderPropType[]
) {
  const props: VeWidgetPropDefine[] = [];
  if (!ignoreProps?.includes("borderWidth")) {
    props.push(
      createLengthUnit(
        names?.borderWidth ?? "borderWidth",
        t("ve.borderWidth"),
        {
          defaultValue: defaultValues?.borderWidth ?? {
            value: 0,
            unit: "px",
          },
        },
        ["auto", "%"]
      )
    );
  }
  if (!ignoreProps?.includes("borderStyle")) {
    props.push(
      createBorderStyle(names?.borderStyle, defaultValues?.borderStyle)
    );
  }
  if (!ignoreProps?.includes("borderColor")) {
    props.push(
      createColorPicker(
        names?.borderColor ?? "borderColor",
        t("ve.borderColor"),
        {
          defaultValue: defaultValues?.borderColor,
        }
      )
    );
  }
  if (!ignoreProps?.includes("borderRadius")) {
    props.push(
      createPropDefine(names?.borderRadius ?? "borderRadius", {
        displayName: t("ve.borderRadius"),
        controlType: "NumberUnit",
        defaultValue: defaultValues?.borderRadius ?? {
          value: 0,
          unit: "px",
        },
        selectionOptions: lengthUnits,
        settings: {
          value: {
            min: 0,
          },
        },
      })
    );
  }
  return props;
}

export function createLetterSpacing(defaultValue = 0) {
  return createPropDefine("letterSpacing", {
    displayName: t("ve.letterSpacing"),
    controlType: "NumberUnit",
    defaultValue: {
      value: defaultValue,
      unit: "px",
    },
    selectionOptions: lengthUnits.filter((it) => it.value !== "%"),
  });
}

export type BackgroundProps = {
  backgroundColor?: string;
  backgroundImage?: string;
  backgroundRepeat?: string;
  backgroundPosition?: string;
};

export function createBackgrounds(
  names?: BackgroundProps,
  defaultValues?: BackgroundProps
) {
  const defaultBackgroundRepeat = defaultValues?.backgroundRepeat;
  const defaultBackgroundPosition = defaultValues?.backgroundPosition;
  return [
    createColorPicker(
      names?.backgroundColor ?? "backgroundColor",
      t("ve.backgroundColor"),
      {
        defaultValue: defaultValues?.backgroundColor,
      }
    ),
    createPropDefine(names?.backgroundImage ?? "backgroundImage", {
      displayName: t("ve.backgroundImage"),
      controlType: "MediaFile",
      defaultValue: defaultValues?.backgroundImage ?? "",
    }),
    createPropDefine(names?.backgroundRepeat ?? "backgroundRepeat", {
      displayName: t("ve.backgroundRepeat"),
      controlType: "VeTwoSelection",
      defaultValue: defaultBackgroundRepeat
        ? JSON.parse(defaultBackgroundRepeat)
        : ["repeat", "repeat"],
      selectionOptions: [
        {
          key: t("ve.bgRepeat"),
          value: "repeat",
        },
        {
          key: t("ve.bgNoRepeat"),
          value: "no-repeat",
        },
        {
          key: t("ve.bgRound"),
          value: "round",
        },
        {
          key: t("ve.bgSpace"),
          value: "space",
        },
      ],
    }),
    createPropDefine(names?.backgroundPosition ?? "backgroundPosition", {
      displayName: t("ve.backgroundPosition"),
      controlType: "VeTwoSelection",
      defaultValue: defaultBackgroundPosition
        ? JSON.parse(defaultBackgroundPosition)
        : ["center", "center"],
      selectionOptions: [
        {
          key: t("ve.alignTop"),
          value: "top",
        },
        {
          key: t("ve.alignBottom"),
          value: "bottom",
        },
        {
          key: t("ve.alignCenter"),
          value: "center",
        },
        {
          key: t("ve.alignLeft"),
          value: "left",
        },
        {
          key: t("ve.alignRight"),
          value: "right",
        },
      ],
    }),
  ];
}

export function createLengthUnit(
  name: string,
  displayName: string,
  options?: Partial<VeWidgetPropDefine>,
  ignoreUnits?: string[]
) {
  const selectionOptions: KeyValue[] = [
    { key: t("common.auto"), value: "auto" },
    ...lengthUnits,
  ];
  return createPropDefine(name, {
    settings: {
      noValueUnits: ["auto"],
      value: {
        min: 0,
      },
      unit: {},
    },
    ...options,
    displayName,
    controlType: "NumberUnit",
    defaultValue: options?.defaultValue ?? {
      value: null,
      unit: "auto",
    },
    selectionOptions: selectionOptions.filter(
      (it) => !ignoreUnits?.includes(it.value)
    ),
  });
}

export function createSizeProps(
  names?: { width?: string; height?: string },
  defaultValues?: {
    width?: NumberUnit;
    height?: NumberUnit;
  },
  disableUnits?: {
    width?: string[];
    height?: string[];
  }
) {
  return [
    createLengthUnit(
      names?.width ?? "width",
      t("common.width"),
      {
        defaultValue: defaultValues?.width ?? {
          value: null,
          unit: "auto",
        },
      },
      disableUnits?.width
    ),
    createLengthUnit(
      names?.height ?? "height",
      t("common.height"),
      {
        defaultValue: defaultValues?.height ?? {
          value: null,
          unit: "auto",
        },
      },
      disableUnits?.height ?? ["%"]
    ),
  ];
}

export function createTextProps(defaultValues?: {
  color?: string;
  letterSpacing?: number;
}) {
  return [
    createColorPicker("color", t("common.textColor"), {
      defaultValue: defaultValues?.color,
    }),
    createLetterSpacing(defaultValues?.letterSpacing),
  ];
}

export function createFontProps(defaultValues?: {
  fontSize?: number;
  fontWeight?: "normal" | "bold";
  fontSizeUnit?: "px" | "%" | "em";
}) {
  return [
    createFontSize(
      defaultValues?.fontSize ?? null,
      defaultValues?.fontSizeUnit
    ),
    createFontFamily(),
    createFontWeight(defaultValues?.fontWeight),
  ];
}
