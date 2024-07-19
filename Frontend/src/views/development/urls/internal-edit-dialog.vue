<script lang="ts" setup>
import type { UrlItem } from "@/api/url";
import { getRoutes, updateUrl } from "@/api/url";
import { computed, ref } from "vue";
import { requiredRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { KeyValue } from "@/global/types";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  route: UrlItem;
}>();

const { t } = useI18n();

const show = ref(true);
const form = ref();
const routes = ref<(KeyValue & { parameters: any })[]>([]);

if (props.route.resourceType == "Route") {
  getRoutes().then((rsp) => {
    routes.value = rsp.sort((a, b) => a.value.localeCompare(b.value));
  });
}

const model = ref({
  type: "internal",
  id: props.route.id,
  value: props.route.name,
  destinationObjectId: props.route.objectId,
});

const onSave = async () => {
  await form.value?.validate();
  await updateUrl(model.value);
  show.value = false;
  emit("reload");
};

const parameters = computed(() => {
  if (!routes.value) return [];
  const route = routes.value.find(
    (f) => f.key == model.value.destinationObjectId
  );
  if (!route) return [];
  return Object.values(route.parameters);
});
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.editURL')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form ref="form" label-position="top" :model="model" @submit.prevent>
      <el-form-item
        label="URL"
        prop="value"
        :rules="[requiredRule(t('common.urlRequiredTips'))]"
      >
        <el-input
          v-model="model.value"
          data-cy="url-input"
          @keydown.enter="onSave"
          @input="model.value = model.value.replace(/\s+/g, '')"
        />
      </el-form-item>

      <el-form-item
        v-if="route.resourceType == 'Route' && routes.length"
        :label="t('common.redirectTo')"
        prop="id"
        :rules="[requiredRule(t('common.urlRequiredTips'))]"
      >
        <el-select
          v-model="model.destinationObjectId"
          class="w-full"
          filterable
        >
          <el-option
            v-for="item of routes"
            :key="item.key"
            :label="item.value"
            :value="item.key"
          >
            {{ item.value }}
          </el-option>
        </el-select>
        <div class="flex flex-wrap gap-4 p-4">
          <ElTag v-for="item of parameters" :key="item as string" round>{{
            item
          }}</ElTag>
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :permission="{ feature: 'link', action: 'edit' }"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
