<script lang="ts" setup>
import { site, events } from "./settings";
import GroupPanel from "./group-panel.vue";
import { computed, ref, watch } from "vue";
import { useUrlSiteId } from "@/hooks/use-site-id";
import type { MediaFileItem } from "@/components/k-media-dialog";
import KMediaDialog from "@/components/k-media-dialog";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";

const { t } = useI18n();
const pwaManifest = ref();
const pwaServiceWorker = ref();
const showMediaDialog = ref(false);
const siteStore = useSiteStore();

if (site.value) {
  pwaManifest.value = JSON.parse(site.value.pwa.manifest);
  pwaServiceWorker.value = JSON.parse(site.value.pwa.serviceWorker);
}

events.onPwaSave = (s) => {
  s.pwa.manifest = JSON.stringify(pwaManifest.value);
  s.pwa.serviceWorker = JSON.stringify(pwaServiceWorker.value);
};

const onAdd = () => {
  pwaServiceWorker.value.CacheSettings.push({
    Method: "get",
    Pattern: "",
    ExpiresIn: 0,
  });
};

const onDelete = (key: number) => {
  pwaServiceWorker.value.CacheSettings.splice(key, 1);
};

const icon = computed(() => {
  return useUrlSiteId(pwaManifest.value.icons[0].src);
});

const onSelectedLog = (items: MediaFileItem[]) => {
  if (items.length === 1) {
    for (const i of pwaManifest.value.icons) {
      i.src = items[0].url;
      let height = items[0].height;
      let width = items[0].width;
      if (!height) height = 32;
      if (!width) width = 32;
      i.sizes = `${height}x${width}`;
    }
  }
  showMediaDialog.value = false;
};

//为了使site.value.manifest能够实时变化
watch(
  () => pwaManifest.value,
  () => {
    if (site.value) {
      site.value.pwa.manifest = JSON.stringify(pwaManifest.value);
    }
  },
  {
    deep: true,
  }
);

//为了使site.value.serviceWorker能够实时变化
watch(
  () => pwaServiceWorker.value,
  () => {
    if (site.value) {
      site.value.pwa.serviceWorker = JSON.stringify(pwaServiceWorker.value);
    }
  },
  {
    deep: true,
  }
);
</script>

<template>
  <template v-if="site && pwaManifest">
    <GroupPanel v-model="site.pwa.enable" label="PWA">
      <el-form-item :label="t('common.icon')">
        <div
          class="w-88px h-88px rounded-4px bg-gray flex items-center justify-center"
          @click="showMediaDialog = true"
        >
          <img :src="combineUrl(siteStore.site.baseUrl, icon)" />
        </div>
      </el-form-item>
      <el-form-item :label="t('common.name')">
        <el-input v-model="pwaManifest.name" :placeholder="t('common.name')" />
      </el-form-item>
      <el-form-item :label="t('common.themeColor')">
        <el-input
          v-model="pwaManifest.theme_color"
          :placeholder="t('common.themeColor')"
        />
      </el-form-item>
      <el-form-item :label="t('common.bgColor')">
        <el-input
          v-model="pwaManifest.background_color"
          :placeholder="t('common.bgColor')"
        />
      </el-form-item>
      <el-form-item :label="t('common.startUrl')">
        <el-input
          v-model="pwaManifest.start_url"
          :placeholder="t('common.startUrl')"
        />
      </el-form-item>

      <el-form-item :label="t('common.display')">
        <el-select v-model="pwaManifest.display" class="w-full">
          <el-option value="fullscreen" label="fullscreen" />
          <el-option value="standalone" label="standalone" />
          <el-option value="minimal-ui" label="minimal-ui" />
          <el-option value="browser" label="browser" />
        </el-select>
      </el-form-item>
      <el-form-item label="Service worker">
        <div class="space-y-4">
          <div
            v-for="(item, index) of pwaServiceWorker.CacheSettings"
            :key="index"
            class="flex items-center space-x-4"
          >
            <el-select v-model="item.Method" class="w-120px">
              <el-option value="get" label="GET" />
              <el-option value="put" label="PUT" />
              <el-option value="post" label="POST" />
              <el-option value="delete" label="DELETE" />
              <el-option value="head" label="HEAD" />
              <el-option value="any" label="ALL" />
            </el-select>
            <el-input
              v-model="item.Pattern"
              class="w-220px"
              :placeholder="t('common.exampleUrl')"
            />
            <el-input v-model.number="item.ExpiresIn" class="w-150px">
              <template #append>
                <span class="px-12">{{ t("common.minutes") }}</span>
              </template>
            </el-input>
            <div>
              <IconButton
                circle
                class="text-orange"
                icon="icon-delete"
                :tip="t('common.delete')"
                @click="onDelete(index)"
              />
            </div>
          </div>
          <IconButton
            circle
            class="text-blue"
            icon="icon-a-addto"
            :tip="t('common.add')"
            @click="onAdd"
          />
        </div>
      </el-form-item>
      <KMediaDialog
        v-if="showMediaDialog"
        v-model="showMediaDialog"
        @choose="onSelectedLog"
      />
    </GroupPanel>
  </template>
</template>
