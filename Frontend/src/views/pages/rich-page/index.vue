<script lang="ts" setup>
import KEditor from "@/components/k-editor/index.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { onBeforeRouteLeave, useRouter } from "vue-router";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { computed, ref, watch } from "vue";
import { emptyGuid } from "@/utils/guid";
import { getQueryString } from "@/utils/url";
import type { PostRichPage } from "@/api/pages/types";
import { toRichPost } from "@/api/pages/types";
import { useShortcut } from "@/hooks/use-shortcuts";
import { usePageStore } from "@/store/page";
import { useSaveTip } from "@/hooks/use-save-tip";
import EditForm from "./edit-form.vue";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
const { t } = useI18n();
const router = useRouter();
const pageStore = usePageStore();
const saveTip = useSaveTip();
const oldUrl = ref();

const model = ref<PostRichPage>({
  id: getQueryString("id") || emptyGuid,
  body: "",
  title: "",
  name: "",
  url: "",
  cacheByVersion: false,
  cacheMinutes: 3,
  enableCache: false,
  cacheQueryKeys: " ",
  published: true,
});

const isEdit = computed(() => model.value.id !== emptyGuid);
const form = ref();

if (isEdit.value && model.value.id) {
  pageStore.getPage(model.value.id).then((rsp) => {
    model.value = toRichPost(rsp);
    saveTip.init(model.value);
    oldUrl.value = model.value.url;
  });
} else {
  saveTip.init(model.value);
  oldUrl.value = model.value.url;
}

const onSave = async () => {
  await form.value.validate();
  const isNewPage = model.value.id === emptyGuid;
  model.value.id = await pageStore.updateRichPage(model.value);
  ElMessage.success(t("common.saveSuccess"));
  oldUrl.value = model.value.url;
  saveTip.init(model.value);
  if (isNewPage) {
    router.replace(
      useRouteSiteId({ name: "rich-page-edit", query: { id: model.value.id } })
    );
  }
};

const onBack = () => {
  router.goBackOrTo(useRouteSiteId({ name: "pages" }));
};

const saveAndReturn = async () => {
  await onSave();
  onBack();
};

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    await saveTip
      .check(model.value)
      .then(() => {
        next();
      })
      .catch(() => {
        next(false);
      });
  }
});
watch(
  () => model.value,
  () => {
    saveTip.changed(model.value);
  },
  {
    deep: true,
  }
);
useShortcut("save", onSave);
</script>

<template>
  <el-scrollbar>
    <div class="w-1120px mx-auto pt-32 pb-40px">
      <div class="rounded-normal bg-fff dark:bg-[#333] shadow-s-10 p-24">
        <EditForm
          ref="form"
          :model="model"
          class="flex justify-between"
          :old-url="oldUrl"
          inline
        >
          <el-form-item
            :label="t('common.content')"
            class="w-full"
            data-cy="content"
          >
            <KEditor v-model="model.body" />
          </el-form-item>
        </EditForm>
      </div>
    </div>
  </el-scrollbar>

  <KBottomBar
    back
    :permission="{ feature: 'pages', action: 'edit' }"
    @cancel="onBack"
    @save="onSave"
  >
    <template #extra-buttons>
      <el-button
        v-hasPermission="{ feature: 'pages', action: 'edit' }"
        round
        type="primary"
        data-cy="save-and-return"
        @click="saveAndReturn"
      >
        {{ t("common.saveAndReturn") }}
      </el-button>
    </template>
  </KBottomBar>
</template>
<style scoped>
:deep(.el-form-item.w-full) {
  margin-right: 0px !important;
}
:deep(.el-form-item.is-required) {
  margin-right: 0px;
}
</style>
