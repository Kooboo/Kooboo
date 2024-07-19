<script lang="ts" setup>
import type { Relation } from "@/api/site/relation";
import { showBy } from "@/api/site/relation";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { computed, ref } from "vue";
import { openInNewTab } from "@/utils/url";

import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { useSiteStore } from "@/store/site";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import { getDesignerPage } from "../../utils/page";

const { onPreview } = usePreviewUrl();

defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  id: string;
  by: string;
  type: string;
  displayName?: string;
}>();
const { t } = useI18n();

const siteStore = useSiteStore();
const router = useRouter();
const relations = ref<Relation[]>([]);
const show = ref(true);
const realName = computed(() => {
  return props.displayName ? props.displayName : props.by;
});

showBy(props.id, props.by, props.type).then((rsp) => {
  relations.value = rsp;
});
const onEdit = (
  row: Readonly<{
    modelValue: boolean;
    id: string;
    by: string;
    type: string;
    objectId: string;
    extensions: {
      folderId?: string;
      type?: string;
      layout?: string;
    };
  }>
) => {
  let routeName = realName.value + "-edit";
  if (props.by === "textContent" && siteStore.hasAccess("content", "view")) {
    let path = router.resolve(
      useRouteSiteId({
        name: "content",
        query: {
          id: row.objectId,
          folder: row.extensions?.folderId,
        },
      })
    );
    openInNewTab(path.href);
  } else if (
    siteStore.hasAccess(
      realName.value === "page" ? "pages" : realName.value,
      "view"
    )
  ) {
    if (row.extensions?.type === "Designer") {
      const routeRaw = getDesignerPage(row.objectId, row.extensions.layout);
      openInNewTab(router.resolve(routeRaw).href);
      return;
    }
    const path = router.resolve(
      useRouteSiteId({
        name: routeName,
        query: {
          id: row.objectId,
        },
      })
    );
    openInNewTab(path.href);
  }
};
</script>

<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.relation')"
    @closed="$emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="400px">
      <el-table :data="relations" class="el-table--gray">
        <el-table-column :label="t('common.name')">
          <template #default="{ row }">
            <a
              class="cursor-pointer underline"
              data-cy="name"
              @click="onPreview(row.url)"
            >
              {{ row.name }}
            </a>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.remark')">
          <template #default="{ row }">
            <a
              class="cursor-pointer"
              data-cy="remark"
              @click="onPreview(row.url)"
            >
              {{ row.remark }}
            </a>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.edit')" width="80px" align="right">
          <template
            v-if="
              by !== 'dataMethodSetting' &&
              by !== 'resourceGroup' &&
              by !== 'route'
            "
            #default="{ row }"
          >
            <IconButton
              :permission="{
                feature: realName === 'page' ? 'pages' : realName,
                action: 'view',
              }"
              icon="icon-a-writein"
              class="hover:text-blue"
              :tip="t('common.edit')"
              data-cy="edit"
              @click="onEdit(row)"
            />
          </template>
          <template v-else #default>
            <div>-</div>
          </template>
        </el-table-column>
      </el-table>
    </el-scrollbar>
  </el-dialog>
</template>
