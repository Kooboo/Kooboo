<script lang="ts" setup>
import type { DigitalItem } from "@/api/commerce/product";
import { deleteDigitalFile, uploadDigitalFile } from "@/api/commerce/product";
import { ElButton } from "element-plus";
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { bytesToSize } from "@/utils/common";
import { newGuid } from "@/utils/guid";

const { t } = useI18n();
const showTextDialog = ref(false);
const showLinkDialog = ref(false);

const item = ref();
const uploadRef = ref();

const props = defineProps<{
  items: DigitalItem[];
  id: string;
  deleteRemoteFile?: boolean;
}>();

const emit = defineEmits<{
  (e: "update:items", items: DigitalItem[]): void;
}>();

async function uploadFile({ raw }: { raw: File }) {
  const formData = new FormData();
  formData.append("file", raw);
  formData.append("variantId", props.id);
  await uploadDigitalFile(formData);
  emit("update:items", [
    ...props.items,
    {
      id: newGuid(),
      type: "file",
      name: raw.name,
      size: raw.size,
      contentType: raw.type,
      value: props.id,
    },
  ]);
}

async function onDelete(file: DigitalItem) {
  if (file.type == "file" && props.deleteRemoteFile) {
    deleteDigitalFile(file.value, file.name);
  }

  emit(
    "update:items",
    props.items.filter((f) => f != file)
  );
}

function getTypeDisplay(type: string) {
  switch (type) {
    case "file":
      return t("common.file");
    case "text":
      return t("common.text");
    case "link":
      return t("common.link");
    default:
      return type;
  }
}

function onAdd(type: string) {
  switch (type) {
    case "file":
      uploadRef.value.click();
      break;
    case "text":
      item.value = {
        id: newGuid(),
        type: "text",
        name: "",
        value: "",
      };
      showTextDialog.value = true;
      break;
    case "link":
      item.value = {
        id: newGuid(),
        type: "link",
        name: "",
        value: "",
      };
      showLinkDialog.value = true;
      break;
    default:
      break;
  }
}
</script>

<template>
  <div>
    <div class="space-y-8">
      <div class="flex items-center">
        <ElUpload
          class="hidden"
          action=""
          :auto-upload="false"
          :on-change="uploadFile"
          :show-file-list="false"
        >
          <div ref="uploadRef" />
        </ElUpload>
        <ElDropdown trigger="click" @command="onAdd">
          <ElButton type="primary" round>
            <el-icon class="iconfont icon-a-addto" />{{ t("common.add") }}
            <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
          </ElButton>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="file">
                {{ t("common.file") }}
              </el-dropdown-item>
              <el-dropdown-item command="link">{{
                t("common.link")
              }}</el-dropdown-item>
              <el-dropdown-item command="text">{{
                t("common.text")
              }}</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </ElDropdown>
        <div class="flex-1" />
        <slot name="bar" />
      </div>
      <el-table :data="items">
        <el-table-column prop="name" :label="t('common.name')">
          <template #default="{ row }">
            <div :title="row.name" class="truncate">{{ row.name }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.type')">
          <template #default="{ row }">
            <ElTag round type="success">
              {{ getTypeDisplay(row.type) }}
            </ElTag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.detail')">
          <template #default="{ row }">
            <template v-if="row.type == 'file'">
              {{ bytesToSize(row.size) }}
            </template>
            <template v-else>
              <div :title="row.value" class="truncate">{{ row.value }}</div>
            </template>
          </template>
        </el-table-column>
        <el-table-column width="40">
          <template #default="{ row }">
            <el-tooltip placement="top" :content="t('common.delete')">
              <el-icon
                class="iconfont icon-delete hover:text-orange text-l"
                @click="onDelete(row)"
              />
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>
    </div>
    <ElDialog v-model="showTextDialog" :title="t('common.text')">
      <ElForm>
        <ElFormItem :label="t('common.name')">
          <ElInput v-model="item.name" />
        </ElFormItem>
        <ElFormItem :label="t('common.text')">
          <ElInput v-model="item.value" type="textarea" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="showTextDialog = false">{{
          t("common.cancel")
        }}</ElButton>
        <ElButton
          type="primary"
          @click="
            emit('update:items', [...items, item]);
            showTextDialog = false;
          "
          >{{ t("common.ok") }}</ElButton
        >
      </template>
    </ElDialog>
    <ElDialog v-model="showLinkDialog" :title="t('common.link')">
      <ElForm>
        <ElFormItem :label="t('common.name')">
          <ElInput v-model="item.name" />
        </ElFormItem>
        <ElFormItem :label="t('common.link')">
          <ElInput v-model="item.value" type="textarea" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <ElButton @click="showLinkDialog = false">{{
          t("common.cancel")
        }}</ElButton>
        <ElButton
          type="primary"
          @click="
            emit('update:items', [...items, item]);
            showLinkDialog = false;
          "
          >{{ t("common.ok") }}</ElButton
        >
      </template>
    </ElDialog>
  </div>
</template>
