import { BuiltinWidgets } from "../../constants";
import type { VeWidgetType } from "../../types";
import { computed } from "vue";

const widgetMaps = import.meta.globEager("./ve-*.ts");

const widgets = computed(() => {
  const components: VeWidgetType[] = [];
  BuiltinWidgets.forEach((key) => {
    const w = widgetMaps[`./ve-${key}.ts`];
    if (!w) return;
    const item: VeWidgetType = new w.default();

    components.push(item);
  });
  return components;
});

export function useBuiltinWidgets() {
  return {
    widgets,
  };
}
