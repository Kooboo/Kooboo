import { search } from "@/api/template";
import type { PagedTemplate } from "@/api/template/types";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useTemplateStore = defineStore("templateStore", () => {
  const keyword = ref();
  const category = ref([]);
  const color = ref([]);
  const pagedTemplate = ref<PagedTemplate>({
    pageNr: 1,
    list: [],
    pageSize: 12,
    totalCount: 1,
    totalPages: 1,
  });
  const currentType = ref("");
  const changePage = async (value: number) => {
    pagedTemplate.value.pageNr = value;

    // 只有All和sites才有topics和color的过滤器
    if (!["", "Site"].includes(currentType.value)) {
      category.value = [];
      color.value = [];
    }

    pagedTemplate.value = await search({
      pageNr: pagedTemplate.value.pageNr,
      pageSize: pagedTemplate.value.pageSize,
      typeName: currentType.value,
      keyword: keyword.value?.replaceAll(" ", ""),
      category: Array.isArray(category.value) ? category.value.join(",") : "",
      color: Array.isArray(color.value) ? color.value.join(",") : "",
    });
  };

  const clearTemplate = () => {
    currentType.value = "";
    category.value = [];
    color.value = [];
    keyword.value = "";
    pagedTemplate.value.pageNr = 1;
  };

  const clearAll = () => {
    currentType.value = "";
    category.value = [];
    color.value = [];
    keyword.value = "";
    pagedTemplate.value.pageNr = 1;
    changePage(1);
  };

  const removeCategory = (item: string) => {
    category.value = category.value.filter((i) => i !== item);

    changePage(1);
  };

  const removeColor = (item: string) => {
    color.value = color.value.filter((i) => i !== item);

    changePage(1);
  };

  return {
    pagedTemplate,
    changePage,
    keyword,
    currentType,
    category,
    color,
    clearTemplate,
    clearAll,
    removeCategory,
    removeColor,
  };
});
