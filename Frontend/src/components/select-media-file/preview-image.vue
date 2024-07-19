<template>
  <div v-if="url" class="rounded-4px w-161px h-161px mr-32 mb-24">
    <MediaFileItem :src="previewUrl" :advanced="advanced">
      <template #actions>
        <el-tooltip
          placement="top"
          :show-after="300"
          :content="t('common.copyUrl')"
        >
          <span
            class="iconfont icon-copy cursor-pointer hover:text-blue"
            data-cy="copy-in-grid"
            @click.stop="copyText(combineUrl(siteStore.site.baseUrl, url))"
          />
        </el-tooltip>
        <el-tooltip
          v-if="advanced"
          placement="top"
          :show-after="300"
          :content="t('common.editCommonName', { name: t('common.meta') })"
        >
          <span
            class="iconfont icon-a-writein cursor-pointer hover:text-blue"
            data-cy="edit"
            @click.stop="emits('edit')"
          />
        </el-tooltip>
        <el-tooltip
          placement="top"
          :show-after="300"
          :content="t('common.delete')"
        >
          <span
            class="iconfont icon-delete cursor-pointer hover:text-orange"
            data-cy="remove"
            @click.stop="emits('remove')"
          />
        </el-tooltip>
      </template>
    </MediaFileItem>
  </div>
</template>
<script setup lang="ts">
import { useSiteStore } from "@/store/site";
import { combineUrl } from "@/utils/url";
import { computed } from "vue";
import MediaFileItem from "@/components/basic/media-file-item.vue";
import { useI18n } from "vue-i18n";
import { copyText } from "@/hooks/use-copy-text";

const props = defineProps<{
  url: string;
  advanced?: boolean;
}>();
const emits = defineEmits<{
  (e: "remove"): void;
  (e: "edit"): void;
}>();

const { t } = useI18n();

const siteStore = useSiteStore();
const previewUrl = computed(() =>
  combineUrl(siteStore.site.prUrl, props.url + "")
);
</script>
