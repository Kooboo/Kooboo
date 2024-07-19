<template>
  <el-dialog
    v-model="showDialog"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.relation')"
    @closed="showDialog = false"
  >
    <el-scrollbar max-height="400px">
      <el-table :data="ctx?.contents" class="el-table--gray">
        <el-table-column :label="t('common.name')">
          <template #default="{ row }">
            <a
              class="cursor-pointer underline"
              data-cy="name"
              @click="onPreview(row.url)"
            >
              {{ row.title }}
            </a>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.edit')" width="80px" align="right">
          <template #default="{ row }">
            <IconButton
              :permission="{
                feature: 'content',
                action: 'view',
              }"
              icon="icon-a-writein"
              class="hover:text-blue"
              :tip="t('common.edit')"
              data-cy="edit"
              @click="onEdit(row)"
            />
          </template>
        </el-table-column>
      </el-table>
    </el-scrollbar>
  </el-dialog>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { TextContentUsedBy } from "@/api/content/textContent";
import { useRouter } from "vue-router";
import { useSiteStore } from "@/store/site";
import { usePreviewUrl } from "@/hooks/use-preview-url";
const router = useRouter();
const { onPreview } = usePreviewUrl();
const showDialog = ref(false);
const { t } = useI18n();
const ctx = ref<TextContentUsedBy>();

const siteStore = useSiteStore();

const onEdit = (row: any) => {
  if (!siteStore.hasAccess("content", "view")) {
    return;
  }
  const path = router.resolve(
    useRouteSiteId({
      name: "content",
      query: {
        id: row.id,
        folder: ctx.value?.folderId,
      },
    })
  );
  onPreview(path.href);
};

defineExpose({
  show(usedBy: TextContentUsedBy) {
    ctx.value = usedBy;
    showDialog.value = true;
  },
});
</script>
