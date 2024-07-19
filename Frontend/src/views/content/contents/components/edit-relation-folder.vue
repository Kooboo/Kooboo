<template>
  <el-form
    ref="form"
    :model="model"
    label-position="top"
    class="el-form--label-normal"
    @submit.prevent
  >
    <el-form-item :label="t('common.categoryFolders')">
      <div
        v-for="(item, index) in model.category"
        :key="index"
        class="flex items-center space-x-4 mb-16"
      >
        <el-form-item>
          <el-select v-model="item.folderId" data-cy="category-folder-dropdown">
            <el-option
              v-for="opt in availableFolders(item.folderId)"
              :key="opt.id"
              :label="opt.displayName"
              :value="opt.id"
              data-cy="category-folder-opt"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          :rules="aliasRules"
          :prop="'category.' + index + '.alias'"
        >
          <el-input
            v-model="item.alias"
            :placeholder="t('common.alias')"
            class="w-150px"
            data-cy="category-folder-alias"
          />
        </el-form-item>
        <el-form-item :prop="'category.' + index + '.display'">
          <el-input
            v-model="item.display"
            :placeholder="t('common.displayName')"
            class="w-150px"
          />
        </el-form-item>
        <el-checkbox
          v-model="item.multiple"
          :disabled="item.oldMultiple"
          data-cy="multiple"
          >{{ t("common.multiple") }}</el-checkbox
        >
        <div class="flex-1 text-right">
          <el-button
            circle
            data-cy="remove-category-folder"
            @click="removeCategoryFolders(index)"
          >
            <el-icon class="iconfont icon-delete text-orange" />
          </el-button>
        </div>
      </div>
      <div class="w-full">
        <el-button
          v-if="unSelectedFolderIds.length"
          circle
          data-cy="add-category-folder"
          @click="addCategoryFolders()"
        >
          <el-icon class="iconfont icon-a-addto text-blue" />
        </el-button>
      </div>
    </el-form-item>
    <el-form-item :label="t('common.embeddedFolders')">
      <div
        v-for="(item, index) in model.embedded"
        :key="index"
        class="flex items-center space-x-4 mb-16"
      >
        <el-form-item>
          <el-select
            v-model="item.folderId"
            class="w-full"
            data-cy="embedded-folder-dropdown"
          >
            <el-option
              v-for="opt in availableFolders(item.folderId)"
              :key="opt.id"
              :label="opt.displayName"
              :value="opt.id"
              data-cy="embedded-folder-opt"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          :rules="aliasRules"
          :prop="'embedded.' + index + '.alias'"
        >
          <el-input
            v-model="item.alias"
            :placeholder="t('common.alias')"
            data-cy="embedded-folder-alias"
          />
        </el-form-item>
        <el-form-item :prop="'embedded.' + index + '.display'">
          <el-input
            v-model="item.display"
            :placeholder="t('common.displayName')"
          />
        </el-form-item>
        <div class="flex-1 text-right">
          <el-button
            circle
            data-cy="remove-embedded-folder"
            @click="removeEmbeddedFolders(index)"
          >
            <el-icon class="iconfont icon-delete text-orange" />
          </el-button>
        </div>
      </div>
      <div class="w-full">
        <el-button
          v-if="unSelectedFolderIds.length"
          circle
          data-cy="add-embedded-folder"
          @click="addEmbeddedFolders()"
        >
          <el-icon class="iconfont icon-a-addto text-blue" />
        </el-button>
      </div>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { ElForm, FormItemRule } from "element-plus";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { ContentFolder } from "@/api/content/content-folder";
import { simpleNameRule, requiredRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
interface PropsType {
  current: ContentFolder;
  folders: ContentFolder[];
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
}
defineEmits<EmitsType>();
const props = defineProps<PropsType>();
const { t } = useI18n();

const form = ref<InstanceType<typeof ElForm>>();
const model = ref<ContentFolder>({} as ContentFolder);
const unSelectedFolderIds = computed(() => {
  const folderIds = props.folders
    .filter(
      (item) =>
        !model.value.category.some((x) => x.folderId === item.id) &&
        !model.value.embedded.some((x) => x.folderId === item.id)
    )
    .map((item) => item.id);
  return folderIds;
});
const aliasRules: FormItemRule[] = [
  requiredRule(t("common.fieldRequiredTips")),
  simpleNameRule(),
  {
    asyncValidator(_rule, value, callback) {
      const hasDuplicateALias =
        model.value.category
          .concat(model.value.embedded)
          .filter((item) => item.alias.toLowerCase() === value.toLowerCase())
          .length >= 2;
      if (hasDuplicateALias) {
        callback(Error(t("common.valueHasBeenTakenTips")));
      } else {
        callback();
      }
    },
    trigger: "blur",
  },
];
watch(
  () => props.current,
  (val) => {
    model.value = val;
    model.value.category.forEach((x) => (x.oldMultiple = x.multiple));
  },
  { immediate: true }
);
function availableFolders(id: string) {
  return props.folders.filter(
    (item) =>
      !item.isContent &&
      (item.id === id || unSelectedFolderIds.value.includes(item.id))
  );
}
function removeCategoryFolders(index: number) {
  model.value.category.splice(index, 1);
}
function addCategoryFolders() {
  model.value.category.push({
    folderId: unSelectedFolderIds.value[0],
    alias: "",
    multiple: false,
  });
}
function removeEmbeddedFolders(index: number) {
  model.value.embedded.splice(index, 1);
}
function addEmbeddedFolders() {
  model.value.embedded.push({
    folderId: unSelectedFolderIds.value[0],
    alias: "",
  });
}

defineExpose({
  form,
});
</script>
