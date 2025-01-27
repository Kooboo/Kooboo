<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { EventTypeItem } from "@/api/events/types";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: string): void;
}>();

const props = defineProps<{
  modelValue: boolean;
  events: EventTypeItem[];
  excludes: string[];
}>();
const { t } = useI18n();
const show = ref(true);

const availableTypes = computed(() => {
  const result: { group: string; list: EventTypeItem[] }[] = [];

  for (const i of props.events) {
    if (props.excludes.some((s) => s === i.name)) continue;
    let group = result.find((f) => f.group === i.category);
    if (group) {
      group.list.push(i);
    } else {
      group = { group: i.category, list: [i] };
      result.push(group);
    }
  }

  return result;
});

const onSave = async () => {
  //   emit("update:code", innerCode.value);
  show.value = false;
};
</script>

<template>
  <el-dialog
    :model-value="show"
    custom-class="el-dialog--zero-padding"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.events')"
    @closed="emit('update:modelValue', false)"
  >
    <div v-if="availableTypes.length" class="p-24 pt-0">
      <div v-for="item of availableTypes" :key="item.group">
        <el-divider content-position="left">
          {{ item.group }}
        </el-divider>
        <div class="grid grid-cols-3 gap-12">
          <div
            v-for="i of item.list"
            :key="i.name"
            class="bg-gray/25 rounded-normal py-12 px-24 cursor-pointer hover:bg-blue hover:text-fff text-center flex items-center gap-8"
            @click="emit('selected', i.name)"
          >
            <svg
              t="1733384329463"
              class="icon mt-4"
              viewBox="0 0 1024 1024"
              version="1.1"
              xmlns="http://www.w3.org/2000/svg"
              p-id="19691"
              width="20"
              height="20"
            >
              <path
                d="M552.68894 357.136426H784.782129L230.123886 1023.949457l184.886081-469.543929H208.592593L415.009967 0.050543H815.107894z"
                fill="#bfbfbf"
                p-id="19692"
              />
            </svg>
            {{ i.display }}
          </div>
        </div>
      </div>
    </div>
    <el-empty v-else />
  </el-dialog>
</template>
