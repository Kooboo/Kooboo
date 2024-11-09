import type {
  ByFolderQueryParams,
  SearchQueryParams,
  TextContentCategoryOptionItem,
  TextContentColumn,
  TextContentItem,
} from "@/api/content/textContent";
import { Search, getByFolder } from "@/api/content/textContent";
import { camelCase, isEmpty, omitBy } from "lodash-es";
import { reactive, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import type { Pagination } from "@/components/k-table/types";
import type { SortSetting } from "@/global/types";
import type { TableRowItem } from "./types";
import dayjs from "dayjs";
import { searchDebounce } from "@/utils/url";
import { useTime } from "@/hooks/use-date";

export function useContentEffects(
  folderId: string,
  inDialog?: boolean,
  exclude?: string[]
) {
  const list = ref<TableRowItem[]>([]);
  const rawList = ref<any[]>([]);
  const columnLoaded = ref(false);
  const columns = ref<TextContentColumn[]>([]);
  const router = useRouter();
  const route = useRoute();
  const pagination = reactive<Pagination>({
    currentPage: 1,
    pageCount: 0,
    pageSize: 30,
  });
  const currentKeyword = ref("");
  const keywords = ref("");
  const searchCategories = ref<Record<string, string[]>>({});
  const categoryOptions = ref<TextContentCategoryOptionItem[]>([]);

  const sortSetting = ref<SortSetting<TableRowItem>>({
    prop: "",
    order: "ascending",
  });

  async function searchEvent() {
    currentKeyword.value = keywords.value.trim();
    await fetchList(1, pagination.pageSize);
  }
  const search = searchDebounce(searchEvent, 1000);
  watch(
    () => keywords.value,
    () => {
      search();
    }
  );

  function queryData(pageNr: number, pageSize: number, exclude?: string[]) {
    const categories = omitBy(searchCategories.value, (v) => isEmpty(v));
    if (currentKeyword.value) {
      const searchParams: SearchQueryParams = {
        folderId,
        pageNr: pageNr,
        pageSize: pageSize ?? 30,
        keyword: currentKeyword.value,
        categories,
        exclude: exclude,
      };
      if (sortSetting.value.prop) {
        searchParams.sortField = sortSetting.value.prop;
        searchParams.ascending = sortSetting.value.order === "ascending";
      }
      return Search(searchParams);
    } else {
      const folderParams: ByFolderQueryParams = {
        folderId,
        pageNr: pageNr,
        pageSize: pageSize ?? 30,
        categories,
        exclude: exclude,
      };
      if (sortSetting.value.prop) {
        folderParams.sortField = sortSetting.value.prop;
        folderParams.ascending = sortSetting.value.order === "ascending";
      }
      return getByFolder(folderParams);
    }
  }

  async function fetchList(pageNr?: number, pageSize?: number) {
    if (!pageNr) {
      pageNr = +(route.query.pageNr ?? 1);
    }

    const response = await queryData(
      pageNr,
      pageSize ?? pagination.pageSize ?? 30,
      exclude
    );
    categoryOptions.value = response.categories ?? [];
    columns.value = (response.columns ?? []).map((column) => {
      column.name = camelCase(column.name);
      return column;
    });
    rawList.value = response.list;
    setTableList(response.list, columns.value);
    pagination.currentPage = response.pageNr;
    pagination.pageCount = response.totalPages;
    pagination.pageSize = response.pageSize;

    if (!inDialog) {
      router.replace({
        query: {
          ...route.query,
          pageNr,
        },
      });
    }
  }

  function setTableList(contents: TextContentItem[], columns: any) {
    list.value = contents.map((item) => {
      const row: TableRowItem = {
        id: item.id,
        online: item.online,
        lastModified: item.lastModified,
        usedBy: item.usedBy,
        order: item.order,
      };
      for (const key in item.textValues) {
        let value = item.textValues[key];
        const column = columns.find((f: any) => f.name == camelCase(key));
        if (column?.selectionOptions) {
          try {
            const options = JSON.parse(column.selectionOptions);
            let values = [value];
            if (column.controlType == "CheckBox") {
              values = JSON.parse(value);
            }
            const displayValues = [];
            for (const i of values) {
              const option = options.find((f: any) => f.value == i);
              if (option) displayValues.push(option.key);
              else displayValues.push(i);
            }
            value = displayValues.join(",");
          } catch {
            //
          }
        }
        row[camelCase(key)] = value;
        if (
          value &&
          dayjs(value).isValid() &&
          value === dayjs(value).toISOString()
        ) {
          // format iso date
          row[camelCase(key)] = useTime(value);
        }
      }
      return row;
    });

    columnLoaded.value = true;
  }

  function getOrderStateKey() {
    const { SiteId, folder } = route.query;
    return `${SiteId}_TextContent_${folder}`;
  }

  async function onSortChanged(data: SortSetting<TableRowItem>) {
    sortSetting.value.prop = data.prop ?? "";
    sortSetting.value.order = data.order;
    localStorage.setItem(getOrderStateKey(), JSON.stringify(sortSetting.value));
    await searchEvent();
  }

  function initSortSetting() {
    const lastOrderState = localStorage.getItem(getOrderStateKey());
    if (lastOrderState) {
      const { prop, order } = JSON.parse(lastOrderState);
      sortSetting.value.prop = prop;
      sortSetting.value.order = order;
    }
  }

  initSortSetting();
  return {
    keywords,
    currentKeyword,
    pagination,
    list,
    rawList,
    columns,
    fetchList,
    columnLoaded,
    sortSetting,
    searchCategories,
    categoryOptions,
    onSortChanged,
    initSortSetting,
  };
}
