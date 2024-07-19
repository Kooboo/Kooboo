import { camelCase, isEmpty } from "lodash-es";
import { categoryLabels, productLabels, useLabels } from "./useLabels";

import type { CustomField } from "@/api/commerce/settings";
import type { Ref } from "vue";
import type { SummaryColumn } from "@/components/dynamic-columns/index.vue";
import { computedAsync } from "@vueuse/core";
import { ignoreCaseEqual } from "@/utils/string";
import { useCommerceStore } from "@/store/commerce";

type GetColumnType = Pick<
  SummaryColumn,
  "name" | "displayName" | "attrs" | "prop"
> & {
  controlType?: string;
};

function useFields(key: string, labels: Record<string, string>) {
  const commerceStore = useCommerceStore();
  async function getFields(retry = 0) {
    let data: CustomField[] = [];
    if (ignoreCaseEqual(key, "productCustomFields")) {
      data = commerceStore.settings.productCustomFields;
    } else if (ignoreCaseEqual(key, "categoryCustomFields")) {
      data = commerceStore.settings.categoryCustomFields;
    }

    if (isEmpty(data) && retry < 3) {
      await commerceStore.loadSettings();
      return await getFields(retry++);
    }
    return data;
  }

  const fields = computedAsync(async () => {
    const data = await getFields();
    return data ?? [];
  }, []);

  const customFields = computedAsync(async () => {
    const data = await getFields();
    return (data ?? []).filter((it) => !it.isSystemField);
  }, []);
  const { getDisplayName } = useLabels(labels);

  function getColumns(
    columns: GetColumnType[],
    attrsAction?: (key: string) => Record<string, any>
  ): Ref<SummaryColumn[]> {
    return computedAsync(() => {
      return columns.map((c) => {
        const field = fields.value.find((f) => ignoreCaseEqual(f.name, c.name));
        const column: SummaryColumn = {
          name: camelCase(c.name),
          prop: c.prop,
          displayName: getDisplayName(
            c.name,
            c.displayName || field?.displayName
          ),
          controlType: c.controlType ?? field?.type ?? "TextBox",
          multipleValue: field?.multiple ?? false,
          attrs: {
            ...c.attrs,
            ...attrsAction?.call(field, c.name),
          },
        };
        return column;
      });
    }, []);
  }

  return {
    fields,
    customFields,
    getColumns,
  };
}

export function useProductFields() {
  return useFields("productCustomFields", productLabels);
}

export function useCategoryFields() {
  return useFields("categoryCustomFields", categoryLabels);
}
