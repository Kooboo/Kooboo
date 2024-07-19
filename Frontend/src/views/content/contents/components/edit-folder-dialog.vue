<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.settings')"
    custom-class="el-dialog--zero-padding"
    @close="handleClose"
  >
    <el-tabs v-model="currentTab">
      <el-tab-pane :label="tabs[0].displayName" :name="tabs[0].value">
        <el-form
          ref="form"
          :model="model"
          :rules="rules"
          label-position="top"
          :validate-on-rule-change="false"
          @submit.prevent
          @keydown.enter="handleSave"
        >
          <el-form-item v-if="isNew" prop="name" :label="t('common.name')">
            <el-input v-model="model.name" data-cy="name" />
          </el-form-item>
          <el-form-item v-else :label="t('common.name')">
            <el-input v-model="model.name" disabled data-cy="name" />
          </el-form-item>
          <el-form-item prop="displayName" :label="t('common.displayName')">
            <el-input v-model="model.displayName" data-cy="display-name" />
          </el-form-item>
          <el-form-item prop="contentTypeId" :label="t('common.contentType')">
            <el-select
              v-model="model.contentTypeId"
              class="w-full"
              :disabled="!isNew"
              data-cy="content-type-dropdown"
            >
              <el-option
                v-for="item in contentTypes"
                :key="item.id"
                :label="item.name"
                :value="item.id"
                data-cy="content-type-opt"
              />
            </el-select>
          </el-form-item>
          <el-form-item v-if="!isContent" :label="t('common.pageSize')">
            <el-select v-model="model.pageSize">
              <el-option
                v-for="item in [10, 30, 50, 100]"
                :key="item"
                :label="item"
                :value="item"
              />
              <el-option :value="999999" :label="t('common.disablePaging')" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="!isContent" :label="t('common.sortable')">
            <el-switch
              v-model="model.sortable"
              :disabled="!model.contentTypeId"
            />
          </el-form-item>
          <el-form-item
            v-if="model.sortable"
            prop="sortField"
            :label="t('common.sortField')"
          >
            <el-row :gutter="20" class="w-full">
              <el-col :span="12">
                <el-select v-model="model.sortField">
                  <el-option
                    v-for="item in columns"
                    :key="item.name"
                    :label="item.displayName"
                    :value="item.name"
                    data-cy="content-type-opt"
                  />
                  <el-option
                    value="dragAndDrop"
                    :label="t('common.dragAndDrop')"
                  />
                </el-select>
              </el-col>
              <el-col :span="12">
                <el-switch
                  v-model="model.ascending"
                  :disabled="model.sortField === 'dragAndDrop'"
                  :active-text="t('common.ascending')"
                  :inactive-text="t('common.descending')"
                />
              </el-col>
            </el-row>
          </el-form-item>
          <el-form-item :label="t('common.hidden')">
            <el-switch v-model="model.hidden" />
          </el-form-item>
        </el-form>
      </el-tab-pane>
      <el-tab-pane :label="tabs[1].displayName" :name="tabs[1].value">
        <EditRelationFolder
          ref="relationFolder"
          :current="model"
          :folders="folders"
        />
      </el-tab-pane>
    </el-tabs>

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'contentType',
          action: 'edit',
        }"
        @confirm="handleSave"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type {
  ContentFolder,
  RelationFolder,
} from "@/api/content/content-folder";
import { post, isUniqueName } from "@/api/content/content-folder";
import type { ContentTypeItem } from "@/api/content/content-type";
import type { Property } from "@/global/control-type";
import { getContentType, getList } from "@/api/content/content-type";
import {
  requiredRule,
  rangeRule,
  isUniqueNameRule,
  letterAndDigitStartRule,
} from "@/utils/validate";
import EditRelationFolder from "./edit-relation-folder.vue";
import type EditRelationFolderType from "./edit-relation-folder.vue";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  current?: ContentFolder;
  folders: ContentFolder[];
  isContent?: boolean;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);
const tabs = [
  {
    displayName: t("common.basicInfo"),
    value: "basic",
  },
  {
    displayName: t("common.relationFolders"),
    value: "relation",
  },
] as const;

type TabType = typeof tabs[number]["value"];
const currentTab = ref<TabType>("basic");
const contentTypes = ref<ContentTypeItem[]>();
const form = ref<InstanceType<typeof ElForm>>();
const relationFolder = ref<InstanceType<typeof EditRelationFolderType>>();
const model = ref<ContentFolder>({} as ContentFolder);
const rules = computed<Rules>(() => {
  const ruleMaps = {
    name: [
      requiredRule(t("common.fieldRequiredTips")),
      rangeRule(1, 50),
      letterAndDigitStartRule(),
      isUniqueNameRule(isUniqueName, t("common.folderNameExistsTips")),
    ],
    contentTypeId: [requiredRule(t("common.fieldRequiredTips"))],
  } as Rules;
  if (model.value.sortable) {
    ruleMaps["sortField"] = [requiredRule(t("common.fieldRequiredTips"))];
  }
  return ruleMaps;
});

const columns = ref<Property[]>([]);

const isNew = ref(true);
watch(
  () => visible.value,
  (val) => {
    if (val) {
      fetchContentTypes();
      form.value?.resetFields();
      (relationFolder.value as any)?.form?.resetFields();

      currentTab.value = "basic";
      if (props.current) {
        model.value = props.current;
        isNew.value = false;
      } else {
        isNew.value = true;
        model.value = {
          pageSize: 30,
          category: [] as RelationFolder[],
          embedded: [] as RelationFolder[],
        } as ContentFolder;
      }
    }
  }
);

watch(
  () => model.value?.contentTypeId,
  async (contentTypeId) => {
    if (!contentTypeId) {
      columns.value = [];
      return;
    }

    const { properties } = await getContentType({ id: contentTypeId });
    columns.value = properties || [];
  }
);

watch(
  () => model.value.sortable,
  (sortable) => {
    if (sortable && !model.value.sortField) {
      model.value.sortField = columns.value[0]?.name;
    }
  }
);

watch(
  () => model.value.sortField,
  (sortField) => {
    if (sortField === "dragAndDrop") {
      model.value.ascending = false;
    }
  }
);

async function fetchContentTypes() {
  if (contentTypes.value?.length) {
    return;
  }
  contentTypes.value = await getList();
}

async function handleSave() {
  try {
    await form.value?.validate();
  } catch (error) {
    currentTab.value = "basic";
    throw error;
  }
  try {
    await (relationFolder.value as any)?.form?.validate();
  } catch (error) {
    currentTab.value = "relation";
    throw error;
  }

  await post({
    id: model.value.id,
    displayName: model.value.displayName,
    name: model.value.name,
    contentTypeId: model.value.contentTypeId,
    category: model.value.category?.map((folder) => {
      folder.display = folder.display || folder.alias;
      return folder;
    }),
    embedded: model.value.embedded?.map((folder) => {
      folder.display = folder.display || folder.alias;
      return folder;
    }),
    hidden: model.value.hidden,
    sortable: model.value.sortable,
    sortField: model.value.sortable ? model.value.sortField : "",
    ascending: model.value.ascending,
    pageSize: model.value.pageSize,
    isContent: props.isContent,
  });
  handleClose();
  emits("create-success");
}
</script>
