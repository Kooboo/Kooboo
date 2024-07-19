<script lang="ts" setup>
import { computed, onMounted, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import type { Page } from "@/api/pages/types";
import type { KeyValue } from "@/global/types";
import { getDefaultRoute, defaultRouteUpdate } from "@/api/pages";
import Alert from "@/components/basic/alert.vue";
import { usePageStore } from "@/store/page";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<{ modelValue: boolean; pages: Page[] }>();
const { t } = useI18n();
const show = ref(true);
const pageStore = usePageStore();

const model = ref({
  startPage: "",
  notFound: emptyGuid,
  error: emptyGuid,
});

onMounted(async () => {
  model.value = await getDefaultRoute();
  if (model.value.startPage === emptyGuid && props.pages?.length) {
    model.value.startPage = props.pages[0].id;
  }
});

const onSave = async () => {
  await defaultRouteUpdate(model.value);
  show.value = false;
  pageStore.load();
};

const routerSelections = computed(() => {
  var list: KeyValue[] = [
    {
      key: emptyGuid,
      value: t("common.default"),
    },
  ];

  if (props.pages) {
    for (const page of props.pages) {
      if (page.id === model.value.startPage) continue;
      list.push({
        key: page.id,
        value: page.path,
      });
    }
  }

  return list;
});

watch(
  () => model.value.startPage,
  async () => {
    if (model.value.startPage === model.value.notFound) {
      model.value.notFound = emptyGuid;
    }

    if (model.value.startPage === model.value.error) {
      model.value.error = emptyGuid;
    }
  }
);

// watch(
//   () => model.value.notFound,
//   async () => {
//     if (
//       model.value.notFound !== emptyGuid &&
//       model.value.error === model.value.notFound
//     ) {
//       model.value.error = emptyGuid;
//     }
//   }
// );

// watch(
//   () => model.value.error,
//   async () => {
//     if (
//       model.value.error !== emptyGuid &&
//       model.value.notFound === model.value.error
//     ) {
//       model.value.notFound = emptyGuid;
//     }
//   }
// );
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.routeSetting')"
    @closed="emit('update:modelValue', false)"
  >
    <div>
      <Alert
        :title="t('common.redirectRoutes')"
        :content="t('common.setHomePageAndErrorPage')"
      />
      <div class="px-32 py-24">
        <el-form label-width="90px" label-position="top">
          <el-form-item :label="t('common.homePage')">
            <el-select
              v-model="model.startPage"
              class="w-full"
              data-cy="home-url-dropdown"
            >
              <el-option
                v-for="item of props.pages"
                :key="item.id"
                :label="item.path"
                :value="item.id"
                data-cy="home-url-opt"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('common.page404')">
            <el-select
              v-model="model.notFound"
              class="w-full"
              data-cy="404-url-dropdown"
            >
              <el-option
                v-for="item of routerSelections"
                :key="item.key"
                :label="item.value"
                :value="item.key"
                data-cy="404-url-opt"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('common.errorPage')">
            <el-select
              v-model="model.error"
              class="w-full"
              data-cy="error-url-dropdown"
            >
              <el-option
                v-for="item of routerSelections"
                :key="item.key"
                :label="item.value"
                :value="item.key"
                data-cy="error-url-opt"
              />
            </el-select>
          </el-form-item>
        </el-form>
      </div>
    </div>
    <template #footer>
      <DialogFooterBar @confirm="onSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
