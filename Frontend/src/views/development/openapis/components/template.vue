<script lang="ts" setup>
import { getTemplates } from "@/api/openapi";
import type { Template } from "@/api/openapi/types";
import { ref, watch } from "vue";
import Alert from "@/components/basic/alert.vue";

const templates = ref<Template[]>([]);
const selected = ref<Template>();
interface EmitsType {
  (e: "change", value: Template): void;
}
defineProps<{ id: string }>();
const emits = defineEmits<EmitsType>();
watch(selected, (n) => {
  if (n) {
    emits("change", n);
  }
});

getTemplates().then((r) => (templates.value = r.data));
</script>

<template>
  <div>
    <div class="space-x-4 space-y-4">
      <el-tag
        v-for="template of templates"
        :key="template._id"
        :type="template._id === id ? 'success' : 'info'"
        class="cursor-pointer rounded-full px-24"
        size="large"
        @click="selected = template"
        >{{ template.name }}</el-tag
      >
    </div>
    <Alert
      v-if="selected?.description"
      class="mt-12 rounded-normal text-444"
      :content="selected.description"
    />
  </div>
</template>
