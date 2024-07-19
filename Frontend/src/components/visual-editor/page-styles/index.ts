import type { Meta, VeWidgetPropDefine } from "../types";
import { computed, ref } from "vue";
import {
  createBackgrounds,
  createColorPicker,
  createField,
  createFontFamily,
  createFontWeight,
  createLinkStyleProps,
  createPropDefine,
} from "../utils/prop";
import { getLinkStyles, toStyles } from "../render/utils";
import { ignoreCaseContains, ignoreCaseEqual } from "@/utils/string";

import type { Field } from "@/components/field-control/types";
import { generateCssText } from "@/utils/dom";
import { i18n } from "@/modules/i18n";
import { lengthUnits } from "@/global/style";
import { omit } from "lodash-es";

const t = i18n.global.t;

function getGeneralProps(classic?: boolean) {
  const contentWidthDefault = classic
    ? { value: 600, unit: "px" }
    : { value: 100, unit: "%" };
  const contentWidthSelection = classic
    ? [
        {
          key: "px",
          value: "px",
        },
      ]
    : lengthUnits;
  const generalProps: VeWidgetPropDefine[] = [
    createColorPicker("textColor", t("common.textColor"), {
      defaultValue: "#000000",
    }),
    ...createBackgrounds(undefined, {
      backgroundColor: "#FFFFFF",
      backgroundImage: "",
    }),
    createPropDefine("contentWidth", {
      displayName: t("ve.contentWidth"),
      controlType: "NumberUnit",
      defaultValue: contentWidthDefault,
      selectionOptions: contentWidthSelection,
    }),
    createPropDefine("display", {
      isSystemField: true,
      defaultValue: "flex",
    }),
    createPropDefine("flexDirection", {
      isSystemField: true,
      defaultValue: "column",
    }),
    createPropDefine("alignItems", {
      displayName: t("ve.justifyContent"),
      controlType: "Selection",
      defaultValue: "center",
      selectionOptions: [
        {
          key: t("ve.alignLeft"),
          value: "start",
        },
        {
          key: t("ve.alignCenter"),
          value: "center",
        },
        {
          key: t("ve.alignRight"),
          value: "end",
        },
      ],
    }),
    createFontFamily(),
    createFontWeight("normal"),
  ];
  return generalProps;
}

export function getRootStyleObjects(
  props: Record<string, any>,
  classic: boolean
): {
  [key in "body" | "row" | "link" | "linkHover"]: Record<string, string>;
} {
  const body = toStyles(
    {
      display: "flex",
      "flex-direction": "column",
      ...props,
    },
    getGeneralProps(classic),
    {
      textColor: "color",
      contentWidth: "width",
    }
  );

  const row: Record<string, string> = {
    "font-weight": "inherit",
  };
  const { contentWidth } = props;
  if (contentWidth) {
    row.width = `${contentWidth["value"]}${contentWidth["unit"]}`;
  }

  const { link, hover } = getLinkStyles(props);

  return {
    body,
    row,
    link,
    linkHover: hover,
  };
}

export function getRootStyles(props: Record<string, any>, classic?: boolean) {
  if (!props) {
    return "";
  }
  const styles: string[] = [];

  const { body, row, link, linkHover } = getRootStyleObjects(
    props,
    classic ?? false
  );

  const bodyStyles = omit(body, "width");
  if (!classic) {
    bodyStyles["margin"] = "0";
    bodyStyles["padding"] = "0";
  }
  styles.push(
    `@media only screen and (max-width:768px) {
  img.full-width {max-width: 100% !important;width: 100% !important;}
}`
  );
  styles.push(`* {box-sizing: content-box;}`);
  const bodyCssText = generateCssText(bodyStyles);
  if (bodyCssText) {
    styles.push(`.ve-global {${bodyCssText}}`);
  }

  const linkCssText = generateCssText(link);
  if (linkCssText) {
    styles.push(`a,.ve-global a,.ve-global a[href] {${linkCssText}}`);
  }

  const linkHoverCssText = generateCssText(linkHover);
  if (linkHoverCssText) {
    styles.push(
      `a:hover,.ve-global a:hover,.ve-global a[href]:hover {${linkHoverCssText}}`
    );
  }

  const containerStyles = generateCssText(row);
  if (containerStyles) {
    styles.push(`.ve-row-container {${containerStyles}}`);
  }

  return styles.join("\n ");
}
export function usePageStyles(classic?: boolean) {
  const rootProps = ref<Record<string, any>>({});
  const generalPageProps = getGeneralProps(classic);
  const linkProps: VeWidgetPropDefine[] = createLinkStyleProps();

  function initPageStyles(meta: Meta) {
    if (meta) {
      const props = meta.props ?? {};
      generalPageProps.forEach((it) => {
        rootProps.value[it.name] = props[it.name] ?? it.defaultValue;
      });
      linkProps.forEach((it) => {
        rootProps.value[it.name] = props[it.name] ?? it.defaultValue;
      });
      meta.props = rootProps.value;
    }
  }
  const generalFields = computed<Field[]>(() =>
    generalPageProps
      .filter((it) => !it.isSystemField)
      .map((it) => {
        if (classic && ignoreCaseEqual(it.name, "contentWidth")) {
          it.settings["value"] = {
            max: 800,
            min: 500,
            step: 10,
          };
        }
        return createField(it);
      })
  );

  const linkFields = computed<Field[]>(() =>
    linkProps
      .filter((it) => {
        if (it.isSystemField) {
          return false;
        }
        if (classic) {
          return !ignoreCaseContains(
            [
              "linkHoverColor",
              "linkHoverBackgroundColor",
              "linkHoverUnderline",
            ],
            it.name
          );
        }

        return true;
      })
      .map(createField)
  );

  return {
    initPageStyles,
    generalFields,
    linkFields,
    rootProps,
  };
}
