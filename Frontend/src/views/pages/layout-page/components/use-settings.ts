import type { PostPage, Meta } from "@/api/pages/types";
import { buildMetaElement } from "@/components/html-setting/html-meta";
import type { Resource, SortEvent } from "@/global/types";
import { computed } from "vue";
import { useMultilingualStore } from "@/store/multilingual";
import { useStyleStore } from "@/store/style";
import { useScriptStore } from "@/store/script";
import { emptyGuid } from "@/utils/guid";

export function useSettings(page: PostPage) {
  const multilingualStore = useMultilingualStore();
  const styleStore = useStyleStore();
  const scriptStore = useScriptStore();

  if (page.id === emptyGuid) {
    page.metas = [];
  }

  const metas = computed(() => {
    for (const i of page.metas) {
      if (!i.el) {
        i.el = buildMetaElement(i, multilingualStore.default);
      }
    }
    return page.metas;
  });

  const addOrUpdateMeta = (value: Meta) => {
    const oldMeta = page.metas.find((f) => f.el === value.el);
    if (oldMeta) {
      value.el = buildMetaElement(value, multilingualStore.default);
      const position = page.metas.indexOf(oldMeta);
      page.metas.splice(position, 1, value);
    } else {
      value.el = buildMetaElement(value, multilingualStore.default);
      page.metas.push(value);
    }
  };

  const styles = computed(() => {
    const list: Resource[] = [];
    if (!page.styles) return list;

    for (const i of page.styles) {
      const found = styleStore.all.find((s) => s.href === i);
      if (found) {
        list.push({
          content: found.name,
          id: found.id,
        });
      } else {
        list.push({
          content: i,
          id: "",
        });
      }
    }

    return list;
  });

  const deleteStyle = (value: Resource) => {
    const found = styleStore.all.find((s) => s.id === value.id);
    if (found) {
      page.styles = page.styles?.filter((f) => f !== found.href);
    } else {
      page.styles = page.styles?.filter((f) => f !== value.content);
    }
  };

  const insertStyle = (value: string) => {
    const found = styleStore.all.find((s) => s.id === value);
    if (found) {
      page.styles?.push(found.href);
    }
  };

  const sortStyle = (e: SortEvent) => {
    if (!page.styles) return;
    const item = page.styles.splice(e.oldIndex, 1);
    page.styles.splice(e.newIndex, 0, item[0]);
  };

  const scripts = computed(() => {
    const list: Resource[] = [];
    if (!page.scripts) return list;

    for (const i of page.scripts) {
      const found = scriptStore.all.find((s) => s.href === i);
      if (found) {
        list.push({
          content: found.name,
          id: found.id,
          position: "head",
        });
      } else {
        list.push({
          content: i,
          id: "",
          position: "head",
        });
      }
    }

    return list;
  });

  const deleteScript = (value: Resource) => {
    const found = scriptStore.all.find((s) => s.id === value.id);
    if (found) {
      page.scripts = page.scripts?.filter((f) => f !== found.href);
    } else {
      page.scripts = page.scripts?.filter((f) => f !== value.content);
    }
  };

  const insertScript = (value: string) => {
    const found = scriptStore.all.find((s) => s.id === value);
    if (found) {
      page.scripts?.push(found.href);
    }
  };

  const sortScript = (e: SortEvent) => {
    if (!page.scripts) return;
    const item = page.scripts.splice(e.oldIndex, 1);
    page.scripts.splice(e.newIndex, 0, item[0]);
  };

  return {
    metas,
    addOrUpdateMeta,
    styles,
    insertStyle,
    deleteStyle,
    sortStyle,
    scripts,
    insertScript,
    sortScript,
    deleteScript,
  };
}
