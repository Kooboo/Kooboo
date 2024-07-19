import type { Meta, VeWidgetPropDefine, VeWidgetType } from "../../types";
import { setStyles, toStyles } from "../../render/utils";
import { reactive, watch, type Ref } from "vue";
import { isClassic } from "../../utils";
import { ignoreCaseContains } from "@/utils/string";

const containerBorderProps = [
  "veContainerBorderColor",
  "veContainerBorderRadius",
  "veContainerBorderStyle",
  "veContainerBorderWidth",
];

export default abstract class WidgetBase implements VeWidgetType {
  abstract buildPropDefines(): VeWidgetPropDefine[];

  getPropDefines(props: VeWidgetPropDefine[]) {
    if (isClassic()) {
      return props.filter(
        (it) =>
          !ignoreCaseContains(
            [
              "linkHoverColor",
              "linkHoverBackgroundColor",
              "linkHoverUnderline",
              "linkUnderline",
            ],
            it.name
          )
      );
    }
    return props;
  }

  constructor(id: string, displayName: string) {
    this.id = id;
    this.name = displayName;
    this.settings = {};
    this.icon = `icon-ve-${id}`;
    this.tagName = "div";

    const propDefines = reactive(this.getPropDefines(this.buildPropDefines()));

    const props: Record<string, any> = {};
    propDefines.forEach((prop) => {
      props[prop.name] = prop.defaultValue;
    });
    const meta: Meta = {
      id: id,
      name: id,
      props,
      propDefines,
      type: id,
      children: [],
      htmlStr: "",
    };
    this.meta = meta;
    this.tooltip = displayName;
  }

  setStyles(el: HTMLElement, props: Record<string, any>) {
    this.fixedStyles && setStyles(el, this.fixedStyles);
    const styles = toStyles(props, this.meta.propDefines);
    setStyles(el, styles);
  }

  init(props: Ref<Record<string, any>>): void {
    watch(
      () => containerBorderProps.map((it) => props.value[it]),
      () => {
        if (!props.value["veContainerBorderStyle"]) {
          props.value["veContainerBorderStyle"] = "solid";
        }
      },
      {
        deep: true,
      }
    );
  }

  abstract render(meta: Meta): Promise<string>;

  async renderClassic(meta: Meta): Promise<string> {
    return await this.render(meta);
  }

  meta: Meta;

  id: string;
  name: string;
  icon?: string;
  tooltip: string;
  settings: Record<string, string>;
  tagName: string;
  fixedStyles?: Record<string, string>;
}
