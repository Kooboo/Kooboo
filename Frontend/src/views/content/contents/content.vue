<template>
  <div class="p-32 pb-150px">
    <div class="rounded-normal bg-fff py-24 px-56px dark:bg-[#252526]">
      <EditContent
        v-if="!reload"
        :id="id"
        ref="editContent"
        :folder-id="folderId"
        @data-load="editData = $event"
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

        <el-dropdown
          v-if="editData?.previewUrl"
          placement="top"
          trigger="click"
          :disabled="!siteStore.site.enableMultilingual"
          @command="saveAndPreview"
        >
          <el-button
            v-hasPermission="{
              feature: 'content',
              action: 'edit',
            }"
            round
            type="primary"
            data-cy="save-and-preview"
            @click="!siteStore.site.enableMultilingual && saveAndPreview()"
            >{{ t("common.saveAndPreview") }}</el-button
          >
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item
                v-for="(value, key) of siteStore.site.culture"
                :key="key"
                :command="key"
                >{{ value }}</el-dropdown-item
              >
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </template>
    </template>
  </KBottomBar>
</template>

<script lang="ts" setup>
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useRouteSiteId } from "@/hooks/use-site-id";
import {
  combineUrl,
  getQueryString,
  openInNewTab,
  praseBracket,
} from "@/utils/url";
import { computed, nextTick, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import EditContent from "./components/edit-content.vue";
import type EditContentType from "./components/edit-content.vue";

import { useI18n } from "vue-i18n";
import type { EditContentResponse } from "@/api/content/textContent";
import { useSiteStore } from "@/store/site";
const { t } = useI18n();
const router = useRouter();
const folderId = getQueryString("folder") as string;
const id = ref(getQueryString("id"));
const copy = ref(getQueryString("copy"));
const isContent = getQueryString("isContent") == "true";
const editData = ref<EditContentResponse>();
const editContent = ref<InstanceType<typeof EditContentType>>();
const reload = ref(false);
const siteStore = useSiteStore();

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

async function saveAndPreview(culture?: string) {
  const result = await save();
  if (isNew.value) {
    id.value = result.id;
    router.replace({
      name: route.name as string,
      query: {
        ...route.query,
        id: result.id,
      },
    });
  }
  if (!editData.value?.previewUrl) return;
  var previewUrl = editData.value.previewUrl;
  const paras: string[] = [];
  praseBracket(previewUrl, paras);
  const dic = result.values[culture || siteStore.site.defaultCulture];
  for (const i of paras) {
    const value = dic[i];
    previewUrl = previewUrl.replace(`{${i}}`, value);
  }
  var url = new URL(combineUrl(siteStore.site.baseUrl, previewUrl));
  url.searchParams.set("include-unpublished-content", "true");
  if (culture) url.searchParams.set("lang", culture);
  openInNewTab(url.toString());
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
