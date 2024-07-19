<script lang="ts" setup>
import { computed, onMounted, reactive, ref, watch, onUnmounted } from "vue";
import MultilingualSelector from "@/components/multilingual-selector/index.vue";
import {
  rangeRule,
  simpleNameRule,
  requiredRule,
  isUniqueNameRule,
} from "@/utils/validate";
import type { HtmlBlockItem } from "@/api/content/html-block";
import { getHtmlBlock, isUniqueName, update } from "@/api/content/html-block";
import type { ElForm } from "element-plus";
import KEditor from "@/components/k-editor/index.vue";
import { useMultilingualStore } from "@/store/multilingual";
import { emptyGuid } from "@/utils/guid";
import type { Rules } from "async-validator";
import { useI18n } from "vue-i18n";
import { onBeforeRouteLeave } from "vue-router";
import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";

const { t } = useI18n();
const multilingualStore = useMultilingualStore();
const form = ref<InstanceType<typeof ElForm>>();
const siteStore = useSiteStore();
const saveTip = useSaveTip();

const props = defineProps<{
  id: string;
}>();

const model = reactive<HtmlBlockItem>({
  id: emptyGuid,
  name: "",
  values: {},
});

const rules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    rangeRule(1, 50),
    simpleNameRule(),
    isUniqueNameRule(isUniqueName, t("common.valueHasBeenTakenTips")),
  ],
  contentTypeId: [requiredRule(t("common.fieldRequiredTips"))],
} as Rules;

const isNew = computed(() => !props.id);

const multilingualSite = computed(
  () =>
    multilingualStore.visible &&
    Object.keys(multilingualStore.cultures).length > 1
);

onMounted(async () => {
  if (props.id) {
    const response = await getHtmlBlock({ id: props.id, name: props.id });
    model.name = response.name;
    model.id = response.id;
    for (const key in model.values) {
      model.values[key] = response.values[key] || "";
    }
  }
  saveTip.init(model);
});

watch(
  () => multilingualStore.cultures,
  (val) => {
    if (val) {
      const defaultCulture = multilingualStore.default;
      const langKeys = Object.keys(val).sort((a, b) => {
        if (a === defaultCulture) {
          return -1;
        }
        if (b === defaultCulture) {
          return 1;
        }
        return 0;
      });
      langKeys.forEach((key) => (model.values[key] = model.values[key] || ""));
    }
  },
  { immediate: true }
);

function getLabel(culture: string) {
  let content = t("common.content");
  if (culture === multilingualStore.default) {
    return multilingualStore.selected.length > 1
      ? content + " -" + ` ${culture}` + " (" + t("common.default") + ")"
      : content;
  } else return content + " -" + ` ${culture}`;
}

function isVisible(culture: string) {
  return multilingualStore.selected.includes(culture);
}

async function save() {
  await form.value?.validate();
  await update(model);
  saveTip.init(model);
}

defineExpose({
  save,
  model,
});

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check(model)
      .then(() => {
        next();
      })
      .catch(() => {
        siteStore.firstActiveMenu = "htmlblocks";
        next(false);
      });
  }
});
watch(
  () => model,
  () => {
    saveTip.changed(model);
  },
  {
    deep: true,
  }
);
</script>

<template>
  <div>
    <div class="flex justify-end mb-8">
      <MultilingualSelector />
    </div>
    <el-form
      ref="form"
      :model="model"
      :rules="rules"
      label-width="auto"
      @submit.prevent
    >
      <el-form-item v-if="!isNew" :label="t('common.name')">
        {{ model.name }}
      </el-form-item>
      <el-form-item v-else prop="name" :label="t('common.name')">
        <el-input v-model="model.name" />
      </el-form-item>
      <el-form-item
        v-for="(_value, key) in model.values"
        v-show="isVisible(key)"
        :key="key"
        :label="getLabel(key)"
        class="label-right"
      >
        <KEditor v-model="model.values[key]" />
      </el-form-item>
    </el-form>
  </div>
</template>
