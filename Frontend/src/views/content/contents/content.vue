<template>
  <div class="p-32 pb-150px">
    <div class="rounded-normal bg-fff py-24 px-56px dark:bg-[#252526]">
      <EditContent
        v-if="!reload"
        :id="id"
        ref="editContent"
        :folder-id="folderId"
      />
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'content',
      action: 'edit',
    }"
    @cancel="onCancel"
    @save="onSave"
  >
    <template #extra-buttons>
      <template v-if="!isContent">
        <el-button
          v-if="isNew"
          v-hasPermission="{
            feature: 'content',
            action: 'edit',
          }"
          round
          type="primary"
          @click="saveAndCreate"
          >{{ t("common.saveAndCreate") }}</el-button
        >

        <el-button
          v-else
          v-hasPermission="{
            feature: 'content',
            action: 'edit',
          }"
          round
          type="primary"
          data-cy="save-and-return"
          @click="saveAndReturn"
          >{{ t("common.saveAndReturn") }}</el-button
        >
      </template>
    </template>
  </KBottomBar>
</template>

<script lang="ts" setup>
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import { computed, nextTick, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import EditContent from "./components/edit-content.vue";
import type EditContentType from "./components/edit-content.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const router = useRouter();
const folderId = getQueryString("folder") as string;
const id = ref(getQueryString("id"));
const copy = ref(getQueryString("copy"));
const isContent = getQueryString("isContent") == "true";

const editContent = ref<InstanceType<typeof EditContentType>>();
const reload = ref(false);

async function save() {
  const content = await (editContent.value as any)?.save();
  return content;
}

useShortcut("save", onSave);

const isNew = computed(() => !id.value || copy.value);

const route = useRoute();
async function onSave() {
  if (isNew.value || isContent) {
    await saveAndReturn();
  } else {
    await save();
  }
}

function onCancel() {
  goBackOrToPage();
}

async function saveAndReturn() {
  await save();
  goBackOrToPage();
}

async function saveAndCreate() {
  await save();
  delete route.query.copy;
  delete route.query.id;
  id.value = undefined;
  copy.value = undefined;
  router.replace({
    name: route.name as string,
    query: {
      ...route.query,
    },
  });
  reload.value = true;
  nextTick(() => (reload.value = false));
}

function goBackOrToPage() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "textcontentsbyfolder",
      query: {
        folder: folderId,
      },
    })
  );
}
</script>
