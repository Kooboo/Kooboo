<script lang="ts" setup>
import { pageUrlIsUniqueName } from "@/api/pages";
import type { KeyValue } from "@/global/types";
import { useMultilingualStore } from "@/store/multilingual";
import { toList, toObject } from "@/utils/lang";
import { isUniqueNameRule, requiredRule } from "@/utils/validate";
import type { ElForm } from "element-plus";
import { ref, watch, computed } from "vue";
import type { Rules } from "async-validator";

import { useI18n } from "vue-i18n";
const multilingualStore = useMultilingualStore();
const props = defineProps<{
  url: string;
  oldUrlPath?: string;
  titles: Record<string, string>;
  metaBindings?: string[];
  urlParamsBindings?: string[];
  published?: boolean;
}>();
const titleList = ref<KeyValue[]>(toList(props.titles));

const emit = defineEmits<{
  (e: "update:url", value: string): void;
  (e: "update:titles", value: Record<string, string>): void;
  (e: "update:defaultTitle", value: string): void;
  (e: "update:published", value: boolean | string | number): void;
}>();
const { t } = useI18n();

const onTitleChanged = () => {
  emit("update:titles", toObject(titleList.value) as Record<string, string>);
  emit(
    "update:defaultTitle",
    titleList.value.find((f) => f.key === multilingualStore.default)?.value ||
      ""
  );
};

const getTitleLabel = (item: KeyValue) => {
  if (item.key === multilingualStore.default) {
    return multilingualStore.selected.length > 1
      ? t("common.contentTitle") +
          " - " +
          item.key +
          " (" +
          t("common.default") +
          ")"
      : t("common.contentTitle");
  } else return t("common.contentTitle") + " - " + item.key;
};

watch(
  () => multilingualStore.selected,
  () => {
    if (!titleList.value) return;
    for (const i of multilingualStore.selected) {
      if (!titleList.value.find((f) => f.key === i)) {
        titleList.value.push({
          key: i,
          value: "",
        });
      }
    }
    onTitleChanged();
  },
  { immediate: true, deep: true }
);
const model = computed(() => ({ url: props.url, published: props.published }));
const rules = computed(
  () =>
    ({
      url: [
        requiredRule(t("common.urlRequiredTips")),
        model.value.url === props.oldUrlPath
          ? ""
          : isUniqueNameRule(
              (name: string) => pageUrlIsUniqueName(name, props.oldUrlPath),
              t("common.urlOccupied")
            ),
      ],
    } as Rules)
);

const form = ref<InstanceType<typeof ElForm>>();
const changeUrl = (item: string) => {
  emit("update:url", model.value.url + "/" + item);
};

const changeTitle = (item: string, metaBinding: string) => {
  if (titleList.value.filter((f) => f.key === item)[0].value === null) {
    titleList.value.filter((f) => f.key === item)[0].value = "";
  }
  titleList.value.filter((f) => f.key === item)[0].value += metaBinding;
  onTitleChanged();
};
defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <div class="px-24 py-16">
    <el-form ref="form" label-position="top" :rules="rules" :model="model">
      <template v-for="item of titleList" :key="item.key">
        <el-form-item
          v-show="multilingualStore.selected.some((s) => s === item.key)"
          :label="getTitleLabel(item)"
        >
          <el-input
            v-model="item.value"
            data-cy="content-title"
            @input="onTitleChanged"
          />
        </el-form-item>
        <div class="flex-wrap mb-8 -mt-4">
          <el-tag
            v-for="i in metaBindings"
            :key="i"
            type="info"
            class="m-2px cursor-pointer"
            data-cy="title-dynamic-field"
            @click="changeTitle(item.key, i)"
            >{{ i }}</el-tag
          >
        </div>
      </template>
      <el-form-item label="URL" prop="url">
        <el-input
          :model-value="model.url"
          data-cy="url"
          @update:model-value="$emit('update:url', $event)"
        />
      </el-form-item>
      <div class="flex-wrap mb-8 -mt-4">
        <el-tag
          v-for="item in urlParamsBindings"
          :key="item"
          type="info"
          class="m-4 cursor-pointer"
          data-cy="url-dynamic-field"
          @click="changeUrl(item)"
          >{{ item }}</el-tag
        >
      </div>
      <el-form-item :label="t('page.online')" class="mb-0">
        <el-switch
          v-model="model.published"
          @update:model-value="$emit('update:published', $event)"
        />
      </el-form-item>
    </el-form>
  </div>
</template>
