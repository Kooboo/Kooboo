<script lang="ts" setup>
import { useSiteStore } from "@/store/site";
import { toSeoName } from "@/utils/commerce";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  title: string;
  seoName: string;
  path: string;
  label?: string;
}>();

const emit = defineEmits<{
  (e: "update:seoName", value: string): void;
}>();

const isSeoNameDirty = ref(props.seoName != toSeoName(props.title));

const siteStore = useSiteStore();
const { t } = useI18n();

watch([() => props.title, () => isSeoNameDirty.value], () => {
  if (isSeoNameDirty.value) return;
  emit("update:seoName", toSeoName(props.title));
});
</script>

<template>
  <ElFormItem>
    <template #label>
      <div class="inline-flex items-center space-x-4">
        <div>{{ label || t("common.seoName") }}</div>
        <Tooltip :tip="t('common.seoNameTip')" custom-class="ml-4" />
      </div>
    </template>
    <div class="flex items-center space-x-8 w-full">
      <ElInput
        :model-value="props.seoName"
        @update:model-value="emit('update:seoName', $event)"
        @input="() => (isSeoNameDirty = true)"
      />
      <IconButton
        v-if="isSeoNameDirty"
        circle
        class="text-blue"
        icon="icon-Refresh"
        :tip="`Sync from title`"
        @click="() => (isSeoNameDirty = false)"
      />
    </div>
    <div class="text-s text-999">
      {{ siteStore.site.baseUrl }}{{ props.path }}/<span class="text-orange">{{
        props.seoName
      }}</span>
    </div>
  </ElFormItem>
</template>
